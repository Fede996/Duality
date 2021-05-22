using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent( typeof( Rigidbody ) )]
[RequireComponent( typeof( Collider ) )]
[RequireComponent( typeof( NetworkTransform ) )]
public class Bullet : NetworkBehaviour
{
     [Header( "Bullet Settings" )]
     [SerializeField] private float timeToLive = 5;
     public int Damage = 1;
     public float KnockbackIntensity = .5f;

     // =====================================================================

     private void Update()
     {
          if( !isServer ) return;

          if( timeToLive > 0 )
          {
               timeToLive -= Time.deltaTime;
          }
          else
          {
               DestroyBullet();
               Destroy( this.gameObject );
          }
     }

     private void OnTriggerEnter( Collider other )
     {
          if( !isServer ) return;

          if( !other.CompareTag( "Enemy" ) )
          {
               if( other.CompareTag( "Player" ) )
               {
                    Vector3 knockback = ( other.transform.position - transform.position ).normalized * KnockbackIntensity;
                    other.GetComponent<SharedCharacter>().TakeDamage( Damage, knockback );
               }

               DestroyBullet();
               Destroy( this.gameObject );
          }
     }

     // =====================================================================

     [ClientRpc]
     private void DestroyBullet()
     {
          Destroy( this.gameObject );
     }
}
