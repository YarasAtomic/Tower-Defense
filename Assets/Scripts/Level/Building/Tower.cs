using System.Collections.Generic;
using UnityEngine;

public abstract class Tower : Building
{
	// STATIC attributes
	private static int TOWERS_DESTROYED = 0;

	// CONST attributes
	private const int MAX_UPGRADE = 2;
	private readonly List<float> FACTOR_UPGRADE = new() { 1.0f, 1.2f, 1.4f };
	private const int UPGRADE_PRICE = 50;
	private const float BASE_HP_COST = 5.0f;
	private const float BASE_REPAIR_RATE = 0.25f;	// seconds
	protected int BASE_DAMAGE;
	protected float FIRE_RATE;			// seconds
	protected float BASE_SHOOTING_RADIUS;
	protected float BASE_ROTATION_SPEED;
	protected TypeEnemy FAVOURITE_ENEMY;

	// COSTS attributes
	private int currentUpgrade = 0;
	private float maxHp;
	private int repairCost;
	private float repairRate;
	private float repairHp = 0.0f;
	private float maxRepairHp = 0.0f;
	private float damage;
	private float shootingRadius;
	
	// ENEMY ATTACKING attributes
	// private readonly List<Enemy> enemiesInRange = new();
	protected Enemy selectedEnemy = null;
	[SerializeField] protected GameObject effectToSpawn;

	// STATE attributes
	protected Quaternion initialRotation;
	protected float initTimer;
	private float repairTimer = 0.0f;
	private bool repairing = false;
	protected float fireTimer = 0.0f;
	private bool patrolling = true;
	private bool attacking = false;
	protected bool firing = false;

	//*---------------------------------------------------------------*//
    //*-------------------------- INITIALISE -------------------------*//
    //*---------------------------------------------------------------*//

	public override void Initialise(BuildingTile buildingTile)
	{
		tile = buildingTile;
	}

	//*---------------------------------------------------------------*//
    //*---------------------------- START ----------------------------*//
    //*---------------------------------------------------------------*//

	protected virtual void Start()
	{
		transform.rotation = Quaternion.Euler(0, 90, 0);

		UpdateStats(true);
		
		// Costes
		repairRate = BASE_REPAIR_RATE * Research.SPEED_OF_REPAIR_FACTOR[SingletonScriptableObject<Save>.Instance.GetSaveFile().GetShootingRadius()];
		MAX_SELLING_PRICE = 0.55f;
		
		// Ataques
		shootingRadius = BASE_SHOOTING_RADIUS * Research.SHOOTING_RADIUS_FACTOR[SingletonScriptableObject<Save>.Instance.GetSaveFile().GetShootingRadius()];
		
		animator = gameObject.GetComponent<Animator>();
	}

	//*---------------------------------------------------------------*//
    //*---------------------------- UPDATE ---------------------------*//
    //*---------------------------------------------------------------*//

	void Update()
	{
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
		if (attacking) AttackEnemy();
	}

	//*---------------------------------------------------------------*//
    //*----------------------------- GET -----------------------------*//
    //*---------------------------------------------------------------*//

	public static int GetTowersDestroyed() => TOWERS_DESTROYED;
	private static void TowerDestroyed() => TOWERS_DESTROYED += 1;

	public override float GetHealthPercentage()
	{
		return hp / maxHp;
	}

	public int GetUpgradePrice()
	{
		return UPGRADE_PRICE;
	}

	public int GetRepairPrice()
	{
		CalculateRepairPrice();
		return repairCost;
	}

	public override int GetSellingPrice()
	{
		CalculateSellingPrice();
		return sellingPrice;
	}

	public bool IsMaxUpgraded()
	{
		return currentUpgrade >= MAX_UPGRADE;
	}

	private void CalculateRepairPrice()
	{
		repairCost = (int) (BASE_HP_COST * (1.0f - GetHealthPercentage()));
	}

	private void CalculateSellingPrice()
	{
		sellingPrice = (int) (MAX_SELLING_PRICE * GetHealthPercentage() * FACTOR_UPGRADE[currentUpgrade] * Research.REFUND_FACTOR[SingletonScriptableObject<Save>.Instance.GetSaveFile().GetRefundForSelling()]) ;
	}

	//*---------------------------------------------------------------*//
    //*--------------------------- ACTIONS ---------------------------*//
    //*---------------------------------------------------------------*//

	// public void CheckEnemiesInRange()
	// {
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

	public void CheckEnemiesInRange()
	{
		if (selectedEnemy == null) {
			Collider[] hitColliders = Physics.OverlapSphere(transform.position, shootingRadius);
			float minDist = float.MaxValue;
			
			foreach (Collider hit in hitColliders) {
				Enemy enemy = hit.GetComponent<Enemy>();
				if (enemy != null && enemy.GetHealthPercentage() > 0 && !CheckForObstacles(enemy.transform.position)) {
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
			}

			if (selectedEnemy != null) {
				patrolling = false;
				attacking = true;
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

	public abstract void AttackEnemy();

	public void UpgradeTower()
	{
		if (IsMaxUpgraded()) {
			currentUpgrade = MAX_UPGRADE;
			return;
		}

		currentUpgrade += 1;
		UpdateStats(false);

		animator.SetTrigger("upgradeTower");
	}

	public void RepairTower()
	{
		repairHp = 0.0f;
		maxRepairHp = maxHp - hp;

		repairing = true;
	}

	public override void DestroyBuilding()
	{
		animator.SetTrigger("destroyTower");

		TowerDestroyed();
		tile.EmptyTile();

		Destroy(gameObject);
		Destroy(this);
	}

	//-----------------------------------------------------------------//

	protected abstract void RotateHead(Transform childTransform);

	protected void Fire(Transform childTransform)
	{
		if (firing) {
			fireTimer += GameTime.DeltaTime;

			if (fireTimer >= FIRE_RATE) {
				FireAnimation(childTransform.rotation);
				selectedEnemy.Damage((int) damage);
				if (selectedEnemy.GetHealthPercentage() <= 0) {
					EnemyOutOfRange(selectedEnemy);
				}
				
				fireTimer = 0.0f;
			}
		}
	}

	protected abstract void FireAnimation(Quaternion rotation);

	private void UpdateStats(bool init)
	{
		// General
		float prevMaxHp = maxHp;
		maxHp = BASE_HP * FACTOR_UPGRADE[currentUpgrade];
		hp = init ? maxHp : (hp + maxHp - prevMaxHp);

		// Ataques
		damage = BASE_DAMAGE * FACTOR_UPGRADE[currentUpgrade];
	}

	private void Repair()
	{
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

	// void OnTriggerEnter(Collider collision)
	// {
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

	private bool CheckForObstacles(Vector3 enemyPosition)
	{
		Vector3 dir = (transform.position - enemyPosition).normalized;
		Ray ray = new(transform.position, dir);
		// Debug.DrawRay(transform.position, dir);

		bool hit = false;
		RaycastHit[] hits = Physics.RaycastAll(ray);

		float obstacleDist = 0.0f;
		float enemyDist = Vector3.Distance(transform.position, enemyPosition);
		foreach (RaycastHit raycastHit in hits) {
			Enemy enemy = raycastHit.transform.GetComponent<Enemy>();
			if (enemy == null) {
				obstacleDist = Vector3.Distance(transform.position, raycastHit.transform.position);
				if (obstacleDist > 0) break;
			}
		}

		// Debug.Log(obstacleDist);
		// Debug.Log(enemyDist);

		if (enemyDist >= obstacleDist) hit = true;

		// Enemy enemy = hitInfo.collider.gameObject.GetComponent<Enemy>();
		// if (enemy == null) {
		// 	Debug.Log("Obstaculo");
		// 	hit = true;
		// }

		// bool hit = Physics.Raycast(
		// 	transform.position,
		// 	dir,
		// 	out RaycastHit raycastHit,
		// 	shootingRadius,
		// 	LayerMask.GetMask("Terrain")
		// );

		return hit;
	}

	private void EnemyOutOfRange(Enemy enemy)
	{
		// enemiesInRange.Remove(enemy);

		if (enemy == selectedEnemy) {
			selectedEnemy = null;

			patrolling = true;
			attacking = false;
			firing = false;
		}
	}

	protected virtual void ActivateAnimation()
	{
		animator.enabled = true;
		animator.SetBool("patrolling", true);
		animator.Play("Patrol", -1, 0.0f);
	}
}
