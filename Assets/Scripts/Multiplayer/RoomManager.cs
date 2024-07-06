using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public GameObject playerLeftMessage; // Assign the UI element in the Inspector

    private void Start()
    {
        PhotonNetwork.AddCallbackTarget(this);
        if (playerLeftMessage != null)
        {
            playerLeftMessage.SetActive(false); // Make sure the message is initially inactive
        }
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnCloseRoomButtonClicked()
    {
        if (PhotonNetwork.InRoom)
        {
            //PhotonNetwork.RaiseEvent(CustomEventCodes.CloseRoom, null, RaiseEventOptions.Default, SendOptions.SendReliable);
            PhotonNetwork.RaiseEvent(CustomEventCodes.CloseRoom, null, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        }
    }
    public void PlayAgain()
    {

        if (PhotonNetwork.InRoom)
        {
            //PhotonNetwork.RaiseEvent(CustomEventCodes.CloseRoom, null, RaiseEventOptions.Default, SendOptions.SendReliable);
            PhotonNetwork.RaiseEvent(CustomEventCodes.PlayAgain, null, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == CustomEventCodes.CloseRoom)
        {
            // Leave the room when the custom event is received
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
                Destroy(PhotonManager.instance);
                SceneManager.LoadScene(1);
                // Cleanup networked objects manually if needed
                // PhotonNetwork.DestroyAll(true);
            }

            // Activate the message GameObject
            if (playerLeftMessage != null)
            {
                playerLeftMessage.SetActive(true);
            }
        }
        if (photonEvent.Code == CustomEventCodes.PlayAgain)
        {
            object pieceType;
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("PieceType", out pieceType);
            if ((string) pieceType == "White")
            {
                PhotonNetwork.LoadLevel(3);
            }
            else
            {
                Invoke("DelayLoad", 3f);//need to improve
            }
         
        }
    }

    private void DelayLoad()
    {
        PhotonNetwork.LoadLevel(3);
    }


    public override void OnLeftRoom()
    {
        // Handle what happens after leaving the room
        // For example, load a different scene or show a UI message
        Debug.Log("Left the room");
    }

    public void GoBackToOverWorldLeavedRoom()
    {
        SceneManager.LoadScene(1);// Use PhotonNetwork.LoadLevel instead of SceneManager.LoadScene
    }
}

public class CustomEventCodes
{
    public const byte CloseRoom = 8;
    public const byte PlayAgain = 9;
}
