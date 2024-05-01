using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string Nickname = PlayerPrefs.GetString("Nickname");
        gameObject.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = Nickname;
        PhotonNetwork.NickName = Nickname;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
