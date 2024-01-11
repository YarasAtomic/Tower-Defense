using UnityEngine;

public class BulletParabola
{
	public Vector3 origin;
	public Vector3 end;
	
	public static readonly float gravity = -Physics.gravity.magnitude;
	public static readonly float maxHeight = 4f;

	public Vector3 hDiff;
	public Vector3 hDir;

	public float hDistance;
	public float height;
	public float slope;
	public float curve;
	public float velocity;

	public Vector3 fireDirection;
}