using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class enemy : MonoBehaviour
{
    [SerializeField] int MAX_HEALTH;
    [SerializeField] int health;

    int splineId = -1;

    SplineAnimate animate;

    // Start is called before the first frame update
    void Start()
    {
        health = MAX_HEALTH;
        animate = gameObject.GetComponent<SplineAnimate>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward), Color.yellow);
        
        if(Physics.Raycast(transform.position,transform.TransformDirection(Vector3.forward),out hit,1)){
            if(hit.collider.gameObject.GetComponent<enemy>().getSplineId()==gameObject.GetComponent<enemy>().getSplineId()){
                animate.Pause();
                Debug.Log("mismo spline");
            }
            else if(hit.collider.gameObject.GetComponent<enemy>().isMoving()){
                animate.Pause();
                Debug.Log("otro spline y el otro se mueve");
            }else{
                animate.Play();
                Debug.Log("otro");
            }
            
        }else{
            animate.Play();
        }
    }

    public bool isMoving()
    {
        return animate.IsPlaying;
    }

    public void setSplineId(int id){
        splineId = id;
    }

    public int getSplineId(){
        return splineId;
    }

    // void OnCollisionStay(Collision collision)
    // {
    //     if(collision.gameObject.GetComponent<enemy>()!=null)
    //     {
    //         Debug.Log("Enemigo");
    //         SplineContainer splineContainer = collision.gameObject.GetComponent<SplineContainer>();
    //     }
    // }
    
}
