using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelection : MonoBehaviour
{
     [Header( "Settings" )]
     public GameObject head;
     public GameObject legs;
     public Material material;

     public void SetRole( string role )
     {
          if( role == "HEAD" )
          {
               head.SetActive( true );
               legs.SetActive( false );
          }
          else
          {
               head.SetActive( false );
               legs.SetActive( true );
          }
     }

     public void SetColor( Color color )
     {
          material.color = color;
     }
}
