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

     public GameObject m_shotPrefab;
     
     [Header( "Ammo settings" )]
     public float baseShotDelay = .5f;
     public float fatiguedShotDelay = 2f;
     public int maxAmmo = 100;
     private float shotDelay;
     private int ammo;

     [Header((" Torchlight reference "))] public Light headLight;
     public Light bodyLight;
     
     
     [Header( "Muzzle Flash" )]
     public GameObject muzzlePrefab;
     public Transform[] muzzleTransforms;
     public AudioSource muzzleSoundSource;
     
     public AudioClip muzzleSound;
     public Vector2 audioPitch = new Vector2( .9f, 1.1f );

     public AudioSource RechargeAmmoSound;
     
     public bool isFiring = false;

     private float timeLastFired;
     private UiManager UI;
     private SharedCharacter player;
     private bool alternateMuzzle = false;
     
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

               //foreach( Transform parent in muzzleTransforms )
               //{
               //     Instantiate( muzzlePrefab, parent );
               //}

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
               GameObject laser = Instantiate( m_shotPrefab, alternateMuzzle ? muzzleTransforms[0].transform.position : muzzleTransforms[1].transform.position, cameraTransform.rotation );
               alternateMuzzle = !alternateMuzzle;
               laser.GetComponent<ShotBehavior>().setTarget( hit.point );
               NetworkServer.Spawn( laser );
               StartCoroutine( WaitAndDestroy( 2, laser ) );

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

     [Server]
     private IEnumerator WaitAndDestroy( float seconds, GameObject o )
     {
          yield return new WaitForSecondsRealtime( seconds );

          if( o != null )
               NetworkServer.Destroy( o );
     }

     [ClientRpc]
     private void RpcFireWeapon( bool displayHitmarker )
     {
          if( player.localRole == Role.Legs && !player.isSolo )
          {
               //foreach( Transform parent in muzzleTransforms )
               //{
               //     Instantiate( muzzlePrefab, parent );
               //}

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

     public void ToggleTorchLight()
     {
          CmdToggleTorchLight();

          headLight.enabled = !headLight.enabled;
          bodyLight.enabled = !bodyLight.enabled;
     }

     [Command( requiresAuthority = false )]
     private void CmdToggleTorchLight()
     {
          RpcToggleTorchLight();
     }

     [ClientRpc]
     private void RpcToggleTorchLight()
     {
          if( player.localRole == Role.Legs && !player.isSolo )
          {
               headLight.enabled = !headLight.enabled;
               bodyLight.enabled = !bodyLight.enabled; 
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
          if(RechargeAmmoSound != null)
               RechargeAmmoSound.Play();

          RpcAddAmmo( value );

          if(rechargeAmmoAudioSource != null)
               rechargeAmmoAudioSource.Play(); 

          if ( player.localRole == Role.Head || player.isSolo )
          {
               ammo = Mathf.Min( ammo + value, maxAmmo );
               UI.SetAmmo( ammo, maxAmmo );
          }

     }

     [ClientRpc]
     private void RpcAddAmmo( int value )
     {
          if(RechargeAmmoSound != null)
               RechargeAmmoSound.Play();
          if( player.localRole == Role.Head || player.isSolo )
          {
               ammo = Mathf.Min( ammo + value, maxAmmo );
               UI.SetAmmo( ammo, maxAmmo );
          }
     }
}
