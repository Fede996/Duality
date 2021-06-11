using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Grenade : NetworkBehaviour
{

     public float delay = 3f;
     public float explosionRadius = 5f;
     public float explosionForce = 1000f;
     public GameObject explosionEffect;
     float countdown;
     bool hasExploded = false;
     // Start is called before the first frame update
     void Start()
     {
          countdown = delay;
          //Far fare alla granata il suo movimento di lancio
     }

     // Update is called once per frame
     void Update()
     {
          countdown -= Time.deltaTime;
          if( countdown <= 0 && !hasExploded )
          {
               Explode();
               hasExploded = true;
          }
     }

     void Explode()
     {
          Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
          foreach( Collider nearbyObject in colliders )
          {
               Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
               if( rb != null )
               {
                    rb.AddExplosionForce( explosionForce, transform.position, explosionRadius );
               }
          }

          Instantiate( explosionEffect, transform.position, transform.rotation );
          Destroy( gameObject );
     }
}
