using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColor : MonoBehaviour
{
     void Start()
     {
          GetComponent<Renderer>().material.color = Random.ColorHSV();
     }
}
