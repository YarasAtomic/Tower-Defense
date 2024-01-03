using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tower : Building
{
	// STATIC attributes
	private static int TOWERS_DESTROYED = 0;

	// CONST attributes
	private readonly int MAX_UPGRADE = 2;
	private readonly List<float> FACTOR_UPGRADE = new() { 1.0f, 1.2f, 1.4f};
	private static readonly int PURCHASE_PRICE = 100;
	private readonly int UPGRADE_PRICE = 50;
	private readonly int BASE_HP_COST = 5;
	private readonly float BASE_REPAIR_RATE = 0.5f;		// seconds
	private readonly int BASE_DAMAGE = 10;
	private readonly float FIRE_RATE = 1.0f;			// seconds
	[SerializeField] private readonly float BASE_SHOOTING_RADIUS = 15.0f;
	private readonly float BASE_ROTATION_SPEED = 100.0f;

	// COSTS attributes
	private int currentUpgrade = 0;
	private float maxHp;
	private int repairCost;
	private float repairRate;
	private float repairHp = 0.0f;
	private float maxRepairHp = 0.0f;
	private float damage;
	private float shootingRadius;
	
	// ENEMY DETECTION attributes
	private readonly List<Enemy> enemiesInRange = new();
	private TypeEnemy FAVOURITE_ENEMY;
	private Enemy selectedEnemy = null;

	// STATE attributes
	private Quaternion initialRotation;
	private float initTimer;
	private float repairTimer = 0.0f;
	private bool repairing = false;
	private float fireTimer = 0.0f;
	private bool patrolling = true;
	private bool attacking = false;
	private bool firing = false;

	// BULLET animation
	private bool firePoint = true;
	private GameObject firePointLeft;
	private GameObject firePointRight;
	[SerializeField] private GameObject effectToSpawn;

	//*---------------------------------------------------------------*//
    //*-------------------------- INITIALISE -------------------------*//
    //*---------------------------------------------------------------*//

	public override void Initialise(TypeBuilding typeBuilding, BuildingTile buildingTile) {
		TYPE = typeBuilding;

		switch (TYPE) {
			case TypeBuilding.Tower1: FAVOURITE_ENEMY = TypeEnemy.Enemy1; break;
			case TypeBuilding.Tower2: FAVOURITE_ENEMY = TypeEnemy.Enemy2; break;
			case TypeBuilding.Tower3: FAVOURITE_ENEMY = TypeEnemy.Enemy3; break;
		}

		tile = buildingTile;
	}

	//*---------------------------------------------------------------*//
    //*---------------------------- START ----------------------------*//
    //*---------------------------------------------------------------*//

	void Start() {
		transform.rotation = Quaternion.Euler(0, 90, 0);

		// General
		maxHp = BASE_HP * FACTOR_UPGRADE[currentUpgrade];
		hp = maxHp;
		
		// Costes
		MAX_SELLING_PRICE = PURCHASE_PRICE * 0.75f;
		repairRate = BASE_REPAIR_RATE; // * SPEED_OF_REPAIR_FACTOR[speed_of_repair_upgrade] - de research
		
		// Ataques
		damage = BASE_DAMAGE * FACTOR_UPGRADE[currentUpgrade];
		shootingRadius = BASE_SHOOTING_RADIUS; // * shooting_radius_factor[shooting_radius_upgrade]

		SphereCollider collider = gameObject.GetComponent<SphereCollider>();
		collider.radius = shootingRadius;
		
		// Estados
		initialRotation = transform.Find("Armature/MainBody/NeckLow/NeckUp/Head").rotation;
		
		animator = gameObject.GetComponent<Animator>();
		AnimationClip animationClip = animator.runtimeAnimatorController.animationClips[1];
		initTimer = animationClip.length * 10.0f;

		// Proyectil
		firePointLeft = transform.Find("Armature/MainBody/NeckLow/NeckUp/Head/Head_L/Cannon_L/Cannon_L_end").gameObject;
		Vector3 position = firePointLeft.transform.position;
		position.x += 0.8f;
		firePointLeft.transform.position = position;

		firePointRight = transform.Find("Armature/MainBody/NeckLow/NeckUp/Head/Head_R/Cannon_R/Cannon_R_end").gameObject;
		position = firePointRight.transform.position;
		position.x += 0.8f;
		firePointRight.transform.position = position;
	}

	//*---------------------------------------------------------------*//
    //*---------------------------- UPDATE ---------------------------*//
    //*---------------------------------------------------------------*//

	void Update() {
		if (initTimer > 0.0f) {
			initTimer -= GameTime.DeltaTime;
			return;
		}

		animator.speed = GameTime.GameSpeed;
		animator.SetBool("repairTower", repairing);
		animator.SetBool("patrolling", patrolling);

		if (repairing) Repair();

		if (patrolling) {
			if (!animator.enabled) ActivateAnimation();
			CheckEnemiesInRange();
		}
		else if (attacking) {
			AttackEnemy();
		}
	}

	//*---------------------------------------------------------------*//
    //*----------------------------- GET -----------------------------*//
    //*---------------------------------------------------------------*//

	public static int GetTowersDestroyed() => TOWERS_DESTROYED;
	private static void TowerDestroyed() => TOWERS_DESTROYED += 1;

	public override float GetHealthPercentage() {
		return hp / maxHp;
	}

	public static int GetPurchasePrice() {
		return PURCHASE_PRICE;
	}

	public int GetRepairPrice() {
		CalculateRepairPrice();
		return repairCost;
	}

	public override int GetSellingPrice() {
		CalculateSellingPrice();
		Debug.Log(sellingPrice);
		return sellingPrice;
	}

	public bool IsMaxUpgraded() {
		return currentUpgrade >= MAX_UPGRADE;
	}

	private void CalculateRepairPrice() {
		repairCost = BASE_HP_COST * (1 - (int) GetHealthPercentage());
	}

	private void CalculateSellingPrice() {
		sellingPrice = (int) (MAX_SELLING_PRICE * GetHealthPercentage() * FACTOR_UPGRADE[currentUpgrade]); // * REFUND_FACTOR[refund_upgrade]
	}

	//*---------------------------------------------------------------*//
    //*--------------------------- ACTIONS ---------------------------*//
    //*---------------------------------------------------------------*//

	public void CheckEnemiesInRange() {
		if (selectedEnemy == null && enemiesInRange.Count != 0) {
			float minDist = float.MaxValue;
			
			foreach (Enemy enemy in enemiesInRange) {
				if (enemy.GetTypeEnemy() == FAVOURITE_ENEMY) {
					selectedEnemy = enemy;
					break;
				}
				else {
					float dist = Vector3.Distance(transform.position, enemy.transform.position);
					if (dist < minDist) {
						minDist = dist;
						selectedEnemy = enemy;
					}
				}
			}

			patrolling = false;
			attacking = true;			
		}
	}

	public void AttackEnemy() {		
		Transform childTransform = transform.Find("Armature/MainBody/NeckLow/NeckUp/Head");
		Quaternion newRotation = Quaternion.LookRotation((childTransform.position - selectedEnemy.transform.position).normalized);
		newRotation *= Quaternion.Euler(0, 180, 0);

		float rotationSpeed = BASE_ROTATION_SPEED * GameTime.DeltaTime;

		if (!firing) {
			animator.enabled = false;

			float angle = Quaternion.Angle(childTransform.rotation, newRotation);
			fireTimer = FIRE_RATE - (angle*Mathf.Deg2Rad / rotationSpeed) - 0.25f;

			firing = true;
		}
		childTransform.rotation = Quaternion.RotateTowards(childTransform.rotation, newRotation, rotationSpeed);
		
		if (firing) {
			fireTimer += GameTime.DeltaTime;

			if (fireTimer >= FIRE_RATE) {
				FireBullet(childTransform.rotation);
				// selectedEnemy.Damage((int) damage);
				if (selectedEnemy.GetHealthPercentage() <= 0) {
					EnemyOutOfRange(selectedEnemy);
				}
				
				fireTimer = 0.0f;
			}
		}
	}

	public void UpgradeTower() {
		if (IsMaxUpgraded()) {
			currentUpgrade = MAX_UPGRADE;
			return;
		}

		currentUpgrade += 1; // La comprobación se hace fuera
		animator.SetTrigger("upgradeTower");
	}

	public void RepairTower() {
		repairHp = 0.0f;
		maxRepairHp = maxHp - hp;

		repairing = true;
	}

	private void FireBullet(Quaternion rotation) {
		GameObject initialPosition = firePoint ? firePointLeft : firePointRight;
		GameObject vfx;

		if (initialPosition != null) {
			vfx = Instantiate(effectToSpawn, initialPosition.transform.position, Quaternion.identity);
			vfx.transform.rotation = rotation;
		}
	}

	private void Repair() {
		repairTimer += GameTime.DeltaTime;

		if (repairTimer >= repairRate) {
			hp += 1;
			repairHp += 1;
			repairTimer = 0.0f;

			if (repairHp >= maxRepairHp) {
				if (hp > maxHp) hp = maxHp;
				repairing = false;
			}
		}
	}

	public override void DestroyBuilding() {
		animator.SetTrigger("destroyTower");

		TowerDestroyed();
		tile.EmptyTile();

		Destroy(gameObject);
		Destroy(this);
	}

	//*---------------------------------------------------------------*//
    //*------------------------- COLLISSIONS -------------------------*//
    //*---------------------------------------------------------------*//

	void OnTriggerEnter(Collider collision) {
		// Debug.Log("Nuevo Enemy");
		Enemy enemy = collision.gameObject.GetComponent<Enemy>();
		if (enemy != null && CheckForObstacles(enemy.transform.position) && enemy.GetHealthPercentage() > 0) enemiesInRange.Add(enemy);
	}

	void OnTriggerExit(Collider collision) {
		// Debug.Log("Se va Enemy");
		EnemyOutOfRange(collision.gameObject.GetComponent<Enemy>());
	}

	//*---------------------------------------------------------------*//
    //*--------------------------- AUXILIAR --------------------------*//
    //*---------------------------------------------------------------*//

	private bool CheckForObstacles(Vector3 enemyPosition) {
		Vector3 dir = (transform.position - enemyPosition).normalized;

		bool hit = Physics.Raycast(
			transform.position,
			dir,
			out RaycastHit raycastHit,
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
		}
	}

	private void ActivateAnimation() {
		Transform childTransform = transform.Find("Armature/MainBody/NeckLow/NeckUp/Head");
		childTransform.rotation = Quaternion.Slerp(childTransform.rotation, initialRotation, 2.0f * GameTime.DeltaTime);

		if (Quaternion.Angle(childTransform.rotation, initialRotation) < 0.1f) {
			animator.enabled = true;
			animator.SetBool("patrolling", true);
			animator.Play("Patrol", -1, 0.0f);
		}
	}
}
