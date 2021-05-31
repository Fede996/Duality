using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLoader : MonoBehaviour
{
     public Room parentRoom;

     // =======================================================
     // Unity events 

     void Awake()
     {
          parentRoom = GetComponentInParent<Room>();
     }

     private void OnTriggerEnter( Collider other )
     {
          if( other.CompareTag( "Player" ) )
          {
               // sono entrato nella stanza
               parentRoom.Load();
          }
     }

     private void OnTriggerExit( Collider other )
     {
          if( other.CompareTag( "Player" ) )
          {
               // sono uscito dalla stanza
               parentRoom.Unload();
          }
     }
}
