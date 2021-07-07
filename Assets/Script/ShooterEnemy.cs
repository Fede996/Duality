using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ShooterEnemy : ChaserEnemy
{
     [Header( "Shooting" )]
     public GameObject bulletPrefab;
     public float bulletSpeed;
     public float delayBetweenShots;

     private float timeToNextShot;
     protected bool firing = true;

     // =====================================================================

     protected override void Start()
     {
          timeToNextShot = delayBetweenShots;

          base.Start();
     }

     protected override void Update()
     {
          if( !isServer ) return;

          base.Update();

          if( firing )
          {
               if( timeToNextShot > 0 )
               {
                    timeToNextShot -= Time.deltaTime;
               }
               else
               {
                    ShootBullet();
                    timeToNextShot = delayBetweenShots;
               } 
          }
     }

     // =====================================================================

     [Server]
     protected virtual void ShootBullet()
     {
          GameObject projectile = Instantiate( bulletPrefab, transform.position, Quaternion.identity, null );
          Bullet bullet = projectile.GetComponent<Bullet>();
          bullet.damage = damage;
          bullet.knockbackIntensity = knockbackIntensity;
          bullet.parent = GetComponent<Collider>();

          if( player == null )
               player = FindObjectOfType<SharedCharacter>().transform;
          Vector3 direction = ( player.position - transform.position );
          direction.y = 0;
          bullet.initialVelocity = direction.normalized * bulletSpeed;
          transform.LookAt( player.transform, Vector3.up );

          NetworkServer.Spawn( projectile );
     }
}
