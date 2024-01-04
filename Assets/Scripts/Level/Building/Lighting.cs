using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Lighting : MonoBehaviour
{
    LineRenderer lineRenderer;

    int vertexCount = 10;
    [SerializeField] Vector3 origin;
    [SerializeField] Vector3 end;
    float lightDelayTimer = 0;
    [SerializeField] float LIGHT_DELAY = 0.01f;
    [SerializeField] float LIGHT_MAX_RANDOM = 1.0f;
    // Start is called before the first frame update
    void Start() {
        lineRenderer = GetComponent<LineRenderer>();
        UpdatePos();
    }

    void Initialise(Vector3 o,Vector3 e){
        origin = o;
        end = e;
    }

    void UpdatePos(){
        lineRenderer.positionCount = vertexCount;
        for(int i = 0 ; i < vertexCount; i++){
            Vector3 vertexPos = (end-origin) / vertexCount * i + origin + Random.insideUnitSphere * LIGHT_MAX_RANDOM;
            lineRenderer.SetPosition(i,vertexPos);
        }
    }

    // Update is called once per frame
    void Update() {
        if(lightDelayTimer>LIGHT_DELAY){
            lightDelayTimer-=LIGHT_DELAY;
            UpdatePos();
        }
        lightDelayTimer += GameTime.DeltaTime;
    }


}
