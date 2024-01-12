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
        if (speed != 0) {
			transform.position += GameTime.DeltaTime * speed * transform.forward;
		}
    }

	void OnCollisionEnter (Collision collision)
	{
		if (collision.collider.TryGetComponent<Building>(out _)) return;
		if (collision.collider.TryGetComponent<Enemy>(out var enemy)) enemy.Damage(damage);

		speed = 0.0f;

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
