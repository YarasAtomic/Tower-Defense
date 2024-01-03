using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : Building
{
	private static readonly int PURCHASE_PRICE = 100;
    private readonly float RESOURCE_RATE = 0.5f;
    private readonly int RESOURCE_AMOUNT = 10;
    private float timer;
    private LevelLogic levelLogic;

	public override void Initialise(TypeBuilding typeBuilding, BuildingTile buildingTile) {
		tile = buildingTile;
	}

    public void SetLevelLogic (LevelLogic level){
        levelLogic = level;
    }	

    // Start is called before the first frame update
    void Start() {
        // General
		hp = BASE_HP;

		// Costes
        MAX_SELLING_PRICE = PURCHASE_PRICE * 0.75f;
        sellingPrice = (int) MAX_SELLING_PRICE;
        
		// Estados
		timer = 0.0f;
		
		animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
		timer += GameTime.DeltaTime;
		if(timer > RESOURCE_RATE){
			levelLogic.AddResources(RESOURCE_AMOUNT);
			timer = 0;
		}
    }

	public override float GetHealthPercentage() {
		return hp / BASE_HP;
	}

	public static int GetPurchasePrice() {
		return PURCHASE_PRICE;
	}

    private void CalculateSellingPrice(){
        base.sellingPrice = (int) (MAX_SELLING_PRICE * GetHealthPercentage());
    }

	public override int GetSellingPrice() {
        CalculateSellingPrice();
		return sellingPrice;
	}

	public override void DestroyBuilding() {
		animator.SetBool("destroyGenerator", true);

		tile.EmptyTile();

		Destroy(gameObject);
		Destroy(this);
	}
}
