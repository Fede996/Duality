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
          GameNetworkPlayer player = gamePlayer.GetComponent<GameNetworkPlayer>();
          player.playerName = roomPlayer.GetComponent<RoomPlayer>().playerName;
          player.playerRole = roomPlayer.GetComponent<RoomPlayer>().playerRole;

          return true;
     }

     public override void OnRoomServerPlayersReady()
     {
          // calling the base method calls ServerChangeScene as soon as all players are in Ready state.
#if UNITY_SERVER
            base.OnRoomServerPlayersReady();
#else
          showStartButton = true;
#endif
     }

     public override void OnGUI()
     {
          base.OnGUI();

          if( allPlayersReady && showStartButton && GUI.Button( new Rect( 300, 300, 120, 20 ), "START GAME" ) )
          {
               StartGame();
          }
     }

     // =====================================================================

     private void StartGame()
     {
          if( ( ( RoomPlayer )roomSlots[0] ).playerRole == ( ( RoomPlayer )roomSlots[1] ).playerRole )
          {
               Debug.LogError( "Players must have different roles!" );
               return;
          }

          showStartButton = false;
          ServerChangeScene( GameplayScene );
     }
}
