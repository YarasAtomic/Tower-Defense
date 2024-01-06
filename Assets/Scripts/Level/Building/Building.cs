using UnityEngine;

public abstract class Building : MonoBehaviour
{
	protected TypeBuilding TYPE;

    protected float BASE_HP;
	protected float MAX_SELLING_PRICE;

    protected float hp;
	protected int sellingPrice;

	protected Animator animator;

	protected BuildingTile tile;

	// INIT method

	public abstract void Initialise(BuildingTile buildingTile);

	// GET methods

	public abstract float GetHealthPercentage();

	public TypeBuilding GetTypeBuilding() {
		return TYPE;
	}

	public abstract int GetSellingPrice();

	// ACTION methods

	public void SellBuilding() {
		// animator.SetBool("sellBuilding", true);

		tile.EmptyTile();

		Destroy(gameObject);
		Destroy(this);
	}

	public void DamageBuilding(int dmg) {
		hp -= dmg/* * Research.WEAPONS_ARMORING_FACTOR[SingletonScriptableObject<Save>.Instance.GetSaveFile().GetWeaponsArmoring()]*/;
		if (hp <= 0) DestroyBuilding();
    }

	public abstract void DestroyBuilding();
}