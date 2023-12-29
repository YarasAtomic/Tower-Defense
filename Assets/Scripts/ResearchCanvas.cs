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

// Atributos colores
    private Color pressedColor;
    private Color originalColor;
    private bool isPressed = false;
    public Button shootingRadiusButton_1;
    [SerializeField] GameObject researchGameObject;

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
      doneButton =  transform.Find("DoneButton").gameObject;
      research = researchGameObject.GetComponent<Research>();
      Debug.Log(doneButton);
      //doneButtonTMP =  transform.Find("Panel/doneButton/doneButtonTMP").gameObject;
      pressedColor = Color.red;
      originalColor = Color.blue;
    }

    void Update(){
        //HandleDoneButton();
    }

    public void HandleDoneButton() {
      
      Debug.Log(doneButton);


     //var colors =  doneButton.GetComponent<Button>().c = colors.red;
   /*  
     var colors =  doneButton.GetComponent<Button>().colors;
    
     colors.normalColor = Color.red;
      doneButton.GetComponent<Button>().colors = colors; */
        
    }

    public void setShootingRadiusColors(){
       
    }

    public void HandleShootingRadius() {
     
      research.shootingRadiusUpgrade();
       Debug.Log(research.getShootingRadius());
      
    }
    



}

