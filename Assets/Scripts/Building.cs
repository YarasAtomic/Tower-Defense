using UnityEngine;

public abstract class Building : MonoBehaviour
{
	private TypeBuilding type;

    protected int BASE_HP;
	protected int MAX_SELLING_PRICE;

    protected int hp;
	protected int sellingPrice;

	protected Animator animator;

	protected BuildingTile tile;

	// INIT method

	public void Initialise(BuildingTile buildingTile) {
		tile = buildingTile;
	}

	// GET methods

	public TypeBuilding GetTypeBuilding() {
		return type;
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