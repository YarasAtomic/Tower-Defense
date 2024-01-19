using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
	private float speed = 20.0f;
	private int damage;

	[SerializeField] private GameObject explosionPrefab;
	[SerializeField] private GameObject collisionPrefab;

	public void Initialise(int damage)
	{
		this.damage = damage;
	}

    // Start is called before the first frame update
    void Start()
	{
        if (explosionPrefab != null) {
			GameObject explosionVFX = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
			explosionVFX.transform.forward = gameObject.transform.forward;
			
			if (explosionVFX.TryGetComponent<ParticleSystem>(out var particleSystem)) {
				Destroy(explosionVFX, particleSystem.main.duration);
			}
			else {
				ParticleSystem childParticleSystem = explosionVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
				Destroy(explosionVFX, childParticleSystem.main.duration);
			}
		}
    }

    // Update is called once per frame
    void Update()
	{
		float vel = speed * GameTime.DeltaTime;
        if (vel != 0) {
			transform.position += vel * transform.forward;
			CheckCollisions();
		}
    }

	private void CheckCollisions()
	{
		bool collision = false;

		Quaternion contactRotation = Quaternion.identity;
		Vector3 contactPosition = Vector3.one;

		Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.6f);
		foreach (Collider collider in hitColliders) {
			if (collider.TryGetComponent<Enemy>(out var enemy) && enemy.GetHealthPercentage() > 0f) {
				enemy.Damage(damage);

				Vector3 closestPoint = collider.ClosestPoint(transform.position);
				contactRotation = Quaternion.FromToRotation(Vector3.up, transform.position - closestPoint);
				contactPosition = closestPoint;

				collision = true;
				break;
			}

			collision = !collider.TryGetComponent<Enemy>(out _) &&
						!collider.TryGetComponent<Building>(out _) &&
						!collider.TryGetComponent<Projectile>(out _) &&
						!collider.TryGetComponent<CannonProjectile>(out _);
		}

		Debug.Log(collision);
		if (!collision) return;

		// bool ignore = collision.collider.TryGetComponent<Building>(out _) ||
		// 			  collision.collider.TryGetComponent<Projectile>(out _) ||
		// 			  collision.collider.TryGetComponent<CannonProjectile>(out _);

		// if (ignore) return;
		// if (collision.collider.TryGetComponent<Enemy>(out var enemy)) {
		// 	if (enemy.GetHealthPercentage() <= 0f) return;
		// 	else enemy.Damage(damage);
		// }

		speed = 0.0f;

		if (collisionPrefab != null) {
			GameObject collisionVFX = Instantiate(collisionPrefab, contactPosition, contactRotation);

			if (collisionVFX.TryGetComponent<ParticleSystem>(out var particleSystem)) {
				Destroy(collisionVFX, particleSystem.main.duration);
			}
			else {
				ParticleSystem childParticleSystem = collisionVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
				Destroy(collisionVFX, childParticleSystem.main.duration);
			}
		}

		Destroy(gameObject);
	}
}
