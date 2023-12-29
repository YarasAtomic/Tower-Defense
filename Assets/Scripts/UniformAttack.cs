using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniformAttack : SpecialAttack
{
    [SerializeField] float RADIUS = 4;
    [SerializeField] int DAMAGE = 10;
    [SerializeField] static float COOLDOWN = 20;

    GameObject beam;
    GameObject projector;

    [SerializeField] float BEAM_SPEED = 70;
    float scale = 0;
    float beamTimer = 0;
    float BEAM_DURATION = 0.5f;
    public new void Start(){
        base.Start();
        beam = transform.Find("field").gameObject;
        projector = transform.Find("projector").gameObject;
        
        beam.transform.localScale = Vector3.zero;

        ProjectionTileMesh projectionTileMesh = projector.GetComponent<ProjectionTileMesh>();

        projectionTileMesh.gridHeight = (int)RADIUS * 2;
        projectionTileMesh.gridWidth = (int)RADIUS * 2;
        projectionTileMesh.transform.position = new Vector3(-RADIUS,0,-RADIUS);

    }
    public static float GetCooldown(){
        return COOLDOWN;
    }

    protected override void ExecuteAttack(){
        // Aqui ocurre la explosión, se efectua el daño.
        Debug.Log("explota ataque uniforme");
        projector.SetActive(false);

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

    public new void Update(){
        base.Update();

        if(!exploded) return;

        if(beamTimer < BEAM_DURATION && scale < RADIUS){
            scale+=GameTime.DeltaTime * BEAM_SPEED;
            mainCamera.gameObject.GetComponent<CameraController>().SetShake(0.6f);
        }else if(beamTimer >= BEAM_DURATION && scale > 0){
            scale-=GameTime.DeltaTime * BEAM_SPEED;
        }else if(scale < 0){
            scale = 0;
        }

        beamTimer+=GameTime.DeltaTime;
        
        beam.transform.localScale = new Vector3(scale,100,scale) * 2; // el cilindro tiene radio 0.5
    }
}
