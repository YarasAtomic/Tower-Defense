using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniformAttack : SpecialAttack
{
    
    // Start is called before the first frame update
    protected override void ExecuteAttack(){
        // Aqui ocurre la explosión, se efectua el daño.
        Debug.Log("explota ataque uniforme, RADIUS->"+DAMAGE);

        // Detectar enemigos dentro del radio de explosión
        Collider[] colliders = Physics.OverlapSphere(transform.position, RADIUS);

        foreach (Collider collider in colliders) {
            if (!collider.CompareTag("Enemy")) continue;
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance > RADIUS) continue;

            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null) {
                enemy.Damage(DAMAGE);
                Debug.Log("Daño realizado a un enemigo");
            }
        }
    }
}
