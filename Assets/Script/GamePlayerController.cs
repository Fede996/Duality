using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GamePlayerController : NetworkBehaviour
{
     [SyncVar]
     public string playerName = null;
     [SyncVar]
     public string playerRole = null;
     protected Role role = Role.None;

     [Header( "Movement" )]
     [SerializeField] private float speed = 10f;
     [SerializeField] private float sensitivityX = 100f;
     [SerializeField] private float sensitivityY = 100f;

     protected SharedCharacter player;

     private float _horizontal = 0f;
     private float _vertical = 0f;
     private float _mouseX = 0f;
     private float _mouseY = 0f;
     private float _tilt = 0;


     protected void Start()
     {
          player = FindObjectOfType<SharedCharacter>();
          if( playerRole == "Testa" )
          {
               role = Role.Testa;
          }
          else
          {
               role = Role.Gambe;
          }

          if( isLocalPlayer )
          {
               Cursor.lockState = CursorLockMode.Locked;
               Cursor.visible = false;

               player.Init( role );
          }

          if( isClient )
          {
               GameObject serverCamera = GameObject.Find( "ServerCamera" );
               if( serverCamera != null ) 
                    serverCamera.SetActive( false );
          }

          DontDestroyOnLoad( this.gameObject );
     }

     protected virtual void Update()
     {
          if( !isLocalPlayer ) return;

          if( role == Role.Testa )
          {
               // Raccolgo input TESTA qui...

               _mouseX = Input.GetAxis( "Mouse X" ) * sensitivityX;
               _mouseY = Input.GetAxis( "Mouse Y" ) * sensitivityY;

               if( Input.GetButtonDown( "ToggleFire" ) ) player.ToggleFire();     

               // Forse da spostare in fixed ??
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
               // Raccolgo input GAMBE qui...

               _horizontal = Input.GetAxis( "Horizontal" );
               _vertical = Input.GetAxis( "Vertical" );
          }
     }

     private void FixedUpdate()
     {
          if( !isLocalPlayer ) return;

          if( role == Role.Testa )
          {
               // Applico input TESTA qui...

               //_mouseX *= Time.fixedDeltaTime;
               //_mouseY *= Time.fixedDeltaTime;
               _tilt -= _mouseY;
               _tilt = Mathf.Clamp( _tilt, -90, 90 );

               //player.TestaCamera.transform.localRotation = Quaternion.Euler( _tilt, 90f, 0f );
               player.Rotate( _mouseX, _tilt );
          }
          else
          {
               // Applico input GAMBE qui...

               Vector3 movement = new Vector3( _horizontal, 0, _vertical ).normalized * speed * Time.fixedDeltaTime;

               player.Move( movement );
          }
     }
}

public enum Role
{
     None,
     Testa,
     Gambe,
}
