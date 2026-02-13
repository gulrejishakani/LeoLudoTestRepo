using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace BEKStudio
{
    public class MenuController : MonoBehaviour
    {
        public static MenuController Instance;
        public GameObject dontDestroyPrefab;
        [Header("Splash Screen")]
        public GameObject splashScreen;
        public GameObject homePageScreen;
        public Sprite[] avatars;
        [Header("Top")]
        public Image topAvatarImg;
        public TextMeshProUGUI topUsernameText;
        public TextMeshProUGUI profileusernameText;
        public TextMeshProUGUI topCoinText;
        public TextMeshProUGUI profileCoinText;
        public TextMeshProUGUI playerLevelText;
        [Header("Bottom")]
        public GameObject homeButton;
        public GameObject profileButton;
        public GameObject lockButton;
        public GameObject storeButton;
        [Header("Middle")]
        public GameObject selectModeScreen;
        public GameObject offlineModeButton;
        public GameObject multiplayerScreen;
        public GameObject quickMatchScreen;
        public GameObject offlineMultiplayerScreen;
        public GameObject practiceMatchScreen;
        public GameObject settingScreen;
        public GameObject rewardScreen;
        public GameObject linkGameObject;
        public GameObject shareLinkButtonGameObject;
        [Header("PopUp Text")]
        public GameObject selectColourText;
        [Header("Main")]
        public GameObject mainBottom;
        public GameObject mainBottomHomeActive;
        public GameObject mainBottomStoreActive;
        [Header("Home")]
        public GameObject homeScreen;
        // public RectTransform homeTitle;
        // public RectTransform homeOnline;
        //  public RectTransform homeComputer;
        //  public RectTransform homecomputer_offlinemultiplayer;
        [Header("Store")]
        public GameObject storeScreen;
        public GameObject storePanel;
        [Header("Pawn Select")]
        public GameObject pawnSelectScreen;
        public GameObject pawnSelectPanel;
        [Header("Player Count")]
        public GameObject playerCountScreen;
        public GameObject playerCountPanel;
        public TextMeshProUGUI playerCountEntryFee;
        [Header("Online")]
        public GameObject onlineScreen;
        public GameObject onlinePanel;
        public TextMeshProUGUI onlineInfoText;
        public Button onlineCancelButton;
        [Header("Username")]
        public GameObject usernameScreen;
        public GameObject usernamePanel;
        public TMP_InputField usernameInput;
        [Header("Links")]
        public string BattleLink;
        public string InviteLink;

        private static bool splashPlayed = false;



        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void Start()
        {
            //Strat Splash Screen
            CheckSplashScreenStatus();

            if (PlayerPrefs.HasKey("pawnColor"))
            {
                PlayerPrefs.DeleteKey("pawnColor");
            }

            if (PlayerPrefs.HasKey("IsOfflineMultiplayer"))
            {
                PlayerPrefs.DeleteKey("IsOfflineMultiplayer");
            }

            GameObject dontDestroyObj = GameObject.Find("DontDestroy");
            if (dontDestroyObj == null)
            {
                dontDestroyObj = Instantiate(dontDestroyPrefab);
                dontDestroyObj.name = "DontDestroy";
                DontDestroyOnLoad(dontDestroyObj);
            }


            if (!PlayerPrefs.HasKey("firstTime"))
            {
                PlayerPrefs.SetInt("avatar", Random.Range(0, avatars.Length));
                PlayerPrefs.SetInt("coin", Constants.START_COIN);
                PlayerPrefs.SetInt("firstTime", 1);
                PlayerPrefs.Save();
            }

            topAvatarImg.sprite = avatars[PlayerPrefs.GetInt("avatar")];
            //topAvatarImg.sprite = avatars[PlayerPrefs.GetInt("avatar", 0)];
            UpdateCoinText();

            Time.timeScale = 1f;


            AdsManager.Instance.DestoryBannerAd();

            Debug.Log("Persistent Data Path: " + Application.persistentDataPath);

        }

        private void CheckSplashScreenStatus()
        {
            if (!splashPlayed)
            {
                splashPlayed = true;
                StartCoroutine(StartSplashScreen());
            }
            else
            {
                splashScreen.SetActive(false);
                homePageScreen.SetActive(true);
            }
        }

        private IEnumerator StartSplashScreen()
        {
            splashScreen.SetActive(true);
            homePageScreen.SetActive(false);

            float splashTime = 4.3f;
            yield return new WaitForSeconds(splashTime);

            splashScreen.SetActive(false);
            homePageScreen.SetActive(true);

            //USername assign

            if (!PlayerPrefs.HasKey("username"))
            {
#if UNITY_WEBGL
                string rand = Random.Range(999, 999999).ToString();
                PlayerPrefs.SetString("username", "Player" + rand);
                PlayerPrefs.Save();
                topUsernameText.text = PlayerPrefs.GetString("username");
                HomeShow();
#else
                UsernameShow();
#endif
            }
            else
            {
                topUsernameText.text = PlayerPrefs.GetString("username");
                HomeShow();
            }

        }

        public void UpdateCoinText()
        {
            int coin = PlayerPrefs.GetInt("coin");
            topCoinText.text = FormatCoins(coin);
            profileCoinText.text = FormatCoins(coin);
        }

        string FormatCoins(long coins)
        {
            if (coins < 1000)
                return coins.ToString();

            if (coins < 1_000_000)
                return (coins / 1000f).ToString("0.#") + "K";

            if (coins < 1_000_000_000)
                return (coins / 1_000_000f).ToString("0.#") + "M";

            return (coins / 1_000_000_000f).ToString("0.#") + "B";
        }


        void HomeShow()
        {
            /*  homeTitle.GetComponent<Image>().color = new Color(1, 1, 1, 0);
              homeOnline.anchoredPosition = new Vector2(-765f, -24.7f);
              homeComputer.anchoredPosition = new Vector2(765f, -461.7f);
              homecomputer_offlinemultiplayer.anchoredPosition = new Vector2(0, -1337f);*/

            homeScreen.SetActive(true);
            MainBottomCheckTabs();

            /*  LeanTween.alpha(homeTitle, 1, 0.2f);
              LeanTween.move(homeOnline, new Vector2(0, 192f), 0.2f).setDelay(0.1f);
              LeanTween.move(homeComputer, new Vector2(0, -215f), 0.2f).setDelay(0.2f);
              LeanTween.move(homecomputer_offlinemultiplayer, new Vector2(0, -592f), 0.2f).setDelay(0.3f);*/
        }

        void HomeClose()
        {
            /*  LeanTween.alpha(homeTitle, 0, 0.2f);
              LeanTween.move(homeOnline, new Vector2(-765f, -24.7f), 0.2f).setDelay(0.1f);
              LeanTween.move(homeComputer, new Vector2(765f, -461.7f), 0.2f).setDelay(0.2f).setOnComplete(() =>
              {
                  homeScreen.SetActive(false);
              });*/
        }

        void MainBottomCheckTabs()
        {
            mainBottomHomeActive.SetActive(homeScreen.activeInHierarchy);
            mainBottomStoreActive.SetActive(storeScreen.activeInHierarchy);
        }

        //++++Middle Panel Start
        public void MiddlePlayButton()
        {
            if (selectModeScreen.activeInHierarchy) return;

            DeactivatePages();

            ButtonAlphaState(offlineModeButton, 0.3f, true);

            PlayerPrefs.SetString("mode", "online");
            PlayerPrefs.Save();

            selectModeScreen.SetActive(true);
        }

        //Offline 4 Player Button
        public void OfflineModeButton()
        {
            //Check pawn is selected
            if (PlayerPrefs.HasKey("pawnColor"))
            {
                // PlayerPrefs.SetInt("IsOfflineMultiplayer", 0);

                PlayerPrefs.SetInt("IsOfflineMultiplayer", 1);
                PlayerPrefs.SetInt("IsPractice", 0);   //  PRACTICE OFF


                PlayerPrefs.SetString("mode", "computer");
                PlayerPrefs.Save();
                PlayerCountShow();
            }
            else
            {
                //Actiavte selectColourText for 1 second then deactivate using leetween
                selectColourText.SetActive(true);
                LeanTween.scale(selectColourText, Vector2.one, 0.2f).setEaseOutBack().setOnComplete(() =>
                {
                    LeanTween.scale(selectColourText, Vector2.zero, 0.2f).setDelay(1.0f).setEaseInBack().setOnComplete(() =>
                    {
                        selectColourText.SetActive(false);
                    });
                });
            }

            //DeactivatePages();
        }

        public void CheckRewardButton()
        {
            selectColourText.SetActive(true);
            LeanTween.scale(selectColourText, Vector2.one, 0.2f).setEaseOutBack().setOnComplete(() =>
            {
                LeanTween.scale(selectColourText, Vector2.zero, 0.2f).setDelay(1.0f).setEaseInBack().setOnComplete(() =>
                {
                    selectColourText.SetActive(false);
                });
            });
        }

        private void ButtonAlphaState(GameObject gameObject, float alpha, bool enable)
        {

            /*Color color = gameObject.GetComponent<Image>().color;
            Button button = gameObject.GetComponent<Button>();
            button.interactable = enable;
            color.a = alpha;*/
        }

        public void MiddleMultiplayerButton()
        {
            if (multiplayerScreen.activeInHierarchy) return;

            ButtonAlphaState(quickMatchScreen, 0.3f, false);

            DeactivatePages();

            //  PlayerPrefs.SetInt("IsOfflineMultiplayer", 0);
            PlayerPrefs.SetInt("IsOfflineMultiplayer", 0);
            PlayerPrefs.SetInt("IsPractice", 0);   // PRACTICE OFF
            PlayerPrefs.SetString("mode", "computer");
            PlayerPrefs.Save();

            multiplayerScreen.SetActive(true);
        }

        //Practice Panel 
        public void MiddleOfflineMultiplayerButton()
        {
            if (offlineMultiplayerScreen.activeInHierarchy) return;

            DeactivatePages();

            //ButtonAlphaState(practiceMatchScreen, 0.3f, false);

            //PlayerPrefs.SetInt("IsOfflineMultiplayer", 0);
            //PlayerPrefs.SetString("mode", "computer");
            //PlayerPrefs.Save();

            offlineMultiplayerScreen.SetActive(true);
        }

        public void PratcieButton()
        {
            //Check pawn is selected

            if (PlayerPrefs.HasKey("pawnColor"))
            {
                //    PlayerPrefs.SetInt("IsPractice", 1);   //  NEW FLAG For Practice Mode 3-2-2026

                PlayerPrefs.SetInt("IsOfflineMultiplayer", 0);
                PlayerPrefs.SetInt("IsPractice", 1);//  NEW FLAG For Practice Mode 3-2-2026

                PlayerPrefs.SetString("mode", "computer");
                PlayerPrefs.Save();
                PlayerCountShow();
            }
            else
            {
                //Actiavte selectColourText for 1 second then deactivate using leetween
                selectColourText.SetActive(true);
                LeanTween.scale(selectColourText, Vector2.one, 0.2f).setEaseOutBack().setOnComplete(() =>
                {
                    LeanTween.scale(selectColourText, Vector2.zero, 0.2f).setDelay(1.0f).setEaseInBack().setOnComplete(() =>
                    {
                        selectColourText.SetActive(false);
                    });
                });
            }
            //DeactivatePages();
        }

        //Quick Match Button in Multiplayer Panel
        public void QuickMatchButton()
        {
            if (PlayerPrefs.HasKey("pawnColor"))
            {
                PlayerPrefs.SetString("mode", "quick");
                PlayerPrefs.Save();
                PlayerCountShow();
            }
            else
            {
                //Actiavte selectColourText for 1 second then deactivate using leetween
                selectColourText.SetActive(true);
                LeanTween.scale(selectColourText, Vector2.one, 0.2f).setEaseOutBack().setOnComplete(() =>
                {
                    LeanTween.scale(selectColourText, Vector2.zero, 0.2f).setDelay(1.0f).setEaseInBack().setOnComplete(() =>
                    {
                        selectColourText.SetActive(false);
                    });
                });
            }
        }

        //Friend Match Button in Multiplayer Panel
        public void FriendsMatchButton()
        {
            if (PlayerPrefs.HasKey("pawnColor"))
            {
                PlayerPrefs.SetString("mode", "friend");
                PlayerPrefs.Save();
                PlayerCountShow();
                //friendPanel.SetActive(true);   // 👈 Open panel
            }
            else
            {
                selectColourText.SetActive(true);
                LeanTween.scale(selectColourText, Vector2.one, 0.2f)
                .setEaseOutBack()
                .setOnComplete(() =>
                {
                    LeanTween.scale(selectColourText, Vector2.zero, 0.2f)
                    .setDelay(1.0f)
                    .setEaseInBack()
                    .setOnComplete(() =>
                    {
                        selectColourText.SetActive(false);
                    });
                });
            }
        }



        //OLD FUNCTIONS
        public void MainOnlineBtn()
        {
            AudioController.Instance.PlayButtonSound();
            if (onlineScreen.activeInHierarchy) return;

            PlayerPrefs.SetString("mode", "online");
            PlayerPrefs.Save();
            PlayerCountShow();
        }

        public void MainVsComputerBtn()
        {
            AudioController.Instance.PlayButtonSound();
            if (pawnSelectScreen.activeInHierarchy) return;

            PlayerPrefs.SetInt("IsOfflineMultiplayer", 0);
            PlayerPrefs.SetInt("IsPractice", 0);   //  PRACTICE OFF
            PlayerPrefs.SetString("mode", "computer");
            PlayerPrefs.Save();
            PawnSelectShow();
        }

        public void MainVsComputerBtn_OfflineMultiplayer()
        {
            AudioController.Instance.PlayButtonSound();
            if (pawnSelectScreen.activeInHierarchy) return;

            PlayerPrefs.SetInt("IsOfflineMultiplayer", 0);
            PlayerPrefs.SetInt("IsPractice", 0);   //  PRACTICE OFF
            PlayerPrefs.SetString("mode", "computer");
            PlayerPrefs.Save();
            PawnSelectShow();
        }

        //----Middle Panel End


        //++++Bottom Panel Start
        public void BottomButtons(int index)
        {
            AudioController.Instance.PlayButtonSound();

            DeactivatePages();

            switch (index)
            {
                case 0:
                    homeButton.SetActive(true);
                    break;
                case 1:
                    profileButton.SetActive(true);
                    break;
                case 2:
                    lockButton.SetActive(true);
                    break;
                case 3:
                    storeButton.SetActive(true);
                    break;
                case 4:
                    settingScreen.SetActive(true);
                    break;
                default:
                    break;
            }
        }


        //OLD FUNCTIONS
        public void MainHomeBtn()
        {
            AudioController.Instance.PlayButtonSound();
            if (homeScreen.activeInHierarchy) return;

            if (storeScreen.activeInHierarchy && !LeanTween.isTweening(storePanel))
            {
                StoreClose();
            }
        }

        public void MainWatchVideoBtn()
        {
            if (AdsManager.Instance == null)
            {
                return;
            }

            AdsManager.Instance.ShowRewardedAd();
        }

        public void MainStoreBtn()
        {
            AudioController.Instance.PlayButtonSound();
            if (storeScreen.activeInHierarchy) return;

            StoreShow();
        }

        void StoreShow()
        {
            storePanel.transform.localScale = Vector2.zero;
            storeScreen.SetActive(true);

            if (homeScreen.activeInHierarchy)
            {
                HomeClose();
            }

            LeanTween.scale(storePanel, Vector2.one, 0.2f).setDelay(0.41f).setEaseOutBack().setOnStart(() =>
            {
                MainBottomCheckTabs();
            });
        }

        void StoreClose(bool showHome = true)
        {
            LeanTween.scale(storePanel, Vector2.zero, 0.2f).setEaseInBack().setOnComplete(() =>
            {
                storeScreen.SetActive(false);

                if (showHome)
                {
                    HomeShow();
                    MainBottomCheckTabs();
                }
            });
        }
        //----Bottom Panel Stop

        private void DeactivatePages()
        {
            AudioController.Instance.PlayButtonSound();

            homeButton.SetActive(false);
            profileButton.SetActive(false);
            lockButton.SetActive(false);
            storeButton.SetActive(false);
            selectModeScreen.SetActive(false);
            multiplayerScreen.SetActive(false);
            offlineMultiplayerScreen.SetActive(false);
            settingScreen.SetActive(false);

        }

        public void StoreItemBtn(int id)
        {
            Purchaser.Instance.BuyConsumable(id);
        }

        public void StoreRestoreBtn()
        {
            Purchaser.Instance.RestorePurchases();
        }

        public void StoreCloseBtn()
        {
            AudioController.Instance.PlayButtonSound();
            if (LeanTween.isTweening(storePanel)) return;

            StoreClose();
        }

        void PawnSelectShow()
        {
            pawnSelectPanel.transform.localScale = Vector2.zero;
            pawnSelectScreen.SetActive(true);

            LeanTween.scale(pawnSelectPanel, Vector2.one, 0.2f).setEaseOutBack();
        }

        public void PawnSelectItemBtn(string pawnColor)
        {
            AudioController.Instance.PlayButtonSound();
            PlayerPrefs.SetString("pawnColor", pawnColor);
            PlayerPrefs.Save();

            if (selectModeScreen.activeInHierarchy)
                ButtonAlphaState(offlineModeButton, 1.0f, true);

            if (multiplayerScreen.activeInHierarchy)
                ButtonAlphaState(quickMatchScreen, 1.0f, true);

            if (offlineMultiplayerScreen.activeInHierarchy)
                ButtonAlphaState(practiceMatchScreen, 1.0f, true);

            //Debug.Log(multiplayerScreen.activeInHierarchy);
            //call after offline button
            //PawnSelectClose();
        }

        public void PawnSelectCloseBtn()
        {
            AudioController.Instance.PlayButtonSound();
            if (LeanTween.isTweening(pawnSelectPanel)) return;

            PlayerPrefs.DeleteKey("pawnColor");
            ResetPracticeFlag();   // 🔥 YAHAN ADD
            PawnSelectClose();
        }


        public void PawnSelectClose()
        {
            LeanTween.scale(pawnSelectPanel, Vector2.zero, 0.2f).setEaseInBack().setOnComplete(() =>
            {
                pawnSelectScreen.SetActive(false);

                if (PlayerPrefs.HasKey("pawnColor"))
                {
                    PlayerCountShow();
                }
            });

        }

        /* void PlayerCountShow()
         {
             Debug.Log("PlayerCountShow");
             //playerCountPanel.transform.localScale = Vector2.zero;
             playerCountEntryFee.text = PhotonController.Instance.gameEntryPrice().ToString("###,###,###");
             playerCountScreen.SetActive(true);

             //LeanTween.scale(playerCountPanel, Vector2.one, 0.2f).setEaseOutBack();



         }*/
        //New 3-2-2026 Method for Practice Mode Entry Fee Free
        void PlayerCountShow()
        {

            /* Ensure starting state
          playerCountPanel.transform.localScale = Vector3.zero;


          LeanTween.cancel(playerCountPanel);
          LeanTween.scale(playerCountPanel, Vector2.one, 0.2f).setEaseOutBack();
          int entryFee;

          if (PlayerPrefs.GetInt("IsPractice", 0) == 1)
          {
              entryFee = 0;
              playerCountEntryFee.text = "FREE";
          }
          else
          {
              entryFee = PhotonController.Instance.gameEntryPrice();
              playerCountEntryFee.text = entryFee.ToString("###,###,###");
          }

          playerCountScreen.SetActive(true);*/


            //  Purana tween cancel + start state reset
            LeanTween.cancel(playerCountPanel);
            playerCountPanel.transform.localScale = Vector3.zero;

            // Entry fee logic (same as yours)
            if (PlayerPrefs.GetInt("IsPractice", 0) == 1)
            {
                playerCountEntryFee.text = "FREE";
            }
            else
            {
                int entryFee = PhotonController.Instance.gameEntryPrice();
                playerCountEntryFee.text = entryFee.ToString("###,###,###");
            }

            // Screen ON
            playerCountScreen.SetActive(true);

            //  Open animation
            LeanTween.scale(playerCountPanel, Vector2.one, 0.2f).setEaseOutBack();




        }

        /*
                public void PlayerCountItemBtn(int playerCount)
                {
                    AudioController.Instance.PlayButtonSound();

                    int entryFee = PhotonController.Instance.gameEntryPrice();

                    if (PlayerPrefs.GetInt("coin") < entryFee)
                    {
                        playerCountScreen.SetActive(false);
                        DeactivatePages();
                        StoreShow();
                        return;
                    }

                    PlayerPrefs.SetInt("playerCount", playerCount);
                    PlayerPrefs.Save();
                    if (PhotonController.Instance.gameMode() == "online")
                    {
                        OnlineShow();
                        PhotonController.Instance.Connect();
                    }
                    else
                    {
                        SceneManager.LoadScene("Game");
                    }
                }
        */


        public void PlayerCountItemBtn(int playerCount)
        {
            AudioController.Instance.PlayButtonSound();

            //  PRACTICE = FREE, baaki sab normal
            int entryFee =
                PlayerPrefs.GetInt("IsPractice", 0) == 1
                ? 0
                : PhotonController.Instance.gameEntryPrice();

            if (PlayerPrefs.GetInt("coin") < entryFee)
            {
                playerCountScreen.SetActive(false);
                DeactivatePages();
                StoreShow();
                return;
            }

            PlayerPrefs.SetInt("playerCount", playerCount);
            PlayerPrefs.Save();



            //  PRACTICE FLAG CLEAR (important)


            if (PhotonController.Instance.gameMode() == "quick")
            {
                OnlineShow();
                PhotonController.Instance.Connect();
            }
            else if (PhotonController.Instance.gameMode() == "friend")
            {
                OnlineShow();
                PhotonController.Instance.Connect();
            }
            else if (PhotonController.Instance.gameMode() == "online")
            {
                OnlineShow();
                PhotonController.Instance.Connect();
            }
            else
            {
                SceneManager.LoadScene("Game");
            }
        }

        public void CopyInviteLink()
        {
            string link = PhotonController.Instance.GetInviteLink();
            GUIUtility.systemCopyBuffer = link;
            LinkTextAnimation();
        }

        //New Method to close player count panel and delete pawncolor key
        bool isClosingPlayerCount = false;
        public void ClosePlayerCountButton()
        {
            /*  
              AudioController.Instance.PlayButtonSound();
              PlayerPrefs.DeleteKey("pawnColor");
             // ResetPracticeFlag();

              PlayerCountClose();*/

            if (isClosingPlayerCount) return;   // 🔒 double click / spam block
            isClosingPlayerCount = true;

            AudioController.Instance.PlayButtonSound();
            PlayerPrefs.DeleteKey("pawnColor");

            PlayerCountClose();

            // 🔓 unlock after animation
            Invoke(nameof(UnlockPlayerCountClose), 0.25f);
        }

        void UnlockPlayerCountClose()
        {
            isClosingPlayerCount = false;
        }

        public void PlayerCountCloseBtn()
        {
            AudioController.Instance.PlayButtonSound();
            if (LeanTween.isTweening(playerCountPanel)) return;

            PlayerPrefs.DeleteKey("pawnSelect");
            ResetPracticeFlag();

            if (selectModeScreen.activeInHierarchy)
                ButtonAlphaState(offlineModeButton, 0.3f, false);

            if (multiplayerScreen.activeInHierarchy)
                ButtonAlphaState(quickMatchScreen, 0.3f, false);

            if (offlineMultiplayerScreen.activeInHierarchy)
                ButtonAlphaState(practiceMatchScreen, 0.3f, false);

            PlayerCountClose();
        }

        void PlayerCountClose()
        {
            /* LeanTween.cancel(playerCountPanel);
             playerCountScreen.SetActive(true);
              LeanTween.scale(playerCountPanel, Vector2.zero, 0.2f).setEaseInBack().setOnComplete(() =>
              {
                  playerCountScreen.SetActive(false);
              });*/



            //  Purana tween cancel
            LeanTween.cancel(playerCountPanel);

            //  Close animation
            LeanTween.scale(playerCountPanel, Vector2.zero, 0.2f)
                .setEaseInBack()
                .setOnComplete(() =>
                {
                    playerCountScreen.SetActive(false);
                });





        }

        public void OnlineShow()
        {
            onlineCancelButton.interactable = true;
            onlinePanel.transform.localScale = Vector2.zero;
            onlineInfoText.text = "Connecting to server...";
            onlineScreen.SetActive(true);

            LeanTween.scale(onlinePanel, Vector2.one, 0.2f).setEaseOutBack();
        }

        public void OnlineClose()
        {
            if (LeanTween.isTweening(onlinePanel)) return;

            LeanTween.scale(onlinePanel, Vector2.zero, 0.2f).setEaseInBack().setOnComplete(() =>
            {
                onlineScreen.SetActive(false);
            });
        }

        public void OnlineInfoMsg(string msg)
        {
            onlineInfoText.text = msg;
        }

        public void OnlineCancelBtn()
        {
            AudioController.Instance.PlayButtonSound();
            onlineCancelButton.interactable = false;
            PhotonNetwork.AutomaticallySyncScene = false;

            if (PhotonNetwork.IsConnectedAndReady)
            {
                PhotonNetwork.Disconnect();
            }
            else
            {
                OnlineClose();
            }
        }

        void UsernameShow()
        {
            usernamePanel.transform.localScale = Vector2.zero;
            usernameScreen.SetActive(true);

            LeanTween.scale(usernamePanel, Vector2.one, 0.2f).setEaseOutBack();
        }

        public void UsernameSaveBtn()
        {
            AudioController.Instance.PlayButtonSound();
            if (usernameInput.text.Length >= 4)
            {
                PlayerPrefs.SetString("username", usernameInput.text);
                PlayerPrefs.Save();
                topUsernameText.text = usernameInput.text;
                UsernameClose();
                //  Profile UI update (agar profile script active hai)
                //   OnClickProfileButton profile = FindObjectOfType<OnClickProfileButton>();
                OnClickProfileButton profile = FindFirstObjectByType<OnClickProfileButton>();

                if (profile != null)
                {
                    profile.RefreshUsernameFromServer();
                }
            }
        }

        void UsernameClose()
        {
            LeanTween.scale(usernamePanel, Vector2.zero, 0.2f).setEaseInBack().setOnComplete(() =>
            {
                usernameScreen.SetActive(false);

                if (!homeScreen.activeInHierarchy)
                {
                    HomeShow();
                }
            });
        }

        public void CopyBattleLink()
        {
            GUIUtility.systemCopyBuffer = BattleLink;
            LinkTextAnimation();
        }

        //public void CopyInviteLink()
        //{
        //    GUIUtility.systemCopyBuffer = InviteLink;
        //    LinkTextAnimation();
        //}
        public void LinkTextAnimation()
        {
            linkGameObject.SetActive(true);
            LeanTween.scale(linkGameObject, Vector2.one, 0.2f).setEaseOutBack().setOnComplete(() =>
            {
                LeanTween.scale(linkGameObject, Vector2.zero, 0.2f).setDelay(1.0f).setEaseInBack().setOnComplete(() =>
                {
                    linkGameObject.SetActive(false);
                });
            });
        }

        public void ResetFullGameData()
        {
            Debug.Log(" RESETTING FULL GAME DATA");

            PlayerPrefs.DeleteAll();   //  Sab data clear
            PlayerPrefs.Save();

            Debug.Log(" Game data reset complete");

            // Optional: App ya scene restart
            // UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }

        void ResetPracticeFlag()
        {
            PlayerPrefs.SetInt("IsPractice", 0);
            PlayerPrefs.Save();
            Debug.Log("IsPractice reset to 0");
        }






    }
}