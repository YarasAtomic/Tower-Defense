using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class enemySpawner : MonoBehaviour
{
    public GameObject enemy;
    public List<GameObject> splines;
    public float delay;
    List<SplineContainer> splineContainers;
    // Start is called before the first frame update
    void Start()
    {
        splineContainers = new List<SplineContainer>();
        foreach (GameObject s in splines)
        {
            Debug.Log(s.GetComponent<SplineContainer>());
            splineContainers.Add(s.GetComponent<SplineContainer>());
        }
    }

    // Update is called once per frame
    float timer = 0;
    int pathIterator = 0;
    void Update()
    {
        timer+=Time.deltaTime;
        if(timer>delay)
        {
            spawn(pathIterator);
            timer = 0;
            pathIterator = (pathIterator + 1) % splines.Count;
        }
    }

    void spawn(int id){
        GameObject newEnemy = Instantiate(enemy, new Vector3(0,0,0), Quaternion.identity);
        newEnemy.GetComponent<enemy>().setSplineId(id);
        SplineAnimate animate = newEnemy.GetComponent<SplineAnimate>();
        animate.Container = splineContainers[id];
        animate.enabled = true;
    }

    // TODO lo que realmente hay que comprobar es si el objeto A se para por el objeto B, el objeto B no debe pararse por el objeto A
}
