using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCharacter : MonoBehaviour
{
     [Header( "References" )]
     [SerializeField] public CharacterController Controller;
     [SerializeField] public Transform TestaCamera;
     [SerializeField] public Camera GambeCamera;
     [SerializeField] public Transform Body;
}
