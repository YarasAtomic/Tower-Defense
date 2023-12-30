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

    Image healthImage;
    void Start()
    {
        mainCamera = Camera.main;
        bar = transform.Find("bar").gameObject;
        health = transform.Find("bar/health").gameObject;

        healthImage = health.GetComponent<Image>();

        enemyParent = transform.parent.GetComponent<Enemy>();
        towerParent = transform.parent.GetComponent<Tower>();
		generatorParent = transform.parent.GetComponent<Generator>();

    }

    // Update is called once per frame
    void Update()
    {
        bar.transform.position = mainCamera.WorldToScreenPoint(transform.parent.position) + new Vector3(0,30,0);
        if(enemyParent!=null) {
            healthImage.fillAmount = enemyParent.GetHealthPercentage();
        }
        if(towerParent!=null) {
			healthImage.fillAmount = towerParent.GetHealthPercentage();
		}
        if(generatorParent!=null) {
			healthImage.fillAmount = generatorParent.GetHealthPercentage();
		}

        if(healthImage.fillAmount <= 0){
            Destroy(gameObject);
            Destroy(this);
        }
    }
}
