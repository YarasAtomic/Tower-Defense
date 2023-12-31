using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
        saveFiles,levelSelection,levelInfo
    }

    [SerializeField] MenuMode menuMode = MenuMode.levelSelection;
    [SerializeField] GameObject cameraSupport;
    [SerializeField] GameObject planet;

    GameObject saveFilesMenu;
    // Image 
    LevelPoint selectedLevel;

    // Start is called before the first frame update
    void Start()
    {
        selectedLevel = FindAnyObjectByType<LevelPoint>();
        saveFilesMenu = transform.Find("SavesMenu").gameObject;
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
    void Update(){
        Vector3 camPos = cameraSupport.transform.position;
        Vector3 planetPos = planet.transform.position;
        Quaternion camRot = cameraSupport.transform.rotation;
        
        if(menuMode == MenuMode.levelSelection){
            camPos = Vector3.Lerp(camPos,planetPos,Time.deltaTime*CAMERA_LINEAR_SPEED);
            var targetRotation = Quaternion.FromToRotation(cameraSupport.transform.forward, planetPos-selectedLevel.gameObject.transform.position) * camRot;
            camRot = Quaternion.Slerp(camRot, targetRotation, Time.deltaTime * CAMERA_ROTATION_SPEED);
        }else if(menuMode == MenuMode.saveFiles){
            camPos = Vector3.Lerp(camPos,Vector3.zero,Time.deltaTime*CAMERA_LINEAR_SPEED);
            var targetRotation = Quaternion.FromToRotation(cameraSupport.transform.forward, Vector3.forward) * camRot;
            camRot = Quaternion.Slerp(camRot, targetRotation, Time.deltaTime * CAMERA_ROTATION_SPEED);
        }
        cameraSupport.transform.position = camPos;
        cameraSupport.transform.rotation = camRot;
    }

    public void PlayLevel(int level){
        levelList[level].Load();
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

        // Seleccionamos el level mas alto
        LevelPoint[] allLevelPoints = FindObjectsOfType<LevelPoint>();

        int lastLevelIndex = saveAsset.GetSaveFile().GetLastFinishedLevel();
        lastLevelIndex = lastLevelIndex < levelList.Count - 1 ? lastLevelIndex+1 : lastLevelIndex; // Seleccionamos el siguiente nivel si está disponible

        for(int i = 0; i < allLevelPoints.Length;i++){
            selectedLevel = allLevelPoints[i].GetId() == lastLevelIndex ? allLevelPoints[i] : selectedLevel;
        }
        Debug.Log("lastLevelIndex " +lastLevelIndex + " length " + allLevelPoints.Length);
    }
}
