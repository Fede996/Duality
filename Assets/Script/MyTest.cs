using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyTest : NetworkBehaviour
{
     public GameObject myPrefab;

     private void Update()
     {
          if( Input.GetKeyDown( KeyCode.H ) )
          {
               if( isServer )
               {
                    Instantiate( myPrefab, transform.position, transform.rotation, null );
               }
               else
               {
                    CmdMySpawn();
               }
          }
     }

     [Command]
     private void CmdMySpawn()
     {
          Instantiate( myPrefab, transform.position, transform.rotation, null );
     }
}
