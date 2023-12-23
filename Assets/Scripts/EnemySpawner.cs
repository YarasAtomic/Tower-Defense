using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class EnemySpawner
{    
    List<GameObject> splines;
    [SerializeField] float delay;

    [SerializeField] int maxEnemies;

    [SerializeField] int spawnedEnemies;

    List<Tuple<GameObject, int>> enemyWaves;

    // Update is called once per frame
    float timer = 0;
    
    int pathIterator = 0;

    public EnemySpawner(List<Tuple<GameObject, int>> enemyWaves, 
                        List<GameObject> splines) {
        this.enemyWaves = enemyWaves;
        this.splines = splines;
    }

    // Si se ha realizado el update (mientras sigamos spawneando), devolvemos true
    public bool Update(){
        if (spawnedEnemies >= maxEnemies) {
            return false; 
        }

        timer+=Time.deltaTime;
        if(timer>delay){
            timer = 0.0f;
            Spawn(pathIterator);
            pathIterator = (pathIterator + 1) % splines.Count;
            spawnedEnemies++;
        }
        return true;
            
    }

    void Spawn(int pathId){
        GameObject newEnemy = MonoBehaviour.Instantiate(enemy, new Vector3(0,0,0), Quaternion.identity);
        SplineContainer splineContainer = splines[pathId].GetComponent<SplineContainer>();
        newEnemy.GetComponentInChildren<Enemy>().Initialise(splineContainer,pathId,newEnemy);
    }
}
