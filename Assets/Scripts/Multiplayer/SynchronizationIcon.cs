using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using UnityEngine.UI;

namespace Scripts.Multiplayer
{
    public class SynchronizationIcon : MonoBehaviourPunCallbacks, IPunObservable
    {
        new PhotonView photonView;
        [SerializeField] private Image myImg;

        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
            myImg = GetComponent<Image>();
        }


        public override void OnJoinedRoom()
        {
            print("On joined room of the icon");
            base.OnJoinedRoom();
            object pieceType;
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("PieceType", out pieceType);
            if (photonView.IsMine)
            {
                Board.instance.player1Icon.sprite = GameManager.instance.selectPlayerIcon.sprite;
                Board.instance.player1Icon.enabled=(true);

                // Convert the texture to byte array
                Texture2D tex = GameManager.instance.selectPlayerIcon.sprite.texture;
                byte[] bytes = tex.EncodeToPNG();
                photonView.RPC("SynchMeInBoard", RpcTarget.AllBufferedViaServer, (string)pieceType, bytes);
            }
           

        }
        [PunRPC]
        public void SynchMeInBoard(string pieceType, byte[] imageBytes)
        {
            if(pieceType=="White" && !PhotonNetwork.IsMasterClient)//hey i am the remote one for white
            {
                print("Synch me in Board!");

                print("PieceType from photon received in Board: " + (string)pieceType + photonView.IsMine);
                Texture2D tex = new Texture2D(1, 1);
                tex.LoadImage(imageBytes);
                Sprite newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                Board.instance.player2Icon.sprite = newSprite;
                Board.instance.player2Icon.enabled = (true);

            }
            else if(pieceType== "Black" &&  PhotonNetwork.IsMasterClient)//hey i am the remote one for black
            {
                print("Synch me in Board!");
                print("PieceType from photon received in Board: " + (string)pieceType + photonView.IsMine);
                Texture2D tex = new Texture2D(1, 1);
                tex.LoadImage(imageBytes);
                Sprite newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                Board.instance.player2Icon.sprite = newSprite;
                Board.instance.player2Icon.enabled = (true);
            }


        }

            public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            //if (stream.IsWriting)
            //{
            //    // We own this player: send the others our data
            //    Texture2D tex = GameManager.instance.selectPlayerIcon.sprite.texture;
            //    byte[] bytes = tex.EncodeToPNG();
            //    stream.SendNext(bytes);
            //}
            //else
            //{
            //    // Network player, receive data
            //    byte[] bytes = (byte[])stream.ReceiveNext();
            //    Texture2D tex = new Texture2D(1, 1);
            //    tex.LoadImage(bytes);
            //    Sprite newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            //    myImg.sprite = newSprite;
            //}
        }
    }
}
