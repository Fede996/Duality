using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent( typeof( Collider ) )]
public class TestaPointsTarget : DestroyableTarget
{
     [Header( "Settings" )]
     [SerializeField] private int points = 1;

     [Server]
     public override void OnHit()
     {
          FindObjectOfType<SharedCharacter>().TestaPoints += points;

          base.OnHit();
     }
}
