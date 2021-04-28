using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ShooterEnemy : ChaserEnemy
{
     [Header( "Shooting" )]
     [SerializeField] private GameObject bulletPrefab;
     [SerializeField] private float bulletSpeed = 1;
     [SerializeField] private float delayBetweenShots = 1;

     private float timeToNextShot;

     // =====================================================================

     protected override void Start()
     {
          timeToNextShot = delayBetweenShots;

          base.Start();
     }

     [Server]
     protected override void Update()
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

          base.Update();
     }

     // =====================================================================

     [Server]
     private void ShootBullet()
     {
          GameObject projectile = Instantiate( bulletPrefab, transform.position, Quaternion.identity, null );
          Bullet bullet = projectile.GetComponent<Bullet>();
          bullet.Damage = Damage;
          bullet.KnockbackIntensity = KnockbackIntensity;

          Vector3 direction = ( player.position - transform.position );
          direction.y = 0;
          bullet.GetComponent<Rigidbody>().velocity = direction.normalized * bulletSpeed;

          NetworkServer.Spawn( projectile );
     }
}
