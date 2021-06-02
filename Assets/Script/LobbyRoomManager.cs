using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class LobbyRoomManager : NetworkRoomManager
{
     private UiManager UI;

     // =====================================================================
     // Unity events

     public override void Start()
     {
          UI = FindObjectOfType<UiManager>();

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

          return true;
     }

     public override void OnClientSceneChanged( NetworkConnection conn )
     {
          base.OnClientSceneChanged( conn );

          if( SceneManager.GetActiveScene().name != "Main Menu" )
          {
               UI.DisableMainMenuUI();

               foreach( LobbyRoomPlayer lobbyPlayer in FindObjectsOfType<LobbyRoomPlayer>() )
                    lobbyPlayer.enabled = false;
          }
     }

     public override void OnServerSceneChanged( string sceneName )
     {
          base.OnServerSceneChanged( sceneName );

          UI.DisableMainMenuUI();
     }

     // =====================================================================

     public bool StartGame( string sceneName )
     {
          //if( numPlayers != 2 )
          //{
          //     Debug.LogError( $"Number of player must be 2! ==> {numPlayers}" );
          //     return false;
          //}

          //if( ( ( LobbyRoomPlayer )roomSlots[0] ).playerData.role == ( ( LobbyRoomPlayer )roomSlots[1] ).playerData.role )
          //{
          //     Debug.LogError( "Players must have different roles!" );
          //     return false;
          //}

          if( !allPlayersReady ) return false;

          ServerChangeScene( sceneName );

          return true;
     }
}
