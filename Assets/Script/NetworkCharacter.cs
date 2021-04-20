using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkCharacter : NetworkBehaviour
{
     [Header( "References" )]
     public CharacterController Controller;
     public Transform TestaCamera;
     public Camera GambeCamera;
     public Transform Body;

     public TestaPlayerController TestaController;
     public GambePlayerController GambeController;
}
