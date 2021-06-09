using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GamePlayerController : NetworkBehaviour
{
     [Header( "User data" )]
     public UserData playerData;
     public Role role = Role.None;
     public float gainedExp;
     public float gainedCash;

     [Header( "Movement" )]
     public float sensitivityX = 100f;
     public float sensitivityY = 100f;

     protected SharedCharacter player;
     private UiManager UI;
     private float tilt = 0;
     public GameObject grenadePrefab;
     public float grenadeLaunchForce = 10f;

     // =====================================================================
     // Sync data

     [SyncVar( hook = nameof( OnPlayerDataChanged ) )]
     public string playerDataJson;

     private void OnPlayerDataChanged( string oldValue, string newValue )
     {
          playerData = JsonUtility.FromJson<UserData>( newValue );
     }

     [Command]
     private void CmdSetPlayerData( string jsonData )
     {
          playerData = JsonUtility.FromJson<UserData>( jsonData );
          playerDataJson = jsonData;
     }

     // =====================================================================
     // Unity events

     protected void Start()
     {
          player = FindObjectOfType<SharedCharacter>();
          UI = FindObjectOfType<UiManager>();
          if( playerData.role == "HEAD" )
          {
               role = Role.Head;
          }
          else
          {
               role = Role.Legs;
          }

          if( isLocalPlayer )
          {
               Cursor.lockState = CursorLockMode.Locked;
               Cursor.visible = false;

               UI.SetupGameUI( role );
               player.Init( role );
          }
     }

     private bool disableInput = false;

     protected virtual void Update()
     {
          if( !isLocalPlayer || disableInput ) return;

          if( role == Role.Head )
          {
               float mouseX = Input.GetAxis( "Mouse X" ) * sensitivityX;
               float mouseY = Input.GetAxis( "Mouse Y" ) * sensitivityY;

               tilt -= mouseY;
               tilt = Mathf.Clamp( tilt, -90, 90 );

               player.Rotate( mouseX, tilt );

               if( Input.GetButtonDown( "ToggleFire" ) ) 
                    player.weapon.ToggleFire();


               if (Input.GetButton("Grenade"))
               {

                    // Mostro la traiettoria della granata
                    if (Input.GetButton("Fire"))
                    {
                         ThrowGrenade();
                         //Sparo la granata
                         //Farò lo spawn della granata nella posizione del player
                         //Cancello la traiettoria
                         return;
                    }
               }
               else if( player.weapon.autoFire )
               {
                    player.weapon.isFiring = Input.GetButton( "Fire" );
               }
               else
               {
                    if( Input.GetButtonDown( "Fire" ) )
                         player.weapon.FireWeapon();
                         
               }

               //Cancello la traiettoria
          }
          else 
          {
               float horizontal = Input.GetAxis( "Horizontal" );
               float vertical = Input.GetAxis( "Vertical" );

               Vector3 movement = new Vector3( horizontal, 0, vertical ).normalized;

               player.Move( movement );
          }
     }


     void ThrowGrenade()
     {
          GameObject grenade = Instantiate(grenadePrefab, player.transform.position, player.transform.rotation);
          Rigidbody rb = grenade.GetComponent<Rigidbody>();
          rb.AddForce(player.headCameraSocket.transform.forward * grenadeLaunchForce, ForceMode.VelocityChange);
     }
      
     // =====================================================================
     // Game events

     [Server]
     public void OnEndLevel( bool gameOver )
     {
          RpcOnEndLevel( gameOver );
     }

     [ClientRpc]
     private void RpcOnEndLevel( bool gameOver )
     {
          if( isLocalPlayer )
          {
               disableInput = true;

               if( !gameOver )
               {
                    GetRewards();
               }
          }
     }

     private void GetRewards()
     {
          playerData.AddExp( gainedExp );
          playerData.cash += gainedCash;
          FindObjectOfType<CameraController>().playerData = playerData;
          playerData.Save();
     }
}

public enum Role
{
     None,
     Head,
     Legs,
}
