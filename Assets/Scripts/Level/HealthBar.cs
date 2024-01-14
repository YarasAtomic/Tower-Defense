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
    GameObject star0, star1, star2;
    void Start()
    {
        // mainCamera = Camera.main;
        mainCamera = GameObject.Find("CameraSupport/Main Camera").GetComponent<Camera>();
        bar = transform.Find("bar").gameObject;
        health = transform.Find("bar/health").gameObject;

        star0 = transform.Find("bar/star0").gameObject;
        star1 = transform.Find("bar/star1").gameObject;
        star2 = transform.Find("bar/star2").gameObject;

        healthImage = health.GetComponent<Image>();

        gameObjectParent = transform.parent.gameObject;

        enemyParent = transform.parent.GetComponent<Enemy>();
        towerParent = transform.parent.GetComponent<Tower>();
        baseParent = transform.parent.GetComponent<Base>();
		generatorParent = transform.parent.GetComponent<Generator>();

        bar.SetActive(false); 
        star0.SetActive(false);
        star1.SetActive(false);
        star2.SetActive(false);
        transform.SetParent(GameObject.Find("HealthBars").transform); // Eliminamos el parent para que se fije en la escena y no cause artifacts
    }



    // Update is called once per frame
    void Update()
    {
        if(gameObjectParent==null) {
            Destroy(gameObject);
            return;
        }
        if(mainCamera==null||mainCamera.enabled==false){
            bar.SetActive(false);
            return;
        } 
        bar.transform.position = mainCamera.WorldToScreenPoint(gameObjectParent.transform.position) + new Vector3(0,30,0);
        if(enemyParent!=null) {
            healthImage.fillAmount = enemyParent.GetHealthPercentage();
        }
        if(towerParent!=null) {
            switch(towerParent.GetTowerUpgrade()){
            case 1:
                star0.SetActive(true);
                break;
            case 2:
                star1.SetActive(true);
                break;
            case 3:
                star2.SetActive(true);
                break;
            }
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
