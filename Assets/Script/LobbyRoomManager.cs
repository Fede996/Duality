using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class LobbyRoomManager : NetworkRoomManager
{
     [Header( "Local player data" )]
     public string localPlayerName = null;
     public UserData localPlayerData;

     // =====================================================================

     public override bool OnRoomServerSceneLoadedForPlayer( NetworkConnection conn, GameObject roomPlayer, GameObject gamePlayer )
     {
          LobbyRoomPlayer lrp = roomPlayer.GetComponent<LobbyRoomPlayer>();
          DisableUI();

          GamePlayerController player = gamePlayer.GetComponent<GamePlayerController>();
          player.playerName = lrp.playerData.username;
          player.playerRole = lrp.playerData.role;

          return true;
     }

     public override void OnRoomServerPlayersReady()
     {
          // calling the base method calls ServerChangeScene as soon as all players are in Ready state.
          // base.OnRoomServerPlayersReady();
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

     private void DisableUI()
     {
          foreach( NetworkRoomPlayer lrp in roomSlots )
          {
               ( ( LobbyRoomPlayer )lrp ).DisableUI();
          }
     }
}
