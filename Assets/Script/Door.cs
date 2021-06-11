using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent( typeof( Collider ) )]
[RequireComponent( typeof( NetworkSpawner ) )]
public class Door : NetworkBehaviour
{
     public Material closed;
     public Material open;

     private ContentLoader loader;

     private void Start()
     {
          if( isServer )
          {
               StartCoroutine( EnableCollider() );
               StartCoroutine( WaitForCompletion() );
          }
     }

     [Server]
     private IEnumerator EnableCollider()
     {
          yield return new WaitForSecondsRealtime( 0.5f );

          GetComponent<Collider>().enabled = true;
     }

     [Server]
     private IEnumerator WaitForCompletion()
     {
          loader = GetComponent<NetworkSpawner>().loader;

          while( !loader.IsCompleted() )
          {
               yield return new WaitForSecondsRealtime( 1 );
          }

          Open();
     }

     [Server]
     private void Open()
     {
          GetComponent<Collider>().enabled = false;
          GetComponent<Renderer>().material = open;
          RpcOpen();
     }

     [Server]
     private void Close()
     {
          GetComponent<Collider>().enabled = true;
     }

     [ClientRpc]
     private void RpcOpen()
     {
          GetComponent<Renderer>().material = open;
     }
}
