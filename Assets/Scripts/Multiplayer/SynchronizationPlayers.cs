using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Scripts.Multiplayer
{
    public class SynchronizationPlayers : MonoBehaviour, IPunObservable
    {

        private PhotonView photonView;
        private Vector3 networkedPosition;
        private Vector3 networkedRotation;
        bool networkWhiteTurn;
        private Board boardInstance => Board.instance;
        private bool startSynching = false;

        MonsPiece monsPiece;
        //private Quaternion networkedRotation;

        private void Awake()
        {
            monsPiece = GetComponent<MonsPiece>();
            photonView = GetComponent<PhotonView>();
        }
        #region Unity Methods
        private void Start()
        {
            
            networkedPosition = new Vector3();
            networkWhiteTurn = true;
            if(boardInstance != null)
            {
                Board.instance.onUpdatePlayerTurn -= OnUpdatePlayerTurn;
                Board.instance.onUpdatePlayerTurn += OnUpdatePlayerTurn;
             
              
            }

         
            //DontDestroyOnLoad(this);

            //networkedRotation = new Quaternion();
        }

        private void OnEnable()
        {
            ////State
            Board.instance.onUpdatePlayerState -= OnUpdatePlayerState;
            Board.instance.onUpdatePlayerState += OnUpdatePlayerState;

        }
        private void StartSynching()
        {
            startSynching = true;
        }


        private void OnUpdatePlayerTurn(bool canSwapTurn)
        {
            photonView.RPC(nameof(UpdatePlayerTurn), RpcTarget.All,canSwapTurn);
        }

        private void OnUpdatePlayerState()
        {
            string monsData = JsonUtility.ToJson(monsPiece.monsPieceDataType);
            photonView.RPC(nameof(UpdatePlayerState), RpcTarget.All, monsData);

        }

        [PunRPC]
        private void UpdatePlayerState(string monsData)
        {
            // Board.instance.UpdateRemainingMove(playerData);
            monsPiece.monsPieceDataType = JsonUtility.FromJson<MonsPieceDataType>(monsData);
            MonsPieceDataType monsPieceDataType = monsPiece.monsPieceDataType;
            boardInstance.monsPiece[(int)monsPieceDataType.desiredPos.x, (int)monsPieceDataType.desiredPos.y] = monsPiece;
            print("Mons desired pos:" + monsPieceDataType.desiredPos);

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
               // transform.localEulerAngles = networkedRotation;


            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)//Owner/Local player
            {
                stream.SendNext(transform.localPosition);
               // stream.SendNext(transform.localEulerAngles);
            }
            else
            {
                //the owner in the remote player area
                networkedPosition = (Vector3)stream.ReceiveNext();
               // networkedRotation = (Vector3)stream.ReceiveNext();
                print("On PhotonSerializeView called!" + networkedPosition);
                //networkedRotation =(Quaternion) stream.ReceiveNext();
            }
        }
        #endregion


    }
}
