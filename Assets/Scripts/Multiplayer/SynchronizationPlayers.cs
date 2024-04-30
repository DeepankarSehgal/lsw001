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
        //private Quaternion networkedRotation;
        #region Unity Methods
        private void Start()
        {
            photonView = GetComponent<PhotonView>();
            networkedPosition = new Vector3();
            networkWhiteTurn = true;
            Board.instance.onUpdatePlayerTurn += OnUpdatePlayerTurn;
            //networkedRotation = new Quaternion();
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
        private void Update()
        {
            if(!photonView.IsMine)
            {
                //transform.localPosition = Vector3.MoveTowards(transform.localPosition, networkedPosition, Time.deltaTime);
                transform.localPosition = networkedPosition;
            }
        }
        #endregion
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)//Owner/Local player
            {
                stream.SendNext(transform.localPosition);
                stream.SendNext(Board.instance.isWhiteTurn);
            }
            else
            {
                //the owner in the remote player area
                networkedPosition =(Vector3) stream.ReceiveNext();
                networkWhiteTurn = (bool) stream.ReceiveNext();
                //networkedRotation =(Quaternion) stream.ReceiveNext();
            }
        }

    }
}
