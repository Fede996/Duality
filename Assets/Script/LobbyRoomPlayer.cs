using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System.Linq;

public class LobbyRoomPlayer : NetworkRoomPlayer
{
     [SyncVar]
     public string playerName = null;
     [SyncVar]
     public string playerRole = null;

     // =====================================================================

     public override void OnStartLocalPlayer()
     {
          CmdSetPlayerName( ( ( LobbyRoomManager )NetworkManager.singleton ).localPlayerName );
          CmdTogglePlayerRole();

          base.OnStartLocalPlayer();
     }

     public override void OnGUI()
     {
          base.OnGUI();

          LobbyRoomManager room = NetworkManager.singleton as LobbyRoomManager;
          if( room )
          {
               if( !NetworkManager.IsSceneActive( room.RoomScene ) )
                    return;

               DrawPlayerState();
               DrawPlayerButtons();
          }
     }

     void DrawPlayerState()
     {
          GUILayout.BeginArea( new Rect( 100 + ( index * 120 ), 350, 120, 130 ) );

          GUILayout.Label( $"Player [{index + 1}]" );

          GUILayout.Label( $"Name: {playerName}" );
          GUILayout.Label( $"Role: {playerRole}" );

          if( readyToBegin )
               GUILayout.Label( "Ready" );
          else
               GUILayout.Label( "Not Ready" );

          if( ( ( isServer && index > 0 ) || isServerOnly ) && GUILayout.Button( "REMOVE" ) )
          {
               // This button only shows on the Host for all players other than the Host
               // Host and Players can't remove themselves (stop the client instead)
               // Host can kick a Player this way.
               GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
          }

          GUILayout.EndArea();
     }

     void DrawPlayerButtons()
     {
          if( NetworkClient.active && isLocalPlayer )
          {
               GUILayout.BeginArea( new Rect( 100, 500, 120, 100 ) );

               if( readyToBegin )
               {
                    if( GUILayout.Button( "Cancel" ) )
                         CmdChangeReadyState( false );
               }
               else
               {
                    if( GUILayout.Button( "Ready" ) )
                         CmdChangeReadyState( true );
               }

               if( GUILayout.Button( "Toggle Role" ) )
                    CmdTogglePlayerRole();

               GUILayout.EndArea();
          }
     }

     // =====================================================================

     [Command]
     private void CmdSetPlayerName( string name )
     {
          playerName = name;
     }

     [Command]
     private void CmdTogglePlayerRole()
     {
          if( playerRole == "Testa" ) playerRole = "Gambe";
          else playerRole = "Testa";
     }
}
