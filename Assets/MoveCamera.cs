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

    private bool isTransitioning = false;

    public float stepTransition = 1f;
    
    private Vector3 arrivalPosition;
    private Vector3 startPosition;
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
            //thisCamera.transform.position = new Vector3(player.transform.position.x - offsetX ,initialY ,thisCamera.transform.position.z );
            startPosition = thisCamera.transform.position;
            arrivalPosition = new Vector3(player.transform.position.x - offsetX ,initialY ,thisCamera.transform.position.z );
            isTransitioning = true;

        }

        if (!(checkIsInCamera.y >= 0 && checkIsInCamera.y <= 1))
        {
            offsetZ = thisCamera.transform.position.z - player.transform.position.z;
            startPosition = thisCamera.transform.position;

            //thisCamera.transform.position = new Vector3(thisCamera.transform.position.x ,initialY ,player.transform.position.z - offsetZ );
            arrivalPosition = new Vector3(thisCamera.transform.position.x ,initialY ,player.transform.position.z - offsetZ );
            isTransitioning = true;
            //Use lerp


        }
        
        //if( isVisible(chaser.transform.position)  )
            //chaser.setHasSeenPlayer(true);


            if (isTransitioning)
            {

                thisCamera.transform.position = Vector3.Lerp(thisCamera.transform.position, arrivalPosition, Time.deltaTime * stepTransition);

                if ((thisCamera.transform.position - arrivalPosition).sqrMagnitude < 0.1f)
                    isTransitioning = false;


            }
            

    }


    bool isVisible(Vector3 position)
    {
        Vector3 checkIsInCamera = thisCamera.WorldToViewportPoint(position);

        return (!(checkIsInCamera.x > 0 && checkIsInCamera.x < 1 && checkIsInCamera.y > 0 && checkIsInCamera.y < 1));

        

    }
    
    
    
    
}
