using System.Collections;
using System.Collections.Generic;
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
    public int destroyedTower;
    
    // Número actual de recursos disponibles
    public int currentResources;
    
    // Experiencia máxima obtenida por estrella
    int STAR_XP;

    // Experiencia total obtenida tras finalizar el nivel
    public int obtainedExp;


    int ACC_WAVE_MAX_RESOURCES;
    
    int accWaveResources;

    int ACC_WAVE_MAX_TIME;

    int acc_wave_timer;

    
    // Enemy[] enemies;

    int enemiesLeft;
    
    void Start()
    {
        // Necesitamos un array de Towers?: Tower[] towers;
        // towers = new Tower[MAX_TOWERS];

        // WAVES = new Wave[MAX_WAVES];

        // PATHS = new Path[MAX_PATHS];

        // destroyedTowers = 0;

        // resources = INITIAL_RESOURCES;

        // Se iniciliza acc_wave_timer
        // accWaveTimer = ACC_WAVE_MAX_TIME;

        // enemiesLeft = 0;
        
    }

    
    void Update()
    {   
        HandleTowers();
        HandleWaves();
        
    }

    void HandleWaves(){
        /* PSEUDOCÓDIGO LANZADOR DE OLEADAS Y TIMER */

        // Si Enemy.GetCount() == 0
            // accWaveTimer--;
            // Si accWaveTimer == 0 || pulsamos el botón de accelerate wave
                // Se lanza la oleada. (NUM_ENEMIES) 
                // currentWave++;
                // accWaveResources = accWaveTimer * ACC_WAVE_MAX_RESOURCES / ACC_WAVE_MAX_TIME; 
                // currentResources += accWaveResources;
                // accWaveTimer = ACC_WAVE_MAX_TIME;
        // En caso de que SI haya oleada...
            // Actualización de recursos segun número de generadores.
    }

    void HandleTowers() 
    {
        /* PSEUDCODIGO FUNCION HANDLER-TOWER */

        // for-each (t in towers)
            // Si enemigo destruye Tower t
                // t.destroy();
                // towers.remove(t);
            // Else Si pulsamos el botón de verder Tower t
                // currentResources += t.sell();
            // Else Si pulsamos el botón reparar sobre Tower t
                // currentResourse -= t.repair();
    }

    void HandleEnemy() 
    {
        // for-earch (e in enemy)
    }
}
