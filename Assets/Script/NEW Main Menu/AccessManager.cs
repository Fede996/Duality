using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class AccessManager : MonoBehaviour
{
     [Header( "References" )]
     public LobbyRoomManager lobbyRoomManager;

     private DataLoader dataLoader;
     private UserData userData
     {
          get 
          { 
               return dataLoader?.userData; 
          }
     }

     // ==================================================================================
     // Unity events

     private void Start()
     {
          dataLoader = GetComponent<DataLoader>();
     }

     // ==================================================================================
     // UI events

     public bool Login( string username )
     {
          if( dataLoader.LoadAllUsers().Contains( username ) )
          {
               dataLoader.LoadUserData( username );
               return true;
          }
          else
          {
               return false;
          }
     }

     public bool Create( string username )
     {
          if( dataLoader.LoadAllUsers().Contains( username ) )
          {
               return false;
          }
          else
          {
               dataLoader.CreateUserData( username );
               dataLoader.LoadUserData( username );
               return true;
          }
     }

     public bool Connect( string ip )
     {
          if( ip.ToLower() == "localhost" ) 
               ip = "127.0.0.1";

          if( !IPAddress.TryParse( ip, out IPAddress serverIp ) )
          {
               return false;
          }

          lobbyRoomManager.localPlayerData = userData;
          lobbyRoomManager.localPlayerName = userData.username;
          lobbyRoomManager.networkAddress = ip;
          lobbyRoomManager.StartClient();

          return true;
     } 

     public void OpenServer()
     {
          lobbyRoomManager.StartServer();
     }

     public void CloseServer()
     {
          lobbyRoomManager.StopServer();
          Destroy( lobbyRoomManager.gameObject );
     }

     public void Host()
     {
          lobbyRoomManager.localPlayerData = userData;
          lobbyRoomManager.localPlayerName = userData.username;
          lobbyRoomManager.StartHost();
     }
}
