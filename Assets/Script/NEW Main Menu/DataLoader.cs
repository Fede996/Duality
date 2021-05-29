using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLoader : MonoBehaviour
{
     public bool clearPlayerPrefs = true;

     // ==================================================================================
     // Global data

     public static GlobalData globalData;

     public void LoadGlobalData()
     {
          string json = PlayerPrefs.GetString( "GlobalData" );
          if( string.IsNullOrEmpty( json ) )
          {
               globalData = new GlobalData();
               globalData.firstLaunch = false;
               globalData.Save();
               globalData.firstLaunch = true;
          }
          else
          {
               globalData = JsonUtility.FromJson<GlobalData>( json );
          }
     }

     public void SaveGlobalData()
     {
          globalData.Save();
     }

     // ==================================================================================
     // User data

     private CameraController player;

     public List<string> GetUserList()
     {
          string[] users = PlayerPrefs.GetString( "UserList" ).Split( ';' );
          return new List<string>( users );
     }

     public UserData LoadUserData( string username )
     {
          UserData userData = null;
          if( GetUserList().Contains( username ) )
          {
               userData = JsonUtility.FromJson<UserData>( PlayerPrefs.GetString( username ) );
          }

          return userData;
     }

     public bool CreateUserData( string username )
     {
          if( GetUserList().Contains( username ) )
          {
               return false;
          }

          PlayerPrefs.SetString( "UserList", $"{PlayerPrefs.GetString( "UserList" )};{username}".Trim( ';' ) );
          PlayerPrefs.SetString( username, JsonUtility.ToJson( new UserData( username ) ) );
          PlayerPrefs.Save();

          return true;
     }

     // ==================================================================================
     // Unity events

     private void Start()
     {
          if( clearPlayerPrefs )
          {
               PlayerPrefs.DeleteAll();
          }

          LoadGlobalData();
          player = FindObjectOfType<CameraController>();
     }

     private void OnApplicationQuit()
     {
          if( player != null && player.userData != null && !string.IsNullOrEmpty( player.userData.username ) )
          {
               player.userData.Save();
          }
          SaveGlobalData();
     }
}

[Serializable]
public class UserData
{
     [Header( "User data" )]
     public string  username;

     public int     level               = 0;
     public float   exp                 = 0;
     public float   expToNextLevel      = 100;
     public float   cash                = 0;
     public string  serverIp            = "localhost";

     public string  role                = "HEAD";
     public bool    ready               = false;
     public bool    leader              = false;
     public float   color               = 0;

     public UserData( string username )
     {
          this.username = username;
     }

     public void Save()
     {
          PlayerPrefs.SetString( username, JsonUtility.ToJson( this ) );
          PlayerPrefs.Save();
     }
}

[Serializable]
public class GlobalData
{
     [Header( "Global data" )]
     public bool    firstLaunch    = true;
     public bool    rememberMe     = false;
     public string  lastUsername   = "";

     public void Save()
     {
          PlayerPrefs.SetString( "GlobalData", JsonUtility.ToJson( this ) );
          PlayerPrefs.Save();
     }
}
