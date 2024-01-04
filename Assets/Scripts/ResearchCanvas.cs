using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResearchCanvas : MonoBehaviour
{   
    //-----------------------------------------------------------------//
    //-----------------------------------------------------------------//
    //----------------------- CLASS ATTRIBUTES ------------------------//
    //-----------------------------------------------------------------//
    //-----------------------------------------------------------------//


    //Const attributes
    private readonly int[]   UPGRADE_PRICES          = {20,25,30};
    private readonly float[] SHOOTING_RADIUS_FACTOR  = {1f, 1.2f, 1.4f, 1.6f};
    private readonly float[] SPEED_OF_REEPAIR_FACTOR = {1f ,0.8f, 0.6f, 0.4f};
    private readonly float[] WEAPONS_ARMORING_FACTOR = {1f, 1.2f, 1.4f, 1.6f};
    private readonly float[] REFUND_FACTOR           = {1f, 1.2f, 1.4f, 1.6f};
    private readonly float[] COOLDOWN_FACTOR         = {1f, 0.8f, 0.6f, 0.4f};
    private readonly float[] SUPPORT_POWER_FACTOR    = {1f, 1.2f, 1.4f, 1.6f};

    [SerializeField] Save saveAsset;
    [SerializeField] MainMenu mainMenu;

// Atributos colores
    [SerializeField] Color blockedColor;
    [SerializeField] Color purchasedColor;

    private bool firstFind = true;


// Atributos temp
    private int exp_temp;
    private int shootingRadiusTemp; 
    private int speedOfRepairTemp;
    private int weaponsArmoringTemp;
    private int refundTemp;
    private int cooldownTemp;
    private int supportPowerTemp;


// Atributos botones
    GameObject shootingRadiusButton_1,
               shootingRadiusButton_2,
               shootingRadiusButton_3;

    GameObject speedOfRepairButton_1,
               speedOfRepairButton_2,
               speedOfRepairButton_3;
               
    GameObject weaponsArmoringButton_1,
               weaponsArmoringButton_2,
               weaponsArmoringButton_3;
    GameObject refundForSellingButton_1,
               refundForSellingButton_2,
               refundForSellingButton_3;

    GameObject supportRechargeButton_1,
               supportRechargeButton_2,
               supportRechargeButton_3;
    GameObject supportPowerButton_1,
               supportPowerButton_2,
               supportPowerButton_3;


    //-----------------------------------------------------------------//
    //-----------------------------------------------------------------//
    //------------------------- CLASS METHODS -------------------------//
    //-----------------------------------------------------------------//
    //-----------------------------------------------------------------//

    //*---------------------------------------------------------------*//
    //*---------------------------- START ----------------------------*//
    //*---------------------------------------------------------------*//

    void Start(){
    
      // blockedColor = new Color(0.24f, 0.24f, 0.24f); // No puedo hacer nada con este botón
      // purchasedColor = new Color (0.28f,0.47f, 1f); // Ya he comprado esta mejora
      // availableColor = new Color(0.64f,0.64f,0.64f); // puedo comprar esta mejora
      // firstFind = true;

      // UpdateValues();

    }

    // TODO : Sigue sin funcionar del todo 
    public void UpdateValues(){
      Debug.Log("update values");
      exp_temp = saveAsset.GetSaveFile().GetXp();
      shootingRadiusTemp = saveAsset.GetSaveFile().GetShootingRadius();
      speedOfRepairTemp = saveAsset.GetSaveFile().GetSpeedOfRepair();
      weaponsArmoringTemp = saveAsset.GetSaveFile().GetWeaponsArmoring();
      refundTemp = saveAsset.GetSaveFile().GetRefundForSelling();
      cooldownTemp = saveAsset.GetSaveFile().GetSupportRechargeTime();
      supportPowerTemp = saveAsset.GetSaveFile().GetSupportPower();

      if(firstFind){
        Debug.Log("first find");
        shootingRadiusButton_1 =  transform.Find("shootingRadius/shootingButtons/shootingRadiusButton_1").gameObject;
        shootingRadiusButton_2 =  transform.Find("shootingRadius/shootingButtons/shootingRadiusButton_2").gameObject;
        shootingRadiusButton_3 =  transform.Find("shootingRadius/shootingButtons/shootingRadiusButton_3").gameObject;

        speedOfRepairButton_1 =  transform.Find("speedOfRepair/speedOfRepairButtons/speedOfRepairButton_1").gameObject;
        speedOfRepairButton_2 =  transform.Find("speedOfRepair/speedOfRepairButtons/speedOfRepairButton_2").gameObject;
        speedOfRepairButton_3 =  transform.Find("speedOfRepair/speedOfRepairButtons/speedOfRepairButton_3").gameObject;
        
        weaponsArmoringButton_1 =  transform.Find("weaponsArmoring/weaponsArmoringButtons/weaponsArmoringButton_1").gameObject;
        weaponsArmoringButton_2 =  transform.Find("weaponsArmoring/weaponsArmoringButtons/weaponsArmoringButton_2").gameObject;
        weaponsArmoringButton_3 =  transform.Find("weaponsArmoring/weaponsArmoringButtons/weaponsArmoringButton_3").gameObject;

        refundForSellingButton_1 =  transform.Find("refundForSelling/refundForSellingButtons/refundForSellingButton_1").gameObject;
        refundForSellingButton_2 =  transform.Find("refundForSelling/refundForSellingButtons/refundForSellingButton_2").gameObject;
        refundForSellingButton_3 =  transform.Find("refundForSelling/refundForSellingButtons/refundForSellingButton_3").gameObject;
        
        supportRechargeButton_1 =  transform.Find("supportRechargeTime/supportRechargeButtons/supportRechargeButton_1").gameObject;
        supportRechargeButton_2 =  transform.Find("supportRechargeTime/supportRechargeButtons/supportRechargeButton_2").gameObject;
        supportRechargeButton_3 =  transform.Find("supportRechargeTime/supportRechargeButtons/supportRechargeButton_3").gameObject;

        supportPowerButton_1 =  transform.Find("supportPower/supportPowerButtons/supportPowerButton_1").gameObject;
        supportPowerButton_2 =  transform.Find("supportPower/supportPowerButtons/supportPowerButton_2").gameObject;
        supportPowerButton_3 =  transform.Find("supportPower/supportPowerButtons/supportPowerButton_3").gameObject;
        firstFind = false;
      }
      SetAllButtonsColors();
    }

    //*---------------------------------------------------------------*//
    //*---------------------- BUTTONS COLORS -------------------------*//
    //*---------------------------------------------------------------*//


    public void SetAllButtonsColors(){
      Debug.Log("all buttons");
      // Colores shootingRadius

      SetButtonsColors(shootingRadiusButton_1, shootingRadiusButton_2, shootingRadiusButton_3, shootingRadiusTemp );

      // Colores speedOfRepair
      
      SetButtonsColors(speedOfRepairButton_1, speedOfRepairButton_2, speedOfRepairButton_3, speedOfRepairTemp);
    
      // Colores weaponsArmoring
      SetButtonsColors(weaponsArmoringButton_1, weaponsArmoringButton_2, weaponsArmoringButton_3, weaponsArmoringTemp);

      // Colores refundForSelling
      SetButtonsColors(refundForSellingButton_1, refundForSellingButton_2, refundForSellingButton_3, refundTemp);

      // Colores supportRecharge
      SetButtonsColors(supportRechargeButton_1, supportRechargeButton_2, supportRechargeButton_3, cooldownTemp);

      // Colores supportPower
      SetButtonsColors(supportPowerButton_1, supportPowerButton_2, supportPowerButton_3, supportPowerTemp);

      GameObject research_textGameObject = transform.Find("experience/experienceText").gameObject;
      research_textGameObject.GetComponent<TMP_Text>().text = exp_temp + " XP";
    
    }
    
    public void SetButtonsColors( GameObject button_1, GameObject  button_2, GameObject button_3, int factor){
        GameObject[] buttons = {button_1,button_2,button_3};

        for(int i = 0; i < buttons.Length; i++){
          Button currentButton = buttons[i].GetComponent<Button>();
          ColorBlock colors = currentButton.colors;
          if(factor == i){
            currentButton.interactable = exp_temp >= UPGRADE_PRICES[i];
          }else if(factor < i){
            colors.disabledColor = blockedColor;
            currentButton.colors = colors;
            currentButton.interactable = false;
          }else{
            colors.disabledColor = purchasedColor;
            currentButton.colors = colors;
            currentButton.interactable = false;
          }
        }
     }

    //*---------------------------------------------------------------*//
    //*---------------------- ACTION BUTTONS -------------------------*//
    //*---------------------------------------------------------------*//
    public void HandleDoneButton() {
      saveAsset.GetSaveFile().UpdateStatus(exp_temp, shootingRadiusTemp, speedOfRepairTemp, weaponsArmoringTemp,
                             refundTemp, cooldownTemp, supportPowerTemp);
      
      SetAllButtonsColors();

      mainMenu.CloseResearchMenu();
    }

    // TODO : Creo que al hacer close, ya no se tendria que dar valor a estas opciones
    public void HandleCancelButton(){
      mainMenu.CloseResearchMenu();
    }


    public void HandleShootingRadius() {
      int purchase_exp = UPGRADE_PRICES[shootingRadiusTemp];
      
      if(exp_temp >= purchase_exp){
        shootingRadiusTemp++;
        exp_temp -= purchase_exp;
      }

      SetAllButtonsColors();
    }
    

    public void HandleSpeedOfRepair() {
      int purchase_exp = UPGRADE_PRICES[speedOfRepairTemp];
      
      if(exp_temp >= purchase_exp){
        speedOfRepairTemp++;
        exp_temp -= purchase_exp;
      }
            
      SetAllButtonsColors();     
    }

     public void HandleWeaponsArmoring() {
      int purchase_exp = UPGRADE_PRICES[weaponsArmoringTemp];
      
      if(exp_temp >= purchase_exp){
        weaponsArmoringTemp++;
        exp_temp -= purchase_exp;
      }
     
      SetAllButtonsColors();     
      
    }

    public void HandleRefundForSelling() {
      int purchase_exp = UPGRADE_PRICES[refundTemp];
      
      if(exp_temp >= purchase_exp){
        refundTemp++;
        exp_temp -= purchase_exp;
      }
            
      SetAllButtonsColors();     
    }

    public void HandleSupportRechargeTime() {
      int purchase_exp = UPGRADE_PRICES[cooldownTemp];
      
      if(exp_temp >= purchase_exp){
        cooldownTemp++;
        exp_temp -= purchase_exp;
      }
      
      SetAllButtonsColors();     
    }

    public void HandleSupportPower() {
      int purchase_exp = UPGRADE_PRICES[supportPowerTemp];

      if(exp_temp >= purchase_exp){
        supportPowerTemp++;
        exp_temp -= purchase_exp;
      }
      
      SetAllButtonsColors();     
    }

    //-----------------------------------------------------------------//
    //-----------------------------------------------------------------//
    //--------------------- END OF ResearchCanvas ---------------------//
    //-----------------------------------------------------------------//
    //-----------------------------------------------------------------//
}

