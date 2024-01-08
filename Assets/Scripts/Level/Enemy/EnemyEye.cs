using UnityEngine;

public class EnemyEye : Enemy
{
    GameObject laser;
    new void Start(){
        laser = transform.Find("EyeEnemy/body/eye/Laser").gameObject;
        TYPE = TypeEnemy.Enemy4;
        base.Start();
    }

    new void Update(){
        laser.SetActive(attacking);
        base.Update();
    }
}
