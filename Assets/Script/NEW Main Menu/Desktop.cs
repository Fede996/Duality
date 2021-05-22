using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Desktop : MonoBehaviour
{
     public CameraController cameraController;
     public AnimationClip[] clips;

     private Animation anim;

     private void Start()
     {
          anim = GetComponent<Animation>();
          foreach( AnimationClip clip in clips )
          {
               clip.legacy = true;
          }
     }

     // ==================================================

     public void Unlock()
     {
          anim.Play( "Unlock" );
     }

     public void Lock()
     {
          anim.Play( "Lock" );
     }

     public void Login()
     {
          anim.Play( "Login" );
     }

     public void OnLoginFinished()
     {
          cameraController.OnLogin();
     }
}
