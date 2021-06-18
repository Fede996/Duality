using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

[RequireComponent( typeof( Collider ) )]
public class GambePointsTarget : DestroyableTarget
{
     [Header( "Settings" )]
     public int ammo = 40;

     private void OnTriggerEnter( Collider other )
     {
          if( isServer )
          {
               if( other.CompareTag( "Player" ) )
               {
                    other.GetComponent<Weapon>().AddAmmo( ammo );
                    base.OnHit();
               } 
          }
     }

     [Server]
     public override void OnHit() { }
}
