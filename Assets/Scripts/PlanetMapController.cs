using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetMapController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float rotationSpeed = 1;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate( new Vector3 (0,1*GameTime.DeltaTime * rotationSpeed,0));
    }
}
