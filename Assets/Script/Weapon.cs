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
     public float damage = 20f;
     public bool autoFire = false;
     public float range = 100f;

     [Header( "Ammo settings" )]
     public float baseShotDelay = .5f;
     public float fatiguedShotDelay = 2f;
     public int maxAmmo = 100;
     private float shotDelay;
     private int ammo;

    
     
     
     [Header( "Muzzle Flash" )]
     public GameObject muzzlePrefab;
     public Transform[] muzzleTransforms;
     public AudioSource muzzleSoundSource;
     public AudioClip muzzleSound;
     public Vector2 audioPitch = new Vector2( .9f, 1.1f );

     public bool isFiring = false;

     private float timeLastFired;
     private UiManager UI;
     private SharedCharacter player;

     // =====================================================================

     private void Start()
     {
          UI = FindObjectOfType<UiManager>();
          player = GetComponentInParent<SharedCharacter>();

          ammo = maxAmmo;
          UI.SetAmmo( ammo, maxAmmo );
          shotDelay = baseShotDelay;

          if( muzzleSoundSource != null )
               muzzleSoundSource.clip = muzzleSound;
     }

     private void FixedUpdate()
     {
          if( autoFire && isFiring )
          {
               FireWeapon();
          }
     }

     // =====================================================================

     public void FireWeapon()
     {
          if( ( timeLastFired + shotDelay ) <= Time.time )
          {
               timeLastFired = Time.time;
               CmdFireWeapon( cameraTransform.position, cameraTransform.forward );

               if( ammo > 0 )
               {
                    ammo--;
                    UI.SetAmmo( ammo, maxAmmo );
               }

               shotDelay = ammo == 0 ? fatiguedShotDelay : baseShotDelay;

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

     [Command( requiresAuthority = false )]
     private void CmdFireWeapon( Vector3 position, Vector3 forward )
     {
          bool displayHitmarker = false;

          if( Physics.Raycast( position, forward, out RaycastHit hit, range ) )
          {
               Target target = hit.collider.GetComponent<Target>();
               if( target != null )
               {
                    target.OnHit( hit );
                    displayHitmarker = true;
               }

               Enemy enemy = hit.collider.GetComponent<Enemy>();
               if( enemy != null )
               {
                    enemy.TakeDamage( damage );
                    displayHitmarker = true;
               }
          }

          RpcFireWeapon( displayHitmarker );
     }

     [ClientRpc]
     private void RpcFireWeapon( bool displayHitmarker )
     {
          if( player.localRole == Role.Legs )
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
          else
          {
               if( displayHitmarker )
               {
                    UI.ShowHitmarker();
               }
          }
     }

     

     public void ToggleFire()
     {
          autoFire = !autoFire;
          UI.SetFireMode( autoFire ? "AUTO" : "SINGLE" );
     }

     [Server]
     public void AddAmmo( int value )
     {
          RpcAddAmmo( value );
          if( player.localRole == Role.Head )
          {
               ammo = Mathf.Min( ammo + value, maxAmmo );
               UI.SetAmmo( ammo, maxAmmo );
          }
     }

     [ClientRpc]
     private void RpcAddAmmo( int value )
     {
          if( player.localRole == Role.Head )
          {
               ammo = Mathf.Min( ammo + value, maxAmmo );
               UI.SetAmmo( ammo, maxAmmo );
          }
     }
}
