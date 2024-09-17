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
    public TMP_Text entername;
    [SerializeField] private TextMeshProUGUI testingAssemblyCode;
    //Cache Player icon 
    public Image selectPlayerIcon;

    private void Awake()
    {
        instance = this;
    }
    public void EnterWorld()
    {
        if(chatField.text != string.Empty)//ss
        {
            PhotonNetwork.NickName = chatField.text;
            PlayerPrefs.SetString("Nickname", chatField.text);
            selectPlayerIcon = CharacterSelectionPanel.transform.GetChild(CharacterSelectionPanel.characterSelectionIndex).GetComponent<Image>();
            //CheckIfUserUpdatedNickname();
            SceneManager.LoadScene(1);
        }
        else
        {
            entername.text = "Please enter name";
        }
        
    }

    public void CheckIfUserUpdatedNickname()
    {
        if (PlayFabManager.instance.nicknameInputField.text != null)
        {
            PlayFabManager.instance.GetNicknameFromUser();
        }
    }
    int assemblyIncrementer = 0;
    private void Update()
    {
#if UNITY_ANDROID
        Debug.Log("UNITY_ANDROID symbol is defined.");
        if (Input.GetMouseButton(0))
        {
            assemblyIncrementer++;
            testingAssemblyCode.text = "Touch is working! with assembly code" + assemblyIncrementer;
        }
#else
 Debug.Log("UNITY_ANDROID symbol is not defined.");
#endif
    }

}
