using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
          GameObject.FindObjectOfType<DataLoader>().SaveUserData( this );
     }
}
