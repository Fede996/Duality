using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent( typeof( Collider ) )]
public class TestaPointsTarget : DestroyableTarget
{
     [Header( "Settings" )]
     public float stamina = 1000;
     public int points = 1000;
     public bool isPoint = false;
     public GameObject rechargeSphere;
     
     [Server]
     public override void OnHit()
     {



          if (!isPoint)
          { 
               RpcAddStamina(stamina);
               RpcAddPoints(points);
               SharedCharacter player = FindObjectOfType<SharedCharacter>();
               if (player.localRole == Role.Legs)
               {
                    player.AddStamina(stamina);
               }
               else player.AddPoints(points, true);

               GameObject o = Instantiate(rechargeSphere, transform.position, Quaternion.identity);
               NetworkServer.Spawn(o);

               base.OnHit();
          } else
          {
               RpcAddPoints(points);
               SharedCharacter player = FindObjectOfType<SharedCharacter>();
               if (player.localRole == Role.Head)
               {
                    player.AddPoints(points, true);
               }

               GameObject o = Instantiate(rechargeSphere, transform.position, Quaternion.identity);
               NetworkServer.Spawn(o); 

               base.OnHit();

          }
     }

     [ClientRpc]
     private void RpcAddStamina( float value )
     {
          SharedCharacter player = FindObjectOfType<SharedCharacter>();
          if( player.localRole == Role.Legs )
          {
               player.AddStamina( value );
          }
     }

     [ClientRpc]
     private void RpcAddPoints(int value)
     {
          SharedCharacter player = FindObjectOfType<SharedCharacter>();
          if (player.localRole == Role.Head)
          {
               player.AddPoints(value, true); 
          }
     }
}
