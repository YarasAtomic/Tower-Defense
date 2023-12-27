using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialAttack : MonoBehaviour
{   
    protected float RADIUS;
    protected int DAMAGE;
    protected float COOLDOWN;
    protected Camera mainCamera;
    protected float TIME_TO_EXPLODE;
    protected float TIME_TO_VANISH;
    protected float deployTimer;

    protected bool deployed;
    protected bool exploding;

    // Start is called before the first frame update
    public void Start()
    {
        deployed = false;
        exploding = false;
        TIME_TO_EXPLODE = 1f;
        TIME_TO_VANISH = 1.25f;
        deployTimer = 0f;
        RADIUS = 5;
        DAMAGE = 10; // jaja. my sanity is slipping from my soul.
        transform.localScale = new Vector3(RADIUS*2,1,RADIUS*2);
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

        if (deployTimer >= TIME_TO_EXPLODE) {
            ExecuteAttack();
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
