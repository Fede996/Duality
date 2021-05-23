using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessManager : MonoBehaviour
{
     [Header( "References" )]
     public LobbyRoomManager lobbyRoomManager;

     private DataLoader dataLoader;
     private UserData userData;

     // ==================================================================================
     // Unity events

     private void Start()
     {
          dataLoader = GetComponent<DataLoader>();
          userData = GetComponent<UserData>();
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
}
