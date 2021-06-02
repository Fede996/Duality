using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class NetworkStats : MonoBehaviour
{
     public Text rtt;
     public Text standardDeviation;
     public Text fps;
     public float interval;

     private void Start()
     {
          StartCoroutine( CheckRTT() );
     }

     private IEnumerator CheckRTT()
     {
          for( ; ; )
          {
               rtt.text = ( NetworkTime.rtt * 1000 ).ToString( "F0" );
               standardDeviation.text = ( NetworkTime.rttStandardDeviation * 1000 ).ToString( "F0" );
               fps.text = ( 1f / Time.unscaledDeltaTime ).ToString( "F0" );

               yield return new WaitForSeconds( interval );
          }
     }
}
