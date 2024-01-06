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
		FIRE_RATE = 5.0f;
		BASE_SHOOTING_RADIUS = 20.0f;
		BASE_ROTATION_SPEED = 60.0f;
		FAVOURITE_ENEMY = TypeEnemy.Enemy3;
		
		base.Start();

		// Costes
		MAX_SELLING_PRICE *= PURCHASE_PRICE;

		// Estados
		initialRotation = transform.Find("Armature/Base/Support").rotation;

		AnimationClip animationClip = animator.runtimeAnimatorController.animationClips[0];
		initTimer = animationClip.length * 10.0f;
	}

	//*---------------------------------------------------------------*//
    //*----------------------------- GET -----------------------------*//
    //*---------------------------------------------------------------*//

	public static int GetPurchasePrice() {
		return PURCHASE_PRICE;
	}

	protected override void Fire(Quaternion rotation) {

	}
}