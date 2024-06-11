using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using System;
using UnityEngine.SceneManagement;

namespace Scripts.Multiplayer
{
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        [Header("LoginUI")]
        [SerializeField] private TMP_InputField nameInputField;
        [SerializeField] private Button playButton;

        [Header("QuickPlay")]
        [SerializeField] private GameObject quickMatchUI;
        [SerializeField] private Button quickMatchBtn;
        #region Unity Methods
        // Start is called before the first frame update
        void Start()
        {
            BindListener();
            UpdateUIDetails();
        }

        #endregion

        #region Logical Methods
        private void BindListener()
        {
            playButton.onClick.AddListener(OnPlayButtonClicked);
            quickMatchBtn.onClick.AddListener(OnQuickMatchButtonClicked);
        }

   

        private void UpdateUIDetails()
        {
            if(PhotonNetwork.IsConnected)
            {
                //Show quick Play UI...
                quickMatchUI.SetActive(true);
            }
            else
            {
                //Show login UI since photon server has not been connected yet..
                quickMatchUI.SetActive(false);

            }
        }
        #endregion

        #region UI Callbacks 
        private void OnPlayButtonClicked()
        {
            string playerName = nameInputField.text;
            if (!string.IsNullOrEmpty(playerName))
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                print("Invalid Player name!");
            }

        }
        private void OnQuickMatchButtonClicked()
        {
            PhotonNetwork.LoadLevel(3);
            //PhotonManager.instance.DisconnectFromCurrentRoom();
            //SceneManager.LoadScene(3);
        }
        #endregion

        #region Photon Callback Methods
        public override void OnConnected()
        {
            print($"Connected to the internet!");
        }
        public override void OnConnectedToMaster()
        {
            print($"{PhotonNetwork.LocalPlayer.NickName} is connected to the photon server!");
            quickMatchUI.SetActive(true);
        }
        #endregion
    }
}
