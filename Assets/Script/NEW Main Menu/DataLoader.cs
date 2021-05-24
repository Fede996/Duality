using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLoader : MonoBehaviour
{
     private UserData userData;

     private List<string> users = new List<string>();

     // ==================================================================================
     // Global data

     public bool firstLaunch;
     public bool rememberMe;
     public string lastUsername;

     private void LoadGlobalData()
     {
          firstLaunch = PlayerPrefs.GetInt( "FirstLaunch" ) == 1;
          rememberMe = PlayerPrefs.GetInt( "RememberMe" ) == 1;
          lastUsername = PlayerPrefs.GetString( "LastUser" );
     }

     public void SaveGlobalData()
     {
          PlayerPrefs.SetInt( "FirstLaunch", firstLaunch ? 1 : 0 );
          PlayerPrefs.SetInt( "RememberMe", rememberMe ? 1 : 0 );
          PlayerPrefs.SetString( "LastUser", lastUsername );
          PlayerPrefs.Save();
     }

     // ==================================================================================
     // Unity events

     private void Start()
     {
          userData = GetComponent<UserData>();

          LoadGlobalData();
     }

     // ==================================================================================
     // User data

     public List<string> LoadAllUsers()
     {
          // load all username from file...
          users.Clear();
          users.AddRange( PlayerPrefs.GetString( "UserList" ).Split( ';' ) );

          return users;
     }

     public bool LoadUserData( string username )
     {
          if( LoadAllUsers().Contains( username ) )
          {
               userData.username = username;
               userData.serverIp = "localhost";

               // load user data from file...

               return true;
          }

          return false;
     }

     public bool CreateUserData( string username )
     {
          if( users.Contains( username ) )
          {
               return false;
          }

          PlayerPrefs.SetString( "UserList", $"{PlayerPrefs.GetString( "UserList" )};{username}".Trim( ';' ) );
          PlayerPrefs.Save();

          return true;
     }
}
