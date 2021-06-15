using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;
using System.Linq;

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

     [Header( "Navigation" )]
     public GameObject floor;
     public float navMeshHeight;
     public bool debugNavMesh;

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
          {
               content.Load();
               BuildNavMesh();

               foreach( Door door in content.childObjects.Select( o => o.GetComponent<Door>() ) )
               {
                    if( door != null )
                    {
                         door.Init( doors.Contains( door.direction ) );
                    }
               }
          }

          RpcSetupCamera();
     }

     [Server]
     public void Unload()
     {
          // disabilita il contenuto della stanza
          if( content != null )
          {
               DisableNavMesh();
               content.Unload();
          }
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

     [Server]
     private void BuildNavMesh()
     {
          foreach( NavMeshAgent agent in FindObjectsOfType<NavMeshAgent>() )
          {
               agent.enabled = false;
          }

          NavMeshBuildSettings settings = new NavMeshBuildSettings()
          {
               agentHeight = 2,
               agentRadius = 0.5f,
               agentSlope = 45,
               agentClimb = 0.75f,
               agentTypeID = 0
          };

          List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
          Bounds bounds = new Bounds( transform.position, new Vector3( roomSize.x, navMeshHeight, roomSize.y ) );
          NavMeshBuilder.CollectSources( bounds, LayerMask.GetMask( "Default", "Ground" ), NavMeshCollectGeometry.RenderMeshes, 0, new List<NavMeshBuildMarkup>(), sources );

          NavMeshData navMesh = NavMeshBuilder.BuildNavMeshData( settings, sources, bounds, Vector3.zero, Quaternion.identity );
          NavMesh.RemoveAllNavMeshData();
          NavMesh.AddNavMeshData( navMesh );

          if( debugNavMesh )
          {
               NavMeshVisualizator vis = FindObjectOfType<NavMeshVisualizator>();
               vis.transform.position = navMesh.position;
               vis.transform.rotation = navMesh.rotation;
               vis.ShowMesh();
          }

          foreach( GameObject o in content.childObjects )
          {
               if( o != null )
               {
                    NavMeshAgent agent = o.GetComponent<NavMeshAgent>();
                    if( agent != null )
                    {
                         agent.enabled = true;
                    }
               }
          }
     }

     [Server]
     private void DisableNavMesh()
     {
          foreach( GameObject o in content.childObjects )
          {
               if( o != null )
               {
                    NavMeshAgent agent = o.GetComponent<NavMeshAgent>();
                    if( agent != null )
                    {
                         agent.enabled = false;
                    }
               }
          }
     }

     [ClientRpc]
     private void RpcSetupCamera()
     {
          StartCoroutine( SetupCamera() );
     }

     private IEnumerator SetupCamera()
     {
          SharedCharacter player = FindObjectOfType<SharedCharacter>();

          for(; ; )
          {
               if( player.initialized )
               {
                    if( player.localRole == Role.Legs )
                    {
                         Camera.main.transform.parent = roomCameraSocket;
                         Camera.main.transform.Reset();
                         Camera.main.orthographic = true;
                         Camera.main.orthographicSize = roomSize.y / 2;
                    }

                    break;
               }

               yield return null;
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
