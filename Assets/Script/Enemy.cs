using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Enemy : SolidTarget
{
     [Header( "Enemy Settings" )]
     public float maxHealth = 100;
     public int damage = 1;
     public float knockbackIntensity = 1000f;
     public bool needToKillToComplete;
     public float rewardExp;
     public float rewardCash;

     private float health;

     protected virtual void Start()
     {
          health = maxHealth;
     }

     [Server]
     private void OnCollisionEnter( Collision collision )
     {
          if( collision.collider.gameObject.CompareTag( "Player" ) )
          {
               Vector3 knockback = new Vector3 ( collision.rigidbody.position.x - transform.position.x, 0, collision.rigidbody.position.z - transform.position.z ).normalized * knockbackIntensity;
               SharedCharacter player = collision.gameObject.GetComponent<SharedCharacter>();
               if( player != null )
                    player.TakeDamage( damage, knockback );
          }
     }

     [Server]
     private void OnCollisionStay( Collision collision )
     {
          if( collision.collider.gameObject.CompareTag( "Player" ) )
          {
               Vector3 knockback = new Vector3 ( collision.rigidbody.position.x - transform.position.x, 0, collision.rigidbody.position.z - transform.position.z ).normalized * knockbackIntensity;
               SharedCharacter player = collision.gameObject.GetComponent<SharedCharacter>();
               if( player != null )
                    player.TakeDamage( damage, knockback );
          }
     }

     // =====================================================================

     [Server]
     public void TakeDamage( float damage )
     {
          health -= damage;

          if( health <= 0 )
          {
               NetworkServer.Destroy( gameObject );
               foreach( GamePlayerController player in FindObjectsOfType<GamePlayerController>() )
               {
                    player.gainedExp += rewardExp;
                    player.gainedCash += rewardCash;
               }
          }
     }
}
