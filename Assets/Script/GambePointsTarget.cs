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
     public int points = 1000;
     public bool isPoint = false;

     private void OnTriggerEnter( Collider other )
     {
          if( isServer )
          {
               if( other.CompareTag( "Player" ) )
               {
                    if (!isPoint) 
                    {
                         other.GetComponent<Weapon>().AddAmmo(ammo);
                         base.OnHit();
                    }
                    else
                    {
                         SharedCharacter player = FindObjectOfType<SharedCharacter>();
                         player.AddPoints(points, false);
                         base.OnHit();
                    }
               } 
          }
     }

     [Server]
     public override void OnHit() { }
}
