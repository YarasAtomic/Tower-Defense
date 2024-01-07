using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelCanvas : MonoBehaviour
{   
    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------- CLASS ATTRIBUTES -----------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//

    GameObject resourcePanel,
               wavesTMP,
               resourcesTMP;

    GameObject skipButton,
               skipButtonTMP,
               skipTimerIMG;

    GameObject pauseButton,
               pauseIMG,
               playIMG;

    GameObject accelerateButton,
               accelerateTMP;

    GameObject levelFinishedPanel,
               levelFinishedTMP,
               menuButton,
               menuButtonTMP,
               experienceTMP,
               firstStarIMG,
               secondStarIMG,
               thirdStarIMG;


    GameObject tower1Button,
               tower2Button,
               tower3Button,
               generatorButton;

    GameObject buildingSubmenu,
               sellButton, 
               repairButton, 
               upgradeButton;

    GameObject splashAttackButton,
               uniformAttackButton,
               splashAttackTimerIMG,
               uniformAttackTimerIMG;

    [SerializeField] GameObject levelLogicGameObject;

    LevelLogic levelLogic;

    enum InteractionMode{
        Build, SpecialAttack, None
    };

    InteractionMode interactionMode;
    TypeBuilding typeBuilding;

    Building selectedBuilding;

    [SerializeField] GameObject mainCamera;


    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//
    //!------------------------ CLASS METHODS ------------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//

    //*---------------------------------------------------------------*//
    //*---------------------------- START ----------------------------*//
    //*---------------------------------------------------------------*//

    void Start()
    {
        resourcePanel = transform.Find("resourcePanel").gameObject;
        wavesTMP      = transform.Find("resourcePanel/wavesTMP").gameObject;
        resourcesTMP  = transform.Find("resourcePanel/resourcesTMP").gameObject;

        skipButton    = transform.Find("skipButton").gameObject;
        skipButtonTMP = transform.Find("skipButton/skipButtonTMP").gameObject;
        skipTimerIMG  = transform.Find("skipButton/skipTimerIMG").gameObject;

        pauseButton = transform.Find("pauseButton").gameObject;
        pauseIMG    = transform.Find("pauseButton/pauseIMG").gameObject;
        playIMG     = transform.Find("pauseButton/playIMG").gameObject;

        accelerateButton = transform.Find("accelerateButton").gameObject;
        accelerateTMP    = transform.Find("accelerateButton/accelerateTMP").gameObject;
    
        levelFinishedPanel = transform.Find("levelFinishedPanel").gameObject;
        levelFinishedTMP   = transform.Find("levelFinishedPanel/levelFinishedTMP").gameObject;
        menuButton         = transform.Find("levelFinishedPanel/menuButton").gameObject;
        menuButtonTMP      = transform.Find("levelFinishedPanel/menuButton/menuButtonTMP").gameObject;
        experienceTMP      = transform.Find("levelFinishedPanel/experienceTMP").gameObject;
        firstStarIMG       = transform.Find("levelFinishedPanel/firstStarIMG").gameObject;
        secondStarIMG      = transform.Find("levelFinishedPanel/secondStarIMG").gameObject;
        thirdStarIMG       = transform.Find("levelFinishedPanel/thirdStarIMG").gameObject;  

        tower1Button = transform.Find("tower1Button").gameObject;
        // ...

        buildingSubmenu = transform.Find("buildingSubmenu").gameObject;
        sellButton      = transform.Find("buildingSubmenu/sellButton").gameObject;
        repairButton    = transform.Find("buildingSubmenu/repairButton").gameObject;
        upgradeButton   = transform.Find("buildingSubmenu/upgradeButton").gameObject;

        splashAttackButton = transform.Find("splashAttackButton").gameObject;
        uniformAttackButton = transform.Find("uniformAttackButton").gameObject;

        splashAttackTimerIMG = transform.Find("splashAttackButton/splashAttackTimerIMG").gameObject;
        uniformAttackTimerIMG = transform.Find("uniformAttackButton/uniformAttackTimerIMG").gameObject;

        levelLogic = levelLogicGameObject.GetComponent<LevelLogic>();

        interactionMode = InteractionMode.None;
        typeBuilding = TypeBuilding.Tower1;
    }  

    //*---------------------------------------------------------------*//
    //*---------------------------- UPDATE ---------------------------*//
    //*---------------------------------------------------------------*//

    void Update()
    {
        HandleResourcePanel();
        HandleAccelerateButton();
        HandleSkipButton();
        HandleFinishedLevel();
        HandleMouseInputs();
        HandleSpecialAttack();
    }

    //*---------------------------------------------------------------*//
    //*--------------------------- HANDLERS --------------------------*//
    //*---------------------------------------------------------------*//

    void HandleResourcePanel() {
        wavesTMP.GetComponent<TMP_Text>().text = 
            "wave: " + levelLogic.GetCurrentWave() 
                     + " / " 
                     + levelLogic.GetTotalWaves();
        
        resourcesTMP.GetComponent<TMP_Text>().text = 
            levelLogic.GetCurrentResources() + "";
    }

    //-----------------------------------------------------------------//

    void HandleSkipButton() {
        skipTimerIMG.GetComponent<Image>().fillAmount = levelLogic.GetWaveTimer();
        skipButton.SetActive(
            !levelLogic.InWave() && !levelLogic.LevelFinished()
        );
    }

    //-----------------------------------------------------------------//

    void HandleAccelerateButton() {
        if (GameTime.IsPaused()) return;
        
        accelerateTMP.GetComponent<TMP_Text>().text = 
            "x" + (int)GameTime.GameSpeed;
        
    }

    //-----------------------------------------------------------------//

    void HandleFinishedLevel() {
        if (!levelLogic.LevelFinished()) return;

        levelFinishedPanel.SetActive(true);

        experienceTMP.GetComponent<TMP_Text>().text = 
            "Obtained  XP: " + levelLogic.ObtainedExp();

        if (levelLogic.ObtainedStars() >= 1) {
            firstStarIMG.GetComponent<RawImage>().color = new Color(1,1,1,1);
        }
        if (levelLogic.ObtainedStars() >= 2) {
            secondStarIMG.GetComponent<RawImage>().color = new Color(1,1,1,1);
        }
        if (levelLogic.ObtainedStars() >= 3) {
            thirdStarIMG.GetComponent<RawImage>().color = new Color(1,1,1,1);
        }
    }

    //-----------------------------------------------------------------//

    public void LoadMap(){
        SceneManager.LoadScene("mainMenu");
    }

    //-----------------------------------------------------------------//

    void HandleMouseInputs() {
        if (Input.GetMouseButtonDown(0)) {
            LeftClick();
        }
    }

    //-----------------------------------------------------------------//
    void HandleSpecialAttack(){
        if(!levelLogic.IsSpecialAttackAvailable(TypeAttack.SplashAttack)){
            splashAttackTimerIMG.GetComponent<Image>().fillAmount = 1-levelLogic.GetSpecialAttackTimer(TypeAttack.SplashAttack);
            splashAttackButton.GetComponent<Button>().interactable = false;
        }else{
            splashAttackButton.GetComponent<Button>().interactable = !GameTime.IsPaused();
        }
        if(!levelLogic.IsSpecialAttackAvailable(TypeAttack.UniformAttack)){
            uniformAttackTimerIMG.GetComponent<Image>().fillAmount = 1-levelLogic.GetSpecialAttackTimer(TypeAttack.UniformAttack);
            uniformAttackButton.GetComponent<Button>().interactable = false;
        }else{
            uniformAttackButton.GetComponent<Button>().interactable = !GameTime.IsPaused();
        }
    }

    //*---------------------------------------------------------------*//
    //*---------------------- PAUSE/ACC BUTTONS ----------------------*//
    //*---------------------------------------------------------------*//

    public void TogglePauseButton() {
        playIMG.SetActive(GameTime.IsPaused());
        pauseIMG.SetActive(!GameTime.IsPaused());

        skipButton.GetComponent<Button>().interactable = !GameTime.IsPaused();
        accelerateButton.GetComponent<Button>().interactable = !GameTime.IsPaused();
        tower1Button.GetComponent<Button>().interactable = !GameTime.IsPaused();
        // splashAttackButton.GetComponent<Button>().interactable = !GameTime.IsPaused();
        // uniformAttackButton.GetComponent<Button>().interactable = !GameTime.IsPaused();
        if (GameTime.IsPaused()) {
            levelLogic.HideBuildingTiles();
            levelLogic.DestroySpecialAttack();
            interactionMode = InteractionMode.None;
        }
    }

    //-----------------------------------------------------------------//

    public void PauseGame(){
        levelLogic.PauseGame();
    }

    //-----------------------------------------------------------------//

    public void AccelerateGame() {
        levelLogic.AccelerateGame();
    }

    //*---------------------------------------------------------------*//
    //*------------------------ BUILDING MODE ------------------------*//
    //*---------------------------------------------------------------*//

    public void ToggleBuildingMode(TypeBuilding type) {
        interactionMode = interactionMode != InteractionMode.Build 
                            ? InteractionMode.Build 
                            : InteractionMode.None;
        typeBuilding = type;

        Debug.Log("Interaction mode: " + interactionMode);
        Debug.Log("Type building: "    + type);

        if (interactionMode == InteractionMode.Build) {
            levelLogic.ShowBuildingTiles();
            levelLogic.DestroySpecialAttack();
        } else {
            levelLogic.HideBuildingTiles();
        }
    }

    //-----------------------------------------------------------------//

    public void ToggleBuildingModeTower1() {
        ToggleBuildingMode(TypeBuilding.Tower1);
    }

    //-----------------------------------------------------------------//

    public void ToggleBuildingModeTower2() {
        ToggleBuildingMode(TypeBuilding.Tower2);
    }

    //-----------------------------------------------------------------//

    public void ToggleBuildingModeTower3() {
        ToggleBuildingMode(TypeBuilding.Tower3);
    }

    //-----------------------------------------------------------------//

    public void ToggleBuildingModeGenerator() {
        ToggleBuildingMode(TypeBuilding.Generator);
    }

    //*---------------------------------------------------------------*//
    //*---------------------- BUILDING SUBMENU -----------------------*//
    //*---------------------------------------------------------------*//

    void HideBuildingSubmenu() {
        buildingSubmenu.SetActive(false);
    }

    //-----------------------------------------------------------------//

    void ShowBuildingSubmenu() {
        Vector3 screenPos = mainCamera
                            .GetComponent<Camera>()
                            .WorldToScreenPoint(selectedBuilding.transform.position);

        buildingSubmenu.transform.position = screenPos;
        buildingSubmenu.SetActive(true);
    }
    
    //-----------------------------------------------------------------//

    public void SellBuilding() {
        levelLogic.Sell(selectedBuilding);
    }

    //-----------------------------------------------------------------//

    public void RepairBuilding() {
        levelLogic.Repair(selectedBuilding);
    }

    //-----------------------------------------------------------------//

    public void UpgradeBuilding() {
        levelLogic.Upgrade(selectedBuilding);
    }

    //*---------------------------------------------------------------*//
    //*----------------------- SPECIAL ATTACKS -----------------------*//
    //*---------------------------------------------------------------*//

    public void ToggleSpecialAttackMode(TypeAttack type) {
        interactionMode = interactionMode != InteractionMode.SpecialAttack 
                            ? InteractionMode.SpecialAttack 
                            : InteractionMode.None;

        Debug.Log("Interaction mode: "+interactionMode);
        Debug.Log("Type attack: "+type);

        if (interactionMode == InteractionMode.SpecialAttack) {
            levelLogic.InitialiseSpecialAttack(type,mainCamera.GetComponent<Camera>());
            levelLogic.HideBuildingTiles();
        } else {
            levelLogic.DestroySpecialAttack();
        }
    }

    //-----------------------------------------------------------------//

    public void ToggleSplashAttack() {
        ToggleSpecialAttackMode(TypeAttack.SplashAttack);
    }

    //-----------------------------------------------------------------//

    public void ToggleUniformAttack() {
        ToggleSpecialAttackMode(TypeAttack.UniformAttack);
    }


    //*---------------------------------------------------------------*//
    //*---------------------- LEFT CLICK METHOD ----------------------*//
    //*---------------------------------------------------------------*//

    public void LeftClick() {
        Ray ray = mainCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        //Debug.DrawRay(mainCamera.GetComponent<Camera>().transform.position, ray.direction*100);
        RaycastHit hit;
        if(!Physics.Raycast(ray,out hit)) return;
        
        if (GameTime.IsPaused() || Utils.IsPointerOverUIObject()) {
            return;
        }

        if (interactionMode == InteractionMode.Build) {
            BuildingTile tile = hit.collider.gameObject.GetComponent<BuildingTile>();
            if(tile!=null){
                Debug.Log("Tile");
                levelLogic.Build(tile, typeBuilding);
            }
        }

        if (interactionMode == InteractionMode.None || interactionMode == InteractionMode.Build) {
            Building currentBuilding = hit.collider.gameObject.GetComponent<Building>();
            if (currentBuilding != null && currentBuilding != selectedBuilding) {
                selectedBuilding = currentBuilding;
                ShowBuildingSubmenu();
            } else {
                HideBuildingSubmenu();
                selectedBuilding = null;
            }
        }

        if (interactionMode == InteractionMode.SpecialAttack) {
            levelLogic.DeploySpecialAttack();
            interactionMode = InteractionMode.None;
        }
    }

    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------- END OF LevelCanvas ---------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//
}