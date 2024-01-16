using System;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] Vector3 origin;
    [SerializeField] Vector3 end;
    [SerializeField] float gravity = -0.1f; // < 0
    [SerializeField]float time = 0;
    [SerializeField]float param = -10; // < 0
    [SerializeField] float maxHeight;
    float hDistance;
    float slope;
    float vSpeed;
    float totalTime;
    float velocity;
    float height;
    Vector3 hDiff;

    void Start()
    {
        FromTo(origin,end);
    }
    Vector3 PosAt(float t){
        float hPos = velocity * t;
        return origin + hPos * hDiff.normalized + ( gravity * hPos * hPos + (height/hDistance - gravity * hDistance) * hPos) * Vector3.up;
    }
    public Vector3 FromTo(Vector3 origin,Vector3 end){
        height = end.y - origin.y;
        transform.position =  origin;
        hDiff = new Vector3(end.x - origin.x , 0 , end.z-origin.z);
        hDistance = hDiff.magnitude;
        slope = height / hDistance - gravity * hDistance;
        vSpeed = (float)Math.Sqrt(param * hDistance * hDistance * slope * slope / ( - hDistance * slope + height));
        totalTime = hDistance * slope / vSpeed;
        velocity = hDistance / totalTime;
        this.origin = origin;
        this.end = end;

        return new Vector3(hDiff.x*velocity,height/hDistance - gravity*hDistance,hDiff.z*velocity);
    }

    void Update()
    {
        transform.position = PosAt(time);
        time += GameTime.DeltaTime;
        if(time > totalTime){
            FromTo(origin,end);
            time = 0;
        }
    }
}
