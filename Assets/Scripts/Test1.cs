using System;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    [SerializeField] Vector3 origin; // origin.xy != end.xy
    [SerializeField] Vector3 end;
    [SerializeField] float gravity = -0.1f; // < 0
    [SerializeField] float maxHeight;
    [SerializeField] float time;
    // [SerializeField] float totalTime;
    Vector3 hDiff;
    Vector3 hDir;
    float hDistance;
    float height;
    float slope;
    float curve;
    float velocity;
    Vector3 startDir;
    void Start()
    {
        startDir = FromTo(origin,end);
    }
    Vector3 PosAt(float t){

        float hPos = velocity * t;
        return origin + hPos * hDir + ( curve * hPos * hPos + slope * hPos) * Vector3.up;
    }
    public Vector3 FromTo(Vector3 origin,Vector3 end){
        hDiff = new Vector3(end.x - origin.x , 0 , end.z-origin.z);
        hDir = hDiff.normalized;
        hDistance = hDiff.magnitude;
        height = end.y - origin.y;
        curve = (float)(height - 2 * maxHeight - 2 * Math.Sqrt(maxHeight*maxHeight - maxHeight * height))/(hDistance*hDistance);
        slope = height / hDistance - curve * hDistance;
        velocity = (float)Math.Sqrt(gravity/curve);
        this.origin = origin;
        this.end = end;
        return (hDir + Vector3.up * slope).normalized;
    }

    
    void Update()
    {
        Debug.DrawRay(origin,startDir);
        transform.position = PosAt(time);
        time+=GameTime.DeltaTime;
        // if(time > totalTime){
        //     time = 0;
        //     startDir = FromTo(origin,end);
        // }
    }
}
