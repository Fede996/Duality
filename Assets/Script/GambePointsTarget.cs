using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent( typeof( Collider ) )]




public class GambePointsTarget : DestroyableTarget
{
     [Header( "Settings" )]
     [SerializeField] private int points = 1;

     [Header("Objects")]
     public Weapon weapon ;
     
     
     [Server]
     private void OnTriggerEnter( Collider other )
     {
          SharedCharacter player = other.GetComponent<SharedCharacter>();

          if( player != null )
          {

               weapon.numberOfBullets += 20;
               player.GambePoints += points;
               base.OnHit();
          }
     }

     [Server]
     public override void OnHit() { }
}
