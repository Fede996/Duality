using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class DungeonGeneratorDebug : NetworkBehaviour
{
     [Header( "Settings" )]
     public Transform root;
     public Vector2 mapSize;
     public int numberOfRooms;

     [Header( "Structure" )]
     public GameObject roomPrefab;
     public Vector2 roomSize;
     public List<GameObject> possibleRoomContents;
     public GameObject startRoomContent;
     public GameObject endRoomContent;

     private List<Vector2> roomLocations;
     private int[][] map;

     // =======================================================
     // Unity events

     private void Start()
     {
          if( isServer )
          {
               StartCoroutine( WaitPlayersReady() );
          }
     }

     [Server]
     private IEnumerator WaitPlayersReady()
     {
          for( ; ; )
          {
               GamePlayerController[] players = FindObjectsOfType<GamePlayerController>();
               if( players != null && 
                   players.Length == ( ( LobbyRoomManager )NetworkManager.singleton ).roomSlots.Count() && 
                   players.All( p => p.connectionToClient.isReady ) )
               {
                    break;
               }
               
               yield return null;
          }

          Generate( numberOfRooms );
     }

     // =======================================================
     // Methods

     private void InitMap( int x, int y )
     {
          mapSize = new Vector2( x, y );
          InitMap();
     }

     private void InitMap()
     {
          map = new int[( int )mapSize.x][];
          for( int i = 0; i < map.Length; i++ )
          {
               map[i] = new int[( int )mapSize.y];
          }

          for( int r = 0; r < map.Length; r++ )
          {
               for( int c = 0; c < map[r].Length; c++ )
               {
                    map[r][c] = 0;
               }
          }
     }

     [Server]
     public void Generate( int numberOfRooms )
     {
          if( map == null )
               InitMap();

          // valori ammessi per le celle:
          // 0 ==> cella vuota 
          // 1 ==> stanza
          // 2 ==> prima stanza (spawn)
          // 3 ==> ultima stanza (boos)

          int nCells = ( int )mapSize.x * ( int )mapSize.y;
          int x = ( int )Random.Range( 0, mapSize.x );
          int y = ( int )Random.Range( 0, mapSize.y );
          int startX = x;
          int startY = y;

          roomLocations = new List<Vector2>( numberOfRooms );

          // rimepio cella di partenza
          map[x][y] = 2;
          numberOfRooms--;
          roomLocations.Add( new Vector2( x, y ) );

          while( numberOfRooms > 0 )
          {
               if( map[x][y] == 0 )
               {
                    // sono su una cella vuota, la riempio
                    if( numberOfRooms == 1 )
                    {
                         map[x][y] = 3;
                    }
                    else
                    {
                         map[x][y] = 1;
                    }
                    numberOfRooms--;
                    roomLocations.Add( new Vector2( x, y ) );
               }

               // seleziono direzione random
               switch( Random.Range( 0, 4 ) )
               {
                    case 0:
                    {
                         // nord
                         y++;
                         break;
                    }
                    case 1:
                    {
                         // sud
                         y--;
                         break;
                    }
                    case 2:
                    {
                         // est
                         x++;
                         break;
                    }
                    case 3:
                    {
                         // ovest 
                         x--;
                         break;
                    }
               }

               // se sono uscito dalla mappa riparto da capo
               if( y >= ( int )mapSize.y || y < 0 )
               {
                    y = startY;
               }
               if( x >= ( int )mapSize.x || x < 0 )
               {
                    x = startX;
               }
          }

          // generazione finita

          // istanzio le stanze
          foreach( Vector2 index in roomLocations )
          {
               x = ( int )index.x;
               y = ( int )index.y;

               // genero stanza
               Vector3 position = new Vector3( ( x - startX ) * roomSize.x, 0, ( y - startY ) * roomSize.y );

               GameObject room = Instantiate( roomPrefab, root );
               room.transform.localPosition = position;
               GameObject content = null;
               switch( map[x][y] )
               {
                    case 1:
                    {
                         // stanza normale
                         room.name = "Room";
                         content = Instantiate( possibleRoomContents[Random.Range( 0, possibleRoomContents.Count )], position, Quaternion.identity, null );
                         break;
                    }
                    case 2:
                    {
                         // prima stanza
                         room.name = "Start room";
                         if( startRoomContent != null )
                              content = Instantiate( startRoomContent, position, Quaternion.identity, null );
                         break;
                    }
                    case 3:
                    {
                         // ultima stanza
                         room.name = "End room";
                         if( endRoomContent != null )
                              content = Instantiate( endRoomContent, position, Quaternion.identity, null );
                         break;
                    }
               }

               // calcolo porte
               List<Directions> dirs = new List<Directions>();
               if( x + 1 <= map.Length - 1 && map[x + 1][y] != 0 )
               {
                    dirs.Add( Directions.East );
               }

               if( x != 0 && map[x - 1][y] != 0 )
               {
                    dirs.Add( Directions.West );
               }

               if( y + 1 <= map[x].Length - 1 && map[x][y + 1] != 0 )
               {
                    dirs.Add( Directions.North );
               }

               if( y != 0 && map[x][y - 1] != 0 )
               {
                    dirs.Add( Directions.South );
               }
               room.GetComponent<Room>().doors.AddRange( dirs );
               if( content != null )
               {
                    room.GetComponent<Room>().content = content.GetComponent<ContentLoader>(); 
               }

               NetworkServer.Spawn( room );
          }
     }

     [Server]
     private void Clear()
     {
          roomLocations.Clear();
          for( int i = 0; i < root.childCount; i++ )
          {
               NetworkServer.Destroy( root.GetChild( i ).gameObject );
          }
          InitMap();
     }
}
