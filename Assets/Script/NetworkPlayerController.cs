using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkPlayerController : NetworkBehaviour
{
     [Header( "References" )]
     [SerializeField] private GameObject PlayerCharacter;

     private TestaPlayerController _testa = null;
     private GambePlayerController _gambe = null;

     public override void OnStartLocalPlayer()
     {
          _testa = GetComponent<TestaPlayerController>();
          _gambe = GetComponent<GambePlayerController>();

          NetworkManager manager = GameObject.Find( "NetworkManager" ).GetComponent<NetworkManager>();
          print( manager.numPlayers );  
          if( manager.numPlayers % 2 == 1 )
          {
               // sono testa

               CmdCreatePlayer( manager.numPlayers / 2 );
               NetworkCharacter player = GameObject.Find( $"Player{manager.numPlayers / 2}" ).GetComponent<NetworkCharacter>();
               _testa.Camera = player.TestaCamera;
               _testa.Body = player.Body;
               
               _testa.enabled = true;
               _testa.Init();
          }
          else
          {
               // sono gambe

               NetworkCharacter player = GameObject.Find( $"Player{manager.numPlayers / 2}" ).GetComponent<NetworkCharacter>();
               _gambe.Camera = player.GambeCamera;
               _gambe.Controller = player.Controller;

               _gambe.enabled = true;
               _gambe.Init();
          }

          base.OnStartLocalPlayer();
     }

     [Command]
     private void CmdCreatePlayer( int playerNum )
     {
          // creo un nuovo personaggio...
          GameObject.Instantiate( PlayerCharacter ).name = $"Player{playerNum}";
     }
}
