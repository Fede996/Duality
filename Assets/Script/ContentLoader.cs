using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ContentLoader : NetworkBehaviour
{
     public List<GameObject> childObjects = new List<GameObject>();

     [Server]
     public void Load()
     {
          foreach( GameObject obj in childObjects )
          {
               if( obj != null )
               {
                    obj.SetActive( true );
                    obj.GetComponent<NetworkSpawner>().RpcEnable();
               }
          }    
     }

     [Server]
     public void Unload()
     {
          foreach( GameObject obj in childObjects )
          {
               if( obj != null )
               {
                    obj.GetComponent<NetworkSpawner>().RpcDisable();
                    obj.SetActive( false );
               }
          }
     }

     [Server]
     public bool IsCompleted()
     {
          foreach( GameObject obj in childObjects )
          {
               if( obj != null )
               {
                    Enemy enemy = obj.GetComponent<Enemy>();
                    if( enemy != null && enemy.needToKillToComplete )
                    {
                         return false;
                    } 
               }
          }
          return true;
     }
}
