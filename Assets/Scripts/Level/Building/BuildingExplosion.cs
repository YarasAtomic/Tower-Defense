using UnityEngine;

public class BuildingExplosion : MonoBehaviour
{
	void Start()
	{
		Explode();
	}

	private void Explode()
	{
		foreach (Transform t in transform) {
			if (t.TryGetComponent<Rigidbody>(out var rigidbody)) {
				Debug.Log("explosion");
				rigidbody.AddExplosionForce(Random.Range(125, 750), transform.position, 5);
			}
		}
	}
}