using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
		tower2Button = transform.Find("tower2Button").gameObject;
		tower3Button = transform.Find("tower3Button").gameObject;
		generatorButton = transform.Find("generatorButton").gameObject;

        buildingSubmenu = transform.Find("buildingSubmenu").gameObject;
        sellButton      = transform.Find("buildingSubmenu/sellButton").gameObject;
        repairButton    = transform.Find("buildingSubmenu/repairButton").gameObject;
        upgradeButton   = transform.Find("buildingSubmenu/upgradeButton").gameObject;

        splashAttackButton = transform.Find("splashAttackButton").gameObject;
        uniformAttackButton = transform.Find("uniformAttackButton").gameObject;

        splashAttackTimerIMG = transform.Find("splashAttackButton/splashAttackTimerIMG").gameObject;
        uniformAttackTimerIMG = transform.Find("uniformAttackButton/uniformAttackTimerIMG").gameObject;

        levelLogic = levelLogicGameObject.GetComponent<LevelLogic>();

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
        }else if (!GameTime.IsPaused()){
            splashAttackButton.GetComponent<Button>().interactable = true;
        }
        if(!levelLogic.IsSpecialAttackAvailable(TypeAttack.UniformAttack)){
            uniformAttackTimerIMG.GetComponent<Image>().fillAmount = 1-levelLogic.GetSpecialAttackTimer(TypeAttack.UniformAttack);
            uniformAttackButton.GetComponent<Button>().interactable = false;
        }else if (!GameTime.IsPaused()){
            uniformAttackButton.GetComponent<Button>().interactable = true;
        }
    }

    //*---------------------------------------------------------------*//
    //*------------------------ BUTTON METHODS -----------------------*//
    //*---------------------------------------------------------------*//

    public void TogglePauseButton() {
        playIMG.SetActive(GameTime.IsPaused());
        pauseIMG.SetActive(!GameTime.IsPaused());

        skipButton.GetComponent<Button>().interactable = !GameTime.IsPaused();
        accelerateButton.GetComponent<Button>().interactable = !GameTime.IsPaused();
        tower1Button.GetComponent<Button>().interactable = !GameTime.IsPaused();
        tower2Button.GetComponent<Button>().interactable = !GameTime.IsPaused();
        tower3Button.GetComponent<Button>().interactable = !GameTime.IsPaused();
		generatorButton.GetComponent<Button>().interactable = !GameTime.IsPaused();
        splashAttackButton.GetComponent<Button>().interactable = !GameTime.IsPaused();
        uniformAttackButton.GetComponent<Button>().interactable = !GameTime.IsPaused();
		
        if (GameTime.IsPaused()) {
            levelLogic.HideBuildingTiles();
            levelLogic.DestroySpecialAttack();
            levelLogic.SetInteractionMode(LevelLogic.InteractionMode.None);
        }
    }

    //-----------------------------------------------------------------//

    public void ToggleBuildingMode(TypeBuilding type) {
        levelLogic.SetInteractionMode((levelLogic.GetInteractionMode() != LevelLogic.InteractionMode.Build)
			? LevelLogic.InteractionMode.Build
			: LevelLogic.InteractionMode.None
		);
        typeBuilding = type;

        Debug.Log("Interaction mode: " + levelLogic.GetInteractionMode());
        Debug.Log("Type building: "    + type);

        if (levelLogic.GetInteractionMode() == LevelLogic.InteractionMode.Build) {
            levelLogic.ShowBuildingTiles();
            levelLogic.DestroySpecialAttack();
        } else {
            levelLogic.HideBuildingTiles();
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

    //-----------------------------------------------------------------//

    public void ToggleSpecialAttackMode(TypeAttack type) {
		levelLogic.SetInteractionMode((levelLogic.GetInteractionMode() != LevelLogic.InteractionMode.SpecialAttack)
			? LevelLogic.InteractionMode.SpecialAttack
			: LevelLogic.InteractionMode.None
		);

        Debug.Log("Interaction mode: " + levelLogic.GetInteractionMode());
        Debug.Log("Type attack: "+type);

		if (levelLogic.GetInteractionMode() == LevelLogic.InteractionMode.SpecialAttack) {
            levelLogic.InitialiseSpecialAttack(type,mainCamera.GetComponent<Camera>());
            levelLogic.HideBuildingTiles();
        } else {
            levelLogic.DestroySpecialAttack();
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

    //-----------------------------------------------------------------//

    public void PauseGame(){
        levelLogic.PauseGame();
    }

    //-----------------------------------------------------------------//

    public void AccelerateGame() {
        levelLogic.AccelerateGame();
    }

    //-----------------------------------------------------------------//

    public void LeftClick() {
        Ray ray = mainCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        //Debug.DrawRay(mainCamera.GetComponent<Camera>().transform.position, ray.direction*100);
        RaycastHit hit;
        if(!Physics.Raycast(ray,out hit)) return;
        
        if (GameTime.IsPaused() || Utils.IsPointerOverUIObject()) {
            return;
        }

        if (levelLogic.GetInteractionMode() == LevelLogic.InteractionMode.Build) {
            BuildingTile tile = hit.collider.gameObject.GetComponent<BuildingTile>();
            if(tile!=null){
                // Debug.Log("Tile");
                levelLogic.Build(tile, typeBuilding);
            }
        }

        if (levelLogic.GetInteractionMode() == LevelLogic.InteractionMode.None
		||  levelLogic.GetInteractionMode() == LevelLogic.InteractionMode.Build) {
            Building currentBuilding = hit.collider.gameObject.GetComponent<Building>();
            if (currentBuilding != null && currentBuilding != selectedBuilding) {
                selectedBuilding = currentBuilding;
                if (selectedBuilding is Generator) ShowBuildingSubmenu(false);
				else ShowBuildingSubmenu(true, !((Tower) selectedBuilding).IsMaxUpgraded());
            } else {
                HideBuildingSubmenu();
                selectedBuilding = null;
            }
        }

        if (levelLogic.GetInteractionMode() == LevelLogic.InteractionMode.SpecialAttack) {
            levelLogic.DeploySpecialAttack();
            levelLogic.SetInteractionMode(LevelLogic.InteractionMode.None);
        }
    }

    //-----------------------------------------------------------------//

    public void SellBuilding() {
        levelLogic.Sell(selectedBuilding);
		HideBuildingSubmenu();
		selectedBuilding = null;
    }

    //-----------------------------------------------------------------//

    public void RepairBuilding() {
        levelLogic.Repair(selectedBuilding);
		HideBuildingSubmenu();
		selectedBuilding = null;
    }

    //-----------------------------------------------------------------//

    public void UpgradeBuilding() {
        levelLogic.Upgrade(selectedBuilding);
		HideBuildingSubmenu();
		selectedBuilding = null;
    }

    //-----------------------------------------------------------------//

    void HideBuildingSubmenu() {
        buildingSubmenu.SetActive(false);
    }

    //-----------------------------------------------------------------//

    void ShowBuildingSubmenu(bool allActions, bool canUpgrade = true) {
        Vector3 screenPos = mainCamera.GetComponent<Camera>().WorldToScreenPoint(selectedBuilding.transform.position);
        buildingSubmenu.transform.position = screenPos;
        buildingSubmenu.SetActive(true);

		if (allActions) {
			upgradeButton.GetComponent<Button>().interactable = canUpgrade;
			
			repairButton.SetActive(true);
			upgradeButton.SetActive(true);
		}
		else {
			repairButton.SetActive(false);
			upgradeButton.SetActive(false);
		}
    }

    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------- END OF LevelCanvas ---------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//
}
