using UnityEngine;

public class EnemyCrab : Enemy
{
    [SerializeField] float WALK_SPEED;
    [SerializeField] float TURRET_RANGE;
    new void Start(){
        TYPE = TypeEnemy.Enemy2;
        base.Start();
    }

    new void Update(){
        Collider[] colliders = Physics.OverlapSphere(transform.position,TURRET_RANGE);
        bool rolling = true;
        speed = DEFAULT_SPEED;
        foreach(Collider collider in colliders){
            if(collider.gameObject.GetComponent<Building>()!=null){
                rolling = false;
                speed = WALK_SPEED;
            }
        }

        animator.SetBool("roll",rolling);

        base.Update();
    }
}
