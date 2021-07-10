using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent( typeof( Rigidbody ) )]
[RequireComponent( typeof( Collider ) )]
public class SharedCharacter : NetworkBehaviour
{
     [Header( "Movement" )]
     public float movementSpeed = 6f;
     public float groundMultiplier = 10f;
     public float airMultiplier = 0.4f;
     public float jumpForce = 15f;
     public float groundDrag = 6f;
     public float airDrag = 2f;
     public PhysicMaterial physicMaterial;
     public float rotationUpdateInterval = 0.1f;

     //[Header( "Sprint" )]
     //public float walkSpeed = 4f;
     //public float sprintSpeed = 6f;
     //public float acceleration = 10f;

     [Header( "Ground detection" )]
     public Transform groundCheck;
     public float groundDistance = 0.4f;
     public LayerMask groundLayer;
     private bool _isGrounded;

     [Header( "Slope" )]
     public float maxSlope = 45f;
     public float frictionMultiplier = 0.02f;

     [Header( "Player settings" )]
     public int maxLives = 5;
     public float timeBetweenHits = 1;
     public ForceMode knockbackMode = ForceMode.Force;
     public int bullets = 20;

     [Header( "Stamina system" )]
     public float maxStamina = 2000;
     public float staminaCost = 100;
     public float fatiguedSpeedMultiplier = 0.1f;
     public float currentStamina;

     [Header( "References" )]
     public Transform headCameraSocket;
     public Transform legsCameraSocket;
     public Transform mechHead;
     public Transform mechLegs;
     public MeshRenderer hideInHead;
     public Material headMaterial;
     public Material legsMaterial;
     public AudioSource rechargeStaminaAudioSource;
     
     
     [HideInInspector] public Weapon weapon;
     private Animator animator;
     private Rigidbody _rigidbody;
     private float _playerHeight;
     private RaycastHit _slopeHit;

     private float _invincibilityFrame = 0;

     // =====================================================================
     // Commands from controllers

     public Role localRole;
     public bool initialized = false;

     public void Init( Role playerRole )
     {
          localRole = playerRole;
          OnLivesChanged( lives, lives );

          if( playerRole == Role.Head )
          {
               Camera.main.transform.parent = headCameraSocket;
               Camera.main.transform.Reset();
               Camera.main.orthographic = false;
               Camera.main.fieldOfView = 60;
               hideInHead.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
               StartCoroutine( UpdateRotation() );
          }
          else
          {
               Camera.main.transform.parent = legsCameraSocket;
               Camera.main.transform.Reset();
               currentStamina = maxStamina;
          }

          initialized = true;
     }

     [Server]
     public void OnEndLevel( bool gameOver )
     {
          foreach( GamePlayerController player in FindObjectsOfType<GamePlayerController>() )
          {
               player.OnEndLevel( gameOver );
          }

          Camera.main.transform.parent = null;
          if( localRole == Role.Legs )
          {
               Camera.main.orthographic = false;
               DontDestroyOnLoad( Camera.main.gameObject );
          }

          RpcOnEndLevel( gameOver );

          NetworkServer.Destroy( gameObject );

          ( ( LobbyRoomManager )NetworkManager.singleton ).ReturnToLobby();
     }

     [ClientRpc]
     private void RpcOnEndLevel( bool gameOver )
     {
          Camera.main.transform.parent = null;
          if( localRole == Role.Legs )
          {
               Camera.main.orthographic = false;
               DontDestroyOnLoad( Camera.main.gameObject );
          }
     }

     // =====================================================================
     // Unity events

     private void Start()
     {
          UI = FindObjectOfType<UiManager>();
          weapon = GetComponent<Weapon>();
          animator = GetComponentInChildren<Animator>();

          animator.SetFloat( "WalkSpeed", 0 );

          _rigidbody = GetComponent<Rigidbody>();
          _rigidbody.freezeRotation = true;
          _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
          if( isServer )
          {
               _rigidbody.isKinematic = false;
               GetComponent<Collider>().enabled = true;
          }
          else
          {
               _rigidbody.isKinematic = true;
               GetComponent<Collider>().enabled = false;
          }

          Collider collider = GetComponent<Collider>();
          _playerHeight = collider.bounds.size.y;
          collider.material = physicMaterial;

          lives = maxLives;

          DontDestroyOnLoad( gameObject );
     }

     private void Update()
     {
          // solo per server
          if( isServer )
          {
               _isGrounded = Physics.CheckSphere( groundCheck.position, groundDistance, groundLayer );

               SetDrag();
               SetFriction();

               if( _invincibilityFrame > 0 )
               {
                    _invincibilityFrame -= Time.deltaTime;
               }
          }

          // solo per client
          if( isClient && localRole == Role.Legs )
          {
               mechHead.rotation = Quaternion.Euler( mechHead.rotation.eulerAngles.x, Mathf.LerpAngle( mechHead.rotation.eulerAngles.y, turn, Time.deltaTime * 10 ), mechHead.rotation.eulerAngles.z );
          }

          // per tutti
          Vector3 planarVelocity = _rigidbody.velocity;
          planarVelocity.y = 0;
          mechLegs.LookAt( transform.position + planarVelocity );
          animator.SetFloat( "WalkSpeed", planarVelocity.magnitude );
     }

     private void FixedUpdate()
     {
          if( isServer )
          {
               MovePlayer( moveDirection );
          }
     }

     [Server]
     public void TakeDamage( int damage )
     {
          TakeDamage( damage, Vector3.zero );
     }

     [Server]
     public void TakeDamage( int damage, Vector3 knockback )
     {
          if( _invincibilityFrame <= 0 )
          {
               lives -= damage;

               if( lives == 0 )
               {
                    RpcTakeDamage();
                    Die();
               }
               else if( lives > 0 )
               {
                    RpcTakeDamage();
                    _invincibilityFrame = timeBetweenHits;
                    _rigidbody.AddForce( knockback, knockbackMode );
               }
          }
     }

     [ClientRpc]
     private void RpcTakeDamage()
     {
          UI.ShowDamageOverlay();
     }

     [Server]
     private void Die()
     {
          RpcDie();
     }

     [ClientRpc]
     private void RpcDie()
     {
          foreach( GamePlayerController controller in FindObjectsOfType<GamePlayerController>() )
          {
               controller.DisableInput();
          }

          UI.gameOver.SetActive( true );
          Cursor.lockState = CursorLockMode.Confined;
          Cursor.visible = true;
     }

     [Command( requiresAuthority = false )]
     public void CmdReturnToLobby( bool forced )
     {
          if( forced )
               OnEndLevel( true );
          else
               OnEndLevel( lives <= 0 );
     }

     // =====================================================================
     // Sync movement

     private Vector3 moveDirection;
     private float turn;
     //private float tilt;

     public void Move( Vector3 movement )
     {
          if( currentStamina > 0 && movement.sqrMagnitude != 0 )
          {
               currentStamina -= staminaCost * Time.deltaTime;
               UI.SetFuel( currentStamina / maxStamina );
          }

          if( currentStamina < 0 )
               movement *= fatiguedSpeedMultiplier;

          CmdSetMoveDirection( movement );
     }

     [Command( requiresAuthority = false )]
     private void CmdSetMoveDirection( Vector3 movement )
     {
          moveDirection = movement;
     }

     private void MovePlayer( Vector3 direction )
     {
          if( _isGrounded && !IsOnSlope() )
          {
               // on a plane surface
               _rigidbody.AddForce( direction * movementSpeed * groundMultiplier );
          }
          else if( _isGrounded && IsOnSlope() )
          {
               // on a slope
               _rigidbody.AddForce( Vector3.ProjectOnPlane( moveDirection, _slopeHit.normal ) * movementSpeed * groundMultiplier );
          }
          else
          {
               // airborne
               _rigidbody.AddForce( direction * movementSpeed * airMultiplier );
          }
     }

     public void Rotate( float turnAmount, float tilt )
     {
          headCameraSocket.transform.localRotation = Quaternion.Euler( tilt, 0, 0 );
          mechHead.Rotate( Vector3.up * turnAmount );
     }

     private IEnumerator UpdateRotation()
     {
          float prevTurn = mechHead.rotation.eulerAngles.y;

          for(; ; )
          {
               float currTurn = mechHead.rotation.eulerAngles.y;

               if( currTurn != prevTurn )
               {
                    CmdUpdateRotation( currTurn );
                    prevTurn = currTurn;
               }

               yield return new WaitForSecondsRealtime( rotationUpdateInterval );
          }
     }

     [Command( requiresAuthority = false )]
     private void CmdUpdateRotation( float turn )
     {
          RpcUpdateRotation( turn );
     }

     [ClientRpc]
     private void RpcUpdateRotation( float turn )
     {
          if( localRole == Role.Legs )
          {
               this.turn = turn;
          }
     }

     // =====================================================================
     // Movement checks

     //private void Jump()
     //{
     //     if( _isGrounded )
     //     {
     //          _rigidbody.velocity = new Vector3( _rigidbody.velocity.x, 0, _rigidbody.velocity.z );
     //          _rigidbody.AddForce( Vector3.up * jumpForce, ForceMode.Impulse );
     //     }
     //}

     //private void SetSpeed( bool sprinting )
     //{
     //     movementSpeed = Mathf.Lerp( movementSpeed, sprinting ? sprintSpeed : walkSpeed, acceleration * Time.deltaTime );
     //}     //private void SetSpeed( bool sprinting )
     //{
     //     movementSpeed = Mathf.Lerp( movementSpeed, sprinting ? sprintSpeed : walkSpeed, acceleration * Time.deltaTime );
     //}

     private bool IsOnSlope()
     {
          if( Physics.Raycast( transform.position, Vector3.down, out _slopeHit, _playerHeight / 2 + 0.5f ) )
          {
               if( _slopeHit.normal != Vector3.up )
               {
                    return true;
               }
          }

          return false;
     }

     private void SetDrag()
     {
          _rigidbody.drag = _isGrounded ? groundDrag : airDrag;
     }

     private void SetFriction()
     {
          float angle;
          if( IsOnSlope() && ( angle = Vector3.Angle( Vector3.up, _slopeHit.normal ) ) < maxSlope )
          {
               physicMaterial.staticFriction = angle * frictionMultiplier;
               physicMaterial.dynamicFriction = angle * frictionMultiplier;
          }
          else
          {
               physicMaterial.staticFriction = 0;
               physicMaterial.dynamicFriction = 0;
          }
     }

     // =====================================================================
     // UI events

     private UiManager UI;

     [Header( "Players data" )]
     [SyncVar( hook = nameof( OnTestaPointsChanged ) )]
     public int TestaPoints = 0;
     [SyncVar( hook = nameof( OnGambePointsChanged ) )]
     public int GambePoints = 0;
     [SyncVar( hook = nameof( OnLivesChanged ) )]
     public int lives = 5;
     [SyncVar( hook = nameof( OnHeadHueChanged ) )]
     public float headHue;
     [SyncVar( hook = nameof( OnLegsHueChanged ) )]
     public float legsHue;

     private void OnTestaPointsChanged( int oldValue, int newValue )
     {
          if( localRole == Role.Head )
          {
               UI.SetPoints( newValue );
          }
     }

     private void OnGambePointsChanged( int oldValue, int newValue )
     {
          if( localRole == Role.Legs )
          {
               UI.SetPoints( newValue );
          }
     }

     private void OnLivesChanged( int oldValue, int newValue )
     {
          UI.SetLives( newValue );
     }

     public void makeRechargeSound()
     {
          if(rechargeStaminaAudioSource != null)
               rechargeStaminaAudioSource.Play();
          
          
     }

     public void AddStamina( float value )
     {
          
          
          currentStamina = Mathf.Min( maxStamina, currentStamina + value );
          UI.SetFuel( currentStamina / maxStamina );
     }

     public void AddPoints(int value, bool isHead)
     {
          if (isHead)
               TestaPoints += value;
          else GambePoints += value;

           
          Debug.Log("PUNTI TESTA: " + TestaPoints);
          Debug.Log("PUNTI GAMBE: " + GambePoints);

     }

     public void SetHue( float value, Role role )
     {
          if( role == Role.Head )
          {
               CmdSetHeadHue( value );
          }
          else
          {
               CmdSetLegsHue( value );
          }
     }

     [Command( requiresAuthority = false )]
     public void CmdSetHeadHue( float value )
     {
          headHue = value;
     }

     [Command( requiresAuthority = false )]
     public void CmdSetLegsHue( float value )
     {
          legsHue = value;
     }

     private void OnHeadHueChanged( float oldValue, float newValue )
     {
          headMaterial.color = Color.HSVToRGB( newValue, 0.8f, 0.8f );
     }

     private void OnLegsHueChanged( float oldValue, float newValue )
     {
          legsMaterial.color = Color.HSVToRGB( newValue, 0.8f, 0.8f );
     }
}
