using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System.Linq;

public class LobbyRoomManager : NetworkRoomManager
{
     [Header( "Custom settings" )]
     public GameObject playerPawn;

     private UiManager UI;
     private CameraController player;

     // =====================================================================
     // Unity events

     public override void Start()
     {
          UI = FindObjectOfType<UiManager>();
          player = FindObjectOfType<CameraController>();

          base.Start();
     }

     // =====================================================================
     // Network events

     public override void OnClientDisconnect( NetworkConnection conn )
     {
          UI.OnClientDisconnect();

          base.OnClientDisconnect( conn );
     }

     public override void OnClientConnect( NetworkConnection conn )
     {
          UI.OnClientConnect();

          base.OnClientConnect( conn );
     }

     public override void OnStartServer()
     {
          UI.OnServerStart();

          base.OnStartServer();
     }

     public override void OnStopServer()
     {
          UI.OnServerClose();

          base.OnStopServer();
     }

     public override void OnRoomServerPlayersReady()
     {
          // calling the base method calls ServerChangeScene as soon as all players are in Ready state.
          // base.OnRoomServerPlayersReady();
     }

     public override bool OnRoomServerSceneLoadedForPlayer( NetworkConnection conn, GameObject roomPlayer, GameObject gamePlayer )
     {
          LobbyRoomPlayer lobbyPlayer = roomPlayer.GetComponent<LobbyRoomPlayer>();
          GamePlayerController player = gamePlayer.GetComponent<GamePlayerController>();
          player.playerDataJson = lobbyPlayer.playerDataJson;

          base.OnRoomServerSceneLoadedForPlayer( conn, roomPlayer, gamePlayer );

          return true;
     }

     public override void OnClientSceneChanged( NetworkConnection conn )
     {
          base.OnClientSceneChanged( conn );

          string sceneName = SceneManager.GetActiveScene().path;
          if( sceneName != offlineScene && sceneName != RoomScene )
          {
               UI.DisableMainMenuUI();
          }
          else
          {
               UI.EnableMainMenuUI();
               Cursor.visible = true;
               Cursor.lockState = CursorLockMode.Confined;
               player.SetupLobby();
          }
     }

     public override void OnServerSceneChanged( string sceneName )
     {
          if( sceneName != offlineScene && sceneName != RoomScene )
          {
               Transform spawn = GameObject.FindWithTag( "Spawn" ).transform;
               GameObject pawn = Instantiate( playerPawn, spawn.position, spawn.rotation );
               NetworkServer.Spawn( pawn );

               UI.DisableMainMenuUI();
          }
          else
          {
               UI.EnableMainMenuUI();
               Cursor.visible = true;
               Cursor.lockState = CursorLockMode.Confined;
               player.SetupLobby();
          }

          base.OnServerSceneChanged( sceneName );
     }

     [Server]
     private IEnumerator WaitPlayersReady()
     {
          for(; ; )
          {
               GamePlayerController[] players = FindObjectsOfType<GamePlayerController>();
               if( players != null &&
                   players.Length == ( ( LobbyRoomManager )NetworkManager.singleton ).roomSlots.Count() &&
                   players.All( p => p.connectionToClient.isReady ) )
               {
                    Transform spawn = GameObject.FindWithTag( "Spawn" ).transform;
                    GameObject pawn = Instantiate( playerPawn, spawn.position, spawn.rotation );
                    NetworkServer.Spawn( pawn );

                    break;
               }

               yield return null;
          }

          print( "DONE" );
     }

     // =====================================================================

     [Server]
     public bool StartGame( string sceneName, bool debug )
     {
          if( !debug )
          {
               if( numPlayers != 2 )
               {
                    Debug.LogError( $"Number of player must be 2!" );
                    return false;
               }

               if( ( ( LobbyRoomPlayer )roomSlots[0] ).playerData.role == ( ( LobbyRoomPlayer )roomSlots[1] ).playerData.role )
               {
                    Debug.LogError( "Players must have different roles!" );
                    return false;
               } 
          }

          if( !allPlayersReady ) return false;

          ServerChangeScene( sceneName );

          return true;
     }

     [Server]
     public void ReturnToLobby()
     {
          ServerChangeScene( RoomScene );
     } 
}
