using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TestaPlayerController : NetworkBehaviour
{
     public Transform Camera;
     public Transform Body;

     [Header( "Movement" )]
     [SerializeField] private float sensitivityX = 2f;
     [SerializeField] private float sensitivityY = 2f;

     private float _xRot = 0;
     //private Weapon _weapon = null;

     public void Init()
     {
          Cursor.lockState = CursorLockMode.Locked;
          Cursor.visible = false;

          //_weapon = GetComponentInChildren<Weapon>();
          Camera.GetComponent<Camera>().enabled = true;
     }

     void Update()
     {
          try
          {
               if( !isLocalPlayer ) return;

               float deltaX = Input.GetAxis( "Mouse X" ) * sensitivityX * Time.deltaTime;
               float deltaY = Input.GetAxis( "Mouse Y" ) * sensitivityY * Time.deltaTime;
               _xRot -= deltaY;
               _xRot = Mathf.Clamp( _xRot, -90, 90 );

               bool fire = Input.GetButtonDown( "Fire" );

               Camera.localRotation = Quaternion.Euler( _xRot, 90f, 0f );
               CmdRotate( deltaX );

               //if( fire )
               //     _weapon.Fire();
          }
          catch( System.Exception ex )
          {
               Debug.LogError( $"{ex.Message} in {ex.Source}" );
          }
     }

     [Command]
     private void CmdRotate( float deltaX )
     {
          Body.Rotate( Vector3.up * deltaX );
     }
}
