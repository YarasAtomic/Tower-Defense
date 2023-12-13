using System.Collections.Generic;
using System.Collections.Immutable;

public class Tower : Building
{
	private Building building;

	private static List<float> FACTOR_UPGRADE = new List<float> {1.0f, 1.2f, 1.4f};
	private int UPGRADE_PRICE = 50;
	private int BASE_REPAIR_RATE = 5;			// miliseconds
	private int BASE_DAMAGE = 5;
	private int FAVOURITE_ENEMY = -1;
	private int FIRE_RATE = 10;					// miliseconds
	private float BASE_SHOOTING_RADIUS = 5.0f;

	private int currentUpgrade = 0;
	private int maxHp;
	private int repairCost;
	private int repairRate;
	private int damage;
	private float shootingRadius;
	private Enemy selectedEnemy;

	public Tower() {
		maxHp = base.BASE_HP;
		base.BASE_HP = 1;

		this.BASE_HP = 1;
	}
}
