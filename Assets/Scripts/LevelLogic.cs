using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class LevelLogic : MonoBehaviour
{
    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------- CLASS ATTRIBUTES -----------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//
    public int[,] waves = {{1,0,0,0},{1,0,0,0},{1,0,0,0},{1,0,0,0}};

    // Posición de la base en el nivel
    [SerializeField] BuildingTile BASE_POSITION;

    // Todas las posiciones donde se pueden colocar una torre o generador en el nivel
    public BuildingTile[] ALL_POSITIONS;

    public int currentWave;
    public int destroyedTowers;
    public int currentResources;
    int accWaveResources;
    float accWaveTimer;
    // bool levelFinished;
    
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
    [SerializeField] GameObject enemyPrefab1;
    [SerializeField] GameObject enemyPrefab2;
    [SerializeField] GameObject enemyPrefab3;
    [SerializeField] GameObject enemyPrefab4;
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
        destroyedTowers  = 0;
        currentWave      = 0;
        currentResources = INITIAL_RESOURCES;
        accWaveTimer     = ACC_WAVE_MAX_TIME;
        enemySpawn       = null;
        ALL_POSITIONS    = FindObjectsOfType<BuildingTile>(true);
        Debug.Log(ALL_POSITIONS.Length);
    }

    //*---------------------------------------------------------------*//
    //*---------------------------- UPDATE ---------------------------*//
    //*---------------------------------------------------------------*//

    void Update()
    {   
        HandleWaves();
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
        if  (currentWave == waves.GetLength(0) && !InWave()) {
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
        List<ValueTuple<GameObject,int>> enemyprefabs = new List<ValueTuple<GameObject,int>> {
            new ValueTuple<GameObject,int> (enemyPrefab1, waves[currentWave,0]),
            new ValueTuple<GameObject,int> (enemyPrefab2, waves[currentWave,1]),
            new ValueTuple<GameObject,int> (enemyPrefab3, waves[currentWave,2]),
            new ValueTuple<GameObject,int> (enemyPrefab4, waves[currentWave,3])
        };
        enemySpawn = new EnemySpawner(enemyprefabs, splines, this);
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
        // currentResources += b.GetSellingPrice();
        // b.Sell();
    }

    //-----------------------------------------------------------------//

    public void Repair(Building b) {
        // int price = b.GetRepairPrice();
        // if (price > currentResources) return;
        //
        // currentResources -= price;
        // b.Repair();
    }

    //-----------------------------------------------------------------//

    public void Upgrade(Building b) {
        // int price = b.GetRepairPrice();
        // if (price > currentResources) return;
        //
        // currentResources -= price;
        // b.Repair();
    }

    //-----------------------------------------------------------------//

    public void Build(BuildingTile tile, TypeBuilding type){
        
        if (!tile.IsEmpty()) return;

        int price;
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
        tile.Build(buildingPrefab);
    }

    //*---------------------------------------------------------------*//
    //*---------------------- GETTERS & SETTERS ----------------------*//
    //*---------------------------------------------------------------*//

    public int GetCurrentWave() {
        return currentWave;
    }

    //-----------------------------------------------------------------//

    public int GetTotalWaves() {
        return waves.GetLength(0);
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
    
    public void InitialiseSpecialAttack(TypeAttack typeAttack,Camera mainCamera) {
        switch(typeAttack){
            case TypeAttack.UniformAttack:
                specialAttack = Instantiate(uniformAttackPrefab,Vector3.zero,Quaternion.identity).GetComponent<SpecialAttack>();
                // specialAttack.Initialise(mainCamera);
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

    public void DestroySpecialAttack() {
        if(specialAttack!=null) { //! puede que pete
            Destroy(specialAttack.gameObject);
            specialAttack = null;
        }
    }

    //-----------------------------------------------------------------//

    public void DeploySpecialAttack() {
        specialAttack.Deploy();
        specialAttack = null;
    }

    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------- END OF LevelLogic ----------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//
}