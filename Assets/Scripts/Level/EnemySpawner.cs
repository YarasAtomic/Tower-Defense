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

    List<GameObject> enemyWave;

    LevelLogic levelLogic;

    // Update is called once per frame
    float timer = 0;
    
    int pathIterator = 0;

    IEnumerator<GameObject> enemyEnumerator;

    public EnemySpawner(List<ValueTuple<GameObject, int>> enemyWave, 
                        List<GameObject> splines, LevelLogic levelLogic) {
        this.enemyWave = EnemySplitter(enemyWave);
        this.splines = splines;
        enemyEnumerator = this.enemyWave.GetEnumerator();
        delay = 0.75f;
        timer = delay;
        this.levelLogic = levelLogic;
    }

    private List<GameObject> EnemySplitter(List<ValueTuple<GameObject, int>> enemyWave) {
        List<GameObject> splitted = new List<GameObject>();
        List<int> enemiesLeft = new List<int>();

        int length = 0;
        foreach (ValueTuple<GameObject, int> enemy in enemyWave) {
            length += enemy.Item2;
            enemiesLeft.Add(enemy.Item2);
        }

        while (splitted.Count < length) {
            for (int i=0; i<enemyWave.Count; ++i) {
                if (enemiesLeft[i] <= 0) continue;
                splitted.Add(enemyWave[i].Item1);
                enemiesLeft[i]--;
            }
        }

        return splitted;
    }

    // Si se ha realizado el update (mientras sigamos spawneando), devolvemos true
    public bool Update(){
        timer+=GameTime.DeltaTime;
        if(timer>delay){
            if (!enemyEnumerator.MoveNext()) return false;
            timer = 0.0f;
            Spawn(pathIterator, enemyEnumerator.Current);
            pathIterator = (pathIterator + 1) % splines.Count;
        }
        return true;
            
    }

    void Spawn(int pathId, GameObject enemy){
        GameObject newEnemy = MonoBehaviour.Instantiate(enemy, new Vector3(0,1000,0), Quaternion.identity);
        SplineContainer splineContainer = splines[pathId].GetComponent<SplineContainer>();
        newEnemy.GetComponentInChildren<Enemy>().Initialise(splineContainer, pathId, newEnemy, levelLogic);
    }
}
