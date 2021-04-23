using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
     [Header( "References" )]
     [SerializeField] private LobbyRoomManager lobbyRoomManager;
     [SerializeField] private InputField hostName;
     [SerializeField] private InputField joinName;
     [SerializeField] private InputField joinIp;

     public void OnButtonExit()
     {
          Application.Quit();
     }

     public void OnButtonHost()
     {
          if( string.IsNullOrEmpty( hostName.text ) )
               return;

          lobbyRoomManager.localPlayerName = hostName.text;

          lobbyRoomManager.StartHost();
     }

     public void OnButtonJoin()
     {
          if( string.IsNullOrEmpty( joinName.text ) )
          {
               print( "Please insert a valid name!" );
               return;
          }

          if( joinIp.text == "localhost" || string.IsNullOrEmpty( joinIp.text ) ) 
               joinIp.text = "127.0.0.1";

          if( !IPAddress.TryParse( joinIp.text, out IPAddress ip ) )
          {
               print( "Please insert a valid ip address!" );
               return;
          }

          lobbyRoomManager.localPlayerName = joinName.text;
          lobbyRoomManager.networkAddress = ip.ToString();
          lobbyRoomManager.StartClient();
     }

     public void OnButtonServer()
     {
          lobbyRoomManager.StartServer();
     }
}
