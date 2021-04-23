using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SharedCharacter : NetworkBehaviour
{
     [Header( "References" )]
     public CharacterController Controller;
     public Camera TestaCamera;
     public Camera GambeCamera;
     public Transform Body;
     public Weapon Weapon;

     private void Start()
     {
          DontDestroyOnLoad( this.gameObject );
     }
}
