using UnityEngine;
using UnityEngine.Splines;

public class Enemy : MonoBehaviour
{
    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------- CLASS ATTRIBUTES -----------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//

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

    [SerializeField] TypeEnemy TYPE;
    [SerializeField] float DEATH_TIME;
    [SerializeField] float ATTACK_DELAY;
    [SerializeField] int DAMAGE;

    // States
    bool dying = false;
    float deathTimer = 0;
    bool attacking = false;
    float attackTimer = 0;
    bool isMoving = true;

    Vector3 splineToGroundRay = Vector3.down;

    // Start is called before the first frame update
    const float MAX_GROUND_RAY_VARIATION = 0.05f;

    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//
    //!------------------------ CLASS METHODS ------------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//

    //*---------------------------------------------------------------*//
    //*-------------------------- INITIALISE -------------------------*//
    //*---------------------------------------------------------------*//

    public void Initialise(SplineContainer spline, int pathId,GameObject guide, 
                           LevelLogic levelLogic){
        parent = guide;
        //esto se coloca aqui porque Start() no ocurre tras la Instanciacion
        splineAnimationController = GetSplineAnimationController(); 
        splineAnimationController.spline = spline;
        splineId = pathId;
        splineAnimationController.enabled = true;
        this.levelLogic = levelLogic;
    }

    //*---------------------------------------------------------------*//
    //*---------------------------- START ----------------------------*//
    //*---------------------------------------------------------------*//

    void Start(){
        id = (enemyCount++)-1;
        health = MAX_HEALTH;
        splineAnimationController = GetSplineAnimationController();
        rb = gameObject.GetComponent<Rigidbody>();

        float randomVariation = UnityEngine.Random.Range(
            -MAX_GROUND_RAY_VARIATION,
            +MAX_GROUND_RAY_VARIATION
        );
        splineToGroundRay = new Vector3(randomVariation,-1,randomVariation);

        animator = gameObject.GetComponentInChildren<Animator>();
        animator.SetFloat("idleBlend",UnityEngine.Random.value);
    }


    //*---------------------------------------------------------------*//
    //*---------------------------- UPDATE ---------------------------*//
    //*---------------------------------------------------------------*//

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
            }else if(otherBuilding!=null && hit.collider is BoxCollider){ //TODO lo ideal sería hacer un overlap sphere
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

    //-----------------------------------------------------------------//

    const float FRONT_RAY_LENGTH = 1f;
    const float RAY_DELTA = 0.5f;

    private bool UpdateRaycast(out RaycastHit raycastHit){
        float i=-1;
        do {
            bool raycast = Utils.Raycast(
                transform.position,
                transform.TransformDirection(new Vector3(i,0,1).normalized),
                FRONT_RAY_LENGTH,
                LayerMask.GetMask("Enemy","Building"),
                out raycastHit
            );
            if(raycast) return true;
            i+=RAY_DELTA;
        } while(i <=1);

        return false;
    }

    //-----------------------------------------------------------------//

    const float RAY_LENGTH = 15f;
    const float SPEED_ROTATION = 1f;

    void UpdatePos(){
        RaycastHit hit;
        bool raycast = Utils.Raycast(
                parent.transform.position, splineToGroundRay,
                RAY_LENGTH, LayerMask.GetMask("Terrain"),
                out hit
        );

        if(!raycast) return;
        
        gameObject.transform.position = hit.point + groundOffset;
        // Hit normal
        Debug.DrawRay(gameObject.transform.position,hit.normal);
        Utils.DrawLocator(hit.point);

        // Update rotation
        var targetRotation = 
            Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

        transform.rotation = Quaternion.Slerp(
            transform.rotation, 
            targetRotation, 
            GameTime.DeltaTime * SPEED_ROTATION
        );
    }

    //*---------------------------------------------------------------*//
    //*------------------------ DEATH HANDLER ------------------------*//
    //*---------------------------------------------------------------*//

    private void HandleDeath() {
        if(deathTimer == 0 && !dying){
            Die();
        }
        deathTimer += GameTime.DeltaTime;
    }

    //-----------------------------------------------------------------//

    private void Die() {
        enemyCount--;
        // Dies
        levelLogic.AddResources(RESOURCE_DROP);
        // Remove animation and activate physics
        animator.SetBool("alive",false);
        dying          = true;
        isMoving       = false;
        rb.isKinematic = false;

        // Apply force
        rb.AddForce(new Vector3(
            Random.Range(-1,1),
            Random.Range(400,600),
            Random.Range(-1,1))
        );
        const float MAX_TORQUE = 10000;
        rb.AddTorque(new Vector3(
            Random.Range(-MAX_TORQUE, MAX_TORQUE),
            Random.Range(-MAX_TORQUE, MAX_TORQUE),
            Random.Range(-MAX_TORQUE, MAX_TORQUE))
        );

        // Deactivate collisions
        rb.excludeLayers = LayerMask.GetMask("Enemy");
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    //*---------------------------------------------------------------*//
    //*-------------------------- COLLISIONS -------------------------*//
    //*---------------------------------------------------------------*//

    private void HandleCollisionWithEnemy(Enemy otherEnemy) {
        attacking = false;
        // el otro enemigo no se mueve por mí
        if(otherEnemy.GetOtherEnemyId()!=id){
            isMoving = false;
            animator.SetBool("moving",false);
            otherEnemyId = otherEnemy.GetId();
        }else{
            isMoving = true;
            animator.SetBool("moving",true);
        }
    }

    // ! esto permite evitar bloqueos binarios 
    // ? (A para por B y B para por A)
    // ! pero bloqueos ternarios y superiores no 
    // ? (A para por B, B por C y C por A)

    //-----------------------------------------------------------------//

    private void HandleCollisionWithBuilding(Building otherBuilding) {
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

    //*---------------------------------------------------------------*//
    //*--------------------------- GETTERS ---------------------------*//
    //*---------------------------------------------------------------*//

    public static int GetCount(){
        return enemyCount;
    }

    //-----------------------------------------------------------------//

    public float GetHealthPercentage(){
        return (float)health/MAX_HEALTH;
    }

    //-----------------------------------------------------------------//

    public int GetSplineId(){
        return splineId;
    }

    //-----------------------------------------------------------------//

    public int GetId(){
        return id;
    }

    //-----------------------------------------------------------------//

    public int GetOtherEnemyId(){
        return otherEnemyId;
    }

    //-----------------------------------------------------------------//

    public TypeEnemy GetTypeEnemy(){
        return TYPE;
    }

    //-----------------------------------------------------------------//

    public bool IsMoving(){
        return isMoving;
    }

    //-----------------------------------------------------------------//

    public void Damage(int dmg){
        health-=dmg;
    }

    //-----------------------------------------------------------------//

    public SplineAnimationController GetSplineAnimationController(){
        SplineAnimationController anim = 
            parent.GetComponent<SplineAnimationController>();
        return anim;
    }

    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//
    //!------------------------- END OF Enemy ------------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//
}
