using TMPro;
using UnityEngine;

public class LevelPoint : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Save saveAsset;

    SaveFile saveFile;
    [SerializeField] int levelId;
    Camera mainCamera ;
    MainMenu mainMenu;
    Material material;
    TMP_Text text;
    GameObject[] stars;
    GameObject playButton;
    Material playButtonMaterial;
    float viewMinDistance = 4;
    float viewMaxDistance = 4.1f;

    void Start()
    {
        saveFile = saveAsset.GetSaveFile();
        mainCamera = Camera.main;
        material = GetComponent<MeshRenderer>().material;
        
        mainMenu = FindAnyObjectByType<MainMenu>();
        text = GetComponentInChildren<TMP_Text>();

        stars = new GameObject[3];
        stars[0] = transform.Find("Star0").gameObject; 
        stars[1] = transform.Find("Star1").gameObject; 
        stars[2] = transform.Find("Star2").gameObject;
        playButton = transform.Find("Play").gameObject;

        playButtonMaterial = transform.Find("Play/Play").gameObject.GetComponent<MeshRenderer>().material;

        material.color = new Color(1,1,1,0);
        playButtonMaterial.color = new Color(1,1,1,0);
        HideInfo(); 
    }

    // Update is called once per frame
    void Update()
    {
        saveFile = saveAsset.GetSaveFile();
        transform.rotation = mainCamera.gameObject.transform.rotation;
        if(IsPlayableLevel()&&UpdateDistanceColor()){
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit,100)) {
                if(hit.collider.gameObject == gameObject){
                    material.color = new Color(0.8f,0.8f,0.8f);
                    if(Input.GetMouseButtonDown(0)){
                        mainMenu.SetSelectedLevel(this);
                    }
                }else if(hit.collider.gameObject == playButton){
                    playButtonMaterial.color = new Color(0.8f,0.8f,0.8f);
                    if(Input.GetMouseButtonDown(0)){
                        mainMenu.PlayLevel(levelId);
                    }
                }
            }
        }else{
            text.color = new Color(0,0,0,0);
            material.color = new Color(0,0,0,0);
            playButtonMaterial.color = new Color(0,0,0,0);
        }
    }
    float FIXED_SIZE = 0.0015f;
    bool UpdateDistanceColor(){
        float distance = Vector3.Distance(transform.position,mainCamera.gameObject.transform.position);
        float size = distance * FIXED_SIZE * mainCamera.fieldOfView;
        transform.localScale = Vector3.one * size;
        if(mainMenu.IsShowingLevels()&&mainMenu.GetSelectedLevel().GetId()==levelId){
            ShowInfo(saveFile.StarsAtLevel(levelId));
        }else{
            HideInfo();
        }
            
        
        float alpha = 1 - (distance - viewMinDistance) / (viewMaxDistance - viewMinDistance);
        alpha = alpha < 0 ? 0 : alpha;
        alpha = alpha > 1 ? 1 : alpha;
        alpha = mainMenu.IsShowingLevels() ? alpha : 0;
        material.color = new Color(1,1,1,alpha);
        playButtonMaterial.color = material.color; 
        text.color = new Color(0,0,0,alpha);
        text.text = levelId+1+"";
        return alpha>0;
    }

    void HideInfo(){
        for(int i = 0; i < stars.Length;i++){
            stars[i].SetActive(false);
        }
        playButton.SetActive(false);
    }

    void ShowInfo(int n){
        for(int i = 0; i < stars.Length && i < n;i++){
            stars[i].SetActive(true);
        }
        playButton.SetActive(true);
    }

    public int GetId(){
        return levelId;
    }

    bool IsPlayableLevel(){
        if(saveFile==null) return false;
        if(levelId == 0) return true;
        return saveFile.StarsAtLevel(levelId-1) > 0;
    }
}


