using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

[RequireComponent( typeof( NetworkTransform ) )]
[RequireComponent( typeof( NavMeshAgent ) )]
public class ChaserEnemy : Enemy
{
     [Header( "Movement" )]
     [SerializeField] private float movementSpeed = 1;

     private NavMeshAgent agent;
     protected Transform player;

     // =====================================================================

     protected override void Start()
     {
          player = GameObject.Find( "Player" ).transform;

          agent = GetComponent<NavMeshAgent>();
          agent.speed = movementSpeed;

          base.Start();
     }

     protected virtual void Update()
     {
          if( !isServer ) return;

          if( movementSpeed != 0 )
          {
               agent.SetDestination( player.position ); 
          }
     }
}
