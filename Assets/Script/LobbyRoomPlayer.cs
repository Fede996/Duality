using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class LobbyRoomPlayer : NetworkRoomPlayer
{
     private UiManager UI;
     private CameraController player;
     public UserData playerData 
     {
          get
          {
               return player.playerData;
          }
          set
          {
               player.playerData = value;
          }
     }

     // =====================================================================
     // Unity / Mirror events

     public override void OnStartClient()
     {
          UI = FindObjectOfType<UiManager>();

          base.OnStartClient();
     }

     public override void OnStopClient()
     {
          if( isLocalPlayer )
          {
               playerData.leader = false;
               playerData.Save();
          }
          else
          {
               UI.ResetPlayer2();

               player = FindObjectOfType<CameraController>();
               playerData.leader = index == 0;
               playerData.ready = false;
               UI.SetupLobby( playerData.leader, false );
          }

          base.OnStopClient();
     }

     public override void OnClientEnterRoom()
     {
          if( isLocalPlayer )
          {
               player = FindObjectOfType<CameraController>();

               bool isLeader = index == 0;
               playerData.leader = isLeader;
               playerData.ready = false;

               UI.roomPlayer = this;
               UI.SetupLobby( isLeader );
          }

          base.OnClientEnterRoom();
     }

     // =====================================================================
     // User data synchronization

     [SyncVar( hook = nameof( OnPlayerDataChanged ) )]
     public string playerDataJson;

     private void OnPlayerDataChanged( string oldValue, string newValue )
     {
          if( !isLocalPlayer )
          {
               UserData newPlayerData = JsonUtility.FromJson<UserData>( newValue );
               if( UI == null ) UI = FindObjectOfType<UiManager>();
               UI.UpdatePlayer2( newPlayerData );
          }
     }

     [Command]
     private void CmdSetPlayerData( string jsonData )
     {
          playerDataJson = jsonData;
     }

     public void UpdatePlayerData()
     {
          CmdSetPlayerData( JsonUtility.ToJson( playerData ) );
     }

     // =====================================================================
     // UI events

     public void SetPlayerReady( bool ready )
     {
          CmdChangeReadyState( ready );
     }

     [Command]
     public void CmdStartGame( string sceneName, bool debug )
     {
          ( ( LobbyRoomManager )NetworkManager.singleton ).StartGame( sceneName, true );
     }

     [Server]
     public void DisableUI()
     {
          RpcDisableUI();
     }

     [ClientRpc]
     private void RpcDisableUI()
     {
          player.gameObject.SetActive( false );
     }
}
