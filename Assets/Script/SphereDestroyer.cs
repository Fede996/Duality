using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class SphereDestroyer : MonoBehaviour
{

    [Header("Projectile life")]
    public float ProjectileLife;


    private Vector3 initialPosition;



    // Start is called before the first frame update
    void Start()
    {

        initialPosition = transform.position;

    }

    // Update is called once per frame
    void Update()
    {

        if (Vector3.Distance(transform.position, initialPosition) > ProjectileLife)
            Destroy(gameObject);


    }
}
