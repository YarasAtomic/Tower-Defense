using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class Enemy : MonoBehaviour
{
    static int lastId = -1;
    [SerializeField] int MAX_HEALTH;
    [SerializeField] int health;
    
    int splineId = -1;
    int id;

    int otherEnemyId = -1;
    SplineAnimate animate;

    [SerializeField] int type;

    // States
    bool dying = false;
    float deathTimer = 0;
    [SerializeField] float DEATH_TIME;

    bool attacking = false;

    float attackTimer = 0;

    [SerializeField] float ATTACK_DELAY;

    [SerializeField] int DAMAGE;

    Rigidbody rb;
    // Start is called before the first frame update
    void Start(){
        id = lastId++;
        health = MAX_HEALTH;
        animate = gameObject.GetComponent<SplineAnimate>();
        rb = gameObject.GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    void Update(){
        if(health > 0){
            RaycastHit hit;
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward), Color.yellow);
            
            if(Physics.Raycast(transform.position,transform.TransformDirection(Vector3.forward),out hit,1)){
                Enemy otherEnemyObject = hit.collider.gameObject.GetComponent<Enemy>();
                Building otherBuildingObject = hit.collider.gameObject.GetComponent<Building>();
                if(otherEnemyObject!=null){
                    attacking = false;
                    if(otherEnemyObject.GetOtherEnemyId()!=id){
                        animate.Pause();
                        otherEnemyId = otherEnemyObject.GetId();
                    }else{
                        animate.Play();
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

            }else{
                attacking = false;
                animate.Play();
            }
        }else{
            if(deathTimer == 0){
                dying = true;
                animate.Pause();
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
        return animate.IsPlaying;
    }

    public void Initialise(SplineContainer spline, int pathId){
        animate = gameObject.GetComponent<SplineAnimate>(); //esto se coloca aqui porque Start() no ocurre tras la Instanciacion
        animate.Container = spline;
        splineId = pathId;
        animate.enabled = true;
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
        return type;
    }

    public void Damage(int dmg){
        health-=dmg;
    }
}
