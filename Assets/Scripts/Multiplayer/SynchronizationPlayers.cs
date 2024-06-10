using ExitGames.Client.Photon;
using Org.BouncyCastle.Asn1.BC;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Multiplayer
{
    public class SynchronizationPlayers : MonoBehaviour, IPunObservable
    {

        public  PhotonView photonView;
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
                Board.instance.onUpdatePlayerVisuals -= UpdatePlayerVisuals;
                Board.instance.onUpdatePlayerVisuals += UpdatePlayerVisuals;
                photonView.RPC(nameof(UpdatePlayerVisuals), RpcTarget.All,true);
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

        public void OnUpdatePlayerState(bool canResetPos)
        {
            if (monsPiece == null) return;
                string monsData = JsonUtility.ToJson(monsPiece.monsPieceDataType);
                photonView.RPC(nameof(UpdatePlayerState), RpcTarget.All, monsData, canResetPos);
                print("Remote update for " + monsPiece.monsPieceDataType.monsPieceType + " Team: " + monsPiece.monsPieceDataType.team);
        }
        
        [PunRPC]
        private void UpdatePlayerState(string monsData, bool canResetPos)
        {
            // Board.instance.UpdateRemainingMove(playerData);
           
                monsPiece.monsPieceDataType = JsonUtility.FromJson<MonsPieceDataType>(monsData);
            if(monsPiece.monsPieceDataType.monsPieceType == MonsPieceType.drainer)
            {
                print("Drainer remote " + monsPiece.monsPieceDataType.isCarryingMana);
                if (!PhotonNetwork.IsMasterClient)
                {
                    if (monsPiece.monsPieceDataType.isCarryingMana && transform.childCount<=0)
                    {
                        GameObject childMana = Instantiate(boardInstance.childManaSuperMana[0], transform);
                        childMana.transform.SetParent(transform, false);
                    }
                    else
                    {
                        if (transform.childCount > 0)
                        {
                            Destroy(transform.GetChild(0).gameObject);
                        }

                    }
                   
                }
              
            }
            if (monsPiece.monsPieceDataType.monsPieceType == MonsPieceType.mana)
            {
                print("Drainer remote " + monsPiece.monsPieceDataType.isCarryingMana);
                if (monsPiece.monsPieceDataType.isCarriedByDrainer)
                {
                    gameObject.SetActive(false);
                }

            }
            if (monsPiece.monsPieceDataType.monsPieceType == MonsPieceType.mystic)
            {
                print("Mystic remote " + monsPiece.monsPieceDataType.isCarryingMana);
                if (monsPiece.monsPieceDataType.isFainted)
                {
                    gameObject.transform.localRotation = Quaternion.Euler(0, 0, -90f);
                }

            }
            if (monsPiece.monsPieceDataType.monsPieceType == MonsPieceType.demon)
            {
                print("demon remote: " + canResetPos + " Previous position: " +  monsPiece.monsPieceDataType.previousPosition + " desired pos" + monsPiece.monsPieceDataType.desiredPos + " reset pos: " + monsPiece.monsPieceDataType.resetPos);
                if (monsPiece.monsPieceDataType.isFainted)
                {
                    gameObject.transform.localRotation = Quaternion.Euler(0, 0, -90f);
                    boardInstance.monsPiece[(int)monsPiece.monsPieceDataType.resetPos.x, (int)monsPiece.monsPieceDataType.resetPos.y] = monsPiece;
                    boardInstance.monsPiece[(int)monsPiece.monsPieceDataType.desiredPos.x, (int)monsPiece.monsPieceDataType.desiredPos.y] = monsPiece;


                    
                }

            }
            //if (monsPiece.monsPieceDataType != null && monsPiece.monsPieceDataType.monsPieceType == monsPiece.monsPieceDataType.monsPieceType && monsPieceDataType.team == monsPiece.monsPieceDataType.team || forceInitialize)
            //{

            //}

            MonsPieceDataType monsPieceDataType = monsPiece.monsPieceDataType;
            if(monsPieceDataType.mySpecialAbilityUsed)
            {
                monsPieceDataType.mySpecialAbilityUsed = false;
            }

            if (!networkWhiteTurn && monsPiece.monsPieceDataType.team == 0 && monsPiece.monsPieceDataType.isFainted && monsPiece.monsPieceDataType.whiteFaintGaps > 1)//for white faint players
            {
                monsPiece.transform.localEulerAngles = Vector3.zero;
                monsPiece.monsPieceDataType.isFainted = false;

                Vector2Int previousPosition1 = monsPiece.monsPieceDataType.previousPosition;
                boardInstance.monsPiece[(int)previousPosition1.x, (int)previousPosition1.y] = null;

                monsPiece.monsPieceDataType.currentX = (int)monsPiece.monsPieceDataType.resetPos.x;
                monsPiece.monsPieceDataType.currentY = (int)monsPiece.monsPieceDataType.resetPos.y;
                print("remote reset after faint: " + canResetPos + " Previous position: " + monsPiece.monsPieceDataType.previousPosition + " desired pos" + monsPiece.monsPieceDataType.desiredPos + " reset pos: " + monsPiece.monsPieceDataType.resetPos);

            }
            else if (networkWhiteTurn && monsPiece.monsPieceDataType.team == 1 && monsPiece.monsPieceDataType.isFainted && monsPiece.monsPieceDataType.blackFaintGaps > 1)//for black faint players
            {
                monsPiece.transform.localEulerAngles = Vector3.zero;
                monsPiece.monsPieceDataType.isFainted = false;

                Vector2Int previousPosition1 = monsPiece.monsPieceDataType.previousPosition;
                boardInstance.monsPiece[(int)previousPosition1.x, (int)previousPosition1.y] = null;
                monsPiece.monsPieceDataType.currentX = (int)monsPiece.monsPieceDataType.resetPos.x;
                monsPiece.monsPieceDataType.currentY = (int)monsPiece.monsPieceDataType.resetPos.y;
                print("remote reset after faint: " + canResetPos + " Previous position: " + monsPiece.monsPieceDataType.previousPosition + " desired pos" + monsPiece.monsPieceDataType.desiredPos + " reset pos: " + monsPiece.monsPieceDataType.resetPos);

            }



            Vector2Int previousPosition = monsPieceDataType.previousPosition;
            if (canResetPos)
            {
                boardInstance.monsPiece[(int)previousPosition.x, (int)previousPosition.y] = null;

            }
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



        [PunRPC]
        public void UpdatePlayerVisuals(bool updateVisuals)

        {
            print("Updating Player Visuals:");
            if (!photonView.IsMine)
            {
                transform.parent = boardInstance.PieceHolder;
                transform.localEulerAngles = Vector3.zero;
                
            }
            photonView.RPC(nameof(UpdatePlayerIcon), RpcTarget.All);

           
        }
        [PunRPC]
        private void UpdatePlayerIcon()
        {
            boardInstance.UpdatePlayerIcon();
        }
        public void UpdateScore(int whiteScore, int blackScore)
        {
         photonView.RPC(nameof(SynchScore), RpcTarget.All,whiteScore, blackScore);
        }
        [PunRPC]
        private void SynchScore(int whiteScore, int blackScore)
        {
            boardInstance.SynchScore(whiteScore, blackScore);
        }
        private void Update()
        {
            if(!photonView.IsMine && PhotonNetwork.IsConnectedAndReady)
            {
                //transform.localPosition = Vector3.MoveTowards(transform.localPosition, networkedPosition, Time.deltaTime);
                transform.position = networkedPosition;
               // transform.localEulerAngles = networkedRotation;


            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)//Owner/Local player
            {
                stream.SendNext(transform.position);
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
