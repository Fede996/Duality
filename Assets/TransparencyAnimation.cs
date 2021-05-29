using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparencyAnimation : MonoBehaviour
{

    private Color color;
    private Color endColor;
    private Color startColor;
    private bool inTransition = false;

    public bool debugAnimation = false;
    
    public float transitionDuration = 1000f;

    private float transitionDurationModifiable;
    
    public int endColorAlpha = 110;
    
    public float colorTransitionStep = 1f;
    // Start is called before the first frame update
    void Start()
    {

        color = this.GetComponent<Renderer>().material.color;
        startColor = color;
        endColor = new Color(color.r, color.g, color.b, endColorAlpha);
        transitionDurationModifiable = transitionDuration;


    }

    // Update is called once per frame
    void Update()
    {

        // if (inTransition || debugAnimation)
        // {
        //     while ((transitionDurationModifiable -= colorTransitionStep * Time.deltaTime) > 0.01f)
        //     {
        //         Color.Lerp(startColor, endColor, colorTransitionStep * Time.deltaTime);
        //         transitionDurationModifiable = transitionDuration;
        //     }
        //
        //     while ((transitionDurationModifiable -= colorTransitionStep * Time.deltaTime) > 0.01f)
        //     {
        //         Color.Lerp(startColor, endColor, colorTransitionStep * Time.deltaTime);
        //         transitionDurationModifiable = transitionDuration;
        //
        //     }
        //
        //     inTransition = false;
        // }
        
        
        
    }


    public void startAnimation()
    {

        inTransition = true;

    }
    
    
}
