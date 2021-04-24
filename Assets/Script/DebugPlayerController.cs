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
               player.ResetPlayer();

               if( role == Role.Testa ) playerRole = "Gambe";
               else playerRole = "Testa";

               base.Start();
          }
     }

     // =====================================================================
}
