using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class EnemySpawner
{    
    public enum Splitter{
        OneByOne,ConsumeTypes,SeparateLast
    }
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
                        List<GameObject> splines, LevelLogic levelLogic,Splitter splitterMethod) {
        switch(splitterMethod){
        case Splitter.OneByOne:
            this.enemyWave = SplitterOneByOne(enemyWave);
            break;
        case Splitter.ConsumeTypes:
            this.enemyWave = SplitterConsumeTypes(enemyWave);
            break;
        case Splitter.SeparateLast:
            this.enemyWave = SplitterSeparateLast(enemyWave);
            break;
        }
        
        this.splines = splines;
        enemyEnumerator = this.enemyWave.GetEnumerator();
        delay = 0.75f;
        timer = delay;
        this.levelLogic = levelLogic;
    }

    // Reparte lo enemigos de la siguietne forma:
    // Gasta uno de cada tipo, hasta agotar cada uno de los tipos
    private List<GameObject> SplitterOneByOne(List<ValueTuple<GameObject, int>> enemyWave) {
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

    // Reparte los enemigos de la siguiente forma:
    // Gasta todos los de un tipo antes de pasar al siguiente tipo
    private List<GameObject> SplitterConsumeTypes(List<ValueTuple<GameObject, int>> enemyWave) {
        List<GameObject> splitted = new List<GameObject>();
        for(int i = 0; i < enemyWave.Count; i++){
            int count = enemyWave[i].Item2;
            for(int j = 0; j < count; j++){
                splitted.Add(enemyWave[i].Item1);
            }
        }
        return splitted;
    }

    // Reparte lo enemigos de la siguietne forma:
    // Gasta uno de cada tipo excepto del ultimo tipo, hasta agotar cada uno de los tipos restantes
    // despues gasta todos los del ultimo tipo
    private List<GameObject> SplitterSeparateLast(List<ValueTuple<GameObject, int>> enemyWave) {
        List<GameObject> splitted = new List<GameObject>();
        List<int> enemiesLeft = new List<int>();

        int length = 0;
        for (int i = 0; i < enemyWave.Count-1;i++) {
            length += enemyWave[i].Item2;
            enemiesLeft.Add(enemyWave[i].Item2);
        }

        while (splitted.Count < length) {
            for (int i=0; i<enemyWave.Count-1; ++i) {
                if (enemiesLeft[i] <= 0) continue;
                splitted.Add(enemyWave[i].Item1);
                enemiesLeft[i]--;
            }
        }
        for(int i = 0; i < enemyWave[enemyWave.Count-1].Item2; i++){
            splitted.Add(enemyWave[enemyWave.Count-1].Item1);
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
