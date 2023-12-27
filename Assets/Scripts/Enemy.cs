using UnityEngine;
using UnityEngine.Splines;

public class Enemy : MonoBehaviour
{
    static int enemyCount = 0;
    [SerializeField] int MAX_HEALTH;
    [SerializeField] int health;
    [SerializeField] float speed;
    [SerializeField] int RESOURCE_DROP;
    
    int splineId = -1;
    int id;

    int otherEnemyId = -1;

    LevelLogic levelLogic;

    SplineAnimationController splineAnimationController;
    Animator animator;
    Rigidbody rb;

    GameObject parent;

    Vector3 groundOffset = new Vector3(0,0.5f,0);

    // Values

    [SerializeField] float DEATH_TIME;
    [SerializeField] float ATTACK_DELAY;
    [SerializeField] int DAMAGE;

    [SerializeField] TypeEnemy TYPE;

    // States
    bool dying = false;
    float deathTimer = 0;

    bool attacking = false;

    float attackTimer = 0;

    bool isMoving = true;

    Vector3 splineToGroundRay = Vector3.down;

    // Start is called before the first frame update
    float MAX_GROUND_RAY_VARIATION = 0.05f;
    void Start(){
        id = (enemyCount++)-1;
        health = MAX_HEALTH;
        splineAnimationController = GetSplineAnimationController();
        rb = gameObject.GetComponent<Rigidbody>();
        splineToGroundRay = new Vector3(UnityEngine.Random.Range(-MAX_GROUND_RAY_VARIATION,MAX_GROUND_RAY_VARIATION),-1,UnityEngine.Random.Range(-MAX_GROUND_RAY_VARIATION,MAX_GROUND_RAY_VARIATION));
        animator = gameObject.GetComponentInChildren<Animator>();
        Debug.Log(animator);
        animator.SetFloat("idleBlend",UnityEngine.Random.value);
    }


    // Update is called once per frame
    void Update(){

        splineAnimationController.speed = isMoving ? speed : 0;
        animator.speed = GameTime.GameSpeed;

        if (health <= 0) {
            HandleDeath();
            return;
        }

        animator.SetBool("attacking",attacking);
        UpdatePos();

        RaycastHit hit;
        
        if(UpdateRaycast(out hit)){
            Enemy otherEnemy = hit.collider.gameObject.GetComponent<Enemy>();
            Building otherBuilding = hit.collider.gameObject.GetComponent<Building>();
            if(otherEnemy!=null){
                HandleCollisionWithEnemy(otherEnemy);
            }else if(otherBuilding!=null){
                HandleCollisionWithBuilding(otherBuilding);

            }
        }else if(splineAnimationController.distancePercentage < 1){
            attacking = false;
            isMoving = true;
            animator.SetBool("moving",true);
        }else{
            animator.SetBool("moving",false);
        }
        
    }

    private void HandleDeath() {
        if(deathTimer == 0 && !dying){
            Die();
        }

        deathTimer += GameTime.DeltaTime;
    }

    // Executes one time when health reaches 0
    private void Die() {
        enemyCount--;
        // Dies
        levelLogic.AddResources(10);
        // Remove animation and activate physics
        animator.SetBool("alive",false);
        dying = true;
        // splineAnimationController.Pause();
        isMoving = false;
        rb.isKinematic = false;

        // Apply force
        rb.AddForce(new Vector3(UnityEngine.Random.Range(-1,1),UnityEngine.Random.Range(400,600),UnityEngine.Random.Range(-1,1)));
        float maxTorque = 10000;
        rb.AddTorque(new Vector3(UnityEngine.Random.Range(-maxTorque,maxTorque),UnityEngine.Random.Range(-maxTorque,maxTorque),UnityEngine.Random.Range(-maxTorque,maxTorque)));

        // Deactivate collisions
        rb.excludeLayers = LayerMask.GetMask("Enemy");
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    private void HandleCollisionWithEnemy(Enemy otherEnemy) {
        attacking = false;
        if(otherEnemy.GetOtherEnemyId()!=id){
            // splineAnimationController.Pause();
            isMoving = false;
            animator.SetBool("moving",false);
            otherEnemyId = otherEnemy.GetId();
        }else{
            //splineAnimationController.Play();
            isMoving = true;
            animator.SetBool("moving",true);
        }
        // ! esto permite evitar bloqueos binarios (A para por B y B para por A)
        // ! pero bloqueos ternarios y superiores no (A para por B, B por C y C por A)
    }

    private void HandleCollisionWithBuilding(Building otherBuilding) {
        // splineAnimationController.Pause();
        isMoving = false;
        animator.SetBool("moving",false);
        if(attacking==false){
            attacking=true;
            attackTimer = 0;
        }
        attackTimer+=GameTime.DeltaTime;
        if(attackTimer>ATTACK_DELAY){
            attackTimer = 0;
            otherBuilding.DamageBuilding(DAMAGE);
        }
    }


    public bool IsMoving(){
        return isMoving;
    }

    public void Initialise(SplineContainer spline, int pathId,GameObject guide, LevelLogic levelLogic){
        parent = guide;
        splineAnimationController = GetSplineAnimationController(); //esto se coloca aqui porque Start() no ocurre tras la Instanciacion
        splineAnimationController.spline = spline;
        splineId = pathId;
        splineAnimationController.enabled = true;
        this.levelLogic = levelLogic;
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

    public TypeEnemy GetTypeEnemy(){
        return TYPE;
    }

    public void Damage(int dmg){
        health-=dmg;
    }

    // ! Esto seguramente se quite, es temporal
    public SplineAnimationController GetSplineAnimationController(){
        SplineAnimationController anim = parent.GetComponent<SplineAnimationController>();
        return anim;
    }

    float rayLength = 15;
    float speedRotation = 1;
    void UpdatePos(){
        RaycastHit hit ;
        if(Utils.Raycast(parent.transform.position,splineToGroundRay,rayLength,LayerMask.GetMask("Terrain"),out hit)){
            gameObject.transform.position = hit.point + groundOffset;
            // Hit normal
            Debug.DrawRay(gameObject.transform.position,hit.normal);
            Utils.DrawLocator(hit.point);

            // Update rotation
            var targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, GameTime.DeltaTime * speedRotation);
        }
    }

    public static int GetCount(){
        return enemyCount;
    }

    const float FRONT_RAY_LENGTH = 1f;
    const float RAY_DELTA = 0.5f;
    private bool UpdateRaycast(out RaycastHit raycastHit){
        
        float i=-1;

        do {
            if(Utils.Raycast(
                    transform.position,
                    transform.TransformDirection(new Vector3(i,0,1).normalized),
                    FRONT_RAY_LENGTH,
                    LayerMask.GetMask("Enemy","Building"),
                    out raycastHit)){
                return true;
            }
            i+=RAY_DELTA;
        } while(i <=1);

        return false;
    }
}
