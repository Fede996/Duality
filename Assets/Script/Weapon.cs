using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
     [Header( "References" )]
     [SerializeField] private Transform cameraTransform;

     [Header( "Settings" )]
     [SerializeField] private float range = 100f;

     public void Fire()
     {
          Debug.DrawRay( cameraTransform.position, cameraTransform.forward * range, Color.red, 1 );

          RaycastHit hit;
          if( Physics.Raycast( cameraTransform.position, cameraTransform.forward, out hit, range ) )
          {
               print( "HIT" );
               hit.collider.GetComponent<Target>()?.OnHit();
          }
     }
}
