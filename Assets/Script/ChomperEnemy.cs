using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent( typeof( NetworkAnimator ) )]
public class ChomperEnemy : ChaserEnemy
{
     private Animator anim;

     public AudioSource AudioSourceHit;
     public AudioSource AudioSourceDie;
     
     
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
          
          AudioSourceHit.Play();
          
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
          agent.speed = 0;
          GetComponent<Collider>().enabled = false;
          AudioSourceDie.Play();
          anim.StopPlayback();
          anim.Play( "ChomperDie" );

          foreach( GamePlayerController player in FindObjectsOfType<GamePlayerController>() )
          {
               player.gainedExp += rewardExp;
               player.gainedCash += rewardCash;
          }

          yield return new WaitForSecondsRealtime( 1 );
          NetworkServer.Destroy( gameObject );          
     }
}
