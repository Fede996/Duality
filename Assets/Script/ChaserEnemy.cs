using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent( typeof( Collider ) )]
public class ChaserEnemy : Enemy
{
     [Server]
     private void OnTriggerEnter( Collider other )
     {
          SharedCharacter player = other.GetComponent<SharedCharacter>();

     }
}
