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
		firePosition = new(transform.position.x, transform.position.y + 8.7f, transform.position.z);
		initialRotation = transform.rotation;
		transform.Find("Lightning").gameObject.SetActive(false);

		initTimer = 1.0f;

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
		firing = true;
		Fire(transform);
	}

	//-----------------------------------------------------------------//

	protected override void RotateHead(Transform childTransform) {}

	protected override void FireAnimation(Quaternion rotation)
	{
		Transform initialPosition = transform.Find("Armature/Body/NeckBottom/NeckMiddle/NeckTop/Tip/Tip_end");
		Lighting vfx = Instantiate(effectToSpawn, initialPosition.position, Quaternion.identity).GetComponent<Lighting>();
		vfx.Initialise(initialPosition.position, selectedEnemy.transform.position);

		selectedEnemy.Damage((int) damage);
	}

	//*---------------------------------------------------------------*//
    //*--------------------------- AUXILIAR --------------------------*//
    //*---------------------------------------------------------------*//
}