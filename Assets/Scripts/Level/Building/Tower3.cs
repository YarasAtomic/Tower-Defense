using Unity.VisualScripting;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class Tower3 : Tower
{
	// CONST attributes
	private static readonly int PURCHASE_PRICE = 200;

	// STATE attributes
	private Quaternion initialCannonRotation;
	private BulletParabola bulletParabola;

	//*---------------------------------------------------------------*//
    //*---------------------------- START ----------------------------*//
    //*---------------------------------------------------------------*//
	
	protected override void Start()
	{
		// Constantes
		BASE_HP = 200.0f;
		BASE_DAMAGE = 50;
		FIRE_RATE = 4.0f;
		BASE_SHOOTING_RADIUS = 25.0f;
		BASE_ROTATION_SPEED = 80.0f;
		FAVOURITE_ENEMY = TypeEnemy.Enemy3;
		
		base.Start();

		// Costes
		MAX_SELLING_PRICE *= PURCHASE_PRICE;

		// Estados
		initialRotation = transform.Find("Armature/Base/Support").rotation;
		initialCannonRotation = transform.Find("Armature/Base/Support/Cannon").localRotation;
		initialCannonRotation *= Quaternion.Euler(0, 0, 35);

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

		// Rotación del cañón (cálculo de la parábola)
		Vector3 fireDirection = FromTo(cannonTransform.position, selectedEnemy.transform.position);
		float cannonAngle = Vector3.Angle(new Vector3(fireDirection.x, 0, fireDirection.z).normalized, fireDirection);
		Quaternion rotation = Quaternion.Euler(0, cannonTransform.localRotation.eulerAngles.y, cannonAngle) * Quaternion.Euler(0, 0, -90);
		
		childTransform.rotation = Quaternion.RotateTowards(childTransform.rotation, newRotation, rotationSpeed);
		cannonTransform.localRotation = Quaternion.RotateTowards(cannonTransform.localRotation, rotation, rotationSpeed);
	}

	protected override void FireAnimation(Quaternion rotation)
	{
		Vector3 origin = new(bulletParabola.origin.x, bulletParabola.origin.y + 2, bulletParabola.origin.z);

		CannonProjectile vfx = Instantiate(effectToSpawn, origin, Quaternion.identity).GetComponent<CannonProjectile>();
		vfx.Initialise(bulletParabola);
	}

	//*---------------------------------------------------------------*//
    //*--------------------------- AUXILIAR --------------------------*//
    //*---------------------------------------------------------------*//

	private Vector3 FromTo(Vector3 origin, Vector3 end){
		bulletParabola = new();

		bulletParabola.origin = origin;
		bulletParabola.end = end;

		bulletParabola.hDiff = new(end.x - origin.x, 0, end.z - origin.z);
		bulletParabola.hDir = bulletParabola.hDiff.normalized;

        bulletParabola.hDistance = bulletParabola.hDiff.magnitude;
        bulletParabola.height = end.y - origin.y;
        bulletParabola.curve = (float) (
			bulletParabola.height -
			2 * BulletParabola.maxHeight -
			2 * Mathf.Sqrt(
				BulletParabola.maxHeight*BulletParabola.maxHeight -
				BulletParabola.maxHeight * bulletParabola.height
			)
		) /
		(bulletParabola.hDistance*bulletParabola.hDistance);
        bulletParabola.slope = bulletParabola.height / bulletParabola.hDistance - bulletParabola.curve * bulletParabola.hDistance;
        bulletParabola.velocity = (float) Mathf.Sqrt(BulletParabola.gravity/bulletParabola.curve);

        return (bulletParabola.hDir + Vector3.up * bulletParabola.slope).normalized;
    }

	protected override void ActivateAnimation()
	{
		Transform childTransform = transform.Find("Armature/Base/Support");
		Transform cannonTransform = transform.Find("Armature/Base/Support/Cannon");

		childTransform.rotation = Quaternion.Slerp(childTransform.rotation, initialRotation, 2.0f * GameTime.DeltaTime);
		cannonTransform.localRotation = Quaternion.Slerp(cannonTransform.localRotation, initialCannonRotation, 2.0f * GameTime.DeltaTime);

		float childAngle = Quaternion.Angle(childTransform.rotation, initialRotation);
		float cannonAngle = Quaternion.Angle(cannonTransform.localRotation, initialRotation);

		if (childAngle < 0.001f && cannonAngle > 179.999f) base.ActivateAnimation();
	}
}