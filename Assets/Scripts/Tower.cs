using System.Collections.Generic;
using System.Collections.Immutable;
using UnityEngine;

public class Tower : Building
{
	// STATIC attributes
	private static int TOWERS_DESTROYED = 0;

	// CONST attributes
	private List<float> FACTOR_UPGRADE = new List<float> {1.0f, 1.2f, 1.4f};
	private static int PURCHASE_PRICE = 100;
	private int UPGRADE_PRICE = 50;
	private int BASE_HP_COST = 5;
	private float BASE_REPAIR_RATE = 2.0f;		// miliseconds
	private int BASE_DAMAGE = 10;
	private int FAVOURITE_ENEMY = -1;
	private float FIRE_RATE = 1.0f;				// miliseconds
	[SerializeField] private float BASE_SHOOTING_RADIUS = 10.0f;
	private float BASE_ROTATION_SPEED = 100.0f;

	// COSTS attributes
	private int currentUpgrade;
	private int maxHp;
	private int repairCost;
	private float repairRate;
	private int damage;
	private float shootingRadius;
	
	// ENEMY DETECTION attributes
	private List<Enemy> enemiesInRange;
	private TypeEnemy favouriteEnemyType;
	private Enemy selectedEnemy;

	// STATE attributes
	[SerializeField] Quaternion initialRotation;
	[SerializeField] Quaternion currentRotation;
	private float fireTimer;
	private bool patrolling;
	private bool attacking;
	private bool firing;

	public override void Initialise(BuildingTile buildingTile) {
		base.tile = buildingTile;
	}

	void Start() {
		// General
		currentUpgrade = 0;
		maxHp = (int) (base.BASE_HP * FACTOR_UPGRADE[currentUpgrade]);
		base.hp = maxHp;
		
		// Costes
		CalculateSellingPrice();
		repairCost = BASE_HP_COST * (1 - base.hp/maxHp);
		repairRate = BASE_REPAIR_RATE; // * SPEED_OF_REPAIR_FACTOR[speed_of_repair_upgrade] - de research
		
		// Ataques
		damage = (int) (BASE_DAMAGE * FACTOR_UPGRADE[currentUpgrade]);
		shootingRadius = BASE_SHOOTING_RADIUS; // * shooting_radius_factor[shooting_radius_upgrade]

		SphereCollider collider = gameObject.GetComponent<SphereCollider>();
		collider.radius = shootingRadius;

		enemiesInRange = new List<Enemy>();
		favouriteEnemyType = TypeEnemy.Enemy1;
		selectedEnemy = null;
		
		// Estados
		initialRotation = transform.Find("Armature/MainBody/NeckLow/NeckUp/Head").rotation;
		Debug.Log(initialRotation.x + " " + initialRotation.y + " " + initialRotation.z + " " + initialRotation.w);
		fireTimer = 0.0f;
		patrolling = true;
		attacking = false;
		firing = false;
		
		animator = gameObject.GetComponent<Animator>();
		base.tile = null;
	}

	void Update() {
		Transform childTransform = transform.Find("Armature/MainBody/NeckLow/NeckUp/Head");
		currentRotation = childTransform.rotation;

		if (patrolling) {
			if (!animator.enabled) ActivateAnimation();
			CheckEnemiesInRange();
		}
		else if (attacking) {
			AttackEnemy();
		}
	}

	public static int GetTowersDestroyed() => TOWERS_DESTROYED;
	private static void TowerDestroyed() => TOWERS_DESTROYED += 1;

	public static int GetPurchasePrice() {
		return PURCHASE_PRICE;
	}

	public override int GetSellingPrice() {
		CalculateSellingPrice();
		return base.sellingPrice;
	}
	private void CalculateSellingPrice() {
		base.sellingPrice = (int) (base.MAX_SELLING_PRICE * (base.hp/maxHp) * FACTOR_UPGRADE[currentUpgrade]); // * REFUND_FACTOR[refund_upgrade]
	}

	// ACTION methods

	public void CheckEnemiesInRange() {
		if (selectedEnemy == null && enemiesInRange.Count != 0) {
			float minDist = float.MaxValue;
			
			foreach (Enemy enemy in enemiesInRange) {
				if (enemy.GetTypeEnemy() == favouriteEnemyType) {
					selectedEnemy = enemy;
					break;
				}
				else {
					float dist = Vector3.Distance(this.transform.position, enemy.transform.position);
					if (dist < minDist) {
						minDist = dist;
						selectedEnemy = enemy;
					}
				}
			}

			Debug.Log(selectedEnemy);

			patrolling = false;
			attacking = true;

			// animator.SetBool("attackingEnemy", attacking);
			animator.SetBool("patrolling", false);
		}
	}

	public void AttackEnemy() {		
		Transform childTransform = transform.Find("Armature/MainBody/NeckLow/NeckUp/Head");
		Quaternion newRotation = Quaternion.LookRotation((childTransform.position - selectedEnemy.transform.position).normalized);
		newRotation *= Quaternion.Euler(0, 180, 0);

		float rotationSpeed = BASE_ROTATION_SPEED * Time.deltaTime;

		if (!firing) {
			animator.enabled = false;

			float angle = Quaternion.Angle(childTransform.rotation, newRotation);
			// float rotatingTime = angle / rotationSpeed;
			// fireTimer = (fireTimer >= rotatingTime) ? fireTimer : rotatingTime;
			fireTimer = FIRE_RATE - ((angle*Mathf.Deg2Rad) / rotationSpeed) - 0.25f;

			firing = true;
		}
		childTransform.rotation = Quaternion.RotateTowards(childTransform.rotation, newRotation, rotationSpeed);
		
		if (firing) {
			fireTimer += Time.deltaTime;

			if (fireTimer >= FIRE_RATE) {
				selectedEnemy.Damage(damage);
				if (selectedEnemy.GetHealthPercentage() <= 0) {
					EnemyOutOfRange(selectedEnemy);
				}
				
				fireTimer = 0.0f;
			}
		}
	}

	public void UpgradeTower() {
		currentUpgrade += 1; // La comprobación se hace fuera

		animator.SetInteger("towerLevel", currentUpgrade);
	}

	public void RepairTower(bool repairing) {
		animator.SetBool("repairTower", repairing);
	}

	public override void DestroyBuilding() {
		Debug.Log("Tower destroyed");
		animator.SetBool("destroyTower", true);

		TowerDestroyed();
	}

	// COLLISIONS methods

	void OnTriggerEnter(Collider collision) {
		// Debug.Log("Nuevo Enemy");
		Enemy enemy = collision.gameObject.GetComponent<Enemy>();
		if (CheckForObstacles(enemy.transform.position) && enemy.GetHealthPercentage() > 0) enemiesInRange.Add(enemy);
	}

	void OnTriggerExit(Collider collision) {
		// Debug.Log("Se va Enemy");
		EnemyOutOfRange(collision.gameObject.GetComponent<Enemy>());
	}

	// PRIVATE auxiliar methods

	private bool CheckForObstacles(Vector3 enemyPosition) {
		RaycastHit raycastHit;
		Vector3 dir = (transform.position - enemyPosition).normalized;

		bool hit = Physics.Raycast(
			transform.position,
			dir,
			out raycastHit,
			shootingRadius,
			LayerMask.GetMask("Terrain")
		);

		return hit;
	}

	private void EnemyOutOfRange(Enemy enemy) {
		enemiesInRange.Remove(enemy);

		if (enemy == selectedEnemy) {
			selectedEnemy = null;

			patrolling = true;
			attacking = false;
			firing = false;

			// Transform childTransform = transform.Find("Armature/MainBody/NeckLow/NeckUp/Head");
			// Quaternion newRotation = Quaternion.LookRotation((childTransform.position - initialTransform.position).normalized);
			// newRotation *= Quaternion.Euler(0, 180, 0);
			// childTransform.rotation = Quaternion.RotateTowards(childTransform.rotation, Quaternion.Euler(0.0f, 180.0f, 0.0f), BASE_ROTATION_SPEED * Time.deltaTime);
			// childTransform.rotation = initialTransform.rotation;
			
		}
	}

	private void ActivateAnimation() {
		Transform childTransform = transform.Find("Armature/MainBody/NeckLow/NeckUp/Head");
		childTransform.rotation = Quaternion.Slerp(childTransform.rotation, initialRotation, 2.0f * Time.deltaTime);

		Debug.Log(childTransform.rotation.x + " " + childTransform.rotation.y + " " + childTransform.rotation.z + " " + childTransform.rotation.w);
		Debug.Log(initialRotation.x + " " + initialRotation.y + " " + initialRotation.z + " " + initialRotation.w);
		if (Quaternion.Angle(childTransform.rotation, initialRotation) < 0.1f) {
			animator.enabled = true;
			animator.SetBool("patrolling", true);
			animator.Play("Patrol", -1, 0.0f);
		}
	}
}
