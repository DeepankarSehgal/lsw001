using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using TMPro;
using System;

namespace Scripts.Multiplayer
{
    public class GameplayManager : MonoBehaviourPunCallbacks
    {
        [Header("RoomInfo UI")]
        [SerializeField] private TextMeshProUGUI playerRoomInfoText;
        private ExitGames.Client.Photon.Hashtable playerCustomProperties = new ExitGames.Client.Photon.Hashtable();
        public static Action onJoinedRoom;
        public static Action startSynch;
        #region Unity Methods
        private void Start()
        {
            OnJoinedRoom();
            //JoinRandomRoom();
        }
  
        #endregion
        #region UI Callbacks
        private void JoinRandomRoom()
        {
            PhotonNetwork.JoinRandomRoom();
        }
        #endregion

        #region Photon Callback Methods
        //This is called when there is no room and player want's to join.
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            print("On Join random Failed: " + message);
            playerRoomInfoText.text = message;
            CreateAndJoinRoom();
        }
        bool startGenerateBoard = false;
        //This called when player join the room
        public override void OnJoinedRoom()
        {
            print("OnJoinedRoom!");
            if (!PhotonNetwork.IsMasterClient)//Remote player
            {
                playerCustomProperties.Clear();
                playerCustomProperties.Add("PieceType", "Black");
                PhotonNetwork.LocalPlayer.CustomProperties = (playerCustomProperties);
                print("Player count: " + PhotonNetwork.CurrentRoom.PlayerCount + "PieceType " + playerCustomProperties["PieceType"]);
                playerRoomInfoText.text = PhotonNetwork.NickName + "Joined the" + PhotonNetwork.CurrentRoom.Name + "the piece type is" + playerCustomProperties["PieceType"];
                onJoinedRoom?.Invoke();
                startGenerateBoard = true;
                return;
               // onJoinedRoom?.Invoke();
            }
            else//Master/owner of the room
            {
                playerCustomProperties.Add("PieceType", "White");
                PhotonNetwork.LocalPlayer.CustomProperties =  (playerCustomProperties);
                playerRoomInfoText.text = PhotonNetwork.NickName + "Joined the" + PhotonNetwork.CurrentRoom.Name + "the piece type is" + playerCustomProperties["PieceType"];
            }
            onJoinedRoom?.Invoke();
            print($"{PhotonNetwork.NickName} joined the {PhotonNetwork.CurrentRoom.Name}");

           
        }

        //This is called when the remote player join the room.
        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            playerRoomInfoText.text = newPlayer.NickName + "Joined the" + PhotonNetwork.CurrentRoom.Name;
            print($"{newPlayer.NickName} join the {PhotonNetwork.CurrentRoom.Name} with total Player count is {PhotonNetwork.CurrentRoom.PlayerCount} the piece type is {newPlayer.CustomProperties["PieceType"]}");
        }
        #endregion

        #region Logical Methods
        private void CreateAndJoinRoom()
        {
            string randomRoomName = "Room" +  UnityEngine.Random.Range(0, 1000);
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = true;
            roomOptions.MaxPlayers = 2;
            playerCustomProperties.Clear();
            //playerCustomProperties.Add("PieceType", "White");
            //PhotonNetwork.LocalPlayer.CustomProperties = (playerCustomProperties);
            PhotonNetwork.CreateRoom(randomRoomName, roomOptions, TypedLobby.Default);
        }
        #endregion
    }
}
