using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
     [Header( "Settings" )]
     public float serverSyncTime = 1f;

     [Header( "UI Components" )]
     [Space]
     public InputField username;
     public Toggle rememberMe;
     public Button loginButton;
     public Text playerName;
     public Text playerLevel;
     public Scrollbar levelBar;
     public Text playerCash;
     public InputField serverIp;
     [Space]
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

     [Header( "Animation" )]
     public AnimationClip[] clips;

     private Animation anim;
     private CameraController player;
     private AccessManager access;
     [HideInInspector] public LobbyRoomPlayer roomPlayer;
     private UserData playerData
     {
          get
          {
               return player.userData;
          }
          set
          {
               player.userData = value;
          }
     }

     // ==================================================================================
     // Unity events

     private void Start()
     {
          access = FindObjectOfType<AccessManager>();
          player = FindObjectOfType<CameraController>();

          anim = GetComponent<Animation>();
          foreach( AnimationClip clip in clips )
          {
               clip.legacy = true;
          }
          anim.Play( "UI_start" );

          if( DataLoader.globalData.rememberMe )
          {
               username.text = DataLoader.globalData.lastUsername;
               rememberMe.isOn = DataLoader.globalData.rememberMe;
          }

          if( Application.isBatchMode )
          {
               OnButtonServer();
          }
     }

     private float elapsed = 0f;
     private bool dirty = false;

     private void Update()
     {
          elapsed += Time.deltaTime;
          if( elapsed >= serverSyncTime )
          {
               elapsed = elapsed % serverSyncTime;

               if( dirty )
               {
                    roomPlayer.UpdatePlayerData();
                    dirty = false;
               }
          }
     }

     // ==================================================================================
     // UI

     // Login page
     // ----------

     public void OnButtonLogin()
     {
          if( string.IsNullOrEmpty( username.text ) )
          {
               Debug.LogWarning( "Please insert a valid Username!" );
               return;
          }

          if( access.Login( username.text ) )
          {
               Debug.Log( "Login succesful!" );
               OnLogin();
          }
          else
          {
               Debug.LogWarning( "Username not found!" );
          }
     }

     public void OnButtonCreate()
     {
          if( string.IsNullOrEmpty( username.text ) )
          {
               Debug.LogWarning( "Please insert a valid Username!" );
               return;
          }

          if( access.Create( username.text ) )
          {
               Debug.Log( "User succesfully created!" );
               loginButton.interactable = true;
          }
          else
          {
               Debug.LogWarning( "User already exists!" );
          }
     }

     private void OnLogin()
     {
          DataLoader.globalData.rememberMe = rememberMe.isOn;
          DataLoader.globalData.lastUsername = username.text;
          DataLoader.globalData.Save();

          anim.Play( "UI_login" );

          playerName.text = player.userData.username;
          playerLevel.text = player.userData.level.ToString();
          levelBar.size = player.userData.exp / player.userData.expToNextLevel;
          playerCash.text = player.userData.cash.ToString();
          if( !string.IsNullOrEmpty( player.userData.serverIp ) )
               serverIp.text = player.userData.serverIp;
     }

     // Connect page
     // ------------

     private bool isHost = false;

     public void OnButtonConnect()
     {
          if( !access.Connect( serverIp.text ) )
          {
               Debug.LogWarning( "Please insert a valid IP address!" );
          }
     }

     public void OnClientConnect()
     {
          Debug.Log( "Joined MATCHING sever!" );

          if( !isHost )
          {
               player.userData.serverIp = serverIp.text;
               player.userData.Save();
          }

          anim.Play( "UI_connect" );
     }

     public void OnClientDisconnect()
     {
          Debug.LogWarning( "Disconnected from MATCHING server!" );

          anim.Play( "UI_disconnect" );
     }

     public void OnButtonServer()
     {
          isHost = false;
          access.OpenServer();
     }

     public void OnServerStart()
     {
          Debug.Log( "Started MATCHING sever!" );

          if( !isHost )
          {
               anim.Play( "UI_server" );
          }
     }

     public void OnServerClose()
     {
          Debug.Log( "Closed MATCHING server!" );

          if( !isHost )
          {
               anim.Play( "UI_disconnect" );
          }
     }

     public void OnButtonHost()
     {
          isHost = true;
          access.Host();
     }

     // Server page
     // -----------

     public void OnButtonCloseServer()
     {
          access.StopServer();
          anim.Play( "UI_server_close" );
     }

     // Lobby page
     // ----------

     public void SetupLobby( bool leader, bool setDirty = true )
     {
          roleP1.text = playerData.role;
          readyP1.text = "<color=red>NOT READY</color>";

          if( leader )
          {
               startButton.gameObject.SetActive( true );
               levelList.gameObject.SetActive( true );
               levelList.options.Clear();
               for( int i = 1; i < SceneManager.sceneCountInBuildSettings; i++ )
               {
                    levelList.options.Add( new Dropdown.OptionData( Path.GetFileNameWithoutExtension( SceneUtility.GetScenePathByBuildIndex( i ) ) ) );
               }
               levelList.RefreshShownValue();
               leaderP1.enabled = true;
          }
          else
          {
               levelList.gameObject.SetActive( false );
               startButton.gameObject.SetActive( false );
               leaderP1.enabled = false;
          }

          dirty = setDirty;
     }

     public void ResetPlayer2()
     {
          nameP2.text = "Waiting player...";
          roleP2.text = "HEAD";
          readyP2.text = "<color=red>NOT READY</color>";
          leaderP2.enabled = false;
          imageP2.color = new Color( 0.2f, 0.2f, 0.2f );
     }

     public void UpdatePlayer2( UserData playerData )
     {
          nameP2.text = playerData.username;
          roleP2.text = playerData.role;
          readyP2.text = playerData.ready ? "<color=green>READY</color>" : "<color=red>NOT READY</color>";
          leaderP2.enabled = playerData.leader;
          imageP2.color = Color.HSVToRGB( playerData.color, 0.8f, 0.8f );
     }

     public void OnColorSliderChanged()
     {
          imageP1.color = Color.HSVToRGB( colorSlider.value, 0.8f, 0.8f );
          playerData.color = colorSlider.value;
          dirty = true;
     }

     public void OnButtonRole()
     {
          playerData.role = playerData.role == "HEAD" ? "LEGS" : "HEAD";
          roleP1.text = playerData.role;
          dirty = true;
     }

     public void OnButtonReady()
     {
          playerData.ready = !playerData.ready;
          readyP1.text = playerData.ready ? "<color=green>READY</color>" : "<color=red>NOT READY</color>";
          roomPlayer.SetPlayerReady( playerData.ready );
          dirty = true;
     }

     public void OnButtonStart()
     {
          roomPlayer.CmdStartGame( levelList.captionText.text );
     }

     public void OnButtonBack()
     {
          if( isHost )
               access.StopHost();
          else
               access.StopClient();

          anim.Play( "UI_disconnect" );
     }

     // ==================================================================================
     // Animations

     public void OnScreenFocused()
     {
          anim.Play( "UI_unlock" );
     }

     public void OnUnlockAnimationEnded()
     {
          player.OnUnlockAnimationEnded();
          if( DataLoader.globalData.firstLaunch )
          {
               loginButton.interactable = false;
               Debug.Log( "Create your first local account!\n" +
                          "You can share your desktop with multiple people, they just need to have a local account.\n" );
          }
     }

     public void OnLoginAnimationEnded()
     {
          if( DataLoader.globalData.firstLaunch )
          {
               Debug.Log( "This is your Main Page;\n" +
                          "From here you can JOIN your colleagues and start EXPEDITIONS.\n" +
                          "Remember that you always need a partner,\n" +
                          "otherwise you won't be able to pilot our MECHS!\n" );

               Debug.Log( "The CONNECT button will let you join one of our MATCHING server.\n" );
          }
     }

     public void OnLockAnimationEnded()
     {
          player.OnLockAnimationEnded();
     }
}
