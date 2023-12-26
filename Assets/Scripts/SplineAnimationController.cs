using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineAnimationController: MonoBehaviour
{
    public SplineContainer spline;
    public float speed = 1f;
    public float distancePercentage = 0f;

    float splineLength;

    void Start()
    {
        splineLength = spline.CalculateLength();
    }

    void Update()
    {
        if (GameTime.GameSpeed == 0) return;

        distancePercentage += speed * GameTime.DeltaTime / splineLength;

        Vector3 currentPosition = spline.EvaluatePosition(distancePercentage);
        transform.position = currentPosition;

        Vector3 nextPosition = spline.EvaluatePosition(distancePercentage + 0.0005f);
        Vector3 direction = nextPosition - currentPosition;
        transform.rotation = Quaternion.LookRotation(direction, transform.up);
    }
}