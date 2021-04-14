using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class NetworkPlayerController : NetworkBehaviour
{
     [Header( "References" )]
     [SerializeField] private GameObject PlayerPrefab;

     private TestaPlayerController _testa = null;
     private GambePlayerController _gambe = null;

     [SyncVar]
     private bool _isTesta = false;

     public override void OnStartServer()
     {
          base.OnStartServer();

          print( "=== SERVER ===" );

          int numPlayers = GameObject.Find( "NetworkManager" ).GetComponent<NetworkManager>().numPlayers;
          print( "GIOCATORI CONNESSI: " + numPlayers );

          if( numPlayers == 1 )
          {
               // sono testa
               GameObject player = Instantiate( PlayerPrefab );
               NetworkServer.Spawn( player );

               _isTesta = true;
          }
          else
          {
               // sono gambe
               _isTesta = false;
          }

          _testa = GetComponent<TestaPlayerController>();
          _gambe = GetComponent<GambePlayerController>();

          if( _isTesta )
          {
               NetworkCharacter nc = FindObjectOfType<NetworkCharacter>();
               _testa.Camera = nc.TestaCamera;
               _testa.Body = nc.Body;
               nc.TestaController = _testa;
          }
          else
          {
               NetworkCharacter nc = FindObjectOfType<NetworkCharacter>();
               _gambe.Camera = nc.GambeCamera;
               _gambe.Controller = nc.Controller;
               nc.GambeController = _gambe;
          }
     }

     public override void OnStartClient()
     {
          base.OnStartClient();

          print( "=== CLIENT ===" );

          _testa = GetComponent<TestaPlayerController>();
          _gambe = GetComponent<GambePlayerController>();

          print( "SONO TESTA? " + _isTesta );

          if( _isTesta )
          {
               NetworkCharacter nc = FindObjectOfType<NetworkCharacter>();
               _testa.Camera = nc.TestaCamera;
               _testa.Body = nc.Body;
               nc.TestaController = _testa;
          }
          else
          {
               NetworkCharacter nc = FindObjectOfType<NetworkCharacter>();
               _gambe.Camera = nc.GambeCamera;
               _gambe.Controller = nc.Controller;
               nc.GambeController = _gambe;
          }
     }

     public override void OnStartLocalPlayer()
     {
          base.OnStartLocalPlayer();

          print( "=== LOCAL PLAYER ===" );

          if( _isTesta )
          {
               _testa.enabled = true;
               _testa.Init(); 
          }
          else
          {
               _gambe.enabled = true;
               _gambe.Init();
          }
     }
}
