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
      //doneButton =  transform.Find("DoneButton").gameObject;
      research = researchGameObject.GetComponent<Research>();
      //Debug.Log(doneButton);
      //doneButtonTMP =  transform.Find("Panel/doneButton/doneButtonTMP").gameObject;

      // TODO : Poner los colores bonitos
      bloquedColor = Color.black; // No puedo hacer nada con este botón
      purchasedColor = Color.blue; // Ya he comprado esta mejora
      availableColor = Color.red; // puedo comprar esta mejora
      setShootingRadiusColors();
    }

    void Update(){
        
    }

    public void HandleDoneButton() {
      
      Debug.Log(doneButton);
        
    }


    
    public void HandleShootingRadius() {
     int total_exp = research.getExp();
     int purchase_exp = research.getPurchaseExp(research.getShootingRadius());
     // Aquí lo único que hacemos es comprobar si podemos comprarlo según la experiencia, el quitar la experiencia y aumentar el upgrade se hace en 
     // research
      if(total_exp >= purchase_exp){
        research.shootingRadiusUpgrade();
      }
      setShootingRadiusColors(); // Cambiamos los colores una vez que se haya comprado algo
      
    }
    

public void setShootingRadiusColors(){
      GameObject button_1 =  transform.Find("shootingButtons/shootingRadiusButton_1").gameObject;
      GameObject button_2 =  transform.Find("shootingButtons/shootingRadiusButton_2").gameObject;
      GameObject button_3 =  transform.Find("shootingButtons/shootingRadiusButton_3").gameObject;
 
      int factor = research.getShootingRadius();
      int exp = research.getExp();
      var colors = button_1.GetComponent<Button>().colors; // TODO : Esto habria que cambiarlo pero no se como porque da error
      switch (factor){

        // Todavia no hay nada comprado
        case 0: 
            if(exp >= research.getPurchaseExp(0)){
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
          if(exp >= research.getPurchaseExp(1)){ // Puede que tenga suficiente exp o puede que no
            colors.normalColor = availableColor;
          }else
            colors.normalColor = bloquedColor;

          button_2.GetComponent<Button>().colors = colors; // Actualizo su color
          
          colors.disabledColor = purchasedColor; // Cambio el color a 1 a ya comprado
          button_1.GetComponent<Button>().interactable = false; // No puedo comprar el 1 porque esta ya comprado
          button_1.GetComponent<Button>().colors = colors;

        break;

        //Segundo nivel comprado
        case 2:
          button_3.GetComponent<Button>().interactable = true; // Ahora puedo interaccionar con este
          if(exp >= research.getPurchaseExp(2)){
            colors.normalColor = availableColor;
          }else
            colors.normalColor = bloquedColor;

          colors.disabledColor = purchasedColor; // Cambio el color a 2 a ya comprado
          button_2.GetComponent<Button>().colors = colors;
          button_3.GetComponent<Button>().colors = colors;
          
          button_2.GetComponent<Button>().interactable = false;
        break;
        
        // Tercer nivel comprado
        case 3: 
          colors.disabledColor = purchasedColor; // Cambio el color a 3 a ya comprado

          button_3.GetComponent<Button>().interactable = false;
          button_3.GetComponent<Button>().colors = colors;

        break;
      }
      
       
    }


}

