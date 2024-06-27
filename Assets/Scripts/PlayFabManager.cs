using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using Photon.Pun;

public class PlayFabManager : MonoBehaviour
{

    public static PlayFabManager instance;
    public TMP_InputField nicknameInputField;
    public static string PublicKey;
    public string Nickname;
    public int coins;
    public int characterIndex;
    public bool characterSelectionDataRetrieved { get; private set; }

    public void Awake()
    {
        instance = this;
    }

    void Start()
    {
        PlayFabSettings.staticSettings.TitleId = "F8894";
    }

    public void AuthenticateUser(string solanaPublicKey)
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = solanaPublicKey,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    public void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login Success");
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnGetUserDataSuccess, OnGetUserUserDataFailure);
    }

    public void OnLoginFailure(PlayFabError error)
    {
        Debug.Log("PlayFab Error: " + error.GenerateErrorReport());
    }

    public void OnGetUserDataSuccess(GetUserDataResult result)
    {

        if (result.Data != null && result.Data.ContainsKey("Nickname"))
        {
            string nickname = result.Data["Nickname"].Value;
            nicknameInputField.text = nickname;
            Nickname = nickname;
            PlayerPrefs.SetString("Nickname", Nickname);
        }
        else
        {
            string nickname = SetDefaultNickname(PublicKey);
            UpdateNickname(nickname);
            nicknameInputField.text = nickname;
            Nickname = nickname;
            PlayerPrefs.SetString("Nickname", Nickname);
        }

    }

    public void OnGetUserUserDataFailure(PlayFabError error)
    {
        Debug.Log("PlayFab Error: " + error.GenerateErrorReport());
    }

    public void UpdateNickname(string nickname)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {
                    "Nickname", nickname
                }
            }
        };
        PhotonNetwork.NickName = nickname;
        PlayerPrefs.SetString("Nickname", nickname);
        PlayFabClientAPI.UpdateUserData(request, OnUpdateUserSuccess, OnUpdateUserFailure);
    }

    public void OnUpdateUserSuccess(UpdateUserDataResult result)
    {
        Debug.Log("User Data Updated Successfully");
    }

    public void OnUpdateUserFailure(PlayFabError error)
    {
        Debug.Log("PlayFab Error: " + error.GenerateErrorReport());
    }
    //public void SaveCharacterSelection(int characterIndex)
    //{
    //    var request = new UpdateUserDataRequest
    //    {
    //        Data = new Dictionary<string, string>
    //        {
    //            {"CharacterIndex", characterIndex.ToString()}
    //        }
    //    };
    //    PlayFabClientAPI.UpdateUserData(request, OnSaveCharacterSuccess, OnSaveCharacterFailure);
    //}

    //public void RetrieveCharacterSelection()
    //{
    //    PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnRetrieveCharacterSuccess, OnRetrieveCharacterFailure);
    //}

    //private void OnSaveCharacterSuccess(UpdateUserDataResult result)
    //{
    //    Debug.Log("Character selection saved successfully");
    //}

    //private void OnSaveCharacterFailure(PlayFabError error)
    //{
    //    Debug.LogError("Failed to save character selection: " + error.GenerateErrorReport());
    //}

    //private void OnRetrieveCharacterSuccess(GetUserDataResult result)
    //{
    //    if (result.Data.TryGetValue("CharacterIndex", out UserDataRecord characterIndexData))
    //    {
    //        string characterIndexValue = characterIndexData.Value;
    //        characterIndex = int.Parse(characterIndexValue);
    //        Debug.Log("Character index retrieved successfully: " + characterIndex);
    //        // Set the retrieved character index
    //        CharSelection.instance.characterSelectionIndex = characterIndex;
    //        // Update the active character based on the retrieved character index
    //        PlayerPrefs.SetInt("characterIndex", characterIndex);
    //        CharSelection.instance.SetActiveCharacter();
    //        getCharacterIndex();
    //    }
    //    else
    //    {
    //        Debug.Log("Character index not found in user data");
    //    }

    //    characterSelectionDataRetrieved = true;
    //}



    //private void OnRetrieveCharacterFailure(PlayFabError error)
    //{
    //    Debug.LogError("Failed to retrieve character selection: " + error.GenerateErrorReport());
    //}

    public int getCharacterIndex()
    {
        return characterIndex;
    }



    public void GetNicknameFromUser()
    {
        string nickname = nicknameInputField.text;
        UpdateNickname(nickname);
    }

    public string SetDefaultNickname(string publicKey)
    {
        string last4characters = publicKey.Substring(publicKey.Length - 4);
        return last4characters;
    }

}