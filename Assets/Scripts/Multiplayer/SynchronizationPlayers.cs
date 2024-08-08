using ExitGames.Client.Photon;
using MoreMountains.TopDownEngine;
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

        public PhotonView photonView;
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
            if (boardInstance != null)
            {
                Board.instance.onUpdatePlayerTurn -= OnUpdatePlayerTurn;
                Board.instance.onUpdatePlayerTurn += OnUpdatePlayerTurn;
                Board.instance.onUpdatePlayerVisuals -= UpdatePlayerVisuals;
                Board.instance.onUpdatePlayerVisuals += UpdatePlayerVisuals;
                photonView.RPC(nameof(UpdatePlayerVisuals), RpcTarget.AllViaServer, true);
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
            photonView.RPC(nameof(UpdatePlayerTurn), RpcTarget.AllBuffered, canSwapTurn);
        }

        public void OnUpdatePlayerState(bool canResetPos)
        {
            if (monsPiece == null) return;
            string monsData = JsonUtility.ToJson(monsPiece.monsPieceDataType);
            photonView.RPC(nameof(UpdatePlayerState), RpcTarget.AllBuffered, monsData, canResetPos);
            print("Remote update for " + monsPiece.monsPieceDataType.monsPieceType + " Team: " + monsPiece.monsPieceDataType.team);
        }



        private void ResetFaintPlayers()
        {

            

                print("remote reset after faint ABOVE: " + monsPiece.monsPieceDataType.monsPieceType +  monsPiece.monsPieceDataType.whiteFaintGaps + monsPiece.monsPieceDataType.blackFaintGaps  + networkWhiteTurn + monsPiece.monsPieceDataType.isFainted +  " Previous position: " + monsPiece.monsPieceDataType.previousPosition + " desired pos" + monsPiece.monsPieceDataType.desiredPos + " reset pos: " + monsPiece.monsPieceDataType.resetPos);

            if (networkWhiteTurn && monsPiece.monsPieceDataType.team == 0 && monsPiece.monsPieceDataType.isFainted && monsPiece.monsPieceDataType.whiteFaintGaps > 1)//for white faint players
            {
                print("remote reset after faint: " + monsPiece.monsPieceDataType.monsPieceType +  monsPiece.monsPieceDataType.whiteFaintGaps + " Previous position: " + monsPiece.monsPieceDataType.previousPosition + " desired pos" + monsPiece.monsPieceDataType.desiredPos + " reset pos: " + monsPiece.monsPieceDataType.resetPos);

                monsPiece.transform.localEulerAngles = Vector3.zero;
                monsPiece.monsPieceDataType.isFainted = false;
                monsPiece.monsPieceDataType.mySpecialAbilityUsed = false;
                monsPiece.monsPieceDataType.onceAbilityUsed = false;
                monsPiece.monsPieceDataType.whiteFaintGaps = 0;

                Vector2Int previousPosition1 = monsPiece.monsPieceDataType.previousPosition;
                boardInstance.monsPiece[(int)previousPosition1.x, (int)previousPosition1.y] = null;

                monsPiece.monsPieceDataType.currentX = (int)monsPiece.monsPieceDataType.resetPos.x;
                monsPiece.monsPieceDataType.currentY = (int)monsPiece.monsPieceDataType.resetPos.y;

            }
            else if (!networkWhiteTurn && monsPiece.monsPieceDataType.team == 1 && monsPiece.monsPieceDataType.isFainted && monsPiece.monsPieceDataType.blackFaintGaps > 1)//for black faint players
            {
                print("remote reset after faint: " + monsPiece.monsPieceDataType.blackFaintGaps + " Previous position: " + monsPiece.monsPieceDataType.previousPosition + " desired pos" + monsPiece.monsPieceDataType.desiredPos + " reset pos: " + monsPiece.monsPieceDataType.resetPos);

                monsPiece.transform.localEulerAngles = Vector3.zero;
                monsPiece.monsPieceDataType.isFainted = false;
                monsPiece.monsPieceDataType.mySpecialAbilityUsed = false;
                monsPiece.monsPieceDataType.onceAbilityUsed = false;
                monsPiece.monsPieceDataType.blackFaintGaps = 0;
                Vector2Int previousPosition1 = monsPiece.monsPieceDataType.previousPosition;
                boardInstance.monsPiece[(int)previousPosition1.x, (int)previousPosition1.y] = null;
                monsPiece.monsPieceDataType.currentX = (int)monsPiece.monsPieceDataType.resetPos.x;
                monsPiece.monsPieceDataType.currentY = (int)monsPiece.monsPieceDataType.resetPos.y;

            }
        }


        [PunRPC]
        private void UpdatePlayerState(string monsData, bool canResetPos)
        {
            // Board.instance.UpdateRemainingMove(playerData);

            monsPiece.monsPieceDataType = JsonUtility.FromJson<MonsPieceDataType>(monsData);


            Vector2Int previousPosition =monsPiece.monsPieceDataType.previousPosition;
            if (canResetPos)
            {
                boardInstance.monsPiece[(int)previousPosition.x, (int)previousPosition.y] = null;
                monsPiece.monsPieceDataType.blackFaintGaps = 0;
                monsPiece.monsPieceDataType.whiteFaintGaps = 0;

            }
            ResetFaintPlayers();
            if (monsPiece.monsPieceDataType.monsPieceType == MonsPieceType.drainer)
            {
                print("Drainer remote " + monsPiece.monsPieceDataType.isCarryingMana);




                if (true)
                {

                    //super mana reset logic on remote
                    if (monsPiece.monsPieceDataType.isCarryingSuperMana && monsPiece.monsPieceDataType.isFainted)
                    {
                        monsPiece.monsPieceDataType.isCarryingSuperMana = false;
                        boardInstance.superManaRef.monsPieceDataType.desiredPos = new Vector3(5f, 5f, 5f);
                        boardInstance.superManaRef.gameObject.SetActive(true);
                        boardInstance.superManaRef.transform.localEulerAngles = Vector3.zero;
                        gameObject.transform.localRotation = Quaternion.Euler(0, 0, -90f);
                        boardInstance.superManaRef.monsPieceDataType.isFainted = false;
                        boardInstance.superManaRef.monsPieceDataType.isCarriedByDrainer = false;
                        boardInstance.monsPiece[(int)5, (int)5] = boardInstance.superManaRef;
                        print("SuperMana board ref set : " + boardInstance.monsPiece[5, 5]);
                        if (transform.childCount > 0)
                        {
                            Destroy(transform.GetChild(0).gameObject);
                        }
                        print("enable supermana on it: " + boardInstance.superManaRef);
                    }
                    //mana reset logic on remote
                    if ((monsPiece.monsPieceDataType.isCarryingMana || monsPiece.monsPieceDataType.isCarryingOppMana) && monsPiece.monsPieceDataType.isFainted)
                    {
                        if(monsPiece.monsPieceDataType.isCarryingMana)
                        {
                            monsPiece.monsPieceDataType.isCarryingMana = false;
                            monsPiece.monsPieceDataType.isCarryingOppMana = false;
                            boardInstance.currentManaPickedByWhiteRef.monsPieceDataType.desiredPos = new Vector3(monsPiece.monsPieceDataType.currentX, monsPiece.monsPieceDataType.currentY, 0f);
                            boardInstance.currentManaPickedByWhiteRef.gameObject.SetActive(true);
                            gameObject.transform.localRotation = Quaternion.Euler(0, 0, -90f);
                            boardInstance.currentManaPickedByWhiteRef.monsPieceDataType.isFainted = false;
                            boardInstance.currentManaPickedByWhiteRef.monsPieceDataType.isCarriedByDrainer = false;
                            boardInstance.monsPiece[monsPiece.monsPieceDataType.currentX, monsPiece.monsPieceDataType.currentY] = boardInstance.currentManaPickedByWhiteRef;
                            //boardInstance.monsPiece[(int)boardInstance.currentManaRef.monsPieceDataType.desiredPos.x, (int)boardInstance.currentManaRef.monsPieceDataType.desiredPos.y] = boardInstance.currentManaRef;
                            boardInstance.currentManaPickedByWhiteRef.monsPieceDataType.currentX = (int)boardInstance.currentManaPickedByWhiteRef.monsPieceDataType.desiredPos.x;
                            boardInstance.currentManaPickedByWhiteRef.monsPieceDataType.currentY = (int)boardInstance.currentManaPickedByWhiteRef.monsPieceDataType.desiredPos.y;
                            print("Mana board ref set : " + boardInstance.monsPiece[monsPiece.monsPieceDataType.currentX, monsPiece.monsPieceDataType.currentY] + " " + monsPiece.monsPieceDataType.currentX + ": " + monsPiece.monsPieceDataType.currentY);
                            if (transform.childCount > 0)
                            {
                                Destroy(transform.GetChild(0).gameObject);
                            }
                            print("enable Mana on it: " + boardInstance.currentManaPickedByWhiteRef);
                            print("else part Drainer holding mana of team white: " + boardInstance.currentManaPickedByWhiteRef);

                        }
                        else if (monsPiece.monsPieceDataType.isCarryingOppMana)
                        {
                            monsPiece.monsPieceDataType.isCarryingMana = false;
                            monsPiece.monsPieceDataType.isCarryingOppMana = false;
                            boardInstance.currentManaPickedByBlackRef.monsPieceDataType.desiredPos = new Vector3(monsPiece.monsPieceDataType.currentX, monsPiece.monsPieceDataType.currentY, 0f);
                            boardInstance.currentManaPickedByBlackRef.gameObject.SetActive(true);
                            gameObject.transform.localRotation = Quaternion.Euler(0, 0, -90f);
                            boardInstance.currentManaPickedByBlackRef.monsPieceDataType.isFainted = false;
                            boardInstance.currentManaPickedByBlackRef.monsPieceDataType.isCarriedByDrainer = false;
                            boardInstance.monsPiece[monsPiece.monsPieceDataType.currentX, monsPiece.monsPieceDataType.currentY] = boardInstance.currentManaPickedByBlackRef;
                            //boardInstance.monsPiece[(int)boardInstance.currentManaRef.monsPieceDataType.desiredPos.x, (int)boardInstance.currentManaRef.monsPieceDataType.desiredPos.y] = boardInstance.currentManaRef;
                            boardInstance.currentManaPickedByBlackRef.monsPieceDataType.currentX = (int)boardInstance.currentManaPickedByBlackRef.monsPieceDataType.desiredPos.x;
                            boardInstance.currentManaPickedByBlackRef.monsPieceDataType.currentY = (int)boardInstance.currentManaPickedByBlackRef.monsPieceDataType.desiredPos.y;
                            if (transform.childCount > 0)
                            {
                                Destroy(transform.GetChild(0).gameObject);
                            }
                            print("enable Mana on it: " + boardInstance.currentManaPickedByBlackRef);
                            print("else part Drainer holding mana of team black: " + boardInstance.currentManaPickedByBlackRef);
                        }
                    
                    }

                    if (monsPiece.monsPieceDataType.isCarryingMana && transform.childCount <= 0)
                    {
                        if (monsPiece.monsPieceDataType.team == 0)//white
                        {
                            GameObject childMana = Instantiate(boardInstance.childManaSuperMana[0], transform);
                            childMana.transform.SetParent(transform, false);
                        }
                        else
                        {
                            GameObject childMana = Instantiate(boardInstance.childManaSuperMana[1], transform);
                            childMana.transform.SetParent(transform, false);
                        }

                    }
                    else if (monsPiece.monsPieceDataType.isCarryingOppMana && transform.childCount <= 0)
                    {
                        if (monsPiece.monsPieceDataType.team == 0)//white
                        {
                            GameObject childMana = Instantiate(boardInstance.childManaSuperMana[1], transform);
                            childMana.transform.SetParent(transform, false);
                        }
                        else
                        {
                            GameObject childMana = Instantiate(boardInstance.childManaSuperMana[0], transform);
                            childMana.transform.SetParent(transform, false);
                        }

                    }
                    else if (monsPiece.monsPieceDataType.isCarryingSuperMana && transform.childCount <= 0)
                    {
                        GameObject childMana = Instantiate(boardInstance.childManaSuperMana[2], transform);
                        childMana.transform.SetParent(transform, false);

                    }
           
                    else if((!monsPiece.monsPieceDataType.isCarryingSuperMana && !monsPiece.monsPieceDataType.isCarryingMana && !monsPiece.monsPieceDataType.isCarryingOppMana &&
                        !monsPiece.monsPieceDataType.isCarryingBomb && !monsPiece.monsPieceDataType.isCarryingPortion) && transform.childCount > 0)
                    {
                        Destroy(transform.GetChild(0).gameObject);
                    }
                    else
                    {
                        if (monsPiece.monsPieceDataType.isFainted)
                        {
                            if(transform.childCount>0)
                            Destroy(transform.GetChild(0).gameObject);
                            gameObject.transform.localRotation = Quaternion.Euler(0, 0, -90f);


                        }
                    }

              

                }

            }

            //FOR BOMB and potion remote bomb show logic
            if (monsPiece.monsPieceDataType.isCarryingBomb && transform.childCount <= 0)//bomb
            {
                GameObject bomb = Instantiate(boardInstance.BombOrPortionObj[0], transform);
                bomb.transform.SetParent(transform, false);
                bomb.transform.localPosition = new Vector3(0.1f, -0.22f);
                bomb.transform.localScale = new Vector3(0.6f, 0.6f);
                bomb.SetActive(true);

            }
            else if (monsPiece.monsPieceDataType.isCarryingPortion && transform.childCount <= 0)//potion
            {
                GameObject potion = Instantiate(boardInstance.BombOrPortionObj[1], transform);
                potion.transform.SetParent(transform, false);
                potion.transform.localPosition = new Vector3(0.1f, -0.22f);
                potion.transform.localScale = new Vector3(0.6f, 0.6f);
                potion.SetActive(true);


            }
            //FOR BOMB and potion remote bomb destroy logic
            if (monsPiece.monsPieceDataType.monsPieceType != MonsPieceType.drainer && !monsPiece.monsPieceDataType.isCarryingBomb && !monsPiece.monsPieceDataType.isCarryingPortion && transform.childCount > 0)//bomb
            {
                Destroy(transform.GetChild(0).gameObject);

            }
            //else if (monsPiece.monsPieceDataType.monsPieceType != MonsPieceType.drainer && !monsPiece.monsPieceDataType.isCarryingBomb && transform.childCount > 0)//potion
            //{
            //    Destroy(transform.GetChild(0).gameObject);

            //}





            if (monsPiece.monsPieceDataType.monsPieceType == MonsPieceType.mana || monsPiece.monsPieceDataType.monsPieceType == MonsPieceType.supermana)
            {
                print("Drainer remote " + monsPiece.monsPieceDataType.isCarryingMana);
                if(monsPiece.monsPieceDataType.monsPieceType == MonsPieceType.supermana)
                {
                    print("Here i can cached the superMana");

                    boardInstance.superManaRef = monsPiece;
                    //gameObject.SetActive(false);
                }


             
                if (monsPiece.monsPieceDataType.isCarriedByDrainer)
                {
                    if (monsPiece.monsPieceDataType.monsPieceType == MonsPieceType.mana)
                    {
                        if(monsPiece.monsPieceDataType.team==0)
                        boardInstance.currentManaPickedByWhiteRef = monsPiece;
                        else
                            boardInstance.currentManaPickedByBlackRef = monsPiece;

                        print("Here i can cached the Mana");

                    }
                    gameObject.SetActive(false);
                }

            }
            if (monsPiece.monsPieceDataType.monsPieceType == MonsPieceType.mystic || monsPiece.monsPieceDataType.monsPieceType == MonsPieceType.angel)
            {
                print("Mystic  or angel remote " + monsPiece.monsPieceDataType.isFainted);
                if (monsPiece.monsPieceDataType.isFainted)
                {
                    gameObject.transform.localRotation = Quaternion.Euler(0, 0, -90f);
                }

            }
            if (monsPiece.monsPieceDataType.monsPieceType == MonsPieceType.demon)
            {
                print("demon remote: " + canResetPos + " Previous position: " + monsPiece.monsPieceDataType.previousPosition + " desired pos" + monsPiece.monsPieceDataType.desiredPos + " reset pos: " + monsPiece.monsPieceDataType.resetPos);
                if (monsPiece.monsPieceDataType.isFainted)
                {
                    gameObject.transform.localRotation = Quaternion.Euler(0, 0, -90f);
                    boardInstance.monsPiece[(int)monsPiece.monsPieceDataType.resetPos.x, (int)monsPiece.monsPieceDataType.resetPos.y] = monsPiece;
                    boardInstance.monsPiece[(int)monsPiece.monsPieceDataType.desiredPos.x, (int)monsPiece.monsPieceDataType.desiredPos.y] = monsPiece;



                }

            }
            if (monsPiece.monsPieceDataType.monsPieceType == MonsPieceType.bombOrPortion)
            {

                if (monsPiece.monsPieceDataType.isFainted)
                {
                    gameObject.SetActive(false);
                }

            }

            if (monsPiece.monsPieceDataType.isFainted)
            {
                gameObject.transform.localRotation = Quaternion.Euler(0, 0, -90f);
            }
            //if (monsPiece.monsPieceDataType != null && monsPiece.monsPieceDataType.monsPieceType == monsPiece.monsPieceDataType.monsPieceType && monsPieceDataType.team == monsPiece.monsPieceDataType.team || forceInitialize)
            //{

            //}

            MonsPieceDataType monsPieceDataType = monsPiece.monsPieceDataType;
            if (monsPieceDataType.mySpecialAbilityUsed && monsPieceDataType.monsPieceType != MonsPieceType.spirit)
            {
                //monsPieceDataType.mySpecialAbilityUsed = false;
            }




            boardInstance.monsPiece[(int)monsPieceDataType.desiredPos.x, (int)monsPieceDataType.desiredPos.y] = monsPiece;
            if (monsPiece.monsPieceDataType.monsPieceType == MonsPieceType.mana && monsPiece.monsPieceDataType.isScored)
            {
                boardInstance.monsPiece[(int)monsPieceDataType.desiredPos.x, (int)monsPieceDataType.desiredPos.y] = null;
                gameObject.SetActive(false);
            }
            print("Mons desired pos:" + monsPieceDataType.desiredPos);


        }
        [PunRPC]
        private void UpdatePlayerTurn(bool updatePlayerTurn)
        {
            networkWhiteTurn = updatePlayerTurn;
            Board.instance.isWhiteTurn = networkWhiteTurn;
            print("Player turn swapped " + networkWhiteTurn);
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
            photonView.RPC(nameof(UpdatePlayerIcon), RpcTarget.AllViaServer);


        }
        [PunRPC]
        private void UpdatePlayerIcon()
        {
            boardInstance.UpdatePlayerIcon();
        }
        public void UpdateScore(int whiteScore, int blackScore)
        {
            photonView.RPC(nameof(SynchScore), RpcTarget.All, whiteScore, blackScore);
        }
        [PunRPC]
        private void SynchScore(int whiteScore, int blackScore)
        {
            boardInstance.SynchScore(whiteScore, blackScore);
        }
        private void Update()
        {
            if (!photonView.IsMine && PhotonNetwork.IsConnectedAndReady)
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
