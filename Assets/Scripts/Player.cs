using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class Player : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        string Nickname = photonView.Owner.NickName;
        gameObject.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = Nickname;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
