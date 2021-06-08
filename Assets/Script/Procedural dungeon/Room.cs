using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Room : NetworkBehaviour
{
     [Header( "Components" )]
     public Vector2 roomSize;
     public SyncList<Directions> doors = new SyncList<Directions>();
     // Ordine di inserimento mura:
     // 1) muro nord/sud
     // 2) muro est/ovest
     // 3) muro nord/sud con porta
     // 4) muro est/ovest con porta
     public List<GameObject> walls;
     public float wallWidth;
     public Transform roomCameraSocket;
     
     [HideInInspector] public ContentLoader content;

     // =======================================================
     // Unity events 

     void Start()
     {
          SetupWalls();
     }

     // =======================================================
     // Methods

     [Server]
     public void Load()
     {
          // carica il contenuto della stanza
          if( content != null )
               content.Load();

          RpcSetupCamera();
     }

     [Server]
     public void Unload()
     {
          // disabilita il contenuto della stanza
          if( content != null )
               content.Unload();
     }

     private void SetupWalls()
     {
          List<Directions> dirs = new List<Directions>( doors );

          GameObject wall = null;
          if( dirs.Contains( Directions.East ) )
          {
               wall = Instantiate( walls[3], transform );
          }
          else
          {
               wall = Instantiate( walls[1], transform );
          }
          wall.transform.localPosition = new Vector3( roomSize.x / 2 - wallWidth / 2, 0, 0 );

          if( dirs.Contains( Directions.West ) )
          {
               wall = Instantiate( walls[3], transform );
          }
          else
          {
               wall = Instantiate( walls[1], transform );
          }
          wall.transform.localPosition = new Vector3( -roomSize.x / 2 + wallWidth / 2, 0, 0 );

          if( dirs.Contains( Directions.North ) )
          {
               wall = Instantiate( walls[2], transform );
          }
          else
          {
               wall = Instantiate( walls[0], transform );
          }
          wall.transform.localPosition = new Vector3( 0, 0, roomSize.y / 2 - wallWidth / 2 );

          if( dirs.Contains( Directions.South ) )
          {
               wall = Instantiate( walls[2], transform );
          }
          else
          {
               wall = Instantiate( walls[0], transform );
          }
          wall.transform.localPosition = new Vector3( 0, 0, -roomSize.y / 2 + wallWidth / 2 );
     }

     [ClientRpc]
     private void RpcSetupCamera()
     {
          if( FindObjectOfType<SharedCharacter>().localRole == Role.Legs )
          {
               Camera.main.transform.parent = roomCameraSocket;
               Camera.main.transform.Reset();
               Camera.main.orthographic = true;
               Camera.main.orthographicSize = 4.5f;
          }
     }
}

public enum Directions
{
     North,
     South,
     East,
     West,
}
