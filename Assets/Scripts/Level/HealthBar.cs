using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    // Start is called before the first frame update
    Camera mainCamera;
    GameObject bar;

    GameObject health;

    Enemy enemyParent = null;
    Tower towerParent = null;
	Generator generatorParent = null;
    Base baseParent = null;
    GameObject gameObjectParent = null;
    Image healthImage;
    void Start()
    {
        mainCamera = Camera.main;
        bar = transform.Find("bar").gameObject;
        health = transform.Find("bar/health").gameObject;

        healthImage = health.GetComponent<Image>();

        gameObjectParent = transform.parent.gameObject;

        enemyParent = transform.parent.GetComponent<Enemy>();
        towerParent = transform.parent.GetComponent<Tower>();
        baseParent = transform.parent.GetComponent<Base>();
		generatorParent = transform.parent.GetComponent<Generator>();
        bar.SetActive(false); 
        transform.SetParent(GameObject.Find("HealthBars").transform); // Eliminamos el parent para que se fije en la escena y no cause artifacts
    }



    // Update is called once per frame
    void Update()
    {
        if(gameObjectParent==null) {
            Destroy(gameObject);
            return;
        }
        bar.transform.position = mainCamera.WorldToScreenPoint(gameObjectParent.transform.position) + new Vector3(0,30,0);
        if(enemyParent!=null) {
            healthImage.fillAmount = enemyParent.GetHealthPercentage();
        }
        if(towerParent!=null) {
			healthImage.fillAmount = towerParent.GetHealthPercentage();
		}
        if(generatorParent!=null) {
			healthImage.fillAmount = generatorParent.GetHealthPercentage();
		}
        if(baseParent!=null) {
			healthImage.fillAmount = baseParent.GetHealthPercentage();
		}

        if(healthImage.fillAmount <= 0){
            Destroy(gameObject);
            Destroy(this);
        }
        bar.SetActive(true);
    }
}
