using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResearchCanvas : MonoBehaviour
{   
    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------- CLASS ATTRIBUTES -----------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//


    GameObject doneButton;
    //GameOject doneButton;
    

    Research research;
    [SerializeField] GameObject researchGameObject;


// Atributos colores
    private Color bloquedColor;
    private Color purchasedColor;
    private Color availableColor;
    private Color newPurchaseColor;

// Atributos temp
    private int exp_temp;
    private int shootingRadiusTemp; 
    private int speedOfRepairTemp;
    private int weaponsArmoringTemp;
    private int refundTemp;
    private int cooldownTemp;
    private int supportPowerTemp;
    

    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//
    //!------------------------ CLASS METHODS ------------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//

    //*---------------------------------------------------------------*//
    //*---------------------------- START ----------------------------*//
    //*---------------------------------------------------------------*//


    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------- END OF LevelCanvas ---------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//



    void Start(){
      research = researchGameObject.GetComponent<Research>();
      
      exp_temp = research.getExp();
      shootingRadiusTemp = research.getShootingRadius();
      speedOfRepairTemp = research.getSpeedOfRepair();
      weaponsArmoringTemp = research.getWeaponsArmoring();
      refundTemp = research.getRefundForSelling();
      cooldownTemp = research.getSupportRechargeTime();
      supportPowerTemp = research.getSupportPower();
    
      bloquedColor = new Color(0.66f, 0.66f, 0.66f); // No puedo hacer nada con este botón
      purchasedColor = new Color (0.08f,0.26f, 0.53f); // Ya he comprado esta mejora
      availableColor = new Color(0.94f,0.94f,0.11f); // puedo comprar esta mejora
      newPurchaseColor = new Color(0.58f, 0f, 1f);
      //setShootingRadiusColors();
      setAllButtonsColors();

    }

    // TODO : hacer función que cambie la exp y el estado de los f
    void Update(){
        if(exp_temp < research.getPurchaseExp(0)){
          setAllButtonsColors();
        }
    }

    public void HandleDoneButton() {
      // TODO : navegacion
      research.updateStatus(exp_temp, shootingRadiusTemp, speedOfRepairTemp, weaponsArmoringTemp,
                             refundTemp, cooldownTemp, supportPowerTemp);
      
      
      setAllButtonsColors();
    }

    public void HandleCancelButton(){
      // TODO : navegacion 
    }

    //*---------------------------------------------------------------*//
    //*---------------------- BUTTONS COLORS -------------------------*//
    //*---------------------------------------------------------------*//


    public void setAllButtonsColors(){
      // Colores shootingRadius
      GameObject button_1 =  transform.Find("shootingButtons/shootingRadiusButton_1").gameObject;
      GameObject button_2 =  transform.Find("shootingButtons/shootingRadiusButton_2").gameObject;
      GameObject button_3 =  transform.Find("shootingButtons/shootingRadiusButton_3").gameObject;
      setButtonsColors(button_1,button_2, button_3, shootingRadiusTemp, research.getShootingRadius());

      // Colores speedOfRepair
      button_1 =  transform.Find("speedOfRepairButtons/speedOfRepairButton_1").gameObject;
      button_2 =  transform.Find("speedOfRepairButtons/speedOfRepairButton_2").gameObject;
      button_3 =  transform.Find("speedOfRepairButtons/speedOfRepairButton_3").gameObject;
      setButtonsColors(button_1,button_2, button_3, speedOfRepairTemp, research.getSpeedOfRepair());
    
      // Colores weaponsArmoring
      button_1 =  transform.Find("weaponsArmoringButtons/weaponsArmoringButton_1").gameObject;
      button_2 =  transform.Find("weaponsArmoringButtons/weaponsArmoringButton_2").gameObject;
      button_3 =  transform.Find("weaponsArmoringButtons/weaponsArmoringButton_3").gameObject;
      setButtonsColors(button_1,button_2, button_3,weaponsArmoringTemp ,research.getWeaponsArmoring());

      // Colores refundForSelling
      button_1 =  transform.Find("refundForSellingButtons/refundForSellingButton_1").gameObject;
      button_2 =  transform.Find("refundForSellingButtons/refundForSellingButton_2").gameObject;
      button_3 =  transform.Find("refundForSellingButtons/refundForSellingButton_3").gameObject;
      setButtonsColors(button_1,button_2, button_3, refundTemp, research.getRefundForSelling());

      // Colores supportRecharge
      button_1 =  transform.Find("supportRechargeButtons/supportRechargeButton_1").gameObject;
      button_2 =  transform.Find("supportRechargeButtons/supportRechargeButton_2").gameObject;
      button_3 =  transform.Find("supportRechargeButtons/supportRechargeButton_3").gameObject;
      setButtonsColors(button_1,button_2, button_3,cooldownTemp, research.getSupportRechargeTime());

      // Colores supportPower
      button_1 =  transform.Find("supportPowerButtons/supportPowerButton_1").gameObject;
      button_2 =  transform.Find("supportPowerButtons/supportPowerButton_2").gameObject;
      button_3 =  transform.Find("supportPowerButtons/supportPowerButton_3").gameObject;
      setButtonsColors(button_1,button_2, button_3, supportPowerTemp , research.getSupportPower());
    }
    
    public void setButtonsColors( GameObject button_1, GameObject  button_2, GameObject button_3, int factor, int factor_real){

      // Poner color comprado a los que tengamos ya comprados de antes
      var colors = button_1.GetComponent<Button>().colors; // TODO : Esto habria que cambiarlo pero no se como porque da error
      colors.disabledColor = purchasedColor;
      switch(factor_real){
        case 1:
          button_1.GetComponent<Button>().colors = colors;
          button_1.GetComponent<Button>().interactable = false;
        break;
        case 2:
          button_1.GetComponent<Button>().colors = colors;
          button_1.GetComponent<Button>().interactable = false;
          button_2.GetComponent<Button>().colors = colors;
          button_2.GetComponent<Button>().interactable = false;
        break;
        case 3:
          button_1.GetComponent<Button>().colors = colors;
          button_1.GetComponent<Button>().interactable = false;
          button_2.GetComponent<Button>().colors = colors;
          button_2.GetComponent<Button>().interactable = false;
          button_3.GetComponent<Button>().colors = colors;
          button_3.GetComponent<Button>().interactable = false;
        break;
      }

      int exp = research.getExp();
      switch (factor){

        // Todavia no hay nada comprado
        case 0: 
          if(exp_temp >= research.getPurchaseExp(0)){
            colors.normalColor = availableColor;
          }else
            colors.normalColor = bloquedColor;

            colors.disabledColor = bloquedColor;

            button_1.GetComponent<Button>().colors = colors; // Este puedo comprarlo o no comprarlo dependiendo de la exp
            button_2.GetComponent<Button>().colors = colors; // Queda bloqueado
            button_3.GetComponent<Button>().colors = colors; // Queda bloqueado

            button_2.GetComponent<Button>().interactable = false; // No puedo interaccionar con el
            button_3.GetComponent<Button>().interactable = false; // No puedo interaccionar con el
          break;

          // Primer nivel comprado, puedo comprar el 2, 1 y 3 no puedo interactuar
          case 1: 

            button_2.GetComponent<Button>().interactable = true; // Ahora puedo interaccionar con este
          
            if(exp_temp >= research.getPurchaseExp(1)){ // Puede que tenga suficiente exp o puede que no
              colors.normalColor = availableColor;
            }else
              colors.normalColor = bloquedColor;

            button_2.GetComponent<Button>().colors = colors; // Actualizo su color
            
            if(factor_real != 1){
              colors.disabledColor = newPurchaseColor; // Cambio el color a 1 a ya comprado
              button_1.GetComponent<Button>().interactable = false; // No puedo comprar el 1 porque esta ya comprado
              button_1.GetComponent<Button>().colors = colors;
            }
            
            colors.disabledColor = bloquedColor;
            button_3.GetComponent<Button>().interactable = false; // No puedo comprar el 1 porque esta ya comprado
            button_3.GetComponent<Button>().colors = colors;

          break;

          //Segundo nivel comprado
          case 2:
            button_3.GetComponent<Button>().interactable = true; // Ahora puedo interaccionar con este
            if(exp_temp >= research.getPurchaseExp(2)){
              colors.normalColor = availableColor;
            }else
              colors.normalColor = bloquedColor;

                        
            button_3.GetComponent<Button>().colors = colors;

            if( factor_real != 2) {
              colors.disabledColor = newPurchaseColor; // Cambio el color a 2 a ya comprado
              button_2.GetComponent<Button>().colors = colors;
              button_2.GetComponent<Button>().interactable = false;
            }
            
          break;
          
          // Tercer nivel comprado
          case 3: 

            if(factor_real != 3){
              colors.disabledColor = newPurchaseColor; // Cambio el color a 3 a ya comprado

              button_3.GetComponent<Button>().interactable = false;
              button_3.GetComponent<Button>().colors = colors;
            }
            

          break;
        }
        GameObject research_textGameObject = transform.Find("experience/experienceText").gameObject;
        
        research_textGameObject.GetComponent<TMP_Text>().text = exp_temp + " XP";
        
     }

    //*---------------------------------------------------------------*//
    //*---------------------- ACTION BUTTONS -------------------------*//
    //*---------------------------------------------------------------*//

    public void HandleShootingRadius() {
      int purchase_exp = research.getPurchaseExp(shootingRadiusTemp);
      // Aquí lo único que hacemos es comprobar si podemos comprarlo según la experiencia, el quitar la experiencia y aumentar el upgrade se hace en 
      // research
      if(exp_temp >= purchase_exp){
        shootingRadiusTemp++;
        exp_temp -= purchase_exp;
      }

      GameObject button_1 =  transform.Find("shootingButtons/shootingRadiusButton_1").gameObject;
      GameObject button_2 =  transform.Find("shootingButtons/shootingRadiusButton_2").gameObject;
      GameObject button_3 =  transform.Find("shootingButtons/shootingRadiusButton_3").gameObject;
      setButtonsColors(button_1,button_2, button_3, shootingRadiusTemp, research.getShootingRadius()); // Cambiamos los colores una vez que se haya comprado algo
      

    }
    

    public void HandleSpeedOfRepair() {
      int purchase_exp = research.getPurchaseExp(speedOfRepairTemp);
      // Aquí lo único que hacemos es comprobar si podemos comprarlo según la experiencia, el quitar la experiencia y aumentar el upgrade se hace en 
      // research
      if(exp_temp >= purchase_exp){
        speedOfRepairTemp++;
        exp_temp -= purchase_exp;
      }
      

      GameObject button_1 =  transform.Find("speedOfRepairButtons/speedOfRepairButton_1").gameObject;
      GameObject button_2 =  transform.Find("speedOfRepairButtons/speedOfRepairButton_2").gameObject;
      GameObject button_3 =  transform.Find("speedOfRepairButtons/speedOfRepairButton_3").gameObject;
      
      setButtonsColors(button_1,button_2, button_3, speedOfRepairTemp, research.getSpeedOfRepair());
     
      
    }

     public void HandleWeaponsArmoring() {
      int purchase_exp = research.getPurchaseExp(weaponsArmoringTemp);
      // Aquí lo único que hacemos es comprobar si podemos comprarlo según la experiencia, el quitar la experiencia y aumentar el upgrade se hace en 
      // research
      if(exp_temp >= purchase_exp){
        weaponsArmoringTemp++;
        exp_temp -= purchase_exp;
      }

      GameObject button_1 =  transform.Find("weaponsArmoringButtons/weaponsArmoringButton_1").gameObject;
      GameObject button_2 =  transform.Find("weaponsArmoringButtons/weaponsArmoringButton_2").gameObject;
      GameObject button_3 =  transform.Find("weaponsArmoringButtons/weaponsArmoringButton_3").gameObject;
      
      setButtonsColors(button_1,button_2, button_3, weaponsArmoringTemp, research.getWeaponsArmoring());
     
      
    }

    public void HandleRefundForSelling() {
      int purchase_exp = research.getPurchaseExp(refundTemp);
      // Aquí lo único que hacemos es comprobar si podemos comprarlo según la experiencia, el quitar la experiencia y aumentar el upgrade se hace en 
      // research
      if(exp_temp >= purchase_exp){
        refundTemp++;
        exp_temp -= purchase_exp;
      }
      
      int factor = research.getRefundForSelling();

      GameObject button_1 =  transform.Find("refundForSellingButtons/refundForSellingButton_1").gameObject;
      GameObject button_2 =  transform.Find("refundForSellingButtons/refundForSellingButton_2").gameObject;
      GameObject button_3 =  transform.Find("refundForSellingButtons/refundForSellingButton_3").gameObject;
      
      setButtonsColors(button_1,button_2, button_3, refundTemp, research.getRefundForSelling());
     
      
    }

    public void HandleSupportRechargeTime() {
      int purchase_exp = research.getPurchaseExp(cooldownTemp);
      // Aquí lo único que hacemos es comprobar si podemos comprarlo según la experiencia, el quitar la experiencia y aumentar el upgrade se hace en 
      // research
      if(exp_temp >= purchase_exp){
        cooldownTemp++;
        exp_temp -= purchase_exp;
      }
      
      int factor = research.getSupportRechargeTime();

      GameObject button_1 =  transform.Find("supportRechargeButtons/supportRechargeButton_1").gameObject;
      GameObject button_2 =  transform.Find("supportRechargeButtons/supportRechargeButton_2").gameObject;
      GameObject button_3 =  transform.Find("supportRechargeButtons/supportRechargeButton_3").gameObject;
      
      setButtonsColors(button_1,button_2, button_3, cooldownTemp, research.getSupportRechargeTime());
    }

    public void HandleSupportPower() {
      int purchase_exp = research.getPurchaseExp(supportPowerTemp);
      // Aquí lo único que hacemos es comprobar si podemos comprarlo según la experiencia, el quitar la experiencia y aumentar el upgrade se hace en 
      // research
      if(exp_temp >= purchase_exp){
        supportPowerTemp++;
        exp_temp -= purchase_exp;
      }

      GameObject button_1 =  transform.Find("supportPowerButtons/supportPowerButton_1").gameObject;
      GameObject button_2 =  transform.Find("supportPowerButtons/supportPowerButton_2").gameObject;
      GameObject button_3 =  transform.Find("supportPowerButtons/supportPowerButton_3").gameObject;
      
      setButtonsColors(button_1,button_2, button_3, supportPowerTemp, research.getSupportPower());
     
      
    }

}

