using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : Building
{
    [SerializeField] private float RESOURCE_RATE = 0.5; // Debería de ser const
    [SerializeField] private int RESOURCE_AMOUNT = 10; // Debería de ser const
    [SerializeField] private  float timer;

    // Start is called before the first frame update
    void Start()
    {
        base.BASE_HP = 100;
        base.PURCHASE_PRICE = 100;
        base.MAX_SELLING_PRICE = 90;
        base.hp = base.BASE_HP;
        base.sellingPrice = base.MAX_SELLING_PRICE;
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(hp > 0){ // Si está vivo intenta generar más recursos
            timer += Time.deltaTime;
            if(timer > RESOURCE_RATE){
                // TO DO :
                // Añadir al level RESOURCE_AMOUNT
            }
        }else{ // Está muerto, tendría que desaparecer del mapa
            DestroyBuilding();
        }
    }

   public override void DestroyBuilding(){
        // TO DO :
        // Por hacer
   }

   public override int Sell(){
        // 
        return (int) (base.hp / base.BASE_HP)*base.MAX_SELLING_PRICE;  
   }

   // TO DO
   // Hacer que te devuelva el precio de venta
   // Hacer que se venda el generador
   // Hacer que se destruya el generador
}
