using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Enemy : SolidTarget
{
     [Header( "Enemy Settings" )]
     [SerializeField] private float maxHealth = 100;
     [SerializeField] public int Damage = 1;
     [SerializeField] public float KnockbackIntensity = .5f;

     [SyncVar( hook = nameof( OnHealthChanged ) )]
     public float health;

     protected virtual void Start()
     {
          health = maxHealth;
     }

     // =====================================================================

     [Server]
     public void TakeDamage( float damage )
     {
          health -= damage;

          if( health <= 0 )
               Destroy( this.gameObject );
     }

     // =====================================================================

     [Client]
     private void OnHealthChanged( float newValue, float oldVAlue )
     {
          if( health <= 0 )
               Destroy( this.gameObject );
     }
}
