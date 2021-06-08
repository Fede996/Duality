using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

[RequireComponent( typeof( Collider ) )]
public class GambePointsTarget : DestroyableTarget
{
     [Header( "Settings" )]
     public int points = 1;
     public int ammo = 20;

     [Server]
     private void OnTriggerEnter( Collider other )
     {
          if( other.CompareTag( "Player" ) )
          {
               other.GetComponent<Weapon>().numberOfBullets += ammo;
               other.GetComponent<SharedCharacter>().GambePoints += points;
               base.OnHit();
          }
     }

     [Server]
     public override void OnHit() { }
}
