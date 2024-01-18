using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CannonProjectile : MonoBehaviour
{
	private BulletParabola bulletParabola;
	private int damage;
	private float time = 0f;

	[SerializeField] private GameObject explosionPrefab;
	[SerializeField] private GameObject collisionPrefab;

	public void Initialise(BulletParabola bulletParabola, int damage)
	{
		this.bulletParabola = bulletParabola;
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
		Vector3 currentPos = PosAt(time);
		Vector3 nextPos = PosAt(time + 0.01f);
		Vector3 direction = (nextPos - currentPos).normalized;

		transform.SetPositionAndRotation(currentPos, Quaternion.LookRotation(direction));
		time += GameTime.DeltaTime;
    }

	Vector3 PosAt(float t)
	{
        float hPos = bulletParabola.velocity * t;
        return bulletParabola.origin + hPos*bulletParabola.hDir + (bulletParabola.curve*hPos*hPos + bulletParabola.slope*hPos) * Vector3.up;
    }

	void OnCollisionEnter (Collision collision)
	{
		if (collision.collider.TryGetComponent<Building>(out _)) return;
		if (collision.collider.TryGetComponent<Enemy>(out var enemy)) {
			if (enemy.GetHealthPercentage() <= 0f) return;
			else enemy.Damage(damage);
		}
		
		bulletParabola.velocity = 0.0f;

		Quaternion contactRotation = Quaternion.FromToRotation(Vector3.up, collision.contacts[0].normal);
		Vector3 contactPosition = collision.contacts[0].point;

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
