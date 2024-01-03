using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class LevelLogic : MonoBehaviour
{
    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------- CLASS ATTRIBUTES -----------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//

	public enum InteractionMode{
        Build, SpecialAttack, None
    };

    InteractionMode interactionMode;

    // public int[,] waves = {{1,0,0,0},{1,0,0,0},{1,0,0,0},{1,0,0,0}};
    [HideInInspector] public List<int> waveList;
    // int waveCount = 4;

    // Posición de la base en el nivel
    // [SerializeField] BuildingTile BASE_POSITION;

    // Todas las posiciones donde se pueden colocar una torre o generador en el nivel
    BuildingTile[] ALL_POSITIONS;

    public int currentWave;
    public int destroyedTowers;
    public int currentResources;
    int accWaveResources;
    float accWaveTimer;
    float splashAttackTimer;
    float uniformAttackTimer;
    EnemySpawner enemySpawn;
    SpecialAttack specialAttack;
    
    // Experiencia máxima obtenida por estrella
    [SerializeField] int STAR_XP;
    [SerializeField] int ACC_WAVE_MAX_RESOURCES;
    [SerializeField] int ACC_WAVE_MAX_TIME;
    [SerializeField] int INITIAL_RESOURCES;

    [SerializeField] List<GameObject> splines;

    [SerializeField] GameObject towerPrefab;
    [SerializeField] GameObject generatorPrefab;

    [SerializeField] List<GameObject> enemyPrefabList;
    // [SerializeField] GameObject enemyPrefab1;
    // [SerializeField] GameObject enemyPrefab2;
    // [SerializeField] GameObject enemyPrefab3;
    // [SerializeField] GameObject enemyPrefab4;
    [SerializeField] GameObject splashAttackPrefab;
    [SerializeField] GameObject uniformAttackPrefab;
    
    
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
		interactionMode     = InteractionMode.None;

        destroyedTowers     = 0;
        currentWave         = 0;
        currentResources    = INITIAL_RESOURCES;
        accWaveTimer        = ACC_WAVE_MAX_TIME;
        uniformAttackTimer  = 0;
        splashAttackTimer   = 0;
        enemySpawn          = null;
        ALL_POSITIONS       = FindObjectsOfType<BuildingTile>(true);
		foreach(BuildingTile buildingTile in ALL_POSITIONS) {
			buildingTile.Initialise(this);
		}
        // Debug.Log(ALL_POSITIONS.Length);
    }

    //*---------------------------------------------------------------*//
    //*---------------------------- UPDATE ---------------------------*//
    //*---------------------------------------------------------------*//

    void Update()
    {   
        HandleWaves();
        HandleSpecialAttacks();
    }

	//*---------------------------------------------------------------*//
    //*----------------------- INTERACTION MODE ----------------------*//
    //*---------------------------------------------------------------*//

	public InteractionMode GetInteractionMode() {
		return interactionMode;
	}

	public void SetInteractionMode(InteractionMode interactionMode) {
		this.interactionMode = interactionMode;
	}

    //*---------------------------------------------------------------*//
    //*------------------------- WAVE HANDLER ------------------------*//
    //*---------------------------------------------------------------*//

    void HandleWaves(){

        if (enemySpawn != null) {
           if(!enemySpawn.Update()) {
                enemySpawn = null;
            }
        }

        // Si hay enemigos
        if (Enemy.GetCount() != 0) return; // TODO hay que ver quien gestiona los recursos de los generadores

        // Si estamos en la ultima oleada y no hay enemigos
        if  (currentWave == GetTotalWaves() && !InWave()) {
            // levelFinished = true;
            return;
        }
        
        // Actualizamos el contador
        accWaveTimer -= GameTime.DeltaTime;

        if (accWaveTimer <= 0) {
            StartWave();
        }
    }

    //-----------------------------------------------------------------//

    public void StartWave() {
        if (GameTime.IsPaused()) return;

        InitialiseSpawner();
        currentWave++;
        accWaveResources = ACC_WAVE_MAX_RESOURCES * (int)accWaveTimer / ACC_WAVE_MAX_TIME; 
        currentResources += accWaveResources;
        accWaveTimer = ACC_WAVE_MAX_TIME;
    }

    //-----------------------------------------------------------------//

    void InitialiseSpawner() {
        // List<ValueTuple<GameObject,int>> enemyprefabs = new List<ValueTuple<GameObject,int>> {
        //     new ValueTuple<GameObject,int> (enemyPrefab1, GetEnemyAtWaveOfType(currentWave,0)),
        //     new ValueTuple<GameObject,int> (enemyPrefab2, GetEnemyAtWaveOfType(currentWave,1)),
        //     new ValueTuple<GameObject,int> (enemyPrefab3, GetEnemyAtWaveOfType(currentWave,2)),
        //     new ValueTuple<GameObject,int> (enemyPrefab4, GetEnemyAtWaveOfType(currentWave,3))
        // };
        List<ValueTuple<GameObject,int>> enemyPrefabTuples = new List<ValueTuple<GameObject,int>>();
        for(int i = 0; i < GetEnemyTypeCount();i++){
            enemyPrefabTuples.Add(new ValueTuple<GameObject,int> (enemyPrefabList[i], GetEnemyAtWaveOfType(currentWave,i)));
        }
        enemySpawn = new EnemySpawner(enemyPrefabTuples, splines, this);
    }


    //*---------------------------------------------------------------*//
    //*------------------------- XP AND STARS ------------------------*//
    //*---------------------------------------------------------------*//

    public int ObtainedStars(){
        int numberOfStars = 1;

        if (destroyedTowers == 0) {
            numberOfStars++;
        }
        if (/*!Base.HasBeenDamaged()*/true) {
            numberOfStars++;
        }

        return numberOfStars;
    }

    //-----------------------------------------------------------------//

    public int ObtainedExp(){
        float factorExp = destroyedTowers == 1 ? 0.5f : 0f;

        return (int)((ObtainedStars() + factorExp) * STAR_XP);
    }

    //*---------------------------------------------------------------*//
    //*----------------------- BUILDING METHODS ----------------------*//
    //*---------------------------------------------------------------*//

    public void ShowBuildingTiles(){
        foreach (BuildingTile bt in ALL_POSITIONS) {
            if (bt.IsEmpty()) {
                bt.Show();
            }
        }
    }

    //-----------------------------------------------------------------//

    public void HideBuildingTiles(){
        foreach (BuildingTile bt in ALL_POSITIONS) {
            bt.Hide();
        }
    }

    public void Sell(Building b) {
        currentResources += b.GetSellingPrice();
        b.SellBuilding();
    }

    //-----------------------------------------------------------------//

    public void Repair(Building b) {
        int price = ((Tower) b).GetRepairPrice();
        if (price > currentResources) return;
        
        currentResources -= price;
        ((Tower) b).RepairTower();
    }

    //-----------------------------------------------------------------//

    public void Upgrade(Building b) {
        int price = ((Tower) b).GetRepairPrice();
        if (price > currentResources) return;
        
        currentResources -= price;
        ((Tower) b).UpgradeTower();
    }

    //-----------------------------------------------------------------//

    public void Build(BuildingTile tile, TypeBuilding type){
        
        if (!tile.IsEmpty()) return;

        int price; //TODO
        GameObject buildingPrefab;
        
        if (type == TypeBuilding.Tower1) {
            // price = Tower.GetPurchasePrice();
            buildingPrefab = towerPrefab;
        } else {
            // price = Generator.GetPurchasePrice();
            buildingPrefab = generatorPrefab;
        }

        // if (price > currentResources) return;
        // currentResources -= price;
        tile.Build(type, buildingPrefab);
    }

    //*---------------------------------------------------------------*//
    //*---------------------- GETTERS & SETTERS ----------------------*//
    //*---------------------------------------------------------------*//

    public int GetCurrentWave() {
        return currentWave;
    }

    //-----------------------------------------------------------------//

    public int GetTotalWaves() {
        if(enemyPrefabList.Count <= 0) return 0;
        return waveList.Count / enemyPrefabList.Count;
    }

    //-----------------------------------------------------------------//

    public int GetCurrentResources() {
        return currentResources;
    }

    //-----------------------------------------------------------------//

    public void AddResources(int resources) {
        currentResources += resources;
    }

    //-----------------------------------------------------------------//

    public float GetWaveTimer() {
        return accWaveTimer / ACC_WAVE_MAX_TIME;
    }  

    //-----------------------------------------------------------------//

    public int GetEnemyTypeCount(){
        return enemyPrefabList.Count;
    }

    //-----------------------------------------------------------------//

    int GetEnemyAtWaveOfType(int wave,int enemyType){
        Debug.Log("waveList " + wave + " total " + GetTotalWaves() + " type " + enemyType);
        return waveList[wave * GetEnemyTypeCount() + enemyType];
    }

    //*---------------------------------------------------------------*//
    //*------------------------ BUTTON METHODS -----------------------*//
    //*---------------------------------------------------------------*//

    public void PauseGame() {
        if (GameTime.IsPaused()) {
            GameTime.Resume();
        } else {
            GameTime.Pause();
        }
    }

    //-----------------------------------------------------------------//

    public void AccelerateGame() {
        GameTime.GameSpeed = 
            (GameTime.GameSpeed == 4) ? (1) : (GameTime.GameSpeed * 2);
    }

    //*---------------------------------------------------------------*//
    //*---------------------------- FLAGS ----------------------------*//
    //*---------------------------------------------------------------*//
    
    public bool InWave() {
        return enemySpawn != null || Enemy.GetCount() != 0;
    }

    //-----------------------------------------------------------------//

    public bool LevelFinished() {
        return currentWave == GetTotalWaves() && !InWave();
    }

    //*---------------------------------------------------------------*//
    //*----------------------- SPECIAL ATTACKS -----------------------*//
    //*---------------------------------------------------------------*//
    

    void HandleSpecialAttacks(){
        if(uniformAttackTimer>0){
            uniformAttackTimer-=GameTime.DeltaTime;
        }
        if(splashAttackTimer>0){
            splashAttackTimer-=GameTime.DeltaTime;
        }
    }

    //-----------------------------------------------------------------//
    public void InitialiseSpecialAttack(TypeAttack typeAttack,Camera mainCamera) {
        switch(typeAttack){
            case TypeAttack.UniformAttack:
                specialAttack = Instantiate(uniformAttackPrefab,Vector3.zero,Quaternion.identity).GetComponent<SpecialAttack>();
                break;
            case TypeAttack.SplashAttack:
                specialAttack = Instantiate(splashAttackPrefab, Vector3.zero, Quaternion.identity).GetComponent<SpecialAttack>();
                break;
            default:
                return;
        }

        specialAttack.Initialise(mainCamera);
    }

    //-----------------------------------------------------------------//
    
    public bool IsSpecialAttackAvailable(TypeAttack typeAttack){
        switch(typeAttack){
            case TypeAttack.UniformAttack:
                return uniformAttackTimer<=0;
            case TypeAttack.SplashAttack:
                return splashAttackTimer<=0;
        }
        return false;
    }

    //-----------------------------------------------------------------//
    
    public float GetSpecialAttackTimer(TypeAttack typeAttack){
        switch(typeAttack){
            case TypeAttack.UniformAttack:
                return uniformAttackTimer/UniformAttack.GetCooldown();
            case TypeAttack.SplashAttack:
                return splashAttackTimer/SplashAttack.GetCooldown();
        }
        return 1;
    }

    //-----------------------------------------------------------------//

    public void DestroySpecialAttack() {
        if(specialAttack!=null) { //! puede que pete
            Destroy(specialAttack.gameObject);
            specialAttack = null;
        }
    }

    //-----------------------------------------------------------------//

    public void DeploySpecialAttack() {
        specialAttack.Deploy();
        if(specialAttack.GetType()==typeof(SplashAttack)){
            splashAttackTimer = SplashAttack.GetCooldown();
        }else if(specialAttack.GetType()==typeof(UniformAttack)){
            uniformAttackTimer = UniformAttack.GetCooldown();
        }
        specialAttack = null;
    }

    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------- END OF LevelLogic ----------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//
}