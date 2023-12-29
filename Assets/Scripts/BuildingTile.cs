using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingTile : MonoBehaviour
{
    Building building;

    // Start is called before the first frame update
    void Start(){
        building = null;
        Hide();
    }

    // Update is called once per frame
    void Update(){
        
    }

    public void Build(GameObject buildingPrefab, LevelLogic levelLogic){
        building = Instantiate(buildingPrefab, transform.position, Quaternion.identity).GetComponent<Building>();
        building.Initialise(this);

        if(building is Generator){
            ((Generator) building).SetLevelLogic(levelLogic);
        }
        Hide();
    }

    public bool IsEmpty(){
        return building == null;
    }

    public void Hide(){
        gameObject.SetActive(false);
    }

    public void Show(){
        gameObject.SetActive(true);
    }
}
