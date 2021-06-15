using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent( typeof( Collider ) )]
[RequireComponent( typeof( NetworkSpawner ) )]
public class Door : NetworkBehaviour
{
     [SyncVar]
     public Directions direction;
     public Color locked;
     public Color open;

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

          if( !GetComponent<NetworkSpawner>().loader.IsCompleted() )
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
     public void Init( bool enabled )
     {
          if( enabled )
          {
               GetComponent<ParticleSystem>().Play();
               RpcInit();
          }
          else
          {
               NetworkServer.Destroy( gameObject );
          }
     }

     [ClientRpc]
     private void RpcInit()
     {
          GetComponent<ParticleSystem>().Play();
     }

     [Server]
     private void Open()
     {
          RpcOpen();
          GetComponent<Collider>().enabled = false;
          var c = GetComponent<ParticleSystem>().colorOverLifetime;
          c.color = open;
     }

     [ClientRpc]
     private void RpcOpen()
     {
          var main = GetComponent<ParticleSystem>().main;
          main.startColor = new ParticleSystem.MinMaxGradient( open );
     }

     [Server]
     private void Close()
     {
          GetComponent<Collider>().enabled = true;
     }
}
