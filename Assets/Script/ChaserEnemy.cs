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
     public float movementSpeed;

     protected NavMeshAgent agent;
     protected Transform player;
     
     // =====================================================================

     protected override void Start()
     {
          if( isServer )
          {
               player = FindObjectOfType<SharedCharacter>().transform;

               agent = GetComponent<NavMeshAgent>();
               agent.enabled = true;
               agent.speed = movementSpeed; 
          }
          
          base.Start();
     }

     protected virtual void Update()
     {
          if( !isServer ) return;

          if( agent == null )
               agent = GetComponent<NavMeshAgent>();

          if( player == null )
          {
               SharedCharacter character = FindObjectOfType<SharedCharacter>();
               if( character != null )
                    player = character.transform;
          }

          if( movementSpeed != 0 && player != null && agent.enabled )
          {
               agent.SetDestination( player.position ); 
          }
     }
}
