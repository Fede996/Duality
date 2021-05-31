using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class TriggeringCollider : NetworkBehaviour
{
     public GameObject[] placeholders;
     public GameObject[] prefabs;
    
     [Server]
     private void OnTriggerEnter(Collider collision)
     {
          if (collision.CompareTag("Player"))
          {
               for (int i=0; i<placeholders.Length; i++)
               {
                    GameObject enemy = Instantiate(prefabs[i], placeholders[i].transform.position, Quaternion.identity, transform.parent);
                    enemy.transform.position = placeholders[i].transform.position;
                    NetworkServer.Spawn(enemy); 
               }
          }

          Destroy(gameObject);  
     }

}
 