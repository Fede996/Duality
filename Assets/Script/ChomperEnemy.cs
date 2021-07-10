using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent( typeof( NetworkAnimator ) )]
public class ChomperEnemy : ChaserEnemy
{
     private Animator anim;

     public AudioSource HitAudioSource;
     public AudioSource DieAudioSource;
     
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
          RpcTakeDamage();
          if( HitAudioSource != null )
               HitAudioSource.Play();
          
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

     [ClientRpc]
     private void RpcTakeDamage()
     {
          if( HitAudioSource != null )
               HitAudioSource.Play();
     }

     [Server]
     private IEnumerator Die()
     {
          RpcDie();
          if( DieAudioSource!=null )
               DieAudioSource.Play();
          
          agent.speed = 0;
          GetComponent<Collider>().enabled = false;
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

     [ClientRpc]
     private void RpcDie()
     {
          if( DieAudioSource != null )
               DieAudioSource.Play();
     }
}
