using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class LobbyRoomManager : NetworkRoomManager
{
     [Header( "Custom settings" )]
     public string localPlayerName = null;

     private bool showStartButton;

     // =====================================================================

     public override bool OnRoomServerSceneLoadedForPlayer( NetworkConnection conn, GameObject roomPlayer, GameObject gamePlayer )
     {
          GamePlayerController player = gamePlayer.GetComponent<GamePlayerController>();
          player.playerName = roomPlayer.GetComponent<LobbyRoomPlayer>().playerName;
          player.playerRole = roomPlayer.GetComponent<LobbyRoomPlayer>().playerRole;

          return true;
     }

     public override void OnRoomServerPlayersReady()
     {
          // calling the base method calls ServerChangeScene as soon as all players are in Ready state.
          // base.OnRoomServerPlayersReady();

          showStartButton = true;
     }

     public override void OnGUI()
     {
          base.OnGUI();

          if( allPlayersReady && showStartButton && GUI.Button( new Rect( 100f + ( numPlayers * 120 ), 500, 120, 20 ), "START GAME" ) )
          {
               StartGame();
          }
     }

     // =====================================================================

     private void StartGame()
     {
          // Per DEBUG, da rimuovere!
          if( numPlayers == 1 )
          {
               showStartButton = false;
               ServerChangeScene( GameplayScene );
               return;
          }
          //

          if( numPlayers != 2 )
          {
               Debug.LogError( "Number of player must be 2!" );
               return;
          }

          if( ( ( LobbyRoomPlayer )roomSlots[0] ).playerRole == ( ( LobbyRoomPlayer )roomSlots[1] ).playerRole )
          {
               Debug.LogError( "Players must have different roles!" );
               return;
          }

          showStartButton = false;
          ServerChangeScene( GameplayScene );
     }
}
