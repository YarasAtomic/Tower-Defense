using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveFile
{
    [SerializeField] List<int> levels;
    [SerializeField] int xp = 0;

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
        return levels!=null && level < levels.Count ? levels[level] : 0;
    }

    public void SetMaxStarsAtLevel(int level,int stars){
        levels[level] = levels[level] < stars ? stars : levels[level];
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
}
