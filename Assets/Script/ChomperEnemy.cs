using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent( typeof( NetworkAnimator ) )]
public class ChomperEnemy : ChaserEnemy
{
     private Animator anim;

     protected override void Start()
     {
          if( isServer )
          {
               anim = GetComponentInChildren<Animator>();
               anim.SetFloat( "moveSpeed", movementSpeed );
          }

          base.Start();
     }

     protected override void Update()
     {
          base.Update();

          if( isServer )
          {
               if( anim == null )
                    anim = GetComponentInChildren<Animator>();
               anim.SetFloat( "moveSpeed", agent.velocity.magnitude );
          }
     }

     [Server]
     public override void TakeDamage( float damage )
     {
          health -= damage;

          if( health <= 0 )
          {
               StartCoroutine( Die() );
          }
          else
          {
               anim.Play( "ChomperHit1" );
          }
     }

     [Server]
     private IEnumerator Die()
     {
          anim.Play( "ChomperDie" );

          foreach( GamePlayerController player in FindObjectsOfType<GamePlayerController>() )
          {
               player.gainedExp += rewardExp;
               player.gainedCash += rewardCash;
          }

          yield return new WaitForSecondsRealtime( 2 );
          NetworkServer.Destroy( gameObject );          
     }
}
