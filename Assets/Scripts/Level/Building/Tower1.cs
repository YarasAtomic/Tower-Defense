using Unity.VisualScripting;
using UnityEngine;

public class Tower1 : Tower
{
	// CONST attributes
	private static readonly int PURCHASE_PRICE = 100;

	// BULLET animation
	private bool firePoint = true;
	private GameObject firePointLeft;
	private GameObject firePointRight;
	[SerializeField] private GameObject effectToSpawn;

	//*---------------------------------------------------------------*//
    //*---------------------------- START ----------------------------*//
    //*---------------------------------------------------------------*//

	protected override void Start()
	{
		// Constantes
		BASE_HP = 100.0f;
		BASE_DAMAGE = 10;
		FIRE_RATE = 1.5f;
		BASE_SHOOTING_RADIUS = 15.0f;
		BASE_ROTATION_SPEED = 100.0f;
		FAVOURITE_ENEMY = TypeEnemy.Enemy1;
		
		base.Start();

		// Costes
		MAX_SELLING_PRICE *= PURCHASE_PRICE;

		// Proyectil
		firePointLeft = transform.Find("Armature/MainBody/NeckLow/NeckUp/Head/Head_L/Cannon_L/Cannon_L_end").gameObject;
		Vector3 position = firePointLeft.transform.position;
		position.x += 0.8f;
		firePointLeft.transform.position = position;

		// Estados
		initialRotation = transform.Find("Armature/MainBody/NeckLow/NeckUp/Head").rotation;

		firePointRight = transform.Find("Armature/MainBody/NeckLow/NeckUp/Head/Head_R/Cannon_R/Cannon_R_end").gameObject;
		position = firePointRight.transform.position;
		position.x += 0.8f;
		firePointRight.transform.position = position;
	}

	//*---------------------------------------------------------------*//
    //*----------------------------- GET -----------------------------*//
    //*---------------------------------------------------------------*//

	public static int GetPurchasePrice() {
		return PURCHASE_PRICE;
	}

	//*---------------------------------------------------------------*//
	//*--------------------------- ACTIONS ---------------------------*//
	//*---------------------------------------------------------------*//

	public override void AttackEnemy()
	{
		Transform childTransform = transform.Find("Armature/MainBody/NeckLow/NeckUp/Head");
		Quaternion newRotation = Quaternion.LookRotation((childTransform.position - selectedEnemy.transform.position).normalized);
		newRotation *= Quaternion.Euler(0, 180, 0);

		RotateHead(newRotation, childTransform);
		Fire(childTransform);
	}

	protected override void FireAnimation(Quaternion rotation)
	{
		GameObject initialPosition = firePoint ? firePointLeft : firePointRight;
		GameObject vfx;

		if (initialPosition != null) {
			vfx = Instantiate(effectToSpawn, initialPosition.transform.position, Quaternion.identity);
			vfx.transform.rotation = rotation;
		}

		firePoint = !firePoint;
	}

	//*---------------------------------------------------------------*//
    //*--------------------------- AUXILIAR --------------------------*//
    //*---------------------------------------------------------------*//

	protected override void ActivateAnimation()
	{
		Transform childTransform = transform.Find("Armature/MainBody/NeckLow/NeckUp/Head");
		childTransform.rotation = Quaternion.Slerp(childTransform.rotation, initialRotation, 2.0f * GameTime.DeltaTime);

		if (Quaternion.Angle(childTransform.rotation, initialRotation) < 0.1f) base.ActivateAnimation();
	}
}