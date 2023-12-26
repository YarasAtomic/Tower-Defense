using UnityEngine;
using UnityEngine.Splines;

public class Building : MonoBehaviour
{
    BuildingTile buildingTile;
    public void Damage(int dmg){
        // Debug.Log("damaged");
    }

    public void Initialise(BuildingTile buildingTile) {
        this.buildingTile = buildingTile;
    }
}