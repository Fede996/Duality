using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class LobbyRoomPlayer : NetworkRoomPlayer
{
     [Header( "UI" )]
     public GameObject UI;
     public Button startButton;
     public Dropdown levelList;

     public GameObject UiPlayer1;
     public Text roleP1;
     public Text readyP1;
     public Text leaderP1;
     public Image imageP1;
     public Slider colorSlider;

     public GameObject UiPlayer2;
     public Text nameP2;
     public Text roleP2;
     public Text readyP2;
     public Text leaderP2;
     public Image imageP2;

     // =====================================================================
     // User data synchronization

     [SyncVar( hook = nameof( OnPlayerDataChanged ) )]
     private string playerDataJson;
     public UserData playerData;

     private void OnPlayerDataChanged( string oldValue, string newValue )
     {
          playerData = JsonUtility.FromJson<UserData>( newValue );

          // refresh ui...
          if( isLocalPlayer )
          {
               roleP1.text = playerData.role;
               readyP1.text = playerData.ready ? "<color=green>READY</color>" : "<color=red>NOT READY</color>";
               leaderP1.enabled = playerData.leader;
          }
          else
          {
               nameP2.text = playerData.username;
               roleP2.text = playerData.role;
               readyP2.text = playerData.ready ? "<color=green>READY</color>" : "<color=red>NOT READY</color>";
               leaderP2.enabled = playerData.leader;
               imageP2.color = Color.HSVToRGB( playerData.color, 1, 1 );
          }
     }

     [Command( requiresAuthority = false )]
     private void CmdSetPlayerData( string jsonData )
     {
          playerData = JsonUtility.FromJson<UserData>( jsonData );
          playerDataJson = jsonData;
     }

     private void UpdatePlayerData()
     {
          CmdSetPlayerData( JsonUtility.ToJson( playerData ) );
          playerData.Save();
     }

     // =====================================================================
     // UI events

     public void OnColorSliderChanged()
     {
          imageP1.color = Color.HSVToRGB( playerData.color, 1, 1 );
          playerData.color = colorSlider.value;
          UpdatePlayerData();
     }

     public void OnButtonRole()
     {
          playerData.role = playerData.role == "HEAD" ? "LEGS" : "HEAD";

          UpdatePlayerData();
     }

     public void OnButtonReady()
     {
          playerData.ready = !playerData.ready;
          CmdChangeReadyState( playerData.ready );

          UpdatePlayerData();
     }
     
     public void OnButtonBack()
     {
          if( isClientOnly )
               ( ( LobbyRoomManager )NetworkManager.singleton ).StopClient();
          else if ( isServerOnly )
               ( ( LobbyRoomManager )NetworkManager.singleton ).StopServer();
          else
               ( ( LobbyRoomManager )NetworkManager.singleton ).StopHost();

          FindObjectOfType<CameraController>().OnButtonDisconnect();
     }

     public void OnButtonStart()
     {
          if( !playerData.leader )
          {
               Debug.LogError( "Only the leader can start the game!" );
               return;
          }

          CmdStartGame( levelList.captionText.text );
     }

     [Command]
     private void CmdStartGame( string sceneName )
     {
          ( ( LobbyRoomManager )NetworkManager.singleton ).StartGame( sceneName );
     } 

     [Server]
     public void DisableUI()
     {
          if( isServerOnly )
               UI.SetActive( false );

          RpcDisableUI();
     }

     [ClientRpc]
     private void RpcDisableUI()
     {
          UI.SetActive( false );
     }

     // =====================================================================
     // Unity / Mirror events

     public override void OnStartLocalPlayer()
     {
          UiPlayer1.SetActive( true );
          UiPlayer2.SetActive( false );

          levelList.options.Clear();
          for( int i = 1; i < SceneManager.sceneCountInBuildSettings; i++ )
          {
               levelList.options.Add( new Dropdown.OptionData( Path.GetFileNameWithoutExtension( SceneUtility.GetScenePathByBuildIndex( i ) ) ) );
          }

          playerData = ( ( LobbyRoomManager )NetworkManager.singleton ).localPlayerData;
          colorSlider.value = playerData.color;

          playerData.serverIp = ( ( LobbyRoomManager )NetworkManager.singleton ).networkAddress;
          playerData.leader = index == 0;
          playerData.ready = false;
          UpdatePlayerData();

          if( playerData.leader )
          {
               startButton.enabled = true;
               levelList.enabled = true;
          }
          else
          {
               startButton.enabled = false;
               levelList.enabled = false;
          }

          base.OnStartLocalPlayer();
     }

     public override void OnStartClient()
     {
          if( !isLocalPlayer )
          {
               UiPlayer1.SetActive( false );
               UiPlayer2.SetActive( true );
          }

          base.OnStartClient();
     }

     public override void IndexChanged( int oldIndex, int newIndex )
     {
          playerData.leader = index == 0;
          UpdatePlayerData();

          if( playerData.leader )
          {
               startButton.enabled = true;
               levelList.enabled = true;
          }
          else
          {
               startButton.enabled = false;
               levelList.enabled = false;
          }

          base.IndexChanged( oldIndex, newIndex );
     }
}
