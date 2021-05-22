using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent( typeof( Collider ) )]
public class TestaPointsTarget : DestroyableTarget
{
     [Header( "Settings" )]
     [SerializeField] private int points = 1;

     private SharedCharacter player;
     
     [Server]
     public override void OnHit()
     {
          player = FindObjectOfType<SharedCharacter>();
          
          player.TestaPoints += points;
          player.stamina = 2000;
          
          
          base.OnHit();
     }
}
