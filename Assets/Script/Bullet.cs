using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent( typeof( Rigidbody ) )]
[RequireComponent( typeof( Collider ) )]
public class Bullet : NetworkBehaviour
{
     [Header( "Bullet Settings" )]
     public float timeToLive = 5;
     public int damage = 1;
     public float knockbackIntensity = .5f;
     public Collider parent;

     // =====================================================================

     private void Start()
     {
          Physics.IgnoreCollision( GetComponent<Collider>(), parent );
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

     [Server]
     private void OnTriggerEnter( Collider other )
     {
          if( !other.gameObject.CompareTag( "Enemy" ) )
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

     [Server]
     private void OnTriggerStay( Collider other )
     {
          if( !other.gameObject.CompareTag( "Enemy" ) )
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
