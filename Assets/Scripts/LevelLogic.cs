using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class LevelLogic : MonoBehaviour
{

    public int[,] waves = {{1,1,1},{1,1,1},{1,1,1},{1,1,1}};

    // Posición de la base en el nivel
    [SerializeField]
    Vector3 BASE_POSITION;

    // Todas las posiciones donde se pueden colocar una torre o generador en el nivel
    public Vector3[] ALL_POSITIONS;

    // Posiciones ocupadas dentro del nivel. Se almacena un puntero a la instancia de la construcción que ocupa la posición.
    // Building[] occupiedPositions;
    
    // Caminos que los enemigos pueden seguir 
    // const Path[] PATHS;

    public int currentWave;

    // Número de torres destruidas hasta al momento
    public int destroyedTowers;
    
    // Número actual de recursos disponibles
    public int currentResources;
    
    // Experiencia máxima obtenida por estrella
    int STAR_XP;

    // Experiencia total obtenida tras finalizar el nivel
    public int obtainedExp;


    int ACC_WAVE_MAX_RESOURCES;
    
    int accWaveResources;

    int ACC_WAVE_MAX_TIME;

    float accWaveTimer;

    int INITIAL_RESOURCES;

    
    // Enemy[] enemies;

    int enemiesLeft;

    [SerializeField]
    List<GameObject> splines;

    [SerializeField]
    GameObject towerPrefab;

    [SerializeField]
    GameObject generatorPrefab;

    [SerializeField]
    GameObject enemyPrefab1;

    [SerializeField]
    GameObject enemyPrefab2;

    [SerializeField]
    GameObject enemyPrefab3;
    
    [SerializeField]
    GameObject enemyPrefab4;

    [SerializeField]
    GameObject enemyPrefabs;
    

    EnemySpawner enemySpawn;
    
    void Start()
    {
        // Necesitamos un array de Towers?: Tower[] towers;
        // towers = new Tower[MAX_TOWERS];

        // WAVES = new Wave[MAX_WAVES];

        // PATHS = new Path[MAX_PATHS];

        // Tower.destroyedTowers = 0; // lo hacen en la clase Tower, ¿no?

        currentResources = INITIAL_RESOURCES;

        // Se iniciliza acc_wave_timer
        accWaveTimer = ACC_WAVE_MAX_TIME;

        enemiesLeft = 0;

        currentWave = 0;

        obtainedExp = 0;
        
    }

    
    void Update()
    {   
        HandleWaves();
    }

    void HandleWaves(){
        /* PSEUDOCÓDIGO LANZADOR DE OLEADAS Y TIMER */

        // Si no hay enemigos
        if (Enemy.GetCount() != 0) return; //TODO hay que ver quien gestiona los recursos de los generadores

        // Si estamos en la ultima oleada y no hay enemigos
        if  (currentWave == waves.GetLength(0) - 1 && Enemy.GetCount() == 0) {
            // TODO
            obtainedExp = ObtainedExp();
            return;
        }
        
        // Actualizamos el contador
        accWaveTimer += Time.deltaTime;

        if (accWaveTimer == 0 /*|| pulsamos el boton de accelerate wave*/) {
            // TODO: Se lanza la oleada. (NUM_ENEMIES)
            SpawnWave();
            currentWave++;
            accWaveResources = ACC_WAVE_MAX_RESOURCES * (int)accWaveTimer / ACC_WAVE_MAX_TIME; 
            currentResources += accWaveResources;
            accWaveTimer = ACC_WAVE_MAX_TIME;
        }
    }

    private int ObtainedExp(){
        float factorExp = 1;

        if (destroyedTowers == 0) {
            factorExp++;
        }else if (destroyedTowers == 1) {
            factorExp+=0.5f;
        }

        // if (!Base.HasBeenDamaged()) {
        //     factorExp++;
        // }

        return  (int)(factorExp * STAR_XP);
    }

    void SpawnWave() {
        Tuple<GameObject, int>[] enemyprefabArr = {
            new Tuple<GameObject, int>(enemyPrefab1, waves[currentWave,0]),
            new Tuple<GameObject, int>(enemyPrefab2, waves[currentWave,1]),
            new Tuple<GameObject, int>(enemyPrefab3, waves[currentWave,2]),
            new Tuple<GameObject, int>(enemyPrefab4, waves[currentWave,3])
        };
        
        enemySpawn = new EnemySpawner(enemyprefabArr, splines);
    }

    void Sell(/*Building b*/) {
        // currentResources += b.GetSellingPrice();
        // b.Sell();
    }

    void Repair(/*Building b*/) {
        // int price = b.GetRepairPrice();
        // if (price > currentResources) return;
        //
        // currentResources -= price;
        // b.Repair();
    }

    void Build(BuildingTile tile, TypeBuilding type){
        
        if (!tile.IsEmpty()) return;

        int price;
        GameObject buildingPrefab;
        
        if (type == TypeBuilding.Tower1) {
            price = Tower.GetPurchasePrice();
            buildingPrefab = towerPrefab;
        }else {
            price = Generator.GetPurchasePrice();
            buildingPrefab = generatorPrefab;
        }

        if (price > currentResources) return;
        currentResources -= price;
        tile.Build(buildingPrefab);
    }

    void HandleEnemy() 
    {
        // for-earch (e in enemy)
    }
}
