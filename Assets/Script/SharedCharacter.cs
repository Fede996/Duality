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
     public int bullets = 20;
     public float stamina = 2000;
     public float staminaCost = 100;

     [Header( "References" )]
     public Transform headCameraSocket;
     public Transform legsCameraSocket;
     public Transform Body;
     public Weapon Weapon;

     private Rigidbody _rigidbody;
     private float _playerHeight;
     private RaycastHit _slopeHit;

     private float _invincibilityFrame = 0;
     private Vector3 nullvector;

     // =====================================================================
     // Commands from controllers

     public Role localRole;

     public void Init( Role playerRole )
     {
          localRole = playerRole;
          OnLivesChanged( Lives, Lives );

          if( playerRole == Role.Head )
          {
               Camera.main.transform.parent = headCameraSocket;
               Camera.main.transform.Reset();
          }
          else
          {
               Camera.main.transform.parent = legsCameraSocket;
               Camera.main.transform.Reset();
          }
     }

     public void ToggleFire()
     {
          Weapon.autoFire = !Weapon.autoFire;
          UI.SetFireMode( Weapon.autoFire ? "AUTO" : "SINGLE" );
     }

     // =====================================================================
     // Unity events

     private void Start()
     {
          UI = FindObjectOfType<UiManager>();

          _rigidbody = GetComponent<Rigidbody>();
          _rigidbody.freezeRotation = true;
          _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

          Collider collider = GetComponent<Collider>();
          _playerHeight = collider.bounds.size.y;
          collider.material = physicMaterial;

          Lives = maxLives;

          DontDestroyOnLoad( gameObject );
     }

     private void Update()
     {
          if( isServer )
          {
               _isGrounded = Physics.CheckSphere( groundCheck.position, groundDistance, groundLayer );

               SetDrag();
               SetFriction(); 
          }
     }

     private void FixedUpdate()
     {
          if( isServer )
          {
               MovePlayer( moveDirection );
               RotateBody( zRotation, tiltAngle ); 
          }
     }

     private void OnControllerColliderHit( ControllerColliderHit hit )
     {
          if( !isServer ) return;

          Enemy enemy = hit.gameObject.GetComponent<Enemy>();
          if( enemy != null && _invincibilityFrame <= 0 )
          {
               Lives -= enemy.Damage;
               _invincibilityFrame = timeBetweenHits;
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
               Lives -= damage;
               _invincibilityFrame = timeBetweenHits;
          }
     }

     // =====================================================================
     // Sync movement

     public void Move( Vector3 movement )
     {
          if( stamina > 0 && movement.sqrMagnitude != 0 )
          {
               stamina -= staminaCost;
          }

          if( stamina > 0 )
               CmdSetMoveDirection( movement );
          else
               CmdSetMoveDirection( Vector3.zero );
     }

     public void Rotate( float deltaX, float tilt )
     {
          CmdRotate( deltaX, tilt );
     }

     public Vector3 moveDirection;
     public float zRotation;
     public float tiltAngle;

     [Command( requiresAuthority = false )]
     private void CmdSetMoveDirection( Vector3 movement )
     {
          moveDirection = movement;
     }

     [Command( requiresAuthority = false )]
     private void CmdRotate( float deltaX, float tilt )
     {
          zRotation = deltaX;
          tiltAngle = tilt;
     }

     // =====================================================================
     // Rigidbody movement

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

     //private void Jump()
     //{
     //     if( _isGrounded )
     //     {
     //          _rigidbody.velocity = new Vector3( _rigidbody.velocity.x, 0, _rigidbody.velocity.z );
     //          _rigidbody.AddForce( Vector3.up * jumpForce, ForceMode.Impulse );
     //     }
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

     //private void SetSpeed( bool sprinting )
     //{
     //     movementSpeed = Mathf.Lerp( movementSpeed, sprinting ? sprintSpeed : walkSpeed, acceleration * Time.deltaTime );
     //}     //private void SetSpeed( bool sprinting )
     //{
     //     movementSpeed = Mathf.Lerp( movementSpeed, sprinting ? sprintSpeed : walkSpeed, acceleration * Time.deltaTime );
     //}

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

     private void RotateBody( float zRotation, float tiltAngle )
     {
          headCameraSocket.transform.localRotation = Quaternion.Euler( tiltAngle, 90f, 0f );
          Body.Rotate( Vector3.up * zRotation );
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
     public int Lives = 5;

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
}
