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

		initTimer = 2.0f;
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
		Transform baseTransform = transform.Find("Armature/Base/Support");
		
		RotateHead(baseTransform);
		Fire(baseTransform);
	}

	protected override void RotateHead(Transform childTransform)
	{
		Transform cannonTransform = transform.Find("Armature/Base/Support/Cannon");

		float rotationSpeed = BASE_ROTATION_SPEED * GameTime.DeltaTime;
		
		// Rotación de la base
		Quaternion newRotation = Quaternion.LookRotation((childTransform.position - selectedEnemy.transform.position).normalized);
		newRotation = Quaternion.Euler(0, newRotation.eulerAngles.y, 0);
		newRotation *= Quaternion.Euler(0, 180, 0);

		if (!firing) {
			animator.enabled = false;

			float angle = Quaternion.Angle(childTransform.rotation, newRotation);
			fireTimer = FIRE_RATE - (angle*Mathf.Deg2Rad / rotationSpeed) - 0.25f;
			if (fireTimer > 0) fireTimer = 0.0f;

			firing = true;
		}
		// childTransform.rotation = Quaternion.RotateTowards(childTransform.rotation, newRotation, rotationSpeed);

		// Rotación del cañón
		float gravity = Physics.gravity.magnitude;
		float initialVelocity = 100.0f;
		
		Vector3 direction = selectedEnemy.transform.position - cannonTransform.position;
		float y = direction.y;
		direction.y = 0.0f;
		float x = direction.magnitude;
		
		float powVel = initialVelocity * initialVelocity;
		float rootVal = powVel*powVel - gravity * (gravity*x*x + 2*y*powVel);

		if (rootVal >= 0f) {
			float root = Mathf.Sqrt(rootVal);

			float highAngle = powVel + root;
			float lowAngle = powVel - root;
			
			float cannonAngle = Mathf.Atan2(highAngle, gravity * x) * Mathf.Rad2Deg;
			Quaternion rotation = Quaternion.Euler(0, 0, 360 - cannonAngle);
			// rotation *= Quaternion.Euler(0, 0, 45);
			
			// Quaternion initialRotation = cannonTransform.rotation;
			// initialRotation.y = newRotation.y;
			// cannonTransform.rotation = initialRotation;
			cannonTransform.rotation = Quaternion.RotateTowards(cannonTransform.rotation, rotation, rotationSpeed);
		}
		
		childTransform.rotation = Quaternion.RotateTowards(childTransform.rotation, newRotation, rotationSpeed);
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