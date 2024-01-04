using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon : MonoBehaviour
{

    [SerializeField] float rotationSpeed = 2;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3 (0,1*GameTime.DeltaTime * rotationSpeed,0));
    }
}
