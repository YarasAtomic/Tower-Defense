using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;


// Se encarga de animar un objeto por un Spline teniendo en cuenta
// el GameTime.DeltaTime
public class SplineAnimationController : MonoBehaviour
{
    public SplineContainer spline;
    public float speed = 1f;
    public float distancePercentage = 0f;
    float splineLength;

    void Start()
    {
        splineLength = spline.CalculateLength();
    }

    const float SPLINE_EVALUATION_DELTA = 0.0005f;
    void Update()
    {
        if (GameTime.IsPaused()||distancePercentage>=1) return;

        distancePercentage += speed * GameTime.DeltaTime / splineLength;

        // Cálculo de pseudo-tangente
        Vector3 currentPosition = spline.EvaluatePosition(distancePercentage - SPLINE_EVALUATION_DELTA);
        transform.position = currentPosition;
        Vector3 nextPosition = spline.EvaluatePosition(distancePercentage + SPLINE_EVALUATION_DELTA);
        Vector3 direction = nextPosition - currentPosition;

        // Se aplica la rotación dado el vector direction
        transform.rotation = Quaternion.LookRotation(direction, transform.up);
    }

    public Vector3 GetFuturePos(float time){
        float deltaPercentage = time*speed/splineLength;
        return spline.EvaluatePosition(
            distancePercentage + deltaPercentage > 1 ? 
            distancePercentage : 
            distancePercentage + deltaPercentage
        );
    }
}