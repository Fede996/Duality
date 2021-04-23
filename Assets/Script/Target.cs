using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Target : NetworkBehaviour
{
     [Server]
     public void OnHit()
     {
          CmdOnHit();
     }

     [Command( requiresAuthority = false )]
     private void CmdOnHit()
     {
          RpcOnHit();
          Destroy( gameObject );
     }

     [ClientRpc]
     private void RpcOnHit()
     {
          Destroy( gameObject );
     }
}
