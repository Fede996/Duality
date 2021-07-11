using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CameraController : MonoBehaviour
{
     [Header( "User data" )]
     public UserData playerData;

     [Header( "Camera movement" )]
     public Vector2 maxAngles;
     public float sensitivity = 1f;
     public float maxInteractionDistance = 1f;
     public Transform cameraTargetPosition;

     [Header( "Animations" )]
     public AnimationClip[] clips;

     public Status currentStatus = Status.DoNothing;
     public Status prevStatus = Status.DoNothing;
     private Vector2 initialAngles;
     private Animation anim;
     private UiManager UI;

     // ==================================================================================
     // Unity events

     private void Start()
     {
          UI = FindObjectOfType<UiManager>();

          Cursor.lockState = CursorLockMode.Locked;
          Cursor.visible = false;

          anim = GetComponent<Animation>();
          foreach( AnimationClip clip in clips )
          {
               clip.legacy = true;
          }
          currentStatus = Status.DoNothing;
          anim.Play( "Cam_start" );
     }

     private void Update()
     {
          if( currentStatus == Status.DoNothing || currentStatus == Status.InScreen ) return;

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
     }

     private void OnDisable()
     {
          GetComponent<Camera>().enabled = false;
          GetComponent<AudioListener>().enabled = false;
     }

     private void OnEnable()
     {
          GetComponent<Camera>().enabled = true;
          GetComponent<AudioListener>().enabled = true;
     }

     // ==================================================================================
     // UI and animation events

     // Animations
     // ----------

     public void OnStartAnimationEnded()
     {
          currentStatus = Status.OffScreen;

          initialAngles.y = transform.rotation.eulerAngles.y;
          initialAngles.x = transform.rotation.eulerAngles.x;

          if( DataLoader.globalData.firstLaunch )
          {
               UI.ShowDialogue( "Nathan",
                                "Hey, welcome on board! I see you already familiarized with the coffee machine...",
                                "I am Nathan, leader of the \"Extermination and clean up\" department of RoboCorp Inc.",

                                "As you may know, after the Intergalactic Colonial War, the Human Empire came out victorious; " +
                                "we managed to drive away the vast majority of the alien tribes from our territories...",
                                "...but a moltitude of tiny nests of resilient ones, are still alive and active inside our borders!",
                                "Our mission is to ensure the security of our land by cleaning up these resistent outposts, and maybe make a little profit while doing that also!",

                                "Now let me guide you through the first steps of your daily routine:\n" +
                                "first of all, unlock your pc by pressing the 'Interaction' [E] key..." ) ;
          }
     }

     public void OnUnlockAnimationEnded()
     {
          Cursor.lockState = CursorLockMode.Confined;
          Cursor.visible = true;
          currentStatus = Status.InScreen;
     }

     public void OnLockAnimationEnded()
     {
          currentStatus = Status.OffScreen;
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

          UI.OnScreenFocused();
          currentStatus = Status.InScreen;
     }

     private void LeaveScreen()
     {
          currentStatus = Status.DoNothing;
          anim.Play( "Cam_lock" );
     }

     public void SetupLobby()
     {
          GetComponent<Camera>().fieldOfView = 50;
          Transform final = GameObject.Find( "Camera target position" ).transform;
          transform.position = final.position;
          transform.rotation = final.rotation;
          UI.UpdatePlayerData();
     }
}

// ==================================================================================
// Enums

public enum Status
{
     DoNothing,
     InScreen,
     OffScreen,
}
