using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
     public Transform roomCameraSocket;

     private Color original;
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
          original = GetComponentInChildren<Renderer>().material.color;
          GetComponentInChildren<Renderer>().material.color = Color.yellow;

          player.legsCameraSocket.transform.parent = roomCameraSocket;
     }

     public void Unload()
     {
          // disabilita il contenuto della stanza
          GetComponentInChildren<Renderer>().material.color = original;
     }

}
