using Unity.VisualScripting;
using UnityEngine;

public class Tower2 : Tower
{
	// CONST attributes
	private static readonly int PURCHASE_PRICE = 150;

	//*---------------------------------------------------------------*//
    //*---------------------------- START ----------------------------*//
    //*---------------------------------------------------------------*//
	
	protected override void Start()
	{
		// Constantes
		BASE_HP = 150.0f;
		BASE_DAMAGE = 3;
		FIRE_RATE = 0.3f;
		BASE_SHOOTING_RADIUS = 8.0f;
		BASE_ROTATION_SPEED = 0.0f; // Para esta tener en cuenta que no hay rotación y se dispara directamente
		FAVOURITE_ENEMY = TypeEnemy.Enemy2;
		
		base.Start();

		// Costes
		MAX_SELLING_PRICE *= PURCHASE_PRICE;

		// Estados
		initialRotation = transform.rotation;
	}

	//*---------------------------------------------------------------*//
    //*----------------------------- GET -----------------------------*//
    //*---------------------------------------------------------------*//

	public static int GetPurchasePrice()
	{
		return PURCHASE_PRICE;
	}

	//*---------------------------------------------------------------*//
    //*--------------------------- ACTIONS ---------------------------*//
    //*---------------------------------------------------------------*//

	public override void AttackEnemy()
	{
		if (!firing) {
			animator.enabled = false;
			
			fireTimer = 0.0f;
			firing = true;
		}

		Fire(transform);
	}

	protected override void FireAnimation(Quaternion rotation)
	{

	}

	//*---------------------------------------------------------------*//
    //*--------------------------- AUXILIAR --------------------------*//
    //*---------------------------------------------------------------*//
}