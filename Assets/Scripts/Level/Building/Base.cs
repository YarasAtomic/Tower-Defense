using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : Building
{
    [SerializeField] LevelLogic levelLogic;
    public override void Initialise(BuildingTile buildingTile) {}

    public override int GetSellingPrice() {
		return int.MaxValue;
	}

    public override float GetHealthPercentage() {
		return hp / BASE_HP;
	}

    public bool HasBeenDamaged(){
        return hp != BASE_HP;
    }

    void Start(){
        levelLogic.SetBaseBuilding(this);
        hp = BASE_HP;
    }
    public override void DestroyBuilding() {

        levelLogic.GameOver();

		Destroy(gameObject);
		Destroy(this);
	}
}
