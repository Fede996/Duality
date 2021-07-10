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
               Debug.Log("ARRIVO 0");
               if( other.CompareTag( "Player" ) )
               {
                    Debug.Log("ARRIVO 1");
                    if (!isPoint) 
                    {
                         Debug.Log("ARRIVO 2a");
                         other.GetComponent<Weapon>().AddAmmo(ammo);
                         Debug.Log("ARRIVO 3");
                         SharedCharacter player = FindObjectOfType<SharedCharacter>();
                         Debug.Log("ARRIVO 4");
                         player.AddPoints(points, false);
                         Debug.Log("ARRIVO 5");
                         base.OnHit(); 
                    }
                    else
                    {
                         Debug.Log("ARRIVO 2");
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
