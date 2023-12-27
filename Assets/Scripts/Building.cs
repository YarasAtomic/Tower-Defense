using UnityEngine;

public abstract class Building : MonoBehaviour
{

    BuildingTile buildingTile;

    public void Initialise(BuildingTile buildingTile) {
        this.buildingTile = buildingTile;
    }

	private TypeBuilding type;

    protected int BASE_HP;
	protected int PURCHASE_PRICE;
	protected int MAX_SELLING_PRICE;

    protected int hp;
	protected int sellingPrice;

	protected Animator animator;

	// GET methods

	public TypeBuilding GetTypeBuilding() {
		return type;
	}

	public int GetPurchasePrice() {
		return PURCHASE_PRICE;
	}

	public abstract int GetSellingPrice();

	// ACTION methods

	public void SellBuilding() {
		animator.SetBool("sellBuilding", true);
	}

	public void DamageBuilding(int dmg) {
		hp -= dmg;
		if (hp <= 0) DestroyBuilding();
    }

	public abstract void DestroyBuilding();

}