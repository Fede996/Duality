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

          CmdInitPlayer();
     }

     void Update()
     {
          if( !isLocalPlayer ) return;

          float deltaX = Input.GetAxis( "Mouse X" ) * sensitivityX * Time.deltaTime;
          float deltaY = Input.GetAxis( "Mouse Y" ) * sensitivityY * Time.deltaTime;
          _xRot -= deltaY;
          _xRot = Mathf.Clamp( _xRot, -90, 90 );

          bool fire = Input.GetButtonDown( "Fire" );

          CmdRotate( _xRot, deltaX );

          //if( fire )
          //     _weapon.Fire();
     }

     [Command]
     private void CmdRotate( float xRot, float deltaX )
     {
          Camera.localRotation = Quaternion.Euler( xRot, 90f, 0f );
          Body.Rotate( Vector3.up * deltaX );
          
          RpcRotate( xRot, deltaX );
     }

     [ClientRpc]
     private void RpcRotate( float xRot, float deltaX )
     {
          Camera.localRotation = Quaternion.Euler( xRot, 90f, 0f );
          Body.Rotate( Vector3.up * deltaX );
     }

     [Command]
     private void CmdInitPlayer()
     {
          RpcInitPlayer( Body.GetComponentInParent<NetworkCharacter>().Controller.transform.position,
                         Body.rotation,
                         Camera.localRotation );
     }

     [ClientRpc]
     private void RpcInitPlayer( Vector3 position, Quaternion bodyRotation, Quaternion cameraLocalRotation )
     {
          Body.GetComponentInParent<NetworkCharacter>().Controller.transform.position = position;
          Body.rotation = bodyRotation;
          Camera.localRotation = cameraLocalRotation;
     }
}
