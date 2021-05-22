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

     [Header( "Sprint" )]
     public float walkSpeed = 4f;
     public float sprintSpeed = 6f;
     public float acceleration = 10f;

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

     [Header( "References" )]
     public Camera TestaCamera;
     public Camera GambeCamera;
     public Transform Body;
     public Weapon Weapon;

     [Header( "UI" )]
     [SerializeField] private Text TestaPointsText;
     [SerializeField] private Text GambePointsText;
     [SerializeField] private Text LivesText;
     [SerializeField] private GameObject WeaponPanel;
     [SerializeField] public int bullets = 20;
     [SerializeField] public float stamina = 2000;
     [SerializeField] public float staminaCost = 100;

     
     [Header( "Players data" )]
     [SyncVar( hook = nameof( OnTestaPointsChanged ) )]
     public int TestaPoints = 0;
     [SyncVar( hook = nameof( OnGambePointsChanged ) )]
     public int GambePoints = 0;
     [SyncVar( hook = nameof( OnLivesChanged ) )]
     public int Lives = 5;

     private Rigidbody _rigidbody;
     private Vector3 _moveDirection;
     private float _playerHeight;
     private RaycastHit _slopeHit;

     private float _invincibilityFrame = 0;
     private Vector3 nullvector;

     private void OnTestaPointsChanged( int oldValue, int newValue )
     {
          TestaPointsText.text = $"Testa Points: <color=#9BFFF8>{newValue}</color>";
     }

     private void OnGambePointsChanged( int oldValue, int newValue )
     {
          GambePointsText.text = $"Gambe Points: <color=#9BFFF8>{newValue}</color>";
     }

     private void OnLivesChanged( int oldValue, int newValue )
     {
          LivesText.text = $"Lives: <color=#9BFFF8>{newValue}</color>";
     }

     // =====================================================================

     public void Init( Role playerRole )
     {

          nullvector = new Vector3(0, 0, 0);
          
          if( playerRole == Role.Testa )
          {
               TestaCamera.enabled = true;
               TestaCamera.GetComponent<AudioListener>().enabled = true;
               TestaPointsText.enabled = true;
               WeaponPanel.SetActive( true );
          }
          else
          {
               GambeCamera.enabled = true;
               GambeCamera.GetComponent<AudioListener>().enabled = true;
               GambePointsText.enabled = true;
               WeaponPanel.SetActive( false );
          }
     }

     public void ResetPlayer()
     {
          TestaCamera.enabled = false;
          TestaCamera.GetComponent<AudioListener>().enabled = false;
          GambeCamera.enabled = false;
          GambeCamera.GetComponent<AudioListener>().enabled = false;

          TestaPointsText.enabled = false;
          GambePointsText.enabled = false;
     }

     public void Move( Vector3 movement )
     {
          if ( stamina > 0 && movement.sqrMagnitude != 0)
          {

               stamina -= Time.deltaTime * staminaCost;

          }
          
          if(stamina > 0)
               CmdMove( movement );
          else
               CmdMove(nullvector);

          
     }

     public void Rotate( float deltaX, float tilt )
     {
          CmdRotate( deltaX, tilt );
     }

     public void ToggleFire()
     {
          Weapon.autoFire = !Weapon.autoFire;
          WeaponPanel.GetComponentInChildren<Text>().text = Weapon.autoFire ? "Fire: <color=#9BFFF8>Auto</color>" :
                                                                              "Fire: <color=#9BFFF8>Single</color>";
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

     [Command( requiresAuthority = false )]
     private void CmdMove( Vector3 movement )
     {
          _moveDirection = movement;
     }

     [Command( requiresAuthority = false )]
     private void CmdRotate( float deltaX, float tilt )
     {
          TestaCamera.transform.localRotation = Quaternion.Euler( tilt, 90f, 0f );
          Body.Rotate( Vector3.up * deltaX );
     }

     // =====================================================================

     private void Start()
     {
          _rigidbody = GetComponent<Rigidbody>();
          _rigidbody.freezeRotation = true;
          _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

          Collider collider = GetComponent<Collider>();
          _playerHeight = collider.bounds.size.y;
          collider.material = physicMaterial;

          Lives = maxLives;
          OnLivesChanged( maxLives, maxLives );

          DontDestroyOnLoad( this.gameObject );
     }

     private void Update()
     {
          _isGrounded = Physics.CheckSphere( groundCheck.position, groundDistance, groundLayer );

          SetDrag();
          //SetSpeed( false );
          SetFriction();
     }

     private void FixedUpdate()
     {
          if( !isServer ) return;

          MovePlayer( _moveDirection );
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

     // =====================================================================

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
               _rigidbody.AddForce( Vector3.ProjectOnPlane( _moveDirection, _slopeHit.normal ) * movementSpeed * groundMultiplier );
          }
          else
          {
               // airborne
               _rigidbody.AddForce( direction * movementSpeed * airMultiplier );
          }
     }

     private void Jump()
     {
          if( _isGrounded )
          {
               _rigidbody.velocity = new Vector3( _rigidbody.velocity.x, 0, _rigidbody.velocity.z );
               _rigidbody.AddForce( Vector3.up * jumpForce, ForceMode.Impulse );
          }
     }

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

     private void SetSpeed( bool sprinting )
     {
          movementSpeed = Mathf.Lerp( movementSpeed, sprinting ? sprintSpeed : walkSpeed, acceleration * Time.deltaTime );
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
}
