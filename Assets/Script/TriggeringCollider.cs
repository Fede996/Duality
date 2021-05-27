using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class TriggeringCollider : NetworkBehaviour
{
     public GameObject[] enemies;

     private void Start()
     {
          foreach (GameObject enemy in enemies)
          {
               enemy.SetActive(false); 
          }
     }

     
     private void OnTriggerEnter(Collider collision)
     {
          if (collision.CompareTag("Player"))
          {
               foreach(GameObject enemy in enemies)
               {
                    enemy.SetActive(true);
               }
          }

          Destroy(gameObject);  
     }

}
