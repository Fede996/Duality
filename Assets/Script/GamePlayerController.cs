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
     private Role role = Role.None;

     [Header( "Movement" )]
     [SerializeField] private float speed = 10f;
     [SerializeField] private float sensitivityX = 100f;
     [SerializeField] private float sensitivityY = 100f;

     private Camera Camera;
     private Transform Body;
     private CharacterController Controller;
     private Weapon weapon;

     private float _horizontal = 0f;
     private float _vertical = 0f;
     private float _mouseX = 0f;
     private float _mouseY = 0f;
     private float _tilt = 0;


     private void Start()
     {
          SharedCharacter nc = FindObjectOfType<SharedCharacter>();
          if( playerRole == "Testa" )
          {
               role = Role.Testa;
               Camera = nc.TestaCamera;
               Body = nc.Body;
               weapon = nc.Weapon;
          }
          else
          {
               role = Role.Gambe;
               Camera = nc.GambeCamera;
               Controller = nc.Controller;
          }

          if( isLocalPlayer )
          {
               Cursor.lockState = CursorLockMode.Locked;
               Cursor.visible = false;

               Camera.enabled = true;
               Camera.GetComponent<AudioListener>().enabled = true;
          }

          DontDestroyOnLoad( this.gameObject );
     }

     private void Update()
     {
          if( !isLocalPlayer ) return;

          if( role == Role.Testa )
          {
               // Raccolgo input TESTA qui...

               _mouseX = Input.GetAxis( "Mouse X" ) * sensitivityX;
               _mouseY = Input.GetAxis( "Mouse Y" ) * sensitivityY;
               
               if( weapon.autoFire )
               {
                    weapon.isFiring = Input.GetButton( "Fire" );
               }
               else
               {
                    if( Input.GetButtonDown( "Fire" ) )
                         weapon.FireWeapon();
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

               Camera.transform.localRotation = Quaternion.Euler( _tilt, 90f, 0f );
               CmdRotate( _mouseX );
          }
          else
          {
               // Applico input GAMBE qui...

               Vector3 movement = new Vector3( _horizontal, 0, _vertical ).normalized * speed * Time.fixedDeltaTime;

               CmdMove( movement );
          }
     }

     // =====================================================================


     [Command]
     private void CmdRotate( float deltaX )
     {
          Body.Rotate( Vector3.up * deltaX );
     }

     [Command]
     private void CmdMove( Vector3 movement )
     {
          Controller.Move( movement );
     }
}

public enum Role
{
     None,
     Testa,
     Gambe,
}
