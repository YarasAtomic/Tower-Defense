using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : Building
{
    /*[SerializeField]*/ private float RESOURCE_RATE = 0.5f; // Debería de ser const
    /*[SerializeField]*/ private int RESOURCE_AMOUNT = 10; // Debería de ser const
    /*[SerializeField]*/ private  float timer;

    // Start is called before the first frame update
    void Start()
    {
        base.BASE_HP = 100;
        base.PURCHASE_PRICE = 100;
        base.MAX_SELLING_PRICE = 90;
        base.hp = base.BASE_HP;
        base.sellingPrice = base.MAX_SELLING_PRICE;
        timer = 0;

		animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // if(hp > 0){ // Si está vivo intenta generar más recursos ----> Lo he comentado porque la gestión del daño se hace en el método Damage
            timer += Time.deltaTime;
            if(timer > RESOURCE_RATE){
                // TO DO :
                // Añadir al level RESOURCE_AMOUNT
            }
        // }else{ // Está muerto, tendría que desaparecer del mapa ----> Lo he comentado porque la gestión del daño se hace en el método Damage
        //     DestroyBuilding(); ----> Lo he comentado porque la gestión del daño se hace en el método Damage
        // } ----> Lo he comentado porque la gestión del daño se hace en el método Damage
    }

	public override int GetSellingPrice() {
		return base.sellingPrice;
	}

	public override void DestroyBuilding() {
		animator.SetBool("destroyGenerator", true);
	}

   // TO DO
   // Hacer que te devuelva el precio de venta ----> HECHO
   
   // En cuanto a esto, vamos a usar la clase Animator que me ha comentado Guille que es lo que se usa para gestionar las animaciones
   // y nosotros además solo gestionamos las animaciones en estos métodos, no hacemos nada más
   //
   // Hacer que se venda el generador ----> Esto la parte de la animación lo he pasado al Building porque lo que es el procedimiento es el mismo
   
   // He actualizado también el método Damage para que si la vida es <= 0 se llame al método de destrucción
   //
   // Hacer que se destruya el generador ----> HECHO
}