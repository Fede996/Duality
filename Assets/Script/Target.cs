using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Target : NetworkBehaviour
{
     public void OnHit()
     {
          CmdOnHit();
          //Destroy( gameObject );
     }

     [Command]
     private void CmdOnHit()
     {
          Destroy( gameObject );
     }
}
