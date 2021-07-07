using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent( typeof( NetworkAnimator ) )]
public class GrenadierEnemy : Radial
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
               anim.Play( "GrenadierHit" );
          }
     }

     [Server]
     private IEnumerator Die()
     {
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

     [Server]
     protected override void SpawnProjectile( int numberOfProjectiles )
     {
          anim.Play( "GrenadierShoot" );

          base.SpawnProjectile( numberOfProjectiles );
     }
}
