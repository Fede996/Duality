using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
     public Transform roomCameraSocket;

     private SharedCharacter player;

     // =======================================================
     // Unity events 

     void Awake()
     {
          player = FindObjectOfType<SharedCharacter>();
     }

     // =======================================================
     // Methods

     public void Load()
     {
          // carica il contenuto della stanza
     }

     public void Unload()
     {
          // disabilita il contenuto della stanza
     }

}
