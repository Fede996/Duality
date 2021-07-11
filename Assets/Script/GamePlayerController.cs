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

     [Header( "Grenade" )]
     public GameObject grenadePrefab;
     public float grenadeLaunchForce = 10f;
     public Transform grenadeSocket;

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
          isSolo = UI.isSolo;
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
               player.Init( role, isSolo );
               player.SetHue( playerData.color, role );
          }
     }

     private bool disableInput = false;
     private bool isSolo = false;

     protected virtual void Update()
     {
          if( !isLocalPlayer || disableInput ) return;

          if( role == Role.Head || isSolo )
          {
               float mouseX = Input.GetAxis( "Mouse X" ) * sensitivityX;
               float mouseY = Input.GetAxis( "Mouse Y" ) * sensitivityY;

               tilt -= mouseY;
               tilt = Mathf.Clamp( tilt, -80, 80 );

               player.Rotate( mouseX, tilt );

               if( Input.GetButtonDown( "ToggleFire" ) )
                    player.weapon.ToggleFire();

               if( Input.GetButtonDown( "ToggleTorchlight" ) )
                    player.weapon.ToggleTorchLight();


               if( Input.GetButton( "Grenade" ) )
               {
                    // Mostro la traiettoria della granata
                    if( Input.GetButtonDown( "Fire" ) )
                    {
                         //Sparo la granata
                         //Farò lo spawn della granata nella posizione del player
                         //Cancello la traiettoria
                         ThrowGrenade();
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

          if( role == Role.Legs || isSolo )
          {
               float horizontal = Input.GetAxis( "Horizontal" );
               float vertical = Input.GetAxis( "Vertical" );

               Vector3 movement = Vector3.zero;
               if( isSolo )
               {
                    if( player.headCameraSocket != null )
                    {
                         Vector3 newForward = new Vector3( player.headCameraSocket.transform.forward.x, 0, player.headCameraSocket.transform.forward.z ).normalized;
                         movement = ( newForward * vertical + player.headCameraSocket.transform.right * horizontal ).normalized;
                    }
               }
               else
                    movement = new Vector3( horizontal, 0, vertical ).normalized;

               player.Move( movement );
          }
     }

     private void ThrowGrenade()
     {
          grenadeSocket = GameObject.Find( "Grenade socket" ).transform;
          GameObject grenade = Instantiate( grenadePrefab, grenadeSocket.position, grenadeSocket.rotation, null );
          Rigidbody rb = grenade.GetComponent<Rigidbody>();
          rb.AddForce( player.headCameraSocket.transform.forward * grenadeLaunchForce, ForceMode.Force );
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

     [Server]
     public void ShowWinner()
     {
          RpcShowWinner();

          foreach( GamePlayerController controller in FindObjectsOfType<GamePlayerController>() )
          {
               controller.DisableInput();
          }

          UI.endLevel.SetActive( true );
          UI.SetWinner();
          Cursor.lockState = CursorLockMode.Confined;
          Cursor.visible = true;
     }

     [ClientRpc]
     private void RpcShowWinner()
     {
          foreach( GamePlayerController controller in FindObjectsOfType<GamePlayerController>() )
          {
               controller.DisableInput();
          }

          UI.endLevel.SetActive( true );
          UI.SetWinner();
          Cursor.lockState = CursorLockMode.Confined;
          Cursor.visible = true;
     }

     private void GetRewards()
     {
          playerData.AddExp( gainedExp );
          playerData.cash += gainedCash;
          FindObjectOfType<CameraController>().playerData = playerData;
          playerData.Save();
     }

     public void DisableInput()
     {
          if( isLocalPlayer )
          {
               disableInput = true;
          }
     }

     public void EnableInput()
     {
          if( isLocalPlayer )
          {
               disableInput = false;
          }
     }
}

public enum Role
{
     None,
     Head,
     Legs,
}
