using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class EnemySpawner : MonoBehaviour
{    
    [SerializeField] GameObject enemy;
    [SerializeField] List<GameObject> splines;
    [SerializeField] float delay;

    [SerializeField] int maxEnemies;

    [SerializeField] int spawnedEnemies;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    float timer = 0;
    
    int pathIterator = 0;
    void Update(){
        timer+=Time.deltaTime;
        if(timer>delay && spawnedEnemies < maxEnemies){
            timer = 0.0f;
            Spawn(pathIterator);
            pathIterator = (pathIterator + 1) % splines.Count;
            spawnedEnemies++;
        }
    }

    void Spawn(int pathId){
        GameObject newEnemy = Instantiate(enemy, new Vector3(0,0,0), Quaternion.identity);
        SplineContainer splineContainer = splines[pathId].GetComponent<SplineContainer>();
        newEnemy.GetComponent<Enemy>().Initialise(splineContainer,pathId);
    }
}
