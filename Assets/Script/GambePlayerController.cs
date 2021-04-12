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
          Camera.enabled = true;
     }

     void Update()
     {
          if( !isLocalPlayer ) return;

          float xAxis = Input.GetAxis( "Horizontal" );
          float zAxis = Input.GetAxis( "Vertical" );

          Vector3 movement = new Vector3( xAxis, 0, zAxis ).normalized * speed * Time.deltaTime;

          Controller.Move( movement );
     }
}
