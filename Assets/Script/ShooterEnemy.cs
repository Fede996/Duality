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

     // =====================================================================

     [Server]
     private void ShootBullet()
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

          NetworkServer.Spawn( projectile );
     }
}
