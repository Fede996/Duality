using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoTowardsPlayer : MonoBehaviour
{

    public float speed = 10;
    
    private GameObject target;


    private TransparencyAnimation cameraPlaneAnimationComponent;
    
    // Start is called before the first frame update
    void Awake()
    {

        target = GameObject.Find("Body");
        //cameraPlaneAnimationComponent = GameObject.Find("PlaneCamera").GetComponent<TransparencyAnimation>();

    }

    // Update is called once per frame
    void Update()
    {
        
        float step =  speed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);
        
        
        
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Body")
        {

            //cameraPlaneAnimationComponent.startAnimation();

            
            GameObject.Destroy(gameObject);
            
            
            
        }

    }
}
