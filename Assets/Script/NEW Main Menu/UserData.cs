using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class UserData : MonoBehaviour
{
     [Header( "User data" )]
     public string  username;
     public int     level               = 0;
     public float   exp                 = 0;
     public float   expToNextLevel      = 100;
     public float   cash                = 0;

     public string  serverIp            = "localhost";

     public UserData( string username )
     {
          this.username = username;
     }
}
