using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class AccessManager : MonoBehaviour
{
     [Header( "References" )]
     public LobbyRoomManager lobbyRoomManager;

     private DataLoader dataLoader;
     private CameraController player;
     private UserData userData
     {
          get 
          { 
               return player?.playerData; 
          }
     }

     // ==================================================================================
     // Unity events

     private void Start()
     {
          dataLoader = FindObjectOfType<DataLoader>();
          player = FindObjectOfType<CameraController>();
     }

     // ==================================================================================
     // UI events

     // Login page
     // ----------

     public bool Login( string username )
     {
          if( dataLoader.GetUserList().Contains( username ) )
          {
               player.playerData = dataLoader.LoadUserData( username );
               return true;
          }
          else
          {
               return false;
          }
     }

     public bool Create( string username )
     {
          if( dataLoader.GetUserList().Contains( username ) )
          {
               return false;
          }
          else
          {
               dataLoader.CreateUserData( username );
               return true;
          }
     }

     // Connect page
     // ------------

     public bool Connect( string ip )
     {
          if( ip.ToLower() == "localhost" ) 
               ip = "127.0.0.1";

          if( !IPAddress.TryParse( ip, out IPAddress serverIp ) )
          {
               return false;
          }

          Debug.Log( "Connecting to MATCHING server..." );
          lobbyRoomManager.networkAddress = ip;
          lobbyRoomManager.StartClient();

          return true;
     } 

     public void OpenServer()
     {
          Debug.Log( "Starting MATCHING server..." );
          lobbyRoomManager.StartServer();
     }

     public void Host()
     {
          Debug.Log( "Starting local MATCHING server..." );
          lobbyRoomManager.StartHost();
     }

     // Server page
     // -----------

     public void StopServer()
     {
          lobbyRoomManager.StopServer();
     }

     // Lobby page
     // ----------

     public void StopClient()
     {
          lobbyRoomManager.StopClient();
     }

     public void StopHost()
     {
          lobbyRoomManager.StopHost();
     }
}
