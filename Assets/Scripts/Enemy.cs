using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class Enemy : MonoBehaviour
{
    static int enemyCount = 0;
    [SerializeField] int MAX_HEALTH;
    [SerializeField] int health;
    
    int splineId = -1;
    int id;

    int otherEnemyId = -1;
    SplineAnimate splineAnimate;
    Animator animator;
    Rigidbody rb;

    GameObject parent;

    Vector3 groundOffset = new Vector3(0,0.5f,0);

    // Values

    [SerializeField] int TYPE;
    [SerializeField] float DEATH_TIME;
    [SerializeField] float ATTACK_DELAY;
    [SerializeField] int DAMAGE;


    // States
    bool dying = false;
    float deathTimer = 0;

    bool attacking = false;

    float attackTimer = 0;

    Vector3 splineToGroundRay = Vector3.down;

    // Start is called before the first frame update
    float MAX_GROUND_RAY_VARIATION = 0.05f;
    void Start(){
        id = (enemyCount++)-1;
        health = MAX_HEALTH;
        splineAnimate = GetSplineAnimate();
        rb = gameObject.GetComponent<Rigidbody>();
        splineToGroundRay = new Vector3(Random.Range(-MAX_GROUND_RAY_VARIATION,MAX_GROUND_RAY_VARIATION),-1,Random.Range(-MAX_GROUND_RAY_VARIATION,MAX_GROUND_RAY_VARIATION));
        animator = gameObject.GetComponentInChildren<Animator>();

        animator.SetFloat("idleBlend",Random.value);
    }


    // Update is called once per frame
    void Update(){
        if(health > 0){
            UpdatePos();

            RaycastHit hit;
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward), Color.red);
            
            if(Physics.Raycast(transform.position,transform.TransformDirection(Vector3.forward),out hit,1)){
                Enemy otherEnemyObject = hit.collider.gameObject.GetComponent<Enemy>();
                Building otherBuildingObject = hit.collider.gameObject.GetComponent<Building>();
                if(otherEnemyObject!=null){
                    attacking = false;
                    if(otherEnemyObject.GetOtherEnemyId()!=id){
                        splineAnimate.Pause();
                        animator.SetBool("moving",false);
                        otherEnemyId = otherEnemyObject.GetId();
                    }else{
                        splineAnimate.Play();
                        animator.SetBool("moving",true);
                    }
                    // ! esto permite evitar bloqueos binarios (A para por B y B para por A)
                    // ! pero bloqueos ternarios y superiores no (A para por B, B por C y C por A)
                }else if(otherBuildingObject!=null){
                    if(attacking==false){
                        attacking=true;
                        attackTimer = 0;
                    }
                    attackTimer+=Time.deltaTime;
                    if(attackTimer>ATTACK_DELAY){
                        attackTimer = 0;
                        otherBuildingObject.Damage(DAMAGE);
                    }
                }

            }else if(splineAnimate.elapsedTime<splineAnimate.duration){
                attacking = false;
                splineAnimate.Play();
                animator.SetBool("moving",true);
            }else{
                animator.SetBool("moving",false);
            }
            
        }else{
            if(deathTimer == 0){
                // Dies

                animator.SetBool("alive",false);
                dying = true;
                splineAnimate.Pause();
                rb.isKinematic = false;

                // Apply force
                rb.AddForce(new Vector3(Random.Range(-1,1),Random.Range(400,600),Random.Range(-1,1)));
                float maxTorque = 10000;
                rb.AddTorque(new Vector3(Random.Range(-maxTorque,maxTorque),Random.Range(-maxTorque,maxTorque),Random.Range(-maxTorque,maxTorque)));

                // Deactivate collisions
                rb.excludeLayers = LayerMask.GetMask("Enemy");
                gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            }

            deathTimer += Time.deltaTime;
            if(deathTimer>DEATH_TIME){
                Die();
            }
        }
    }

    private void Die(){
        // ? no se si hace falta
    }

    public bool IsMoving(){
        return splineAnimate.IsPlaying;
    }

    public void Initialise(SplineContainer spline, int pathId,GameObject guide){
        parent = guide;
        splineAnimate = GetSplineAnimate(); //esto se coloca aqui porque Start() no ocurre tras la Instanciacion
        splineAnimate.Container = spline;
        splineId = pathId;
        splineAnimate.enabled = true;
    }

    public int GetSplineId(){
        return splineId;
    }

    public int GetId(){
        return id;
    }

    public int GetOtherEnemyId(){
        return otherEnemyId;
    }

    public int GetEnemyType(){
        return TYPE;
    }

    public void Damage(int dmg){
        health-=dmg;
    }

    // ! Esto seguramente se quite, es temporal
    public SplineAnimate GetSplineAnimate(){
        SplineAnimate anim = parent.GetComponent<SplineAnimate>();
        return anim;
    }

    float rayLength = 15;
    void UpdatePos(){
        RaycastHit hit ;
        if(Utils.Raycast(parent.transform.position,splineToGroundRay,rayLength,LayerMask.GetMask("Terrain"),out hit)){
            gameObject.transform.position = hit.point + groundOffset;
            Utils.DrawLocator(hit.point);
        }

        
        // if(Physics.Raycast(parent.transform.position,parent.transform.TransformDirection(Vector3.down),out hit,rayLength,LayerMask.GetMask("Terrain"))){
        //     Debug.DrawRay(parent.transform.position, parent.transform.TransformDirection(Vector3.down*rayLength), Color.yellow);

        // }else{
        //     Debug.DrawRay(parent.transform.position, parent.transform.TransformDirection(Vector3.down*rayLength), Color.blue);
        // }
        //
    }

    static int GetCount(){
        return enemyCount;
    }
}
