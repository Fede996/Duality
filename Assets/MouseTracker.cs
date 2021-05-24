using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MouseTracker : MonoBehaviour
{

    public float worldScale = 300;
    private float constY;
    // Start is called before the first frame update
    void Start()
    {
        constY = this.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {

        Debug.Log(Input.mousePosition);

        this.transform.position = new Vector3((Input.mousePosition.x + 18.32f) / worldScale, constY ,
            (Input.mousePosition.y + 43.3F) / worldScale);




    }
}
