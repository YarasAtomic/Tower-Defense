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
    Building buildingParent = null;

    Image healthImage;
    void Start()
    {
        mainCamera = Camera.main;
        bar = transform.Find("bar").gameObject;
        health = transform.Find("bar/health").gameObject;

        healthImage = health.GetComponent<Image>();

        enemyParent = transform.parent.GetComponent<Enemy>();
        buildingParent = transform.parent.GetComponent<Building>();
    }

    // Update is called once per frame
    void Update()
    {
        bar.transform.position = mainCamera.WorldToScreenPoint(transform.parent.position) + new Vector3(0,30,0);
        if(enemyParent!=null) {
            healthImage.fillAmount = enemyParent.GetHealthPercentage();
        }
        // if(buildingParent!=null) healthImage.fillAmount = buildingParent.GetHealthPercentage(); //TODO

        if(healthImage.fillAmount <= 0){
            Destroy(gameObject);
            Destroy(this);
        }
    }
}
