using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Room : NetworkBehaviour
{
     public Transform roomCameraSocket;

     // =======================================================
     // Unity events 

     void Start()
     {

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
