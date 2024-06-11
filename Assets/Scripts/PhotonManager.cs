using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update

    public static PhotonManager instance;
    public GameObject[] PlayerPrefabs;
    public GameObject canvas;
    int characterIndex;
    private PlayFabManager playFabManager;

    public void Awake()
    {
        instance = this;
        DontDestroyOnLoad(instance);
    }

    void Start()
    {
        //PhotonNetwork.AutomaticallySyncScene = true;
        characterIndex = PlayerPrefs.GetInt("characterIndex", 0);
        PhotonNetwork.ConnectUsingSettings();
    }

    public void Connect()
    {
        Debug.Log("Connect Method Called");
    }

    #region PhotonCallbacks
    public override void OnConnected()
    {
        Debug.Log("Connected to Internet");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + "Connected to Server");
        PhotonNetwork.JoinRandomRoom();
    }
    private void CreateAndJoinRoom()
    {
        string randomRoomName = "RoomLobby";
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 0;
        //playerCustomProperties.Add("PieceType", "White");
        //PhotonNetwork.LocalPlayer.CustomProperties = (playerCustomProperties);
        PhotonNetwork.CreateRoom(randomRoomName, roomOptions, TypedLobby.Default);
    }
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Join Random Room Failed " + message + " creating a new one ");
        CreateAndJoinRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName +  ":" + PhotonNetwork.LocalPlayer.ActorNumber + "joined the room");
        float randomPositionX = Random.Range(-4.5f, 4.2f);
        float randomPositionY = Random.Range(21f, 40f);
        int characterIndex = PlayerPrefs.GetInt("characterIndex",0);
        PhotonNetwork.Instantiate(PlayerPrefabs[characterIndex].name, new Vector3(randomPositionX, 1, randomPositionY), Quaternion.identity);
        Debug.Log(PhotonNetwork.IsMasterClient);
    }

    private IEnumerator WaitForCharacterSelection(float randomPositionX, float randomPositionY)
    {
        // Wait until character selection data is retrieved
        while (!playFabManager.characterSelectionDataRetrieved)
        {
            yield return null;
        }

        // Once character selection data is retrieved, instantiate player with the selected character
        int characterIndex = playFabManager.getCharacterIndex();
        PhotonNetwork.Instantiate(PlayerPrefabs[characterIndex].name, new Vector3(randomPositionX, randomPositionY, 0), Quaternion.identity);
        Debug.Log(PhotonNetwork.IsMasterClient);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("Player " + newPlayer.ActorNumber + " entered the room");
    }


    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log("Player " + otherPlayer.ActorNumber + " left the room");

        //// Check if the leaving player was the master client
        if (PhotonNetwork.IsMasterClient && otherPlayer.IsMasterClient)
        {
            //Get the list of remaining players
            Photon.Realtime.Player[] remainingPlayers = PhotonNetwork.PlayerListOthers;

            //If there are remaining players, transfer hosting responsibilities to the first player in the list
            if (remainingPlayers.Length > 0)
            {
                PhotonNetwork.SetMasterClient(remainingPlayers[0]);
                Debug.Log("Transferred master client role to Player " + remainingPlayers[0].ActorNumber);
            }
        }
    }
    public void DisconnectFromCurrentRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    #endregion


}
