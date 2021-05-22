using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
     [Header( "User data" )]
     public string playerName;
     public string serverIp;

     [Header( "Settings" )]
     public Vector2 maxAngles;
     public float sensitivity = 1f;
     public float maxInteractionDistance = 1f;

     [Header( "References" )]
     public Transform cameraTarget;
     public Desktop desktop;
     public Text onlineText;
     public GameObject insertName;
     public GameObject insertIp;
     public LobbyRoomManager lobbyRoomManager;
     //public GameObject cursor;

     public AnimationClip[] clips;

     private Status currentStatus = Status.None;
     private Vector2 initialAngles;
     private Animation anim;

     private void Start()
     {
          Cursor.lockState = CursorLockMode.Locked;
          Cursor.visible = false;

          anim = GetComponent<Animation>();
          foreach( AnimationClip clip in clips )
          {
               clip.legacy = true;
          }
          currentStatus = Status.Animating;
          anim.Play( "Cam_start" );
     }

     private void Update()
     {
          if( currentStatus == Status.Animating ) return;

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
                    lobbyRoomManager.StartServer();

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

                    StartCoroutine( Host() );
               }

               if( Input.GetKeyDown( KeyCode.J ) )
               {
                    // Join online lobby...
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                    currentStatus = Status.DoNothing;

                    StartCoroutine( Join() );
               }

               if( Input.GetButtonDown( "Cancel" ) )
               {
                    LeaveScreen();
               }
          }
     }

     // ==================================================================================

     public void OnAnimationEnded()
     {
          currentStatus = Status.OffScreen;

          initialAngles.y = transform.rotation.eulerAngles.y;
          initialAngles.x = transform.rotation.eulerAngles.x;
     }

     public void OnInsertName()
     {
          playerName = insertName.GetComponentInChildren<InputField>().text;
          insertName.SetActive( false );
     }

     public void OnInsertIp()
     {
          serverIp = insertIp.GetComponentInChildren<InputField>().text;
          insertIp.SetActive( false );
     }

     public void OnLogin()
     {
          anim.Play( "Cam_login" );
     }

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
          currentStatus = Status.InScreen;

          while( Quaternion.Angle( cameraTarget.rotation, transform.rotation ) > 0.1 || Vector3.Distance( transform.position, cameraTarget.position ) > 0.001 )
          {
               transform.position = Vector3.Lerp( transform.position, cameraTarget.position, sensitivity * Time.deltaTime );
               transform.rotation = Quaternion.Lerp( transform.rotation, cameraTarget.rotation, sensitivity * Time.deltaTime );
               yield return null;
          }

          desktop.Login();
          Cursor.lockState = CursorLockMode.Confined;
          Cursor.visible = true;
     }

     private void LeaveScreen()
     {
          desktop.Lock();
          currentStatus = Status.OffScreen;
     }

     private IEnumerator Host()
     {
          insertName.SetActive( true );
          playerName = null;

          while( playerName == null )
          {
               yield return null;
          }

          lobbyRoomManager.localPlayerName = playerName;
          lobbyRoomManager.StartHost();
     }

     private IEnumerator Join()
     {
          insertName.SetActive( true );
          playerName = null;

          while( playerName == null )
          {
               yield return null;
          }

          insertIp.SetActive( true );
          serverIp = null;

          while( serverIp == null )
          {
               yield return null;
          }

          lobbyRoomManager.localPlayerName = playerName;
          lobbyRoomManager.networkAddress = serverIp;
          lobbyRoomManager.StartClient();
     }

     enum Status
     {
          None,
          DoNothing,
          Animating,
          InScreen,
          OffScreen,
     }
}
