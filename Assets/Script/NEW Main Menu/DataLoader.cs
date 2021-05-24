using System;
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

     private void OnApplicationQuit()
     {
          SaveUserData();
          SaveGlobalData();
     }

     // ==================================================================================
     // User data

     public List<string> LoadAllUsers()
     {
          users.Clear();
          users.AddRange( PlayerPrefs.GetString( "UserList" ).Split( ';' ) );

          return users;
     }

     public bool LoadUserData( string username )
     {
          if( LoadAllUsers().Contains( username ) )
          {
               userData = JsonUtility.FromJson<UserData>( PlayerPrefs.GetString( username ) );

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
          PlayerPrefs.SetString( username, JsonUtility.ToJson( new UserData( username ) ) );
          PlayerPrefs.Save();

          return true;
     }

     public void SaveUserData()
     {
          if( userData != null && !string.IsNullOrEmpty( userData.username ) )
          {
               PlayerPrefs.SetString( userData.username, JsonUtility.ToJson( userData ) );
               PlayerPrefs.Save();
          }
     }
}
