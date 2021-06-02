using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DebugPlayerController : GamePlayerController
{
     protected override void Update()
     {
          base.Update();

          if( !isLocalPlayer ) return;

          if( Input.GetButtonDown( "DebugSwitch" ) )
          {
               if( role == Role.Head ) playerData.role = "LEGS";
               else playerData.role = "HEAD";

               base.Start();
          }
     }

     // =====================================================================
}
