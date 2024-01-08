using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    [Serializable]
    public class LevelInfo
    {
        [SerializeField] string levelName;
        [SerializeField] string sceneName;

        public void Load(){
            SceneManager.LoadScene(sceneName,LoadSceneMode.Single);
        }
    }
    [SerializeField] List<LevelInfo> levelList;
    [SerializeField] Save saveAsset;
    enum MenuMode{
        saveFiles,levelSelection,research,enteringLevel
    }

    [SerializeField] MenuMode menuMode = MenuMode.levelSelection;
    [SerializeField] GameObject cameraSupport;
    [SerializeField] GameObject planet;

    GameObject saveFilesMenu;
    GameObject mapMenu;
    GameObject researchMenu;
    // Image 
    LevelPoint selectedLevel;
    Image fade;
    AudioSource musicSource;

    // Start is called before the first frame update
    void Start()
    {
        selectedLevel = FindAnyObjectByType<LevelPoint>();
        saveFilesMenu = transform.Find("SavesMenu").gameObject;
        mapMenu = transform.Find("MapMenu").gameObject;
        researchMenu = transform.Find("ResearchMenu").gameObject;
        fade = transform.Find("Fade").gameObject.GetComponent<Image>();
        musicSource = transform.Find("Music").gameObject.GetComponent<AudioSource>();
        mapMenu.SetActive(false);
        researchMenu.SetActive(false);

        // Si el currentLevel != -1, es que se acaba de jugar un nivel
        if(saveAsset.GetCurrentLevel()!=-1){
            menuMode = MenuMode.levelSelection;
            saveAsset.SetCurrentLevel(-1);
            saveFilesMenu.SetActive(false);
            mapMenu.SetActive(true);
            selectedLevel = GetLastLevel();
            //! esto no es buena idea, si se cierra el juego mientras se está en un nivel, 
            //! automaticamente se cargará el archivo de guardado al abrir el juego de nuevo
        }
    }

    public bool IsShowingLevels(){
        return menuMode == MenuMode.levelSelection;
    }

    public void SetSelectedLevel(LevelPoint levelPoint){
        selectedLevel = levelPoint;
    }

    public LevelPoint GetSelectedLevel(){
        return selectedLevel;
    }

    [SerializeField] float CAMERA_LINEAR_SPEED = 1f;
    float CAMERA_ROTATION_SPEED = 0.8f;
    float FADE_SPEED = 1f;
    float MUSIC_MAX_VOLUME = 0.3f;
    int fadeMode = -1;
    void Update(){
        Vector3 camPos = cameraSupport.transform.position;
        Vector3 planetPos = planet.transform.position;
        Quaternion camRot = cameraSupport.transform.rotation;
        
        // Animacion de la camara al menu o al mapa
        if(menuMode == MenuMode.levelSelection || menuMode == MenuMode.research || menuMode == MenuMode.enteringLevel){
            camPos = Vector3.Lerp(camPos,planetPos,Time.deltaTime*CAMERA_LINEAR_SPEED);
            var targetRotation = Quaternion.FromToRotation(cameraSupport.transform.forward, planetPos-selectedLevel.gameObject.transform.position) * camRot;
            camRot = Quaternion.Slerp(camRot, targetRotation, Time.deltaTime * CAMERA_ROTATION_SPEED);
        }else if(menuMode == MenuMode.saveFiles){
            camPos = Vector3.Lerp(camPos,Vector3.zero,Time.deltaTime*CAMERA_LINEAR_SPEED);
            var targetRotation = Quaternion.FromToRotation(cameraSupport.transform.forward, Vector3.forward) * camRot;
            camRot = Quaternion.Slerp(camRot, targetRotation, Time.deltaTime * CAMERA_ROTATION_SPEED);
        }

        if((fadeMode == -1 && fade.color.a > 0)||(fadeMode == 1 && fade.color.a < 1)){
            fade.color = new Color(fade.color.r,fade.color.g,fade.color.b,fade.color.a + GameTime.DeltaTime*FADE_SPEED * fadeMode);
            musicSource.volume = (1 - fade.color.a)*MUSIC_MAX_VOLUME;
        }
        if(fadeMode == 1 && fade.color.a >= 1){
            levelList[saveAsset.GetCurrentLevel()].Load();
        }
        cameraSupport.transform.position = camPos;
        cameraSupport.transform.rotation = camRot;
    }

    public void PlayLevel(int level){
        saveAsset.SetCurrentLevel(level);
        menuMode = MenuMode.enteringLevel;
        fadeMode = 1;
        GetComponent<AudioSource>().Play();
    }

    public void LoadSaveFile(int n){
        // Creamos los save files necesarios
        while(n>=saveAsset.GetTotalSaves()){ //! TODO esto es inapropiado
            saveAsset.AddSaveFile();
        }
        // Selecionamos el file
        saveAsset.SetCurrentFile(n);
        menuMode = MenuMode.levelSelection;
        saveFilesMenu.SetActive(false);
        mapMenu.SetActive(true);

        // Seleccionamos el level mas alto
        selectedLevel = GetLastLevel();
    }

    public void OpenResearchMenu(){
        menuMode = MenuMode.research;
        mapMenu.SetActive(false);
        researchMenu.SetActive(true);
        researchMenu.GetComponent<ResearchCanvas>().UpdateValues();
        
    }

    public void CloseResearchMenu(){
        menuMode = MenuMode.levelSelection;
        researchMenu.SetActive(false);
        mapMenu.SetActive(true);
    }

    public void BackToMenu(){
        menuMode = MenuMode.saveFiles;
        mapMenu.SetActive(false);
        researchMenu.SetActive(false);
        saveFilesMenu.SetActive(true);
    }

    LevelPoint GetLastLevel(){
        LevelPoint[] allLevelPoints = FindObjectsOfType<LevelPoint>();
        LevelPoint levelPoint = null;

        int lastLevelIndex = saveAsset.GetSaveFile().GetLastFinishedLevel();
        lastLevelIndex = lastLevelIndex < levelList.Count - 1 ? lastLevelIndex+1 : lastLevelIndex; // Seleccionamos el siguiente nivel si está disponible

        for(int i = 0; i < allLevelPoints.Length;i++){
            levelPoint = allLevelPoints[i].GetId() == lastLevelIndex ? allLevelPoints[i] : selectedLevel;
        }
        return levelPoint;
    }

}
