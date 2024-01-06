using Unity.VisualScripting;
using UnityEngine;

public class Tower3 : Tower
{
	// CONST attributes
	private static readonly int PURCHASE_PRICE = 200;

	//*---------------------------------------------------------------*//
    //*---------------------------- START ----------------------------*//
    //*---------------------------------------------------------------*//
	
	protected override void Start()
	{
		// Constantes
		BASE_HP = 200.0f;
		BASE_DAMAGE = 50;
		FIRE_RATE = 4.0f;
		BASE_SHOOTING_RADIUS = 20.0f;
		BASE_ROTATION_SPEED = 80.0f;
		FAVOURITE_ENEMY = TypeEnemy.Enemy3;
		
		base.Start();

		// Costes
		MAX_SELLING_PRICE *= PURCHASE_PRICE;

		// Estados
		initialRotation = transform.Find("Armature/Base/Support").rotation;
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
		// Rotación de la base
		Transform baseTransform = transform.Find("Armature/Base/Support");
		Quaternion newRotation = Quaternion.LookRotation((baseTransform.position - selectedEnemy.transform.position).normalized);
		newRotation = Quaternion.Euler(0, newRotation.eulerAngles.y, 0);
		newRotation *= Quaternion.Euler(0, 180, 0);

		// Rotación del cañón
		Transform cannonTransform = transform.Find("Armature/Base/Support/Cannon");
        Vector2 initialPosition = new(cannonTransform.position.x, cannonTransform.position.y);
        Vector2 enemyPosition = new(selectedEnemy.transform.position.x, selectedEnemy.transform.position.y);

        float horizontalDist = Vector2.Distance(initialPosition, enemyPosition);
        float verticalDist = selectedEnemy.transform.position.z - cannonTransform.position.z;
        float gravity = Physics.gravity.magnitude;
        float angle = Mathf.Atan2(verticalDist, Mathf.Sqrt(horizontalDist * horizontalDist + 2 * verticalDist * gravity));
		angle *= Mathf.Rad2Deg;

		RotateHead(newRotation, baseTransform);
		
		Quaternion rotation = Quaternion.Euler(0, 0, angle);
		rotation *= Quaternion.Euler(0, 0, 45);
		float rotationSpeed = BASE_ROTATION_SPEED * GameTime.DeltaTime;
		cannonTransform.rotation = Quaternion.RotateTowards(cannonTransform.rotation, rotation, rotationSpeed);

		Fire(baseTransform);
	}

	protected override void FireAnimation(Quaternion rotation)
	{

	}

	//*---------------------------------------------------------------*//
    //*--------------------------- AUXILIAR --------------------------*//
    //*---------------------------------------------------------------*//

	protected override void ActivateAnimation()
	{
		Transform childTransform = transform.Find("Armature/Base/Support");
		childTransform.rotation = Quaternion.Slerp(childTransform.rotation, initialRotation, 2.0f * GameTime.DeltaTime);

		if (Quaternion.Angle(childTransform.rotation, initialRotation) < 0.1f) base.ActivateAnimation();
	}
}