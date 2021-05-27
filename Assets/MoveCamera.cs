using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{


    public GameObject player;
    public ChaserEnemy chaser;
    public Camera thisCamera;

    private float initialY;

    private float offsetX;

    private float offsetZ;
    // Start is called before the first frame update
    void Start()
    {
        initialY = thisCamera.transform.position.y;
        
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
              
        
        
        Vector3 checkIsInCamera = thisCamera.WorldToViewportPoint(player.transform.position);

        //Debug.Log( checkIsInCamera );
        
        //if (!(checkIsInCamera.x > 0 && checkIsInCamera.x < 1 && checkIsInCamera.y > 0 && checkIsInCamera.y < 1) )
        if (!(checkIsInCamera.x >= 0 && checkIsInCamera.x <= 1) )
        {
            //offset = Mathf.Abs( thisCamera.transform.position.x - player.transform.position.x);
            offsetX = thisCamera.transform.position.x - player.transform.position.x;
            //Debug.Log("Outside!");
            //59-21
            thisCamera.transform.position = new Vector3(player.transform.position.x - offsetX ,initialY ,thisCamera.transform.position.z );

        }

        if (!(checkIsInCamera.y >= 0 && checkIsInCamera.y <= 1))
        {
            offsetZ = thisCamera.transform.position.z - player.transform.position.z;
            
            thisCamera.transform.position = new Vector3(thisCamera.transform.position.x ,initialY ,player.transform.position.z - offsetZ );
            
        }
        
        //if( isVisible(chaser.transform.position)  )
            //chaser.setHasSeenPlayer(true);

    }


    bool isVisible(Vector3 position)
    {
        Vector3 checkIsInCamera = thisCamera.WorldToViewportPoint(position);

        return (!(checkIsInCamera.x > 0 && checkIsInCamera.x < 1 && checkIsInCamera.y > 0 && checkIsInCamera.y < 1));

        

    }
    
    
    
    
}
