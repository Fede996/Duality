using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ServerCameraSocket : NetworkBehaviour
{
     private void Start()
     {
          if( isServerOnly )
          {
               Camera.main.transform.position = transform.position;
               Camera.main.transform.rotation = transform.rotation;
          }
     }
}
