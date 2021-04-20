using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GambePlayerController : NetworkBehaviour
{
     public Camera Camera;
     public CharacterController Controller;

     [Header( "Movement" )]
     [SerializeField] private float speed = 2f;

     public void Init()
     {
          Cursor.lockState = CursorLockMode.Locked;
          Cursor.visible = false;

          Camera.enabled = true;

          CmdInitPlayer();
     }

     void Update()
     {
          if( !isLocalPlayer ) return;

          float xAxis = Input.GetAxis( "Horizontal" );
          float zAxis = Input.GetAxis( "Vertical" );

          Vector3 movement = new Vector3( xAxis, 0, zAxis ).normalized * speed * Time.deltaTime;

          CmdMove( movement );
     }

     [Command]
     private void CmdMove( Vector3 movement )
     {
          Controller.Move( movement );

          RpcMove( movement );
     }

     [ClientRpc]
     private void RpcMove( Vector3 movement )
     {
          Controller.Move( movement );
     }

     [Command]
     private void CmdInitPlayer()
     {
          RpcInitPlayer( Controller.transform.position, 
                         Controller.GetComponent<NetworkCharacter>().Body.rotation,
                         Controller.GetComponent<NetworkCharacter>().TestaCamera.localRotation );
     }

     [ClientRpc]
     private void RpcInitPlayer( Vector3 position, Quaternion bodyRotation, Quaternion cameraLocalRotation )
     {
          Controller.transform.position = position;
          Controller.GetComponent<NetworkCharacter>().Body.rotation = bodyRotation;
          Controller.GetComponent<NetworkCharacter>().TestaCamera.localRotation = cameraLocalRotation;
     }
}
