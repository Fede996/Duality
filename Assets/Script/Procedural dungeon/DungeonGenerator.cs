using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
     public GameObject player;

     [Header( "Settings" )]
     public Transform root;
     public Vector2 mapSize;
     public int numberOfRooms;

     [Header( "Structure" )]
     public GameObject roomPrefab;
     public Vector2 roomSize;
     // Ordine di inserimento mura:
     // 1) muro nord/sud
     // 2) muro est/ovest
     // 3) muro nord/sud con porta
     // 4) muro est/ovest con porta
     public List<GameObject> walls;
     public float wallWidth;

     [Header( "Debug" )]
     public List<Vector2> roomLocations;

     private int[][] map;

     // =======================================================
     // Unity events

     private void Start()
     {
          Generate( numberOfRooms );
          //root.GetComponentInChildren<Room>().Load();
     }

     private void Update()
     {
          if( Input.GetKeyDown( KeyCode.G ) )
          {
               Clear();
               Generate( numberOfRooms );
          }
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

     private void Generate( int numberOfRooms )
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

               // genero pavimento
               Vector3 position = new Vector3( ( x - startX ) * roomSize.x, 0, ( y - startY ) * roomSize.y );
               GameObject room = Instantiate( roomPrefab, root );
               room.transform.localPosition = position;

               switch( map[x][y] )
               {
                    case 1:
                    {
                         room.GetComponentInChildren<Renderer>().material.color = Color.white;
                         room.name = "Room";
                         break;
                    }
                    case 2:
                    {
                         room.GetComponentInChildren<Renderer>().material.color = Color.cyan;
                         room.name = "Start room";
                         break;
                    }
                    case 3:
                    {
                         room.GetComponentInChildren<Renderer>().material.color = Color.red;
                         room.name = "End room";
                         break;
                    }
               }

               // calcolo porte
               GameObject wall = null;
               if( x + 1 <= map.Length - 1 && map[x + 1][y] != 0 )
               {
                    // porta a est
                    wall = GameObject.Instantiate( walls[3], room.transform );
               }
               else
               {
                    wall = GameObject.Instantiate( walls[1], room.transform );
               }
               wall.transform.localPosition = new Vector3( roomSize.x / 2 - wallWidth / 2, 0, 0 );

               if( x != 0 && map[x - 1][y] != 0 )
               {
                    // porta a ovest
                    wall = GameObject.Instantiate( walls[3], room.transform );
               }
               else
               {
                    wall = GameObject.Instantiate( walls[1], room.transform );
               }
               wall.transform.localPosition = new Vector3( -roomSize.x / 2 + wallWidth / 2, 0, 0 );

               if( y + 1 <= map[x].Length - 1 && map[x][y + 1] != 0 )
               {
                    // porta a nord
                    wall = GameObject.Instantiate( walls[2], room.transform );
               }
               else
               {
                    wall = GameObject.Instantiate( walls[0], room.transform );
               }
               wall.transform.localPosition = new Vector3( 0, 0, roomSize.y / 2 - wallWidth / 2 );

               if( y != 0 && map[x][y - 1] != 0 )
               {
                    // porta a sud
                    wall = GameObject.Instantiate( walls[2], room.transform );
               }
               else
               {
                    wall = GameObject.Instantiate( walls[0], room.transform );
               }
               wall.transform.localPosition = new Vector3( 0, 0, -roomSize.y / 2 + wallWidth / 2 );

          }

          // spawno il giocatore
          //Instantiate( player, root );
          //root.GetComponentInChildren<Camera>().enabled = true;
     }

     private void Clear()
     {
          roomLocations.Clear();
          for( int i = 0; i < root.childCount; i++ )
          {
               Destroy( root.GetChild( i ).gameObject );
          }
          InitMap();
     }
}
