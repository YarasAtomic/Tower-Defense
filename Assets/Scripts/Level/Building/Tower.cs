using System.Collections.Generic;
using UnityEngine;

public class Tower : Building
{
	// STATIC attributes
	private static int TOWERS_DESTROYED = 0;

	// CONST attributes
	private readonly int MAX_UPGRADE = 2;
	private readonly List<float> FACTOR_UPGRADE = new() { 1.0f, 1.2f, 1.4f};
	private static readonly int PURCHASE_PRICE_T1 = 100;
	private static readonly int PURCHASE_PRICE_T2 = 150;
	private static readonly int PURCHASE_PRICE_T3 = 200;
	private readonly int UPGRADE_PRICE = 50;
	private readonly float BASE_HP_COST = 5.0f;
	private readonly float BASE_REPAIR_RATE = 0.25f;	// seconds
	[SerializeField] private int BASE_DAMAGE = 10;
	[SerializeField] private float FIRE_RATE = 1.0f;	// seconds
	[SerializeField] private float BASE_SHOOTING_RADIUS = 15.0f;
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
		MAX_SELLING_PRICE = 0.55f;

		switch (TYPE) {
			case TypeBuilding.Tower1:
				MAX_SELLING_PRICE *= PURCHASE_PRICE_T1;
				FAVOURITE_ENEMY = TypeEnemy.Enemy1;
				break;
			case TypeBuilding.Tower2:
				MAX_SELLING_PRICE *= PURCHASE_PRICE_T2;
				FAVOURITE_ENEMY = TypeEnemy.Enemy2;
				break;
			case TypeBuilding.Tower3:
				MAX_SELLING_PRICE *= PURCHASE_PRICE_T3;
				FAVOURITE_ENEMY = TypeEnemy.Enemy3;
				break;
		}

		tile = buildingTile;
	}

	//*---------------------------------------------------------------*//
    //*---------------------------- START ----------------------------*//
    //*---------------------------------------------------------------*//

	void Start() {
		transform.rotation = Quaternion.Euler(0, 90, 0);

		UpdateStats(true);
		
		// Costes
		repairRate = BASE_REPAIR_RATE * Research.SPEED_OF_REPAIR_FACTOR[SingletonScriptableObject<Save>.Instance.GetSaveFile().GetShootingRadius()];
		
		// Ataques
		shootingRadius = BASE_SHOOTING_RADIUS * Research.SHOOTING_RADIUS_FACTOR[SingletonScriptableObject<Save>.Instance.GetSaveFile().GetShootingRadius()];

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
		if (patrolling && !animator.enabled) ActivateAnimation();
		
		CheckEnemiesInRange();
		if (attacking) {
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

	public static int GetPurchasePrice(TypeBuilding type) {
		int purchasePrice = (type == TypeBuilding.Tower1)? PURCHASE_PRICE_T1 : (type == TypeBuilding.Tower2) ? PURCHASE_PRICE_T2 : PURCHASE_PRICE_T3;
		return purchasePrice;
	}

	public int GetUpgradePrice() {
		return UPGRADE_PRICE;
	}

	public int GetRepairPrice() {
		CalculateRepairPrice();
		return repairCost;
	}

	public override int GetSellingPrice() {
		CalculateSellingPrice();
		return sellingPrice;
	}

	public bool IsMaxUpgraded() {
		return currentUpgrade >= MAX_UPGRADE;
	}

	private void CalculateRepairPrice() {
		repairCost = (int) (BASE_HP_COST * (1.0f - GetHealthPercentage()));
	}

	private void CalculateSellingPrice() {
		sellingPrice = (int) (MAX_SELLING_PRICE * GetHealthPercentage() * FACTOR_UPGRADE[currentUpgrade] * Research.REFUND_FACTOR[SingletonScriptableObject<Save>.Instance.GetSaveFile().GetRefundForSelling()]) ;
	}

	//*---------------------------------------------------------------*//
    //*--------------------------- ACTIONS ---------------------------*//
    //*---------------------------------------------------------------*//

	// public void CheckEnemiesInRange() {
	// 	if (selectedEnemy == null && enemiesInRange.Count != 0) {
	// 		float minDist = float.MaxValue;
			
	// 		foreach (Enemy enemy in enemiesInRange) {
	// 			if (enemy.GetTypeEnemy() == FAVOURITE_ENEMY) {
	// 				selectedEnemy = enemy;
	// 				break;
	// 			}
	// 			else {
	// 				float dist = Vector3.Distance(transform.position, enemy.transform.position);
	// 				if (dist < minDist) {
	// 					minDist = dist;
	// 					selectedEnemy = enemy;
	// 				}
	// 			}
	// 		}

	// 		patrolling = false;
	// 		attacking = true;
	// 	}
	// }

	public void CheckEnemiesInRange() {
		if (selectedEnemy == null) {
			Collider[] hitColliders = Physics.OverlapSphere(transform.position, shootingRadius);
			float minDist = float.MaxValue;
			
			foreach (Collider hit in hitColliders) {
				Enemy enemy = hit.GetComponent<Enemy>();
				if (enemy != null && enemy.GetHealthPercentage() > 0) {
					if (enemy.GetTypeEnemy() == FAVOURITE_ENEMY) {
						selectedEnemy = enemy;

						patrolling = false;
						attacking = true;
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
			}
		}
		else {
			float dist = Vector3.Distance(transform.position, selectedEnemy.transform.position);
			if (dist > shootingRadius) {
				EnemyOutOfRange(selectedEnemy);
				CheckEnemiesInRange();
			}
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
				selectedEnemy.Damage((int) damage);
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

		currentUpgrade += 1;
		UpdateStats(false);

		animator.SetTrigger("upgradeTower");
	}

	public void RepairTower() {
		repairHp = 0.0f;
		maxRepairHp = maxHp - hp;

		repairing = true;
	}

	public override void DestroyBuilding() {
		animator.SetTrigger("destroyTower");

		TowerDestroyed();
		tile.EmptyTile();

		Destroy(gameObject);
		Destroy(this);
	}

	//-----------------------------------------------------------------//

	private void FireBullet(Quaternion rotation) {
		GameObject initialPosition = firePoint ? firePointLeft : firePointRight;
		GameObject vfx;

		if (initialPosition != null) {
			vfx = Instantiate(effectToSpawn, initialPosition.transform.position, Quaternion.identity);
			vfx.transform.rotation = rotation;
		}

		firePoint = !firePoint;
	}

	private void UpdateStats(bool init) {
		// General
		float prevMaxHp = maxHp;
		maxHp = BASE_HP * FACTOR_UPGRADE[currentUpgrade];
		hp = init ? maxHp : (hp + maxHp - prevMaxHp);

		// Ataques
		damage = BASE_DAMAGE * FACTOR_UPGRADE[currentUpgrade];
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

	//*---------------------------------------------------------------*//
    //*------------------------- COLLISSIONS -------------------------*//
    //*---------------------------------------------------------------*//

	// void OnTriggerEnter(Collider collision) {
	// 	// Debug.Log("Nuevo Enemy");
	// 	Enemy enemy = collision.gameObject.GetComponent<Enemy>();
	// 	if (enemy != null && CheckForObstacles(enemy.transform.position) && enemy.GetHealthPercentage() > 0) enemiesInRange.Add(enemy);
	// }

	// void OnTriggerExit(Collider collision) {
	// 	// Debug.Log("Se va Enemy");
	// 	EnemyOutOfRange(collision.gameObject.GetComponent<Enemy>());
	// }

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
		// enemiesInRange.Remove(enemy);

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
