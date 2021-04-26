using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Weapon : NetworkBehaviour
{
     [Header( "References" )]
     [SerializeField] private Transform cameraTransform;

     [Header( "Settings" )]
     [SerializeField] private float damage = 20f;
     [SerializeField] public bool autoFire;
     [SerializeField] private float shotDelay = .5f;
     [SerializeField] private float range = 100f;

     [Header( "Muzzle Flash" )]
     [SerializeField] private GameObject muzzlePrefab;
     [SerializeField] private Transform muzzleTransform;
     [SerializeField] private AudioSource muzzleSoundSource;
     [SerializeField] private AudioClip muzzleSound;
     [SerializeField] private Vector2 audioPitch = new Vector2(.9f, 1.1f);

     public bool isFiring = false;

     private float timeLastFired;

     // =====================================================================

     private void Start()
     {
          if( muzzleSoundSource != null )
               muzzleSoundSource.clip = muzzleSound;
     }

     private void FixedUpdate()
     {
          if( autoFire && isFiring && ( ( timeLastFired + shotDelay ) <= Time.time ) )
          {
               FireWeapon();
          }
     }

     // =====================================================================

     public void FireWeapon()
     {
          // Da chiamare in FixedUpdate()
          timeLastFired = Time.time;

          CmdFireWeapon();
     }

     // =====================================================================

     [Command( requiresAuthority = false )]
     private void CmdFireWeapon()
     {
          RpcFireWeapon();

          if( Physics.Raycast( cameraTransform.position, cameraTransform.forward, out RaycastHit hit, range ) )
          {
               Target target = hit.collider.GetComponent<Target>();
               if( target != null )
                    target.OnHit( hit );

               Enemy enemy = hit.collider.GetComponent<Enemy>();
               if( enemy != null )
                    enemy.TakeDamage( damage );
          }
     }

     [ClientRpc]
     private void RpcFireWeapon()
     {
          Instantiate( muzzlePrefab, muzzleTransform );
          if( muzzleSoundSource != null )
          {
               muzzleSoundSource.pitch = Random.Range( audioPitch.x, audioPitch.y );
               muzzleSoundSource.Play();
          }
     }
}
