using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Scripts.Multiplayer
{
    public class SynchronizationPlayers : MonoBehaviour, IPunObservable
    {

        private PhotonView photonView;
        private Vector3 networkedPosition;
        bool networkWhiteTurn;
        private Board boardInstance => Board.instance;
        private bool startSynching = false;
        //private Quaternion networkedRotation;
        #region Unity Methods
        private void Start()
        {
            photonView = GetComponent<PhotonView>();
            networkedPosition = new Vector3();
            networkWhiteTurn = true;
            if(boardInstance != null )
            {
                Board.instance.onUpdatePlayerTurn -= OnUpdatePlayerTurn;
                Board.instance.onUpdatePlayerTurn += OnUpdatePlayerTurn;

            }

         
            //DontDestroyOnLoad(this);

            //networkedRotation = new Quaternion();
        }

        private void OnEnable()
        {
            GameplayManager.startSynch -= StartSynching;
            GameplayManager.startSynch += StartSynching;
        }
        private void StartSynching()
        {
            startSynching = true;
        }


        private void OnUpdatePlayerTurn(bool canSwapTurn)
        {
            photonView.RPC(nameof(UpdatePlayerTurn), RpcTarget.All,canSwapTurn);
        }
        [PunRPC]
        private void UpdatePlayerTurn(bool updatePlayerTurn)
        {
            networkWhiteTurn = updatePlayerTurn;
            Board.instance.isWhiteTurn = networkWhiteTurn;
            print("Player turn swapped "+ networkWhiteTurn);
        }
        [PunRPC]
        private void StartTheGameWhenAllPlayerAreReady()
        {
            //boardInstance.OnJoinedRoom();
            boardInstance.startGameWhenAllReady = false;
        }
        private void Update()
        {
            if(!photonView.IsMine && PhotonNetwork.IsConnectedAndReady)
            {
                //transform.localPosition = Vector3.MoveTowards(transform.localPosition, networkedPosition, Time.deltaTime);
                transform.localPosition = networkedPosition;
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)//Owner/Local player
            {
                stream.SendNext(transform.localPosition);
            }
            else
            {
                //the owner in the remote player area
                networkedPosition = (Vector3)stream.ReceiveNext();
                print("On PhotonSerializeView called!" + networkedPosition);
                //networkedRotation =(Quaternion) stream.ReceiveNext();
            }
        }
        #endregion


    }
}
