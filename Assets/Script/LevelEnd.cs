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
               other.GetComponent<SharedCharacter>().OnEndLevel( false );
          }
     }
}
