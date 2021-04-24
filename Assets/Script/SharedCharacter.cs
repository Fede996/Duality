using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class SharedCharacter : NetworkBehaviour
{
     [Header( "Player references" )]
     public CharacterController Controller;
     public Camera TestaCamera;
     public Camera GambeCamera;
     public Transform Body;
     public Weapon Weapon;

     [Header( "UI" )]
     public Text TestaPointsText;
     public Text GambePointsText;

     [Header( "Players data" )]
     [SyncVar( hook = nameof( OnTestaPointsChanged ) )]
     public int TestaPoints = 0;
     [SyncVar( hook = nameof( OnGambePointsChanged ) )]
     public int GambePoints = 0;


     private void Start()
     {
          DontDestroyOnLoad( this.gameObject );
     }

     private void OnTestaPointsChanged( int oldValue, int newValue )
     {
          TestaPointsText.text = $"Testa Points: <color=#9BFFF8>{newValue}</color>";
     }

     private void OnGambePointsChanged( int oldValue, int newValue )
     {
          GambePointsText.text = $"Gambe Points: <color=#9BFFF8>{newValue}</color>";
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
}
