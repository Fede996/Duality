using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DestroyableTarget : Target
{
     [Server]
     public override void OnHit()
     {
          NetworkServer.Destroy( gameObject );
     }
}
