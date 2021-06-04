using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LevelEnd : MonoBehaviour
{
     [Server]
     private void OnTriggerEnter( Collider other )
     {
          if( other.CompareTag( "Player" ) )
          {
               foreach( GamePlayerController player in FindObjectsOfType<GamePlayerController>() )
               {
                    player.OnEndLevel();
               }
               other.GetComponent<SharedCharacter>().OnEndLevel();

               ( ( LobbyRoomManager )NetworkManager.singleton ).ReturnToLobby();
          }
     }
}
