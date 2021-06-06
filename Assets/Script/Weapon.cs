using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class Weapon : NetworkBehaviour
{
     [Header( "References" )]
     [SerializeField] private Transform cameraTransform;

     [Header( "Settings" )]
     [SerializeField] private float damage = 20f;
     [SerializeField] public bool autoFire = true;
     [SerializeField] private float shotDelay = .5f;
     [SerializeField] private float range = 100f;
     public int numberOfBullets = 20;
     
     [Header( "Muzzle Flash" )]
     [SerializeField] private GameObject muzzlePrefab;
     [SerializeField] private Transform[] muzzleTransforms;
     [SerializeField] private AudioSource muzzleSoundSource;
     [SerializeField] private AudioClip muzzleSound;
     [SerializeField] private Vector2 audioPitch = new Vector2(.9f, 1.1f);
          
     public bool isFiring = false;

     private float timeLastFired;
     private UiManager UI;

     // =====================================================================

     private void Start()
     {
          UI = FindObjectOfType<UiManager>();

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
          timeLastFired = Time.time;

          if (numberOfBullets != 0)
          {
               numberOfBullets--;
               CmdFireWeapon();
          }
     }

     public void ToggleFire()
     {
          autoFire = !autoFire;
          UI.SetFireMode( autoFire ? "AUTO" : "SINGLE" );
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
          foreach( Transform parent in muzzleTransforms )
          {
               Instantiate( muzzlePrefab, parent );
          }

          if( muzzleSoundSource != null )
          {
               muzzleSoundSource.pitch = Random.Range( audioPitch.x, audioPitch.y );
               muzzleSoundSource.Play();
          }
     }
}
