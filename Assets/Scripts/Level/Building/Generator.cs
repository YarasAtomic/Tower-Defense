using UnityEngine;

public class Generator : Building
{
	[SerializeField] private static readonly int PURCHASE_PRICE = 100;
    [SerializeField] private float RESOURCE_RATE = 0.5f;
    [SerializeField] private int RESOURCE_AMOUNT = 10;
    private float timer;
    private LevelLogic levelLogic;

	public override void Initialise(BuildingTile buildingTile) {
		tile = buildingTile;
	}

    public void SetLevelLogic (LevelLogic level){
        levelLogic = level;
    }
	
    void Start() {
        // General
		BASE_HP = 100f;
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
		if(timer > RESOURCE_RATE && levelLogic.InWave()){
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
