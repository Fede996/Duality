using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Examples.Chat;

[RequireComponent( typeof( Collider ) )]
public class TestaPointsTarget : DestroyableTarget
{
     [Header( "Settings" )]
     public float stamina = 1000;
     public GameObject rechargeSphere;
     
     [Server]
     public override void OnHit()
     {
          RpcAddStamina( stamina );
          SharedCharacter player = FindObjectOfType<SharedCharacter>();
          
          player.playRechargeStaminaSound();
          
          if( player.localRole == Role.Legs )
          {
               player.AddStamina( stamina );
          }

          GameObject o = Instantiate( rechargeSphere, transform.position, Quaternion.identity );
          NetworkServer.Spawn( o );

          base.OnHit();
     }

     [ClientRpc]
     private void RpcAddStamina( float value )
     {
          
          
          SharedCharacter player = FindObjectOfType<SharedCharacter>();
          
          player.playRechargeStaminaSound();
          
          if( player.localRole == Role.Legs )
          {
               player.AddStamina( value );
          }
     }
}
