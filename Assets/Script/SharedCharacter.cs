using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class SharedCharacter : NetworkBehaviour
{
     [Header( "Player references" )]
     [SerializeField] public CharacterController Controller;
     [SerializeField] public Camera TestaCamera;
     [SerializeField] private Camera GambeCamera;
     [SerializeField] public Transform Body;
     [SerializeField] public Weapon Weapon;

     [Header( "UI" )]
     [SerializeField] private Text TestaPointsText;
     [SerializeField] private Text GambePointsText;
     [SerializeField] private Text LivesText;

     [Header( "Player settings" )]
     [SerializeField] private int maxLives = 5;
     [SerializeField] private float timeBetweenHits = 1;
     [SerializeField] private float knockbackResolutionSpeed = 1f;

     [Header( "Players data" )]
     [SyncVar( hook = nameof( OnTestaPointsChanged ) )]
     public int TestaPoints = 0;
     [SyncVar( hook = nameof( OnGambePointsChanged ) )]
     public int GambePoints = 0;
     [SyncVar( hook = nameof( OnLivesChanged ) )]
     public int Lives = 5;

     private float invincibilityFrame = 0;
     private Vector3 knockback = Vector3.zero;


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
          if( playerRole == Role.Testa )
          {
               TestaCamera.enabled = true ;
               TestaCamera.GetComponent<AudioListener>().enabled = true;
               TestaPointsText.enabled = true;
          }
          else
          {
               GambeCamera.enabled = true;
               GambeCamera.GetComponent<AudioListener>().enabled = true;
               GambePointsText.enabled = true;
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

     // =====================================================================

     private void Start()
     {
          Lives = maxLives;
          OnLivesChanged( maxLives, maxLives );
          DontDestroyOnLoad( this.gameObject );
     }

     [Server]
     private void Update()
     {
          if( invincibilityFrame > 0 )
               invincibilityFrame -= Time.deltaTime;
     }

     [Server]
     private void FixedUpdate()
     {
          Controller.Move( knockback );
          knockback = Vector3.Slerp( knockback, Vector3.zero, knockbackResolutionSpeed * Time.fixedDeltaTime );
     }

     [Server]
     private void OnControllerColliderHit( ControllerColliderHit hit )
     {
          Enemy enemy = hit.gameObject.GetComponent<Enemy>();
          if( enemy != null && invincibilityFrame <= 0 )
          {
               Lives -= enemy.Damage;
               invincibilityFrame = timeBetweenHits;
               knockback = ( transform.position - hit.transform.position ).normalized * enemy.KnockbackIntensity;
          }
     }
}
