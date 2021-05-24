using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
     [Header( "Settings" )]
     public Vector2 maxAngles;
     public float sensitivity = 1f;
     public float maxInteractionDistance = 1f;
     public Transform cameraTargetPosition;

     [Header( "References" )]
     public GameObject menuManager;

     [Header( "UI" )]
     public InputField username;
     public Toggle rememberMe;
     public Text playerName;
     public Text playerLevel;
     public Scrollbar levelBar;
     public Text playerCash;

     [Header( "Animations" )]
     public AnimationClip[] clips;

     private Status currentStatus = Status.DoNothing;
     private Vector2 initialAngles;
     private Animation anim;

     // ==================================================================================
     // Unity events

     private void Start()
     {
          Cursor.lockState = CursorLockMode.Locked;
          Cursor.visible = false;

          anim = GetComponent<Animation>();
          foreach( AnimationClip clip in clips )
          {
               clip.legacy = true;
          }
          currentStatus = Status.DoNothing;
          anim.Play( "Cam_start" );

          // populate UI with data from the loader
          DataLoader dl = menuManager.GetComponent<DataLoader>();
          if( dl.rememberMe )
          {
               username.text = dl.lastUsername;
               rememberMe.isOn = dl.rememberMe;
          }
     }

     private void Update()
     {
          if( currentStatus == Status.DoNothing ) return;

          float dx = Input.GetAxis( "Mouse X" );
          float dy = Input.GetAxis( "Mouse Y" );

          if( currentStatus == Status.OffScreen )
          {
               MoveCamera( dx, dy );

               if( Input.GetButtonDown( "Interact" ) )
               {
                    StartCoroutine( TurnToScreen() );
               }
          }
          else if( currentStatus == Status.InScreen )
          {
               if( Input.GetKeyDown( KeyCode.S ) )
               {
                    // Start server...
                    //lobbyRoomManager.StartServer();

                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                    currentStatus = Status.DoNothing;
               }

               if( Input.GetKeyDown( KeyCode.H ) )
               {
                    // Start host...
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                    currentStatus = Status.DoNothing;

                    //StartCoroutine( Host() );
               }

               if( Input.GetKeyDown( KeyCode.J ) )
               {
                    // Join online lobby...
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                    currentStatus = Status.DoNothing;

                    //StartCoroutine( Join() );
               }

               if( Input.GetButtonDown( "Cancel" ) )
               {
                    LeaveScreen();
               }
          }
     }

     // ==================================================================================
     // UI and animation events

     public void OnAnimationEnded()
     {
          currentStatus = Status.OffScreen;

          initialAngles.y = transform.rotation.eulerAngles.y;
          initialAngles.x = transform.rotation.eulerAngles.x;
     }

     public void OnLockEnded()
     {
          currentStatus = Status.OffScreen;
     }

     public void OnButtonLogin()
     {
          if( menuManager.GetComponent<AccessManager>().Login( username.text ) )
          {
               InitMainPage();
          }
          else
          {
               username.text = "<i><color=red>User not found!</color></i>";
          }
     }

     public void OnButtonCreate()
     {
          if( menuManager.GetComponent<AccessManager>().Create( username.text ) )
          {
               InitMainPage();    
          }
          else
          {
               username.text = "<i><color=red>User already exists!</color></i>";
          }
     }

     private void InitMainPage()
     {
          DataLoader dl = menuManager.GetComponent<DataLoader>();
          dl.rememberMe = rememberMe.isOn;
          dl.lastUsername = username.text;
          dl.SaveGlobalData();

          anim.Play( "Cam_main" );
          currentStatus = Status.InScreen;

          UserData ud = menuManager.GetComponent<UserData>();
          playerName.text = ud.username;
          playerLevel.text = ud.level.ToString();
          levelBar.size = ud.exp / ud.expToNextLevel;
          playerCash.text = ud.cash.ToString();
     }

     // ==================================================================================
     // Other functions

     private void MoveCamera( float dx, float dy )
     {
          if( dx > 0 )
               dx = Mathf.Lerp( dx, 0, Mathf.Clamp01( ( transform.rotation.eulerAngles.y - initialAngles.y ) / maxAngles.y ) );
          else
               dx = Mathf.Lerp( dx, 0, Mathf.Clamp01( ( initialAngles.y - transform.rotation.eulerAngles.y ) / maxAngles.y ) );

          float temp = transform.rotation.eulerAngles.x > 180 ? transform.rotation.eulerAngles.x - 360 : transform.rotation.eulerAngles.x;
          if( dy > 0 )
               dy = Mathf.Lerp( dy, 0, Mathf.Clamp01( ( initialAngles.x - temp ) / maxAngles.x ) );
          else
               dy = Mathf.Lerp( dy, 0, Mathf.Clamp01( ( initialAngles.x + temp ) / maxAngles.x ) );

          transform.Rotate( Vector3.up, dx, Space.World );
          transform.Rotate( Vector3.left, dy, Space.Self );
     }

     private IEnumerator TurnToScreen()
     {
          currentStatus = Status.DoNothing;

          while( Quaternion.Angle( cameraTargetPosition.rotation, transform.rotation ) > 0.1 || Vector3.Distance( transform.position, cameraTargetPosition.position ) > 0.001 )
          {
               transform.position = Vector3.Lerp( transform.position, cameraTargetPosition.position, sensitivity * Time.deltaTime );
               transform.rotation = Quaternion.Lerp( transform.rotation, cameraTargetPosition.rotation, sensitivity * Time.deltaTime );
               yield return null;
          }

          anim.Play( "Cam_login" );
          Cursor.lockState = CursorLockMode.Confined;
          Cursor.visible = true;
     }

     private void LeaveScreen()
     {
          currentStatus = Status.DoNothing;
          anim.Play( "Cam_lock" );
     }

     //private IEnumerator Host()
     //{
     //     insertName.SetActive( true );
     //     playerName = null;

     //     while( playerName == null )
     //     {
     //          yield return null;
     //     }

     //     lobbyRoomManager.localPlayerName = playerName;
     //     lobbyRoomManager.StartHost();
     //}

     //private IEnumerator Join()
     //{
     //     insertName.SetActive( true );
     //     playerName = null;

     //     while( playerName == null )
     //     {
     //          yield return null;
     //     }

     //     insertIp.SetActive( true );
     //     serverIp = null;

     //     while( serverIp == null )
     //     {
     //          yield return null;
     //     }

     //     lobbyRoomManager.localPlayerName = playerName;
     //     lobbyRoomManager.networkAddress = serverIp;
     //     lobbyRoomManager.StartClient();
     //}

     // ==================================================================================
     // Enums

     enum Status
     {
          DoNothing,
          InScreen,
          OffScreen,
     }
}
