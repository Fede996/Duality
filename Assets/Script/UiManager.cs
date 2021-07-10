using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class UiManager : MonoBehaviour
{
     [Header( "Settings" )]
     public float playerDataSyncTime = 1f;
     public bool debug = false;
     public bool debugScene = false;
     [Header( "Login page" )]
     public GameObject desktop;
     public InputField username;
     public Toggle rememberMe;
     public Button loginButton;
     public Text playerName;
     public Text playerLevel;
     public Scrollbar levelBar;
     public Text exp;
     public Text playerCash;
     public InputField serverIp;

     [Header( "Lobby page" )]
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
     private PlayerSelection selectorP1;
     private PlayerSelection selectorP2;

     [Header( "Esc menu" )]
     public GameObject escMenu;
     public GameObject pausePage;
     public Button exit;
     public Button leave;
     public GameObject leaveDisclaimer;
     public Text quality;
     public Slider qualitySlider;
     public Button applyButton;
     public Toggle statsToggle;
     public GameObject statsPanel;
     public Toggle postProcessingToggle;
     public Toggle introToggle;

     [Header( "Game" )]
     public GameObject gameUiRoot;
     public GameObject points;
     public Text pointsHead;
     public Text pointsLegs;  
     public GameObject sight;
     public GameObject weaponInfo;
     public Text fireMode;
     public GameObject gameOver;
     public GameObject lifePanel;
     public GameObject lifePrefab;
     public GameObject fuel;
     public Scrollbar fuelBar;
     public GameObject ammo;
     public Scrollbar ammoBar;
     public Text ammoText;

     [Header( "Dialogue system" )]
     public Animator dialogueAnimator;
     public Text dialogueText;
     public Text speakerName;

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
               return player.playerData;
          }
          set
          {
               player.playerData = value;
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
     }

     private float elapsed = 0f;
     private bool dirty = false;

     private bool prevCursorVisible = false;
     private CursorLockMode preCursorLock = CursorLockMode.Locked;

     private void Update()
     {
          elapsed += Time.deltaTime;
          if( elapsed >= playerDataSyncTime )
          {
               elapsed %= playerDataSyncTime;

               if( dirty )
               {
                    roomPlayer.UpdatePlayerData();
                    dirty = false;
               }
          }

          if( Input.GetKeyDown( KeyCode.Return ) )
          {
               ShowNextSentence();
          }

          if( Input.GetButtonDown( "Cancel" ) )
          {
               if( escMenu.activeInHierarchy )
               {
                    escMenu.SetActive( false );
                    Cursor.visible = prevCursorVisible;
                    Cursor.lockState = preCursorLock;

                    if( gameUiRoot.activeInHierarchy )
                    {
                         // sono in game
                         foreach( GamePlayerController controller in FindObjectsOfType<GamePlayerController>() )
                         {
                              controller.EnableInput();
                         }
                    }
                    else
                    {
                         // sono in lobby
                         player.currentStatus = player.prevStatus;
                    }
               }
               else
               {
                    prevCursorVisible = Cursor.visible;
                    preCursorLock = Cursor.lockState;
                    escMenu.SetActive( true );
                    leaveDisclaimer.SetActive( false );
                    pausePage.SetActive( true );
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.Confined;

                    if( gameUiRoot.activeInHierarchy )
                    {
                         // sono in game
                         foreach( GamePlayerController controller in FindObjectsOfType<GamePlayerController>() )
                         {
                              controller.DisableInput();
                         }

                         leave.gameObject.SetActive( true );
                         exit.gameObject.SetActive( false );
                    }
                    else
                    {
                         // sono in lobby
                         player.prevStatus = player.currentStatus;
                         player.currentStatus = Status.DoNothing;

                         leave.gameObject.SetActive( false );
                         exit.gameObject.SetActive( true );
                    }
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

          playerName.text = playerData.username;
          playerLevel.text = playerData.level.ToString();
          levelBar.size = playerData.exp / playerData.expToNextLevel;
          exp.text = $"{playerData.exp.ToString( "F0" )} / {playerData.expToNextLevel.ToString( "F0" )}";
          playerCash.text = playerData.cash.ToString();
          if( !string.IsNullOrEmpty( playerData.serverIp ) )
               serverIp.text = playerData.serverIp;
     }

     // Connect page
     // ------------

     private bool isHost = false;
     public bool isSolo = false;

     public void OnButtonConnect()
     {
          isSolo = false;
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
               player.playerData.serverIp = serverIp.text;
               player.playerData.Save();
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
          isSolo = false;
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
          isSolo = false;
          isHost = true;
          access.Host();
     }

     public void OnButtonSolo()
     {
          // to do...
          isSolo = true;
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
          selectorP1 = GameObject.Find( "Player selection 1" ).GetComponent<PlayerSelection>();
          colorSlider.value = playerData.color;

          if( isSolo )
          {
               UiPlayer2.SetActive( false );
               roleP1.transform.parent.gameObject.SetActive( false );
               readyP1.transform.parent.gameObject.SetActive( false );

               StartCoroutine( WaitAndSetReady() );
          }
          else
          {
               UiPlayer2.SetActive( true );
               roleP1.transform.parent.gameObject.SetActive( true );
               readyP1.transform.parent.gameObject.SetActive( true );

               selectorP2 = GameObject.Find( "Player selection 2" ).GetComponent<PlayerSelection>();
               roleP1.text = playerData.role;
               selectorP1.SetRole( playerData.role );
               readyP1.text = "<color=red>NOT READY</color>";
          }

          if( leader )
          {
               startButton.gameObject.SetActive( true );
               if( debug )
               {
                    levelList.gameObject.SetActive( true );
                    levelList.options.Clear();
                    for( int i = 1; i < SceneManager.sceneCountInBuildSettings; i++ )
                    {
                         levelList.options.Add( new Dropdown.OptionData( Path.GetFileNameWithoutExtension( SceneUtility.GetScenePathByBuildIndex( i ) ) ) );
                    }
                    levelList.RefreshShownValue();
               }
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

     private IEnumerator WaitAndSetReady()
     {
          yield return new WaitForSecondsRealtime( 0.5f );

          OnButtonReady();
     }

     public void ResetPlayer2()
     {
          nameP2.text = "Waiting player...";
          roleP2.text = "HEAD";
          selectorP2.SetRole( "HEAD" );
          readyP2.text = "<color=red>NOT READY</color>";
          leaderP2.enabled = false;
          imageP2.color = new Color( 0.2f, 0.2f, 0.2f );
          selectorP2.SetColor( imageP2.color );
     }

     public void UpdatePlayer2( UserData playerData )
     {
          nameP2.text = playerData.username;
          roleP2.text = playerData.role;
          selectorP2.SetRole( playerData.role );
          readyP2.text = playerData.ready ? "<color=green>READY</color>" : "<color=red>NOT READY</color>";
          leaderP2.enabled = playerData.leader;
          imageP2.color = Color.HSVToRGB( playerData.color, 0.8f, 0.8f );
          selectorP2.SetColor( imageP2.color );
     }

     public void OnColorSliderChanged()
     {
          imageP1.color = Color.HSVToRGB( colorSlider.value, 0.8f, 0.8f );
          selectorP1.SetColor( imageP1.color );
          playerData.color = colorSlider.value;
          dirty = true;
     }

     public void OnButtonRole()
     {
          playerData.role = playerData.role == "HEAD" ? "LEGS" : "HEAD";
          roleP1.text = playerData.role;
          selectorP1.SetRole( playerData.role );
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
          if( debugScene )
          {
               roomPlayer.CmdStartGame( "Procedural dungeon Debug", false );
          }
          else
          {
               roomPlayer.CmdStartGame( "Procedural dungeon", false );
          }
     }

     public void OnButtonBack()
     {
          Destroy( FindObjectOfType<EventSystem>().gameObject );

          if( isHost )
               access.StopHost();
          else
               access.StopClient();

          anim.Play( "UI_disconnect" );
     }

     // ESC menu page
     // -------------

     public void OnButtonResume()
     {
          escMenu.SetActive( false );
          Cursor.visible = prevCursorVisible;
          Cursor.lockState = preCursorLock;
     }

     public void OnButtonExit()
     {
          Application.Quit();
     }

     public void OnButtonLeave()
     {
          FindObjectOfType<SharedCharacter>().CmdReturnToLobby( true );
     }

     public void OnButtonOptions()
     {
          qualitySlider.value = QualitySettings.GetQualityLevel();
     }

     // Options menu page
     // -----------------

     public void OnQualitySliderChanged()
     {
          int index = ( int )qualitySlider.value;
          quality.text = QualitySettings.names[index];

          if( index != QualitySettings.GetQualityLevel() )
          {
               applyButton.enabled = true;
          }
          else
          {
               applyButton.enabled = false;
          }
     }

     public void OnButtonApply()
     {
          applyButton.GetComponentInChildren<Text>().text = "WAIT...";
          QualitySettings.SetQualityLevel( ( int )qualitySlider.value );
          applyButton.GetComponentInChildren<Text>().text = "APPLY";
          applyButton.enabled = false;
     }

     public void OnToggleStats()
     {
          statsPanel.SetActive( statsToggle.isOn );
     }

     public void OnTogglePostPorcessing()
     {
          GetComponentInParent<PostProcessLayer>().enabled = postProcessingToggle.isOn;
     }

     public void OnToggleIntro()
     {
          DataLoader.replayIntro = introToggle.isOn;
          DataLoader.globalData.Save( introToggle.isOn );
     }

     // Game UI page
     // ------------

     public void SetLives( int value )
     {
          for( int i = 0; i < lifePanel.transform.childCount; i++ )
          {
               Destroy( lifePanel.transform.GetChild( i ).gameObject );
          }

          for( int i = 0; i < value; i++ )
          {
               GameObject life = Instantiate( lifePrefab, lifePanel.transform );
               life.GetComponent<RectTransform>().anchoredPosition = new Vector2( i * 55, 0 );
          }
     }

     public void SetPoints( int value , bool isHead)
     {
          if(isHead) 
               pointsHead.text = value.ToString();
          else
               pointsLegs.text = value.ToString();
     }

     public void ShowHitmarker()
     {
          anim.Play( "UI_hitmarker" );
     }

     public void ShowDamageOverlay()
     {
          anim.Play( "UI_damage" );
     }

     public void SetFuel( float value )
     {
          fuelBar.size = value;
     }

     public void SetAmmo( int value, int max )
     {
          ammoBar.size = ( float )value / ( float )max;
          ammoText.text = $"{value} / {max}";
     }

     public void SetFireMode( string mode )
     {
          fireMode.text = mode;
     }

     public void OnButtonReturnToLobby()
     {
          FindObjectOfType<SharedCharacter>().CmdReturnToLobby( false );
     }

     // Dialogue system
     // ---------------

     private Queue<string> sentences = new Queue<string>();
     
     public void ShowDialogue( string name, params string[] sentences )
     {
          this.sentences.Clear();
          foreach( string s in sentences )
               this.sentences.Enqueue( s );

          speakerName.text = name;
          dialogueText.text = "";    
          dialogueAnimator.SetBool( "isOpen", true );
          ShowNextSentence();
     }

     private IEnumerator TypeSentence( string sentence )
     {
          dialogueText.text = "";
          foreach( char letter in sentence.ToCharArray() )
          {
               dialogueText.text += letter;
               yield return new WaitForSecondsRealtime( 0.01f );
          }
     }

     public void ShowNextSentence()
     {
          if( sentences.Count > 0 )
          {
               //dialogueText.text = sentences.Dequeue();
               StopAllCoroutines();
               StartCoroutine( TypeSentence( sentences.Dequeue() ) );
          }
          else
          {
               CloseDialogue();
          }
     }

     public void CloseDialogue()
     {
          dialogueText.text = "";
          dialogueAnimator.SetBool( "isOpen", false );
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
               ShowDialogue( "Nathan",
                             "Nice! Since you are new, you will need to create a new local account.",
                             "Insert your name and click the 'Create' button. \nOnce you are done, use the 'Log in' button to access your work area." );
          }
     }

     public void OnLoginAnimationEnded()
     {
          if( DataLoader.globalData.firstLaunch )
          {
               ShowDialogue( "Nathan",
                             "This is the EXPEDITION page, your most important working tool!",
                             "From here you are able to take part in clean up missions.\n" +
                             "You can either choose to travel with a partner or venture alone.",
                             "The JOIN PARTY button will let you join one of your colleagues' expeditions;",
                             "The CREATE PARTY button will give you the lead of your own party, but you will still have to wait for a partner to join it before starting the expedition;",
                             "The VENTURE SOLO button will let you start the expedition alone!" );
          }
     }

     public void OnLockAnimationEnded()
     {
          player.OnLockAnimationEnded();
     }

     // ==================================================================================
     // Commands

     public void EnableMainMenuUI()
     {
          desktop.SetActive( true );
          gameOver.SetActive( false );
          gameUiRoot.SetActive( false );
     }

     public void DisableMainMenuUI()
     {
          desktop.SetActive( false );
     }

     public void SetupGameUI( Role playerRole )
     {
          gameUiRoot.SetActive( true );

          if( isSolo )
          {
               sight.SetActive( true );
               weaponInfo.SetActive( true );
               fuel.SetActive( true );
               SetFuel( 1 );
               ammo.SetActive( true );
               points.SetActive(false);
          }
          else if( playerRole == Role.Head )
          {
               sight.SetActive( true );
               weaponInfo.SetActive( true );
               fuel.SetActive( false );
               ammo.SetActive( true );
          }
          else
          {
               sight.SetActive( false );
               weaponInfo.SetActive( false );
               fuel.SetActive( true );
               SetFuel( 1 );
               ammo.SetActive( false );
          }
     }

     public void UpdatePlayerData()
     {
          playerCash.text = playerData.cash.ToString();
          playerLevel.text = playerData.level.ToString();
          levelBar.size = playerData.exp / playerData.expToNextLevel;
          exp.text = $"{playerData.exp.ToString( "F0" )} / {playerData.expToNextLevel.ToString( "F0" )}";
     }
}
