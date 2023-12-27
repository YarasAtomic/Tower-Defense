using System.Collections.Generic;
using System.Collections.Immutable;
using UnityEngine;

public class Tower : Building
{
	private static int TOWERS_DESTROYED = 0;

	private List<float> FACTOR_UPGRADE = new List<float> {1.0f, 1.2f, 1.4f};
	private static int PURCHASE_PRICE = 100;
	private int UPGRADE_PRICE = 50;
	private int BASE_HP_COST = 5;
	private int BASE_REPAIR_RATE = 5;			// miliseconds
	private int BASE_DAMAGE = 5;
	private int FAVOURITE_ENEMY = -1;
	private int FIRE_RATE = 10;					// miliseconds
	private float BASE_SHOOTING_RADIUS = 5.0f;

	private int currentUpgrade;
	private int maxHp;
	private int repairCost;
	private int repairRate;
	private int damage;
	[SerializeField] private float shootingRadius;
	private float attackTimer;
	
	private List<Enemy> enemiesInRange;
	private TypeEnemy favouriteEnemyType;
	private Enemy selectedEnemy;

	public override void Initialise(BuildingTile buildingTile) {
		base.tile = buildingTile;

		SphereCollider collider = gameObject.GetComponent<SphereCollider>();
		collider.radius = shootingRadius;
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
		attackTimer = 0.0f;

		enemiesInRange = new List<Enemy>();
		favouriteEnemyType = TypeEnemy.Enemy1;
		selectedEnemy = null;
		
		animator = gameObject.GetComponent<Animator>();
		base.tile = null;
	}

	void Update() {
		if (selectedEnemy == null && enemiesInRange.Count != 0) {
			CheckEnemiesInRange();
		}
		else if (selectedEnemy != null) {
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
	}

	public void AttackEnemy() {
		
	}

	public void UpgradeTower() {
		currentUpgrade += 1; // La comprobación se hace fuera

		animator.SetInteger("towerLevel", currentUpgrade);
	}

	public void RepairTower(bool repairing) {
		animator.SetBool("repairTower", repairing);
	}

	public override void DestroyBuilding() {
		animator.SetBool("destroyTower", true);

		TowerDestroyed();
	}

	// COLLISIONS methods

	void OnTriggerEnter(Collider collision) {
		Debug.Log("Nuevo Enemy");
		enemiesInRange.Add(collision.gameObject.GetComponent<Enemy>());
	}

	void OnTriggerExit(Collider collision) {
		Debug.Log("Se va Enemy");
		enemiesInRange.Remove(collision.gameObject.GetComponent<Enemy>());
	}
}
