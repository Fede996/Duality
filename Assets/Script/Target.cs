using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Target : NetworkBehaviour
{
     /// <summary>
     /// Da chiamare sul Server quando un oggetto viene colpito.
     /// Fare override di questa funzione per implementarla.
     /// </summary>
     [Server]
     public virtual void OnHit() { }

     /// <summary>
     /// Da chiamare sul Server quando un oggetto viene colpito.
     /// Fare override di questa funzione per implementarla.
     /// </summary>
     [Server]
     public virtual void OnHit( RaycastHit hit ) 
     {
          OnHit();
     }

     /// <summary>
     /// Da chiamare da OnHit se si vogliono fare operazioni sul Server in seguito alla collisione.
     /// Fare override di questa funzione per implementarla.
     /// </summary>
     [Command( requiresAuthority = false )]
     protected virtual void CmdOnHit() { }

     /// <summary>
     /// Da chiamare da CmdOnHit per trasmettere i cambiamenti ai Client.
     /// Fare override di questa funzione per implementarla.
     /// </summary>
     [ClientRpc]
     protected virtual void RpcOnHit() { }
}
