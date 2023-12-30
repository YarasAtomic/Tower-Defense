using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Research : MonoBehaviour
{
    //Const attributes
    private readonly int[] UPGRADE_PRICES = {20,25,30};
    private readonly float[] SHOOTING_RADIUS_FACTOR = {1f, 1.2f, 1.4f, 1.6f};
    private readonly float[] SPEED_OF_REEPAIR_FACTOR = {1f ,0.8f, 0.6f, 0.4f};
    private readonly float[] WEAPONS_ARMORING_FACTOR = {1f, 1.2f, 1.4f, 1.6f};
    private readonly float[] REFUND_FACTOR  = {1f, 1.2f, 1.4f, 1.6f};
    private readonly float[] COOLDOWN_FACTOR  = {1f, 0.8f, 0.6f, 0.4f};
    private readonly float[] SUPPORT_POWER_FACTOR  = {1f, 1.2f, 1.4f, 1.6f};

    // Dynamic atributes
    private int shooting_radius_upgrade;
    private int speed_of_repair_upgrade;
    private int weapons_armoring_upgrade;
    private int refund_upgrade;
    private int cooldown_upgrade; 
    private int support_power_upgrade; 

// Atributo de prueba, para poder probar el funcionamiento de los botones
    [SerializeField] public int experience;

    //Buttons
    
    // Start is called before the first frame update
    void Start()
    {
       shooting_radius_upgrade = 0;
       speed_of_repair_upgrade = 0;
       weapons_armoring_upgrade = 0;
       refund_upgrade = 0;
       cooldown_upgrade = 0;
       support_power_upgrade = 0;
       experience = 20;
        
    }

    public int getShootingRadius(){
        return shooting_radius_upgrade;
    }
    public void shootingRadiusUpgrade(){
        if(shooting_radius_upgrade == 0){
            shooting_radius_upgrade++;
        }

    }

    
    // Update is called once per frame
    void Update()
    {
        
    }

}
