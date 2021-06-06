using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Radial : ChaserEnemy
{
     [Header( "Projectile Settings" )]
     public int numberOfProjectiles;
     public int projectileSpeed;
     public GameObject projectilePrefab;
     public float frequency = 100;
     public float radialSpeed;

     private const float radius = 3f;
     private Vector3 startPoint;
     private float elapsed = 0;

     protected override void Update()
     {
          if( !isServer ) return;

          elapsed += Time.deltaTime;
          if( frequency - elapsed <= 0 )
          {
               startPoint = transform.position;
               SpawnProjectile( numberOfProjectiles );

               elapsed = 0;
          }

          transform.Rotate( 0f, Time.deltaTime * radialSpeed, 0f );

          base.Update();
     }

     private void SpawnProjectile( int numberOfProjectiles )
     {
          float angleStep = 360f / numberOfProjectiles;
          float angle = 0f;

          for( int i = 0; i < numberOfProjectiles; i++ )
          {
               float projectileDirXPosition = startPoint.x + Mathf.Sin( ( angle + transform.rotation.y * Mathf.PI *radialSpeed ) * Mathf.PI  / 180 ) * radius;
               float projectileDirYPosition = startPoint.y + Mathf.Cos( ( angle + transform.rotation.y * Mathf.PI * radialSpeed ) * Mathf.PI  / 180 ) * radius;

               //Debug.Log(transform.rotation.y * Mathf.PI * radialSpeed);

               Vector3 projectileVector = new Vector3( projectileDirXPosition, projectileDirYPosition, 0 );
               Vector3 projectileMoveDirection = ( projectileVector - startPoint ).normalized * projectileSpeed;

               GameObject tmpObj = Instantiate( projectilePrefab, startPoint , Quaternion.identity );
               tmpObj.GetComponent<Rigidbody>().velocity = new Vector3( projectileMoveDirection.x, 0, projectileMoveDirection.y );
               Bullet bullet = tmpObj.GetComponent<Bullet>();
               bullet.damage = damage;
               bullet.knockbackIntensity = knockbackIntensity;
               bullet.parent = GetComponent<Collider>();

               NetworkServer.Spawn( tmpObj );

               angle += angleStep;
          }
     }
}