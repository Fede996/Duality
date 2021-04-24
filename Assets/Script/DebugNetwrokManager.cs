using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DebugNetwrokManager : NetworkManager
{
     public override void Start()
     {
          base.Start();

          StartHost();
     }
}
