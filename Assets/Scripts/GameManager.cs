using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public TMP_InputField chatField;
    public static GameManager instance;
    [SerializeField] private CharSelection CharacterSelectionPanel;

    //Cache Player icon 
    public Image selectPlayerIcon;

    private void Awake()
    {
        instance = this;
    }
    public void EnterWorld()
    {
        PhotonNetwork.NickName = chatField.text;
        selectPlayerIcon = CharacterSelectionPanel.transform.GetChild(CharacterSelectionPanel.characterSelectionIndex).GetComponent<Image>();
        //CheckIfUserUpdatedNickname();
        SceneManager.LoadScene(1);
    }

    public void CheckIfUserUpdatedNickname()
    {
        if (PlayFabManager.instance.nicknameInputField.text != null)
        {
            PlayFabManager.instance.GetNicknameFromUser();
        }
    }
}
