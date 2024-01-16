using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveFile
{
    [SerializeField] List<int> levels;
    [SerializeField] int xp = 0;
    [SerializeField] bool started = false;

    public int shootingRadiusUpgrade = 0;
    public int speedOfRepairUpgrade = 0;
    public int weaponsArmoringUpgrade = 0;
    public int refundUpgrade = 0;
    public int cooldownUpgrade = 0;
    public int supportPowerUpgrade = 0;

    public void AddXp(int n){
        xp+=n;
    }

    public int GetXp(){
        return xp;
    }

    public void SetXp(int n){
        xp = n;
    }

    public int StarsAtLevel(int level){
        return levels!=null && level < levels.Count && level > -1 ? levels[level] : 0;
    }

    public void SetMaxStarsAtLevel(int level,int stars){
        Debug.Log("setMaxStars");
        if(level > -1){
            while(levels.Count <= level){
                levels.Add(0);
            }
            if(levels!=null && level < levels.Count){
                levels[level] = levels[level] < stars ? stars : levels[level];
            }
        }
        
    }

    public int GetTotalStars(){
        int stars = 0;
        for(int i = 0; i < levels.Count; i++){
            stars += levels[i];
        }
        return stars;
    }

    public bool IsFirstTimeFinish(int level){
        return !(levels[level] > 0);
    }

    public int GetTotalLevels(){
        return levels.Count;
    }

    public int GetLastFinishedLevel(){
        int lastLevel = -1;
        for(int i = 0; i < levels.Count; i++){
            lastLevel = levels[i] > 0 ? i : lastLevel;
        }
        return lastLevel;
    }

    public int GetShootingRadius(){
        return shootingRadiusUpgrade;
    }
    
    public int GetSpeedOfRepair(){
        return speedOfRepairUpgrade;
    }

    public int GetWeaponsArmoring(){
        return weaponsArmoringUpgrade;
    }

    public int GetRefundForSelling(){
        return refundUpgrade;
    }

    public int GetSupportRechargeTime(){
        return cooldownUpgrade;
    }

    public int GetSupportPower(){
        return supportPowerUpgrade;
    }

    public void UpdateResearchStatus(int expTemp, int shootingRadiusTemp, int speedOfRepairTemp, int weaponsArmoringTemp, 
                            int refundTemp, int cooldownTemp, int supportTowerTemp){
    
        xp = expTemp;
        shootingRadiusUpgrade  = shootingRadiusTemp;
        speedOfRepairUpgrade   = speedOfRepairTemp;
        weaponsArmoringUpgrade = weaponsArmoringTemp;
        refundUpgrade          = refundTemp;
        cooldownUpgrade        = cooldownTemp;
        supportPowerUpgrade    = supportTowerTemp;
    }

    public void InitSave(){
        started = true;
        levels.Clear();
        xp = 0;
        shootingRadiusUpgrade = 0;
        speedOfRepairUpgrade = 0;
        weaponsArmoringUpgrade = 0;
        refundUpgrade = 0;
        cooldownUpgrade = 0;
        supportPowerUpgrade = 0;
    }

    public bool IsEmpty(){
        return !started;
    }
}
