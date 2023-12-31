using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingTile : MonoBehaviour
{
	LevelLogic levelLogic;
    Building building;

	public void Initialise(LevelLogic levelLogic) {
		this.levelLogic = levelLogic;
	}

    // Start is called before the first frame update
    void Start(){
        building = null;
        Hide();
    }

    // Update is called once per frame
    void Update(){
        
    }

    public void Build(GameObject buildingPrefab){
        building = Instantiate(buildingPrefab, transform.position, Quaternion.identity).GetComponent<Building>();
        building.Initialise(this);

        if(building is Generator){
            ((Generator) building).SetLevelLogic(levelLogic);
        }
        Hide();
    }

	public void EmptyTile() {
		building = null;
		if (levelLogic.GetInteractionMode() == LevelLogic.InteractionMode.Build) Show();
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
