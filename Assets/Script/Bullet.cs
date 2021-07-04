using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Examples.Chat;

[RequireComponent( typeof( Rigidbody ) )]
[RequireComponent( typeof( Collider ) )]
public class Bullet : NetworkBehaviour
{
     [Header( "Bullet Settings" )]
     [SyncVar] public float timeToLive = 5;
     [SyncVar] public int damage = 1;
     [SyncVar] public float knockbackIntensity = 500f;
     [SyncVar] public Vector3 initialVelocity;
     public Collider parent;

     public bool isDeadlyBullet = false;
     // =====================================================================

     private void Start()
     {
          if( isServer )
               Physics.IgnoreCollision( GetComponent<Collider>(), parent );

          GetComponent<Rigidbody>().velocity = initialVelocity;
     }

     private void Update()
     {
          if( !isServer ) return;

          if( timeToLive > 0 )
          {
               timeToLive -= Time.deltaTime;
          }
          else
          {
               NetworkServer.Destroy( gameObject );
          }
     }

     private void OnTriggerEnter( Collider other )
     {
          if( !isServer ) return;

          if( !other.gameObject.CompareTag( "Enemy" ) && !other.gameObject.CompareTag( "IgnoreOnTriggers" ) )
          {
               if( other.gameObject.CompareTag( "Player" ) )
               {
                    Vector3 knockback = new Vector3 ( other.transform.position.x - transform.position.x, 0, other.transform.position.z - transform.position.z ).normalized * knockbackIntensity;
                    SharedCharacter player = other.gameObject.GetComponent<SharedCharacter>();
                    if( player != null )
                         player.TakeDamage( damage, knockback );
               }


               
          }
     }

     private void OnTriggerStay( Collider other )
     {
          if( !isServer ) return;

          if( !other.gameObject.CompareTag( "Enemy" ) && !other.gameObject.CompareTag( "IgnoreOnTriggers" ) )
          {
               if( other.gameObject.CompareTag( "Player" ) )
               {
                    Vector3 knockback = new Vector3 ( other.transform.position.x - transform.position.x, 0, other.transform.position.z - transform.position.z ).normalized * knockbackIntensity;
                    SharedCharacter player = other.gameObject.GetComponent<SharedCharacter>();
                    if( player != null )
                         player.TakeDamage( damage, knockback );
               }

               NetworkServer.Destroy( gameObject );
          }
     }
}
