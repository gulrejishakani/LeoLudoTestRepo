using Photon.Pun;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;




public class OnClickProfileButton : MonoBehaviour
{

    [Header("Profile Stats")]
    public TextMeshProUGUI winText;
    public TextMeshProUGUI loseText;
    public TextMeshProUGUI matchText;

    [Header("Avatar System")]
    public Sprite[] avatars;                 // ALL avatars list
    public Image homeAvatarImg;              // Home page avatar
    public Image profileCenterAvatarImg;     // Profile center avatar


    [Header("Profile Username")]
    public TextMeshProUGUI profileUsernameText;   //  PROFILE NAME TEXT Chhe
    public TMP_InputField nameInputField;
    public GameObject inputFieldRoot; // Image + InputField parent
    public TextMeshProUGUI thirdUsernameText; //Top Name Profile Text Devani Chhe
    public TextMeshProUGUI saveSuccessText;


    [Header("Buttons")]
    public GameObject editButton;
    public GameObject saveButton;
    public GameObject EditAvtarButton;
    public GameObject EditavatarPanel;

    void OnEnable()
    {
        UpdateProfileStats();
        RefreshUsernameFromServer();
        RefreshAvatarFromServer();
    }

    private void Start()
    {
        profileCenterAvatarImg.sprite = avatars[PlayerPrefs.GetInt("avatar", 0)];
    }

    public void OnClickProfile()
    {

    }

    //  Edit button
    public void OnClickEditProfile()
    {
        editButton.SetActive(false);
        nameInputField.gameObject.SetActive(true);

        profileUsernameText.gameObject.SetActive(false);
        EditAvtarButton.SetActive(true);
        inputFieldRoot.SetActive(true);

        saveButton.SetActive(true);


        nameInputField.text = profileUsernameText.text;
    }

    //  Save button
    public void OnClickSaveName()
    {
        string newName = nameInputField.text;

        if (string.IsNullOrEmpty(newName) || newName.Length < 2)
            return;

        // SINGLE SOURCE UPDATE
        PhotonNetwork.NickName = newName;
        PlayerPrefs.SetString("username", newName);
        PlayerPrefs.Save();

        RefreshUsernameFromServer();

        nameInputField.gameObject.SetActive(false);
        profileUsernameText.gameObject.SetActive(true);
        inputFieldRoot.SetActive(false);
        EditAvtarButton.SetActive(false);
        saveButton.SetActive(false);
        editButton.SetActive(true);

        saveSuccessText.gameObject.SetActive(true);
        Invoke(nameof(HideSaveText), 1f);
    }
    void HideSaveText()
    {
        saveSuccessText.gameObject.SetActive(false);
    }

    // Stats
    public void UpdateProfileStats()
    {
        winText.text = "Win " + PlayerPrefs.GetInt("win", 0);
        loseText.text = "Lose " + PlayerPrefs.GetInt("lose", 0);
        matchText.text = "Match " + PlayerPrefs.GetInt("match", 0);
    }




    public void RefreshUsernameFromServer()
    {
        string finalName;

        Debug.Log("ENTER");

        if (!string.IsNullOrEmpty(PhotonNetwork.NickName))
            finalName = PhotonNetwork.NickName;
        else
            finalName = PlayerPrefs.GetString("username", "Player");

        // Center profile name
        profileUsernameText.text = finalName;

        //  Left side icon ke paas wala name (3rd place)
        if (thirdUsernameText != null)
            thirdUsernameText.text = finalName;
    }


    public void OnClickEditAvatar()
    {

        editButton.SetActive(false);
        EditavatarPanel.SetActive(true);
        EditAvtarButton.SetActive(true);

    }

    public void RefreshAvatarFromServer()
    {
        if (avatars == null || avatars.Length == 0)
        {
            Debug.LogError("Avatars list empty");
            return;
        }

        //int avatarIndex = PlayerPrefs.GetInt("avatar", 0);
        int avatarIndex = PlayerPrefs.GetInt("avatar");

        if (avatarIndex < 0 || avatarIndex >= avatars.Length)
            avatarIndex = 0;

        Sprite avatarSprite = avatars[avatarIndex];


        if (homeAvatarImg != null)
            homeAvatarImg.sprite = avatarSprite;


        if (profileCenterAvatarImg != null)
            profileCenterAvatarImg.sprite = avatarSprite;
    }


    public void OnAvatarSelected(int avatarIndex)
    {
        PlayerPrefs.SetInt("avatar", avatarIndex);
        PlayerPrefs.Save();

        RefreshAvatarFromServer();
    }

    internal void RefreshCoin()
    {
        throw new NotImplementedException();
    }
}
