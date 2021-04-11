using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GambePlayerController : MonoBehaviour
{
     [Header( "Movement" )]
     [SerializeField] private float speed = 2f;

     private CharacterController _controller = null;

     void Start()
     {
          _controller = GetComponent<CharacterController>();     
     }

     void Update()
     {
          float xAxis = Input.GetAxis( "Horizontal" );
          float zAxis = Input.GetAxis( "Vertical" );

          Vector3 movement = new Vector3( xAxis, 0, zAxis ).normalized * speed * Time.deltaTime;

          _controller.Move( movement );
     }
}
