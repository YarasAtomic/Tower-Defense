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
		BASE_ROTATION_SPEED = 120.0f;
		FAVOURITE_ENEMY = TypeEnemy.Enemy1;
		
		base.Start();

		// Costes
		MAX_SELLING_PRICE *= PURCHASE_PRICE;

		// Proyectil
		firePointLeft = transform.Find("Armature/MainBody/NeckLow/NeckUp/Head/Head_L/Cannon_L/Cannon_L_end").gameObject;
		Vector3 position = firePointLeft.transform.position;
		position.x += 0.8f;
		firePointLeft.transform.position = position;

		firePointRight = transform.Find("Armature/MainBody/NeckLow/NeckUp/Head/Head_R/Cannon_R/Cannon_R_end").gameObject;
		position = firePointRight.transform.position;
		position.x += 0.8f;
		firePointRight.transform.position = position;

		// Estados
		Transform initialTransform = transform.Find("Armature/MainBody/NeckLow/NeckUp/Head");
		firePosition = initialTransform.position;
		initialRotation = initialTransform.rotation;

		AnimationClip animationClip = animator.runtimeAnimatorController.animationClips[1];
		initTimer = animationClip.length * 10.0f;
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
		patrolling = false;
		
		Transform childTransform = transform.Find("Armature/MainBody/NeckLow/NeckUp/Head");
		
		RotateHead(childTransform);
		Fire(childTransform);
	}

	//-----------------------------------------------------------------//

	protected override void RotateHead(Transform childTransform)
	{
		Quaternion newRotation = Quaternion.LookRotation((childTransform.position - selectedEnemy.transform.position).normalized);
		newRotation *= Quaternion.Euler(0, 180, 0);

		float rotationSpeed = BASE_ROTATION_SPEED * GameTime.DeltaTime;

		if (!firing) {
			animator.enabled = false;

			float angle = Quaternion.Angle(childTransform.rotation, newRotation);
			float initialFireTimer = (angle*Mathf.Deg2Rad / rotationSpeed) + 0.25f;
			if (initialFireTimer > (FIRE_RATE - fireTimer)) fireTimer = FIRE_RATE - initialFireTimer;

			firing = true;
		}
		childTransform.rotation = Quaternion.RotateTowards(childTransform.rotation, newRotation, rotationSpeed);
	}

	protected override void FireAnimation(Quaternion rotation)
	{
		GameObject initialPosition = firePoint ? firePointLeft : firePointRight;

		if (initialPosition != null) {
			GameObject vfx = Instantiate(effectToSpawn, initialPosition.transform.position, Quaternion.identity);
			vfx.GetComponent<Projectile>().Initialise((int) damage);
			
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