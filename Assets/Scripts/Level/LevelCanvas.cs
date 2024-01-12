using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

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

    GameObject levelGameOverPanel,
               levelFinishedPanel,
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
    GameObject cameraIMG,
               mapIMG;
    Image fade;
    int fadeMode = -1;
    float FADE_SPEED = 1;
    float MUSIC_MAX_VOLUME = 0.15f;
    AudioSource musicSource;
    [SerializeField] GameObject levelLogicGameObject;

    LevelLogic levelLogic;

    TypeBuilding typeBuilding;

    Building selectedBuilding;
    Camera mainCamera;
    Camera currentCamera;


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
        mainCamera = Camera.main;
        currentCamera = mainCamera;
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
    
        levelGameOverPanel = transform.Find("levelGameOverPanel").gameObject;
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

        cameraIMG  = transform.Find("cameraButton/cameraIMG").gameObject;
        mapIMG     = transform.Find("cameraButton/mapIMG").gameObject;

        fade = transform.Find("Fade").gameObject.GetComponent<Image>();

        levelLogic = levelLogicGameObject.GetComponent<LevelLogic>();

        typeBuilding = TypeBuilding.Tower1;

        musicSource = GetComponent<AudioSource>();
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
        HandleFadeInOut();
    }

    //*---------------------------------------------------------------*//
    //*--------------------------- HANDLERS --------------------------*//
    //*---------------------------------------------------------------*//

    void HandleFadeInOut(){
        if((fadeMode == -1 && fade.color.a > 0)||(fadeMode == 1 && fade.color.a < 1)){
            fade.color = new Color(fade.color.r,fade.color.g,fade.color.b,fade.color.a + GameTime.DeltaTime*FADE_SPEED * fadeMode);
            musicSource.volume = (1 - fade.color.a)*MUSIC_MAX_VOLUME;
        }
        if(fadeMode == 1 && fade.color.a >= 1){
            LoadMap();
        }
    }

    //-----------------------------------------------------------------//
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

        if(levelLogic.LevelGameOver()){
            levelGameOverPanel.SetActive(true);
        }

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
        }else{
            splashAttackButton.GetComponent<Button>().interactable = !GameTime.IsPaused() && !(levelLogic.GetInteractionMode()==LevelLogic.InteractionMode.Camera);
        }
        if(!levelLogic.IsSpecialAttackAvailable(TypeAttack.UniformAttack)){
            uniformAttackTimerIMG.GetComponent<Image>().fillAmount = 1-levelLogic.GetSpecialAttackTimer(TypeAttack.UniformAttack);
            uniformAttackButton.GetComponent<Button>().interactable = false;
        }else{
            uniformAttackButton.GetComponent<Button>().interactable = !GameTime.IsPaused() && !(levelLogic.GetInteractionMode()==LevelLogic.InteractionMode.Camera);
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
        tower1Button.GetComponent<Button>().interactable = !GameTime.IsPaused() && !(levelLogic.GetInteractionMode()==LevelLogic.InteractionMode.Camera);
        tower2Button.GetComponent<Button>().interactable = !GameTime.IsPaused() && !(levelLogic.GetInteractionMode()==LevelLogic.InteractionMode.Camera);
        tower3Button.GetComponent<Button>().interactable = !GameTime.IsPaused() && !(levelLogic.GetInteractionMode()==LevelLogic.InteractionMode.Camera);
		generatorButton.GetComponent<Button>().interactable = !GameTime.IsPaused() && !(levelLogic.GetInteractionMode()==LevelLogic.InteractionMode.Camera);
        splashAttackButton.GetComponent<Button>().interactable = !GameTime.IsPaused() && !(levelLogic.GetInteractionMode()==LevelLogic.InteractionMode.Camera);
        uniformAttackButton.GetComponent<Button>().interactable = !GameTime.IsPaused() && !(levelLogic.GetInteractionMode()==LevelLogic.InteractionMode.Camera);
		
        if (GameTime.IsPaused()) {
            levelLogic.HideBuildingTiles();
            levelLogic.DestroySpecialAttack();
            if(levelLogic.GetInteractionMode()!=LevelLogic.InteractionMode.Camera)
                levelLogic.SetInteractionMode(LevelLogic.InteractionMode.None);
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
        levelLogic.SetInteractionMode((levelLogic.GetInteractionMode() == LevelLogic.InteractionMode.Build && typeBuilding == type)
			? LevelLogic.InteractionMode.None
			: LevelLogic.InteractionMode.Build
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
		levelLogic.SetInteractionMode((levelLogic.GetInteractionMode() == LevelLogic.InteractionMode.SpecialAttack)
			? LevelLogic.InteractionMode.None
			: LevelLogic.InteractionMode.SpecialAttack
		);

        Debug.Log("Interaction mode: " + levelLogic.GetInteractionMode());
        Debug.Log("Type attack: "+type);

		if (levelLogic.GetInteractionMode() == LevelLogic.InteractionMode.SpecialAttack) {
            levelLogic.InitialiseSpecialAttack(type,mainCamera);
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

    //*---------------------------------------------------------------*//
    //*---------------------- BUILDING SUBMENU -----------------------*//
    //*---------------------------------------------------------------*//

    void HideBuildingSubmenu() {
        buildingSubmenu.SetActive(false);
    }

    //-----------------------------------------------------------------//

    void ShowBuildingSubmenu() {
        Vector3 screenPos = mainCamera
                            .WorldToScreenPoint(selectedBuilding.transform.position);

        buildingSubmenu.transform.position = screenPos;
        buildingSubmenu.SetActive(true);
    }
    
    //-----------------------------------------------------------------//

    // public void SellBuilding() {
    //     levelLogic.Sell(selectedBuilding);
    // }

    //-----------------------------------------------------------------//

    // public void RepairBuilding() {
    //     levelLogic.Repair(selectedBuilding);
    // }

    //-----------------------------------------------------------------//

    // public void UpgradeBuilding() {
    //     levelLogic.Upgrade(selectedBuilding);
    // }

    //*---------------------------------------------------------------*//
    //*------------------------- CAMERA MODE -------------------------*//
    //*---------------------------------------------------------------*//

    public void ToggleCameraMode(){
        levelLogic.SetInteractionMode((levelLogic.GetInteractionMode() == LevelLogic.InteractionMode.Camera)
			? LevelLogic.InteractionMode.None
			: LevelLogic.InteractionMode.Camera
		);
        if(levelLogic.GetInteractionMode()!=LevelLogic.InteractionMode.Camera){
            if(currentCamera != mainCamera) {
                cameraIMG.SetActive(true);
                mapIMG.SetActive(false);
                currentCamera.enabled = false;
                mainCamera.enabled = true;
            }
            currentCamera = mainCamera;
        }else{
            levelLogic.HideBuildingTiles();
        }

        tower1Button.GetComponent<Button>().interactable = !GameTime.IsPaused() && !(levelLogic.GetInteractionMode()==LevelLogic.InteractionMode.Camera);
        tower2Button.GetComponent<Button>().interactable = !GameTime.IsPaused() && !(levelLogic.GetInteractionMode()==LevelLogic.InteractionMode.Camera);
        tower3Button.GetComponent<Button>().interactable = !GameTime.IsPaused() && !(levelLogic.GetInteractionMode()==LevelLogic.InteractionMode.Camera);
		generatorButton.GetComponent<Button>().interactable = !GameTime.IsPaused() && !(levelLogic.GetInteractionMode()==LevelLogic.InteractionMode.Camera);
        splashAttackButton.GetComponent<Button>().interactable = !GameTime.IsPaused() && !(levelLogic.GetInteractionMode()==LevelLogic.InteractionMode.Camera);
        uniformAttackButton.GetComponent<Button>().interactable = !GameTime.IsPaused() && !(levelLogic.GetInteractionMode()==LevelLogic.InteractionMode.Camera);
    }

    //*---------------------------------------------------------------*//
    //*----------------------- SPECIAL ATTACKS -----------------------*//
    //*---------------------------------------------------------------*//

    // public void ToggleSpecialAttackMode(TypeAttack type) {
    //     interactionMode = interactionMode != InteractionMode.SpecialAttack 
    //                         ? InteractionMode.SpecialAttack 
    //                         : InteractionMode.None;

    //     Debug.Log("Interaction mode: "+interactionMode);
    //     Debug.Log("Type attack: "+type);

    //     if (interactionMode == InteractionMode.SpecialAttack) {
    //         levelLogic.InitialiseSpecialAttack(type,mainCamera.GetComponent<Camera>());
    //         levelLogic.HideBuildingTiles();
    //     } else {
    //         levelLogic.DestroySpecialAttack();
    //     }
    // }

    //-----------------------------------------------------------------//

    // public void ToggleSplashAttack() {
    //     ToggleSpecialAttackMode(TypeAttack.SplashAttack);
    // }

    //-----------------------------------------------------------------//

    // public void ToggleUniformAttack() {
    //     ToggleSpecialAttackMode(TypeAttack.UniformAttack);
    // }


    //*---------------------------------------------------------------*//
    //*---------------------- LEFT CLICK METHOD ----------------------*//
    //*---------------------------------------------------------------*//

    public void LeftClick() {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
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
				else if (selectedBuilding is Tower)  ShowBuildingSubmenu(true, ((Tower) selectedBuilding).GetHealthPercentage() < 1.0f, !((Tower) selectedBuilding).IsMaxUpgraded());
            } else {
                HideBuildingSubmenu();
                selectedBuilding = null;
            }
        }

        if (levelLogic.GetInteractionMode() == LevelLogic.InteractionMode.SpecialAttack) {
            levelLogic.DeploySpecialAttack();
            levelLogic.SetInteractionMode(LevelLogic.InteractionMode.None);
        }

        if(levelLogic.GetInteractionMode() == LevelLogic.InteractionMode.Camera && mainCamera == currentCamera){
            Enemy currentEnemy = hit.collider.gameObject.GetComponent<Enemy>();
            Tower currentTower = hit.collider.gameObject.GetComponent<Tower>();

            if (currentEnemy != null){
                cameraIMG.SetActive(false);
                mapIMG.SetActive(true);
                currentCamera = currentEnemy.GetCamera();
                currentCamera.enabled = true;
                mainCamera.enabled = false;
            }
			if (currentTower != null) {
				cameraIMG.SetActive(false);
                mapIMG.SetActive(true);
                currentCamera = currentTower.GetCamera();
                currentCamera.enabled = true;
                mainCamera.enabled = false;
			}
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

    // void HideBuildingSubmenu() {
    //     buildingSubmenu.SetActive(false);
    // }

    //-----------------------------------------------------------------//

    void ShowBuildingSubmenu(bool allActions, bool canRepair = true, bool canUpgrade = true) {
        Vector3 screenPos = mainCamera.WorldToScreenPoint(selectedBuilding.transform.position);
        buildingSubmenu.transform.position = screenPos;
        buildingSubmenu.SetActive(true);

		if (allActions) {
			repairButton.GetComponent<Button>().interactable = canRepair;
			upgradeButton.GetComponent<Button>().interactable = canUpgrade;
			
			repairButton.SetActive(true);
			upgradeButton.SetActive(true);
		}
		else {
			repairButton.SetActive(false);
			upgradeButton.SetActive(false);
		}
    }
    //*---------------------------------------------------------------*//
    //*---------------------- LOADING AND FADES ----------------------*//
    //*---------------------------------------------------------------*//

    public void EndLevel(){
        fadeMode = 1;
        Debug.Log("endlevel");
    }

    //-----------------------------------------------------------------//

    public void LoadMap(){
        GameTime.Resume();
        SceneManager.LoadScene("mainMenu");
    }

    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------- END OF LevelCanvas ---------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//
}
