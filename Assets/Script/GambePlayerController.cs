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
     }

     void Update()
     {
          try
          {
               if( !isLocalPlayer ) return;

               float xAxis = Input.GetAxis( "Horizontal" );
               float zAxis = Input.GetAxis( "Vertical" );

               Vector3 movement = new Vector3( xAxis, 0, zAxis ).normalized * speed * Time.deltaTime;

               CmdMove( movement );
          }
          catch( System.Exception ex )
          {
               Debug.LogError( $"{ex.Message} in {ex.Source}" );
          }
     }

     [Command]
     private void CmdMove( Vector3 movement )
     {
          Controller.Move( movement );
     }
}
