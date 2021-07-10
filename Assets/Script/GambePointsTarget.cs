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
     public int points = 100;
     public bool isPoint = false;
     public GameObject explosionSphere;

     private void OnTriggerEnter( Collider other )
     {
          if( isServer )
          {
               if( other.CompareTag( "Player" ) )
               {
                    if (!isPoint) 
                    {
                         other.GetComponent<Weapon>().AddAmmo(ammo);
                         SharedCharacter player = FindObjectOfType<SharedCharacter>();
                         player.AddPoints(points, false);
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
     public override void OnHit()
     {
          
          GameObject o = Instantiate(explosionSphere, transform.position, Quaternion.identity);
          NetworkServer.Spawn(o);

          base.OnHit();

          
     }

}
