using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashAttack : SpecialAttack
{
    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------- CLASS ATTRIBUTES -----------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//

    [SerializeField] float RADIUS = 5;
    [SerializeField] int DAMAGE = 15;
    [SerializeField] static float COOLDOWN = 10;
    [SerializeField] float NUKE_SPEED = 100;
    GameObject particle;
    GameObject projector;
    GameObject nuke;
    
    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//
    //!------------------------ CLASS METHODS ------------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//

    //*---------------------------------------------------------------*//
    //*---------------------------- START ----------------------------*//
    //*---------------------------------------------------------------*//
    
    public new void Start(){
        base.Start();
        particle = transform.Find("particle").gameObject;
        projector = transform.Find("projector").gameObject;
        nuke = transform.Find("Nuke").gameObject;

        ProjectionTileMesh projectionTileMesh = projector.GetComponent<ProjectionTileMesh>();

        projectionTileMesh.gridHeight = (int)RADIUS * 2;
        projectionTileMesh.gridWidth = (int)RADIUS * 2;
        projectionTileMesh.transform.position = new Vector3(-RADIUS,0,-RADIUS);

        nuke.SetActive(false);
    }

    //*---------------------------------------------------------------*//
    //*---------------------------- UPDATE ---------------------------*//
    //*---------------------------------------------------------------*//

    public new void Update(){
        if(deployed){
            nuke.SetActive(true);
            nuke.transform.position += Vector3.down * GameTime.DeltaTime * NUKE_SPEED;
        }

        base.Update();
    }

    //*---------------------------------------------------------------*//
    //*------------------------ EXECUTE ATTACK -----------------------*//
    //*---------------------------------------------------------------*//

    protected override void ExecuteAttack(){
        // Aqui ocurre la explosión, se efectua el daño.
        Debug.Log("explota ataque splash");
        projector.SetActive(false);
        particle.SetActive(true);
        mainCamera.gameObject.GetComponent<CameraController>().SetShake(1);

        // Detectar enemigos dentro del radio de explosión
        Collider[] colliders = Physics.OverlapSphere(transform.position, RADIUS);

        foreach (Collider collider in colliders) {
            if (!collider.CompareTag("Enemy")) continue;
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance > RADIUS) continue;

            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null) {
                enemy.Damage((int)((RADIUS-distance)/RADIUS * DAMAGE));
                Debug.Log("Daño realizado a un enemigo");
            }
        }
    }

    //*---------------------------------------------------------------*//
    //*---------------------------- GETTERS --------------------------*//
    //*---------------------------------------------------------------*//

    public static float GetCooldown(){
        return COOLDOWN;
    }

    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//
    //!--------------------- END OF SplashAttack ---------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//
}
