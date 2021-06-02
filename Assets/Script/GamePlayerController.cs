using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GamePlayerController : NetworkBehaviour
{
     [Header( "User data" )]
     public UserData playerData;
     public Role role = Role.None;

     [Header( "Movement" )]
     public float sensitivityX = 100f;
     public float sensitivityY = 100f;

     protected SharedCharacter player;
     private UiManager UI;
     private float tilt = 0;

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

     protected virtual void Update()
     {
          if( !isLocalPlayer ) return;

          if( role == Role.Head )
          {
               float mouseX = Input.GetAxis( "Mouse X" ) * sensitivityX;
               float mouseY = Input.GetAxis( "Mouse Y" ) * sensitivityY;

               tilt -= mouseY;
               tilt = Mathf.Clamp( tilt, -90, 90 );

               player.Rotate( mouseX, tilt );

               if( Input.GetButtonDown( "ToggleFire" ) ) 
                    player.ToggleFire();     

               if( player.Weapon.autoFire )
               {
                    player.Weapon.isFiring = Input.GetButton( "Fire" );
               }
               else
               {
                    if( Input.GetButtonDown( "Fire" ) )
                         player.Weapon.FireWeapon();
               }
          }
          else
          {
               float horizontal = Input.GetAxis( "Horizontal" );
               float vertical = Input.GetAxis( "Vertical" );

               Vector3 movement = new Vector3( horizontal, 0, vertical ).normalized;

               player.Move( movement );
          }
     }
}

public enum Role
{
     None,
     Head,
     Legs,
}
