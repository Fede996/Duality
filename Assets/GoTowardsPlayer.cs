using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GoTowardsPlayer : NetworkBehaviour
{
     [Header( "Settings" )]
     public float speed = 10;

     private Transform target;

     private void Start()
     {
          target = FindObjectOfType<SharedCharacter>().transform;
     }

     void Update()
     {
          float step =  speed * Time.deltaTime;
          transform.position = Vector3.MoveTowards( transform.position, target.position, step );
     }

     private void OnTriggerEnter( Collider other )
     {
          if( other.CompareTag( "Player" ) )
          {
               Destroy( gameObject );
          }
     }
}
