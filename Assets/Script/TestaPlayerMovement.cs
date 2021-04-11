using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestaPlayerMovement : MonoBehaviour
{
     [Header( "References" )]
     [SerializeField] private Transform _camera;
     [SerializeField] private Transform _body;

     [Header( "Movement" )]
     [SerializeField] private float sensitivityX = 2f;
     [SerializeField] private float sensitivityY = 2f;

     private float _xRot = 0;

     void Start()
     {
          Cursor.lockState = CursorLockMode.Locked;
          Cursor.visible = false;
     }

     void Update()
     {
          float deltaX = Input.GetAxis( "Mouse X" ) * sensitivityX * Time.deltaTime;
          float deltaY = Input.GetAxis( "Mouse Y" ) * sensitivityY * Time.deltaTime;
          _xRot -= deltaY;
          _xRot = Mathf.Clamp( _xRot, -90, 90 );

          _camera.localRotation = Quaternion.Euler( _xRot, 90f, 0f );
          _body.Rotate( Vector3.up * deltaX );
     }
}
