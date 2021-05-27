using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DungeonSimoScript : NetworkBehaviour 
{

     public Transform player;
     public GameObject enemy;


     /*[Server]

     private void Start()
     { 
          Debug.Log(enemy.gameObject.activeSelf ? "Active" : "Inactive");
          enemy.SetActive(false);
     }

     // Update is called once per frame
    [Server]
    void Update()
    {
          Vector3 spawnPoint = player.position;
          Collider[] hitColliders = Physics.OverlapSphere(spawnPoint, 1);//1 is purely chosen arbitrarly
          if (hitColliders.Length > 0)
          {
               Debug.Log(hitColliders.Length);
              

               for (int i=0; i<hitColliders.Length; i++)
               {
                    if (hitColliders[i].transform.parent)
                    {
                         switch (hitColliders[i].transform.parent.name)
                         { 
                              case "Room1":
                                   Debug.Log("Room 1");
                                   Transform enemy = hitColliders[i].transform.parent.transform.Find("Shooter (1)");

                                   Debug.Log(enemy);

                                   if (enemy)
                                        //Debug.Log(enemy.gameObject.activeSelf ? "Active" : "Inactive");
                                  enemy.gameObject.SetActive(true); 

                                   break;

                              case "Room2":
                                   Debug.Log("Room 2");
                                   break;

                              case "Room3":
                                   Debug.Log("Room 3");
                                   break;

                              case "Room4":
                                   Debug.Log("Room 4");
                                   break;

                              case "Room5":
                                   Debug.Log("Room 5");
                                   break;

                              case "Room6":
                                   Debug.Log("Room 6");
                                   break;

                              case "Room7":
                                   Debug.Log("Room 7");
                                   break;

                              case "Room8":
                                   Debug.Log("Room 8");
                                   break;

                         }
                    }
               }
                      
               //You have someone with a collider here
          }


    }*/
}
