using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ShooterEnemy : ChaserEnemy
{
     [Header( "Shooting" )]
     public GameObject bulletPrefab;
     public float bulletSpeed = 1;
     public float delayBetweenShots = 1;

     private float timeToNextShot;

     // =====================================================================

     protected override void Start()
     {
          timeToNextShot = delayBetweenShots;

          base.Start();
     }

     protected override void Update()
     {
          if( !isServer ) return;

          if( timeToNextShot > 0 )
          {
               timeToNextShot -= Time.deltaTime;
          }
          else
          {
               ShootBullet();
               timeToNextShot = delayBetweenShots;
          }

          base.Update();
     }

     // =====================================================================

     [Server]
     private void ShootBullet()
     {
          GameObject projectile = Instantiate( bulletPrefab, transform.position, Quaternion.identity, null );
          Bullet bullet = projectile.GetComponent<Bullet>();
          bullet.damage = damage;
          bullet.knockbackIntensity = knockbackIntensity;
          bullet.parent = GetComponent<Collider>();

          Vector3 direction = ( player.position - transform.position );
          direction.y = 0;
          bullet.GetComponent<Rigidbody>().velocity = direction.normalized * bulletSpeed;

          NetworkServer.Spawn( projectile );
     }
}
