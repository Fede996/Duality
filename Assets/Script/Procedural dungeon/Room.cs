using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
     public Camera roomCamera;

     private Color original;
     private SharedCharacter player;

     // =======================================================
     // Unity events 

     void Awake()
     {
          roomCamera = GetComponentInChildren<Camera>();
          player = FindObjectOfType<SharedCharacter>();
     }

     // =======================================================
     // Methods

     public void Load()
     {
          // carica il contenuto della stanza
          original = GetComponentInChildren<Renderer>().material.color;
          GetComponentInChildren<Renderer>().material.color = Color.yellow;

          bool enabled = player.GambeCamera.enabled;
          player.GambeCamera.enabled = false;
          player.GambeCamera = roomCamera;
          player.GambeCamera.enabled = enabled;
     }

     public void Unload()
     {
          // disabilita il contenuto della stanza
          GetComponentInChildren<Renderer>().material.color = original;
     }

}
