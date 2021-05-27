using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserSpawner : MonoBehaviour
{
    [Header("Spawner Configuration")] 
    public int numberOfChaser = 1;
    
    private ChaserEnemy chaserInGame;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        chaserInGame = FindObjectOfType<ChaserEnemy>();
        
        if(chaserInGame == null)
            Debug.Log("Instatiate chaser");
            //Instantiate(  )

    }
}
