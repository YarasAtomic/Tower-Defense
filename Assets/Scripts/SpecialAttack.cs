using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialAttack : MonoBehaviour
{   
    protected Camera mainCamera;
    [SerializeField] static protected float TIME_TO_EXPLODE = 1;
    [SerializeField] static protected float TIME_TO_VANISH = 5f;
    protected float deployTimer;

    protected bool deployed;
    protected bool exploding;
    protected bool exploded;

    // Start is called before the first frame update
    public void Start()
    {
        deployed = false;
        exploding = false;
        exploded = false;

        deployTimer = 0f;
    }

    // Update is called once per frame
    public void Update()
    {
        Ray ray = mainCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(!Physics.Raycast(ray,out hit,100,LayerMask.GetMask("Terrain"))) return;
        
        if (!deployed) {
            transform.position = hit.point;
        }
        else {
            deployTimer += GameTime.DeltaTime;
        }

        if (deployTimer >= TIME_TO_VANISH){
            // Aquí ya no hay ni onda expansiva.
            Destroy(this.gameObject);
            return;
        }

        if (deployTimer >= TIME_TO_EXPLODE && !exploded) {
            ExecuteAttack();
            exploded = true;
        }
    }

    protected abstract void ExecuteAttack();

    public void Deploy() {
        deployed = true;
    }

    public void Initialise(Camera mainCamera){
        this.mainCamera = mainCamera;
    }
}
