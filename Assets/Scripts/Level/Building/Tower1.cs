using Unity.VisualScripting;

public class Tower1 : Tower
{
	// CONST attributes
	private static readonly int PURCHASE_PRICE = 100;

	//*---------------------------------------------------------------*//
    //*---------------------------- START ----------------------------*//
    //*---------------------------------------------------------------*//

	protected override void Start()
	{
		// Constantes
		BASE_HP = 100.0f;
		BASE_DAMAGE = 10;
		FIRE_RATE = 1.0f;
		BASE_SHOOTING_RADIUS = 15.0f;
		BASE_ROTATION_SPEED = 100.0f;
		FAVOURITE_ENEMY = TypeEnemy.Enemy1;
		
		base.Start();

		// Costes
		MAX_SELLING_PRICE *= PURCHASE_PRICE;
	}

	//*---------------------------------------------------------------*//
    //*----------------------------- GET -----------------------------*//
    //*---------------------------------------------------------------*//

	public static int GetPurchasePrice() {
		return PURCHASE_PRICE;
	}
}