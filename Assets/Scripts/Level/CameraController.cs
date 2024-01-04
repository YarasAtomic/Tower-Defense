using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    float shake = 0;
    float DAMPENER = 2;

    // Update is called once per frame
    void Update()
    {
        if(shake>0){
            transform.localPosition = Random.insideUnitCircle * shake;
            shake -= GameTime.DeltaTime * DAMPENER;
        }else{
            transform.localPosition = Vector3.zero;
        }
    }

    public void SetShake(float shake){
        this.shake = this.shake > shake ? this.shake : shake;
    }
}
