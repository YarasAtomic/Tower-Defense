using UnityEngine;

public abstract class Building : MonoBehaviour
{
    protected int BASE_HP;
	protected int PURCHASE_PRICE;
	protected int MAX_SELLING_PRICE;

    protected int hp;
	protected int sellingPrice;

	public void Build() {
		
	}

	public abstract void Sell() {
		
	}

	public void Damage(int dmg) {
		hp -= dmg;
    }

    public abstract void DestroyBuilding() {
        
    }
}