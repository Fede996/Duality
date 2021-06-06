using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

[RequireComponent( typeof( NetworkTransform ) )]
[RequireComponent( typeof( NavMeshAgent ) )]
[RequireComponent( typeof( Rigidbody ) )]
public class ChaserEnemy : Enemy
{
     [Header( "Movement" )]
     public float movementSpeed = 1;

     private NavMeshAgent agent;
     protected Transform player;
     
     // =====================================================================

     protected override void Start()
     {
          if( isServer )
          {
               player = FindObjectOfType<SharedCharacter>().transform;

               agent = GetComponent<NavMeshAgent>();
               agent.speed = movementSpeed; 
          }
          
          base.Start();
     }

     protected virtual void Update()
     {
          if( !isServer ) return;

          if( movementSpeed != 0 && player != null )
          {
               agent.SetDestination( player.position ); 
          }
     }
}
