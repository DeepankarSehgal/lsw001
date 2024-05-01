using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class Menu : MonoBehaviourPunCallbacks
{
    public TMP_Text player1Name;
    public TMP_Text player2Name;
    public TMP_Text GameStartingText;
    public GameObject Canvas;
    public GameObject board;
    public GameObject scoreCanvas;

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        UpdateLobbyUI();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }
    [PunRPC]
    void UpdateLobbyUI()
    {
        player1Name.text = PhotonNetwork.CurrentRoom.GetPlayer(1).NickName;
        player2Name.text = PhotonNetwork.PlayerList.Length == 2 ? PhotonNetwork.CurrentRoom.GetPlayer(2).NickName : "...";

        if (PhotonNetwork.PlayerList.Length == 2)
        {
            GameStartingText.gameObject.SetActive(true);
            if (PhotonNetwork.IsMasterClient)
            {
                Invoke("TryStartingGame", 4);
            }
        }
    }

    void TryStartingGame()
    {
        if (PhotonNetwork.PlayerList.Length == 2)
        {
            Canvas.SetActive(false);
            board.SetActive(true);
            scoreCanvas.SetActive(true);
        }
    }
}
