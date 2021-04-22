using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameNetworkPlayer : NetworkBehaviour
{
     [SyncVar]
     public string playerName = null;
     [SyncVar]
     public string playerRole = null;

     public TestaPlayerController _testa = null;
     public GambePlayerController _gambe = null;

     private void Start()
     {
          _testa = GetComponent<TestaPlayerController>();
          _gambe = GetComponent<GambePlayerController>();

          if( playerRole == "Testa" )
          {
               NetworkCharacter nc = FindObjectOfType<NetworkCharacter>();
               _testa.Camera = nc.TestaCamera;
               _testa.Body = nc.Body;
               nc.TestaController = _testa;

               _testa.enabled = true;
               if( isLocalPlayer ) _testa.Init();
          }
          else
          {
               NetworkCharacter nc = FindObjectOfType<NetworkCharacter>();
               _gambe.Camera = nc.GambeCamera;
               _gambe.Controller = nc.Controller;
               nc.GambeController = _gambe;

               _gambe.enabled = true;
               if( isLocalPlayer ) _gambe.Init();
          }
     }
}
