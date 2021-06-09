using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkSpawner : NetworkBehaviour
{
     [Header( "Settings" )]
     public string spawnablePrefabName;

     private GameObject prefab;
     private ContentLoader loader;

     private void Start()
     {
          if( NetworkServer.active )
          {
               if( transform.parent != null && ( loader = GetComponentInParent<ContentLoader>() ) != null )
               {
                    // sono stato istanziato da un laoder
                    prefab = NetworkManager.singleton.spawnPrefabs.Find( o => o.name == spawnablePrefabName );
                    GameObject obj = Instantiate( prefab, transform.position, transform.rotation, null );
                    
                    obj.GetComponent<NetworkSpawner>().loader = loader;
                    loader.childObjects.Add( obj );
                    
                    Destroy( gameObject );
               }
               else
               {
                    // sono stato già rigenerato senza parent
                    // o non sono filgio di un loader...
                    NetworkServer.Spawn( gameObject );

                    if( loader != null )
                    {
                         RpcDisable();
                         gameObject.SetActive( false );
                    }
               }
          }
     }

     [ClientRpc]
     public void RpcEnable()
     {
          gameObject.SetActive( true );
     }

     [ClientRpc]
     public void RpcDisable()
     {
          gameObject.SetActive( false );
     }
}
