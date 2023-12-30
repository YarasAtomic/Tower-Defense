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
   [SerializeField] private int shooting_radius_upgrade;
   [SerializeField] private int speed_of_repair_upgrade;
   [SerializeField] private int weapons_armoring_upgrade;
   [SerializeField] private int refund_upgrade;
   [SerializeField] private int cooldown_upgrade; 
   [SerializeField] private int support_power_upgrade; 

// Atributo de prueba, para poder probar el funcionamiento de los botones
    [SerializeField] public int experience;

    //Buttons
    
    // Start is called before the first frame update
    void Start()
    {
       shooting_radius_upgrade = 0;
       speed_of_repair_upgrade = 1;
       weapons_armoring_upgrade = 2;
       refund_upgrade = 3;
       cooldown_upgrade = 1;
       support_power_upgrade = 2;
       experience = 100;
        
    }

    public int getShootingRadius(){
        return shooting_radius_upgrade;
    }
    
    public int getSpeedOfRepair(){
        return speed_of_repair_upgrade;
    }

    public int getWeaponsArmoring(){
        return weapons_armoring_upgrade;
    }

    public int getRefundForSelling(){
        return refund_upgrade;
    }

    public int getSupportRechargeTime(){
        return cooldown_upgrade;
    }

    public int getSupportPower(){
        return support_power_upgrade;
    }

    public int getExp(){
        return experience;
    }


    public int getPurchaseExp(int level){
        return UPGRADE_PRICES[level];
    }

    public void shootingRadiusUpgrade(){
        if(shooting_radius_upgrade < 3){
            experience -= UPGRADE_PRICES[shooting_radius_upgrade];
            shooting_radius_upgrade++;
            Debug.Log(shooting_radius_upgrade);
        
        }

    }

    public void speedOfRepairUpgrade(){
        if(speed_of_repair_upgrade < 3){
            experience -= UPGRADE_PRICES[speed_of_repair_upgrade];
            speed_of_repair_upgrade++;
        }

    }
    public void weaponsArmoringUpgrade(){
        if(weapons_armoring_upgrade < 3){
            experience -= UPGRADE_PRICES[weapons_armoring_upgrade];
            weapons_armoring_upgrade++;
        }

    }

    public void refundForSellingUpgrade(){
        if(refund_upgrade < 3){
            experience -= UPGRADE_PRICES[refund_upgrade];
            refund_upgrade++;
        }

    }

    public void supportRechargeTimeUpgrade(){
        if(cooldown_upgrade < 3){
            experience -= UPGRADE_PRICES[cooldown_upgrade];
            cooldown_upgrade++;
        }
    }

    public void supportPowerUpgrade(){
        if(support_power_upgrade < 3){
            experience -= UPGRADE_PRICES[support_power_upgrade];
            support_power_upgrade++;
        }
    }

    public void updateStatus(int expTemp, int shootingRadiusTemp, int speedOfRepairTemp, int weaponsArmoringTemp, 
                            int refundTemp, int cooldownTemp, int supportTowerTemp){
    
        experience = expTemp;
        shooting_radius_upgrade = shootingRadiusTemp;
        speed_of_repair_upgrade = speedOfRepairTemp;
        weapons_armoring_upgrade = weapons_armoring_upgrade;
        refund_upgrade = refundTemp;
        cooldown_upgrade = cooldownTemp;
        support_power_upgrade = supportTowerTemp;
        
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }

}
