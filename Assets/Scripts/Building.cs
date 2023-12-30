using UnityEngine;

public abstract class Building : MonoBehaviour
{
	private TypeBuilding type;

    [SerializeField] protected float BASE_HP = 100.0f;
	protected int MAX_SELLING_PRICE;

    protected float hp;
	protected int sellingPrice;

	protected Animator animator;

	protected BuildingTile tile;

	// INIT method

	public abstract void Initialise(BuildingTile buildingTile);

	// GET methods

	public abstract float GetHealthPercentage();

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