using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent( typeof( NetworkAnimator ) )]
public class GrenadierEnemy : Radial
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
          if(HitAudioSource != null)
               HitAudioSource.Play();
          
          health -= damage;

          if( health <= 0 )
          {
               StartCoroutine( Die() );
          }
          else
          {
               anim.Play( "GrenadierHit" );
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
          if(DieAudioSource != null)
               DieAudioSource.Play();
          
          agent.speed = 0;
          alive = false;
          GetComponent<Collider>().enabled = false;
          anim.StopPlayback();
          anim.Play( "GrenadierDeath" );

          foreach( GamePlayerController player in FindObjectsOfType<GamePlayerController>() )
          {
               player.gainedExp += rewardExp;
               player.gainedCash += rewardCash;
          }

          yield return new WaitForSecondsRealtime( 4.5f );
          NetworkServer.Destroy( gameObject );
     }

     [ClientRpc]
     private void RpcDie()
     {
          if( DieAudioSource != null )
               DieAudioSource.Play();
     }

     [Server]
     protected override void SpawnProjectile( int numberOfProjectiles )
     {
          anim.Play( "GrenadierShoot" );

          base.SpawnProjectile( numberOfProjectiles );
     }
}
