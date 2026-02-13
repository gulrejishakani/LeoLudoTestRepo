using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace BEKStudio
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance;
        public enum GameState { NONE, READY, DICE, MOVE, MOVING, WAIT, FINISHED };
        public GameState gameState;
        public Transform waypointParent;
        public Transform playersParent;
        public Transform colorWayParent;
        public Transform pawnParent;
        public PawnController greenPawn;
        public PawnController bluePawn;
        public PawnController yellowPawn;
        public PawnController redPawn;
        public PawnController myPawnController;
        public PawnController currentPawnController;
        public List<PawnController> activePawnControllers;
        public Pawn[] greenPawns;
        public Pawn[] bluePawns;
        public Pawn[] yellowPawns;
        public Pawn[] redPawns;
        public Pawn[] allPawns;
        public Sprite[] avatars;
        Pawn[] myPawns;
        public string myPlayerColor;
        RaycastHit2D hit2D;
        public int currentDice;
        string[] pawnColors = new string[] { "Green", "Yellow", "Blue", "Red" };
        public int currentPawnID = 0;
        public bool isLocal;
        public PhotonView photonView;
        public GameObject pauseScreen;
        Pawn selectedPawn;
        [Header("Finished")]
        public GameObject finishedScreen;
        public GameObject finishedPanel;
        public Transform finishedPlayersParent;
        string winnerColor;

        List<string> finishOrder = new List<string>(); //New Add 31-1-2026
        internal bool mustKillToWin;
        private int requiredHomePawns;

        [Header("Glow")]
        public GameObject greenGlow;
        public GameObject yellowGlow;
        public GameObject blueGlow;
        public GameObject redGlow;

        [Header("Mask")]
        public GameObject greenMask;
        public GameObject yellowMask;       
        public GameObject blueMask;
        public GameObject redMask;

        public bool freeUndoUsed = false; //Undo feature ke liye lagaya gya he 10-2-2026 me
        // UI refs undo ke liye 10-2-2026 me add kiya gya he
        public GameObject undoSimpleimage;   // "1"
        public GameObject undoAdImage;     // Ad icon



        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void Start()
        {
            if (PhotonController.Instance.gameMode() == "quick")
            {
                SetupQuickMatch();
            }
        }

        private void SetupQuickMatch()
        {
            /// 1.Start with 2 pawns already out
            foreach (PawnController pController in activePawnControllers)
            {
                //Keep 2 pawn outside
                for (int i = 0; i < 2; i++)
                {
                    Pawn pawn = pController.pawns[i];
                    pawn.inBase = false;
                    pawn.isProtected = true;
                    pawn.currentWayID = pawn.firstWayID;
                    pawn.transform.position = waypointParent.GetChild(pawn.currentWayID).position;
                }
            }

            // 2. Enable quick rules
            mustKillToWin = true;
            requiredHomePawns = 1;
        }

        void OnEnable()
        {
            isLocal = PhotonController.Instance.gameMode() == "computer";

            Debug.Log("The game is going to start --- " + isLocal);
            photonView = GetComponent<PhotonView>();

            myPlayerColor = getMyPawnColor();
            myPawnController = getMyPawn();
            myPawns = greenPawns;
            activePawnControllers = new List<PawnController>();

            // Isme Direct Sab me coin Cut ho rahe the 
            /* PlayerPrefs.SetInt("coin", PlayerPrefs.GetInt("coin") - PhotonController.Instance.gameEntryPrice());
             PlayerPrefs.Save();*/

            //  Ke Liye Dala He Idhar (just before coin cut check)

            Debug.Log("IsPractice flag = " + PlayerPrefs.GetInt("IsPractice", 0));
            if (PlayerPrefs.GetInt("IsPractice", 0) == 0)
            {
                PlayerPrefs.SetInt(
                    "coin",
                    PlayerPrefs.GetInt("coin") - PhotonController.Instance.gameEntryPrice()
                );
                PlayerPrefs.Save();
            }

            //upper vale me practice mode me coin cut nhi hoga kyu ki flag laga diya he 


            DisableNotActivePawns();
#if UNITY_ANDROID || UNITY_IPHONE
            AdsManager.Instance.RequestBannerAd();
#endif

        }

        /* void Update()
         {
             if (gameState != GameState.MOVE || gameState == GameState.WAIT) return;

             if (Input.GetMouseButtonDown(0) && (currentPawnController == myPawnController || (PlayerPrefs.GetInt("IsOfflineMultiplayer") == 1)))
             {
                 hit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                 if (hit2D.collider != null)
                 {
                     Debug.Log("hit pawn now check color");
                     if (hit2D.collider.tag == "Pawn" && (hit2D.collider.name.StartsWith(myPlayerColor) || (PlayerPrefs.GetInt("IsOfflineMultiplayer") == 1)))
                     {
                         if (hit2D.collider.GetComponent<Pawn>().inBase && currentDice != 5) return;

                         if (hit2D.collider.GetComponent<Pawn>().moveCount + (currentDice + 1) > 56) return;

                         myPawnController.HighlightDices(false);
                         gameState = GameState.MOVING;
                         selectedPawn = hit2D.collider.GetComponent<Pawn>();
                         hit2D.collider.GetComponent<Pawn>().Move(currentDice + 1);
                     }
                 }
             }
         }
 */

        // YE New Wala HE - 31-1-2026 Game ke Pawn controll karne ke liye

        void Update()
        {
            if (gameState != GameState.MOVE || gameState == GameState.WAIT) return;

            if (Input.GetMouseButtonDown(0))
            {
                // Sirf current player hi click se move kar sakta hai
                if (currentPawnController != myPawnController && PlayerPrefs.GetInt("IsOfflineMultiplayer") != 1)
                    return;

                hit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (hit2D.collider != null)
                {
                    if (hit2D.collider.tag == "Pawn")
                    {
                        Pawn clickedPawn = hit2D.collider.GetComponent<Pawn>();

                        //  Sirf currentPawnController ki pawn allow
                        if (clickedPawn == null || clickedPawn.pawnController != currentPawnController)
                            return;

                        if (clickedPawn.inBase && currentDice != 5) return;
                        if (clickedPawn.moveCount + (currentDice + 1) > 56) return;

                        currentPawnController.HighlightDices(false);
                        gameState = GameState.MOVING;
                        selectedPawn = clickedPawn;
                        clickedPawn.Move(currentDice + 1);
                    }
                }
            }
        }



public void UndoTurnButton()
{
    // Safety check
    if (gameState == GameState.MOVING || gameState == GameState.FINISHED)
        return;

    if (!freeUndoUsed)
    {
        freeUndoUsed = true;
        UpdateUndoButtonUI();
        GiveTurnBack();
    }
    else
    {
        // Show rewarded ad
       AdsManager.Instance.pendingReward = AdsManager.RewardType.UndoTurn;
       AdsManager.Instance.ShowRewardedAd();
      //  });
    }
}







// Ye turn back method he jo player ko uska turn wapas dega jab wo undo karega - 10-2-2026 me add kiya gya HE
public void GiveTurnBack()
{
    // Stop any animations / timers
    currentPawnController.StopAnimation();

    // Reset state so player can roll dice again
    ChangeGameState(GameState.READY);

    // Reset timer
    currentPawnController.time = 10;
    currentPawnController.canPlayAgain = false;

    // Start timer animation again (optional)
    currentPawnController.StartTimer(true);

    Debug.Log("Turn given back to same player");
}

// Ui Update karne ke liye he ye to khali button ke text aur image ko update karega free undo ya ad undo ke hisab se - 10-2-2026 me add kiya gya HE
void UpdateUndoButtonUI()
{
    if (!freeUndoUsed)
    {
        undoSimpleimage.SetActive(true);
        undoAdImage.SetActive(false);
    }
    else
    {
        undoSimpleimage.SetActive(false);
        undoAdImage.SetActive(true);
    }
}









        public void PauseBtn()
        {
            AudioController.Instance.PlayButtonSound();
            pauseScreen.SetActive(true);
        }

        public void PauseYesBtn()
        {
            AudioController.Instance.PlayButtonSound();
            PhotonNetwork.AutomaticallySyncScene = false;
            PhotonNetwork.Disconnect();
            LeanTween.cancelAll();
            AdsManager.Instance.ShowInterstitialAd();
            SceneManager.LoadScene("Menu");
        }

        public void PauseNoBtn()
        {
            AudioController.Instance.PlayButtonSound();
            pauseScreen.SetActive(false);
        }

        [PunRPC]
        void RPCPawnSelect(string arg)
        {
            Pawn p = allPawns.Where(x => x.name == arg).FirstOrDefault();
            if (p == null) return;

            selectedPawn = p;
            currentPawnController.HighlightDices(false);
            gameState = GameState.MOVING;
            p.Move(currentDice + 1);
        }

        string getMyPawnColor()
        {
            if (isLocal)
            {
                return PlayerPrefs.GetString("pawnColor");
            }
            else
            {
                for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                {
                    if (PhotonNetwork.PlayerList[i] == PhotonNetwork.LocalPlayer)
                    {
                        return pawnColors[i];
                    }
                }
            }

            return "";
        }

        int getPlayerCount()
        {
            if (isLocal)
            {
                return PlayerPrefs.GetInt("playerCount");
            }
            else
            {
                return PhotonNetwork.PlayerList.Length;
            }
        }

        PawnController getMyPawn()
        {
            if (myPlayerColor == "Green") return greenPawn;
            if (myPlayerColor == "Yellow") return yellowPawn;
            if (myPlayerColor == "Blue") return bluePawn;
            if (myPlayerColor == "Red") return redPawn;

            return null;
        }

        void DisableNotActivePawns()
        {
            List<string> colors = pawnColors.ToList();
            List<string> disabledColors = new List<string>();

            int removeCount = 4 - getPlayerCount();

            if (isLocal)
            {
                int rand = Random.Range(0, colors.Count);

                for (int i = 0; i < removeCount; i++)
                {
                    rand = Random.Range(0, colors.Count);

                    while (colors[rand] == myPlayerColor)
                    {
                        rand = Random.Range(0, colors.Count);
                    }

                    disabledColors.Add(colors[rand]);
                    colors.Remove(colors[rand]);
                }
            }
            else
            {
                for (int i = getPlayerCount(); i < 4; i++)
                {
                    if (i < colors.Count)
                    {
                        disabledColors.Add(colors[i]);
                        colors.RemoveAt(i);
                        i--;
                    }
                }
            }
            //Ye Flag Ka He 4-1-2026
            Debug.Log("DisablePawns:" + PlayerPrefs.GetInt("IsOfflineMultiplayer"));
            /*  if (PlayerPrefs.GetInt("IsOfflineMultiplayer") == 1)
              {
                  disabledColors.Clear();
              }
  */

            for (int i = 0; i < disabledColors.Count; i++)
            {
                if (disabledColors[i] == "Green")
                {
                    greenPawn.DisablePawn();
                }
                else if (disabledColors[i] == "Yellow")
                {
                    yellowPawn.DisablePawn();
                }
                else if (disabledColors[i] == "Blue")
                {
                    bluePawn.DisablePawn();
                }
                else if (disabledColors[i] == "Red")
                {
                    redPawn.DisablePawn();
                }
            }

            for (int i = 0; i < colors.Count; i++)
            {
                if (colors[i] == "Green")
                {
                    activePawnControllers.Add(greenPawn);
                    greenPawn.isBot = !PhotonNetwork.IsConnectedAndReady && colors[i] != myPlayerColor;
                    if (!isLocal)
                    {
                        greenPawn.SetUserInfo(PhotonNetwork.PlayerList[i].NickName, (int)PhotonNetwork.PlayerList[i].CustomProperties["avatar"]);
                    }
                    else
                    {
                        greenPawn.SetUserInfo();
                    }
                }
                else if (colors[i] == "Yellow")
                {
                    activePawnControllers.Add(yellowPawn);
                    yellowPawn.isBot = !PhotonNetwork.IsConnectedAndReady && colors[i] != myPlayerColor;
                    if (!isLocal)
                    {
                        yellowPawn.SetUserInfo(PhotonNetwork.PlayerList[i].NickName, (int)PhotonNetwork.PlayerList[i].CustomProperties["avatar"]);
                    }
                    else
                    {
                        yellowPawn.SetUserInfo();
                    }
                }
                else if (colors[i] == "Blue")
                {
                    activePawnControllers.Add(bluePawn);
                    bluePawn.isBot = !PhotonNetwork.IsConnectedAndReady && colors[i] != myPlayerColor;
                    if (!isLocal)
                    {
                        bluePawn.SetUserInfo(PhotonNetwork.PlayerList[i].NickName, (int)PhotonNetwork.PlayerList[i].CustomProperties["avatar"]);
                    }
                    else
                    {
                        bluePawn.SetUserInfo();
                    }
                }
                else if (colors[i] == "Red")
                {
                    activePawnControllers.Add(redPawn);
                    redPawn.isBot = !PhotonNetwork.IsConnectedAndReady && colors[i] != myPlayerColor;
                    if (!isLocal)
                    {
                        redPawn.SetUserInfo(PhotonNetwork.PlayerList[i].NickName, (int)PhotonNetwork.PlayerList[i].CustomProperties["avatar"]);
                    }
                    else
                    {
                        redPawn.SetUserInfo();
                    }
                }
            }

            foreach (PawnController pController in activePawnControllers)
            {
                if (pController != myPawnController && (PlayerPrefs.GetInt("IsOfflineMultiplayer") != 1))
                {
                    pController.DisableColliders();
                }
            }


            SetActivePawn();
        }

        void SetActivePawn()
        {
            if (currentPawnController != null)
            {
                currentPawnController.time = 10;
                currentPawnController.canPlayAgain = false;
            }

            if (isLocal)
            {
                currentPawnController = activePawnControllers[currentPawnID];
            }
            else
            {
                currentPawnController = getCurrentPawnController();
            }

            currentPawnController.time = 10;
            currentPawnController.canPlayAgain = false;
            //  YAHAN Pe Glow Update Karne Ke Liye Code Add Kiya He 4-1-2026
                UpdateGlow(currentPawnController);
                //  YAHAN Pe Glow Update Karne Ke Liye Code Add Kiya He 4-1-2026
                UpdateGlow(currentPawnController);

            ChangeGameState(GameState.READY);

            if (isLocal)
            {
                if (currentPawnController != myPawnController)
                {
                    currentPawnController.StartTimer(true);
                    if (PlayerPrefs.GetInt("IsOfflineMultiplayer") != 1)
                    {
                        currentPawnController.Play();
                    }
                }
                else
                {
                    currentPawnController.StartTimer(true);
                }
            }
            else if (currentPawnController == myPawnController)
            {
                currentPawnController.StartTimer(true);
            }
        }

        PawnController getCurrentPawnController()
        {
            int colorID = (int)PhotonNetwork.MasterClient.CustomProperties["colorID"];

            if (colorID == 0)
            {
                return greenPawn;
            }
            else if (colorID == 1)
            {
                return yellowPawn;
            }
            else if (colorID == 2)
            {
                return bluePawn;
            }
            else if (colorID == 3)
            {
                return redPawn;
            }

            return null;
        }

void StartPulse(GameObject obj)
{
    LeanTween.cancel(obj);

    // Zoom / Pulse
    LeanTween.scale(obj, Vector3.one * 1.08f, 0.5f)
        .setEaseInOutSine()
        .setLoopPingPong();
}

void StopPulse(GameObject obj)
{
    if (obj == null) return;

    LeanTween.cancel(obj);
    obj.transform.localScale = Vector3.one;
}








        public void ChangeGameState(GameState newState)
        {


            gameState = newState;

            if (newState == GameState.FINISHED)
            {


                UpdateWinLoseMatch();


                // XP SYSTEM
                //  XPSystem xp = FindObjectOfType<XPSystem>();
                XPSystem xp = FindFirstObjectByType<XPSystem>();





                if (xp != null)
                {
                    int index = finishOrder.IndexOf(myPlayerColor);
                    int myRank = index + 1;



                    if (myRank == 1)
                    {

                        xp.AddXP("Win");
                    }
                    else if (myRank == 2)
                    {

                        xp.AddXP("Second");
                    }
                    else if (myRank == 3)
                    {

                        xp.AddXP("Third");
                    }
                    else
                    {

                        xp.AddXP("Lose");
                    }
                }

                // Photon sync
                if (!isLocal)
                {


                    if (PhotonNetwork.IsMasterClient)
                    {

                        photonView.RPC("WinnerColorRPC", RpcTarget.OthersBuffered, winnerColor);
                    }
                }


                FinishedShow();
            }
        }

        void UpdateWinLoseMatch()
        {
            // Match +1 
            int match = PlayerPrefs.GetInt("match", 0);
            PlayerPrefs.SetInt("match", match + 1);

            if (winnerColor == myPlayerColor)
            {
                // Win +1
                int win = PlayerPrefs.GetInt("win", 0);
                PlayerPrefs.SetInt("win", win + 1);
            }
            else
            {
                // Lose +1
                int lose = PlayerPrefs.GetInt("lose", 0);
                PlayerPrefs.SetInt("lose", lose + 1);
            }

            PlayerPrefs.Save();
        }

        [PunRPC]
        void WinnerColorRPC(string color)
        {
            CheckForFinish(color);
        }

        public void CheckGameStatus()
        {
            ChangeGameState(GameState.WAIT);

            CheckPawnsForSameWay();
        }

        string isSomeoneFinished()
        {
            Pawn[] collectedGreenPaws = greenPawns.Where(x => x.isCollected).ToArray();
            Pawn[] collectedBluePaws = bluePawns.Where(x => x.isCollected).ToArray();
            Pawn[] collectedYellowPaws = yellowPawns.Where(x => x.isCollected).ToArray();
            Pawn[] collectedRedPaws = redPawns.Where(x => x.isCollected).ToArray();

            if (collectedGreenPaws.Length == 4)
            {
                return "Green";
            }

            if (collectedYellowPaws.Length == 4)
            {
                return "Yellow";
            }

            if (collectedBluePaws.Length == 4)
            {
                return "Blue";
            }


            if (collectedRedPaws.Length == 4)
            {
                return "Red";
            }

            return "";
        }

        void CheckPawnsForSameWay()
        {
            bool wait = false;

            foreach (PawnController pController in activePawnControllers)
            {
                if (pController == currentPawnController) continue;

                Pawn[] currentPawns = currentPawnController.pawns;
                Pawn[] activePawns = pController.pawns;

                foreach (Pawn currentPawn in currentPawns)
                {
                    foreach (Pawn activePawn in activePawns)
                    {
                        if (currentPawn.currentWayID != activePawn.currentWayID) continue;
                        if (currentPawn.inBase || activePawn.inBase) continue;
                        if (currentPawn.isProtected || activePawn.isProtected) continue;
                        if (currentPawn.isCollected || activePawn.isCollected) continue;
                        if (currentPawn.inColorWay || activePawn.inColorWay) continue;

                        wait = true;
                        activePawn.ReturnToBase();
                        currentPawnController.canPlayAgain = true;
                        // Mark 1 kill for quick match mode
                        currentPawnController.hasKilledOpponent = true;
                    }
                }
            }

            if (!wait)
            {
                Debug.Log("Check Pawn for sAMe Way");
                CheckForFinish();
            }
        }



        public void CheckForFinish(string color = "")
        {
            Debug.Log("CheckForFinish CALLED");


            foreach (PawnController pc in activePawnControllers)
            {
                string c = pc.pawnColor;
                bool finished = IsColorFinished(c);



                if (finished && !finishOrder.Contains(c))
                {
                    finishOrder.Add(c);

                }
            }


            int totalPlayers = activePawnControllers.Count;


            if (finishOrder.Count >= totalPlayers)
            {
                winnerColor = finishOrder[0];

                ChangeGameState(GameState.FINISHED);
                return;
            }

            if (currentPawnController.canPlayAgain)
            {

                currentPawnController.time = 10;
                ChangeGameState(GameState.READY);

                if (currentPawnController != myPawnController && isLocal)
                {
                    currentPawnController.Play();
                }
                return;
            }


            ChangePlayer();
        }

        bool IsColorFinished(string color)
        {
            bool result = false;

            if (color == "Green") result = greenPawns.All(p => p.isCollected);
            else if (color == "Yellow") result = yellowPawns.All(p => p.isCollected);
            else if (color == "Blue") result = bluePawns.All(p => p.isCollected);
            else if (color == "Red") result = redPawns.All(p => p.isCollected);


            return result;
        }

        public void ChangePlayer()
        {
            currentPawnController.profileTimeImg.fillAmount = 0;
            currentPawnController.time = 10;
            currentPawnController.canPlayAgain = false;
            currentPawnController.StopAnimation();

            if (isLocal)
            {
                currentPawnID = (currentPawnID + 1) % getPlayerCount();
                SetActivePawn();
            }
            else
            {
                if (photonView.IsMine && PhotonNetwork.IsMasterClient && currentPawnController == myPawnController)
                {
                    StartCoroutine(SwitchMasterDelay());
                }
            }
        }

        IEnumerator SwitchMasterDelay()
        {
            yield return new WaitForSecondsRealtime(1f);
            PhotonNetwork.SetMasterClient(PhotonNetwork.MasterClient.GetNext());
        }

        public void MasterClientChanged()
        {
            SetActivePawn();
        }

        public void GameDiceBtn(string color)
        {
            if (currentPawnController.pawnColor != color) return;
            if (gameState != GameState.READY) return;
            if (!isLocal && !PhotonNetwork.IsMasterClient) return;

            gameState = GameState.DICE;
            //currentDice = Random.Range(4, 6);
            currentDice = Random.Range(0, 6);
            AudioController.Instance.PlayDiceSound();

            if (isLocal)
            {
                currentPawnController.PlayDiceAnimation();
                LeanTween.value(0, 1, 0.5f).setOnComplete(() =>
                {
                    currentPawnController.CheckAvailableMovements(currentDice == 5);

                });
            }
            else
            {
                photonView.RPC("RPCDice", RpcTarget.AllBuffered, currentDice);
            }
        }

        [PunRPC]
        void RPCDice(int arg)
        {
            currentDice = arg;
            currentPawnController.PlayDiceAnimation();

            if (photonView.IsMine)
            {
                currentPawnController.CheckAvailableMovements(currentDice == 5);      

            }
        }

        public void CheckRoomPlayers(Player leftPlayer)
        {
            if (gameState == GameState.FINISHED) return;

            int colorID = (int)leftPlayer.CustomProperties["colorID"];

            if (colorID == 0)
            {
                if (activePawnControllers.Contains(greenPawn))
                {
                    activePawnControllers.Remove(greenPawn);
                }
                greenPawn.DisablePawn();
            }
            else if (colorID == 1)
            {
                if (activePawnControllers.Contains(yellowPawn))
                {
                    activePawnControllers.Remove(yellowPawn);
                }
                yellowPawn.DisablePawn();
            }
            else if (colorID == 2)
            {
                if (activePawnControllers.Contains(bluePawn))
                {
                    activePawnControllers.Remove(bluePawn);
                }
                bluePawn.DisablePawn();
            }
            else if (colorID == 3)
            {
                if (activePawnControllers.Contains(redPawn))
                {
                    activePawnControllers.Remove(redPawn);
                }
                redPawn.DisablePawn();
            }

            if (PhotonNetwork.PlayerList.Length == 1)
            {
                winnerColor = myPlayerColor;
                ChangeGameState(GameState.FINISHED);
            }
        }

        // Me Thod  animation me KAmm Kiya tha
        public void FinishedShow()
        {
            if (pauseScreen.activeInHierarchy)
            {
                pauseScreen.SetActive(false);
            }

            AudioController.Instance.PlayWinSound();
            List<GameObject> activePlayerForPanel = new List<GameObject>();
            PhotonNetwork.AutomaticallySyncScene = false;

            finishedPanel.transform.localScale = Vector3.zero;
            finishedScreen.SetActive(true);

            GameObject winnerObject = null;

            foreach (PawnController p in activePawnControllers)
            {
                if (p.pawnColor == "Green")
                {
                    activePlayerForPanel.Add(finishedPlayersParent.GetChild(0).gameObject);
                    finishedPlayersParent.GetChild(0).gameObject.SetActive(true);
                    finishedPlayersParent.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = greenPawn.usernameText.text;
                    finishedPlayersParent.GetChild(0).GetChild(0).GetComponent<Image>().sprite = greenPawn.avatarImg.sprite;

                    if (winnerColor == p.pawnColor)
                    {
                        winnerObject = finishedPlayersParent.GetChild(0).gameObject;
                        LeanTween.scale(finishedPlayersParent.GetChild(0).gameObject, new Vector3(1.05f, 1.05f, 1.05f), 0.3f).setLoopPingPong();
                    }
                }
                else if (p.pawnColor == "Yellow")
                {
                    activePlayerForPanel.Add(finishedPlayersParent.GetChild(2).gameObject);
                    finishedPlayersParent.GetChild(2).gameObject.SetActive(true);
                    finishedPlayersParent.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = yellowPawn.usernameText.text;
                    finishedPlayersParent.GetChild(2).GetChild(0).GetComponent<Image>().sprite = yellowPawn.avatarImg.sprite;

                    if (winnerColor == p.pawnColor)
                    {
                        winnerObject = finishedPlayersParent.GetChild(2).gameObject;
                        LeanTween.scale(finishedPlayersParent.GetChild(2).gameObject, new Vector3(1.05f, 1.05f, 1.05f), 0.3f).setLoopPingPong();
                    }
                }
                else if (p.pawnColor == "Blue")
                {
                    activePlayerForPanel.Add(finishedPlayersParent.GetChild(1).gameObject);
                    finishedPlayersParent.GetChild(1).gameObject.SetActive(true);
                    finishedPlayersParent.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = bluePawn.usernameText.text;
                    finishedPlayersParent.GetChild(1).GetChild(0).GetComponent<Image>().sprite = bluePawn.avatarImg.sprite;

                    if (winnerColor == p.pawnColor)
                    {
                        winnerObject = finishedPlayersParent.GetChild(1).gameObject;
                        LeanTween.scale(finishedPlayersParent.GetChild(1).gameObject, new Vector3(1.05f, 1.05f, 1.05f), 0.3f).setLoopPingPong();
                    }
                }
                else if (p.pawnColor == "Red")
                {
                    activePlayerForPanel.Add(finishedPlayersParent.GetChild(3).gameObject);
                    finishedPlayersParent.GetChild(3).gameObject.SetActive(true);
                    finishedPlayersParent.GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>().text = redPawn.usernameText.text;
                    finishedPlayersParent.GetChild(3).GetChild(0).GetComponent<Image>().sprite = redPawn.avatarImg.sprite;

                    if (winnerColor == p.pawnColor)
                    {
                        winnerObject = finishedPlayersParent.GetChild(3).gameObject;
                        LeanTween.scale(finishedPlayersParent.GetChild(3).gameObject, new Vector3(1.05f, 1.05f, 1.05f), 0.3f).setLoopPingPong();
                    }
                }
            }
            // ANIMATION Yaha se  - Pehle Jo Chipak raaha tha uska 
            LeanTween.scale(finishedPanel, Vector3.one, 0.2f).setEaseOutBack().setOnStart(() =>
            {
                for (int i = 0; i < activePlayerForPanel.Count; i++)
                {
                    GameObject g = activePlayerForPanel[i];
                    LeanTween.alphaCanvas(g.GetComponent<CanvasGroup>(), 1, 0.5f).setDelay(i * 0.25f);
                }
            });

            int gamePrice = PhotonController.Instance.gameEntryPrice();

            //  Practice me UI + reward = 0
            if (PlayerPrefs.GetInt("IsPractice", 0) == 1)
            {
                gamePrice = 0;
            }
            else
            {
                //  Normal match me reward do
                int winnerPrice = PlayerPrefs.GetInt("playerCount") * gamePrice;

                if (winnerColor == myPlayerColor)
                {
                    PlayerPrefs.SetInt("coin", PlayerPrefs.GetInt("coin") + winnerPrice);
                    PlayerPrefs.Save();
                }
            }

            for (int i = 0; i < activePlayerForPanel.Count; i++)
            {
                GameObject g = activePlayerForPanel[i];
                Transform txt = g.transform.Find("Coin");

                txt.GetComponent<TextMeshProUGUI>().text = gamePrice.ToString("###,###,###");

                if (g == winnerObject)
                {
                    LeanTween.value(gamePrice, PlayerPrefs.GetInt("playerCount") * gamePrice, 2f).setOnUpdate((float var) =>
                    {
                        txt.GetComponent<TextMeshProUGUI>().text = var.ToString("###,###");
                    });
                }
                else
                {
                    LeanTween.value(gamePrice, 0, 2f).setOnUpdate((float var) =>
                    {
                        txt.GetComponent<TextMeshProUGUI>().text = var.ToString("###,###");
                    }).setOnComplete(() =>
                    {
                        txt.GetComponent<TextMeshProUGUI>().text = "0";
                    });
                }
            }
        }

//coin Ranking System Ke Liye 4-1-2026 new


int GetRankReward(int totalPlayers, int rank) // rank: 1 = first, 2 = second...
{
    if (totalPlayers == 2)
    {
        if (rank == 1) return 120;
        if (rank == 2) return 30;
    }
    else if (totalPlayers == 3)
    {
        if (rank == 1) return 150;
        if (rank == 2) return 50;
        if (rank == 3) return 25;
    }
    else if (totalPlayers == 4)
    {
        if (rank == 1) return 180;
        if (rank == 2) return 70;
        if (rank == 3) return 30;
        if (rank == 4) return 20;
    }
    return 0;
}












        public void FinishedMenuBtn()
        {
            PlayerPrefs.DeleteKey("IsPractice"); // Clear practice flag on exit
            PhotonNetwork.AutomaticallySyncScene = false;
            PhotonNetwork.Disconnect();
            LeanTween.cancelAll(); // Cancel all LeanTween animations
            AdsManager.Instance.ShowInterstitialAd();
            SceneManager.LoadScene("Menu");
        }


        //Set on button


        //YE Khali Super Test Kelie he 
        public void ForceWinBtn()
        {
            ForcePlayerWin(myPlayerColor);
        }
        public void ForcePlayerWin(string color)
        {
            // Prevent double finish
            if (gameState == GameState.FINISHED)
                return;

            // Collect all pawns of selected color
            Pawn[] targetPawns = null;

            switch (color)
            {
                case "Green":
                    targetPawns = greenPawns;
                    break;
                case "Yellow":
                    targetPawns = yellowPawns;
                    break;
                case "Blue":
                    targetPawns = bluePawns;
                    break;
                case "Red":
                    targetPawns = redPawns;
                    break;
                default:
                    Debug.LogError("Invalid color passed to ForcePlayerWin");
                    return;
            }

            foreach (Pawn p in targetPawns)
            {
                if (!p.isCollected)
                {
                    p.isCollected = true;
                    p.inBase = false;
                    p.gameObject.SetActive(false); // optional visual cleanup
                }
            }

            // Update finish order correctly
            if (!finishOrder.Contains(color))
            {
                finishOrder.Insert(0, color); // Winner always first
            }

            winnerColor = color;

            // Sync winner in Photon
            if (!isLocal && PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("WinnerColorRPC", RpcTarget.OthersBuffered, color);
            }

            // End game
            ChangeGameState(GameState.FINISHED);
        }


     // New Method for Stacked Pawns - 31-1-2026 Ye system Pawns ko Stack hone par Alag se dikhayega - Jaha Jaha same tile par 2 ya usse jyada pawns honge waha waha ye method call hoga aur unko thoda adjust karke dikhayega taki pata chale ki 1 nahi balki 2 ya usse jyada pawns hai
public void UpdateStackedPawns(int wayID)
{
    List<Pawn> sameTilePawns = new List<Pawn>();

    foreach (Pawn p in allPawns)
    {
        if (!p.inBase && !p.isCollected && p.currentWayID == wayID)
        {
            sameTilePawns.Add(p);
        }
    }

    Vector3 centerPos = waypointParent.GetChild(wayID).position;

    // Reset all to default first
    foreach (Pawn p in sameTilePawns)
    {
        p.SetScaleToDefault();
        p.transform.position = centerPos;
    }

    if (sameTilePawns.Count <= 1)
        return;

    float offset = 0.25f; // board size ke hisaab se adjust kar sakte ho
    float scale = 0.85f;

    if (sameTilePawns.Count == 2)
    {
        sameTilePawns[0].transform.position = centerPos + new Vector3(-offset, 0, 0);
        sameTilePawns[1].transform.position = centerPos + new Vector3( offset, 0, 0);
    }
    else if (sameTilePawns.Count == 3)
    {
        sameTilePawns[0].transform.position = centerPos + new Vector3(-offset, -offset * 0.5f, 0);
        sameTilePawns[1].transform.position = centerPos + new Vector3( offset, -offset * 0.5f, 0);
        sameTilePawns[2].transform.position = centerPos + new Vector3( 0,       offset * 0.6f, 0);
    }
    else // 4 or more
    {
        sameTilePawns[0].transform.position = centerPos + new Vector3(-offset,  offset, 0);
        sameTilePawns[1].transform.position = centerPos + new Vector3( offset,  offset, 0);
        sameTilePawns[2].transform.position = centerPos + new Vector3(-offset, -offset, 0);
        sameTilePawns[3].transform.position = centerPos + new Vector3( offset, -offset, 0);
    }

    // Scale down all when stacked
    for (int i = 0; i < sameTilePawns.Count; i++)
    {
        sameTilePawns[i].transform.localScale *= scale;
    }

   
      
    }

//Ring Glow karne Profile Mask Keliye 9-2-2026 10-2-2026

void UpdateGlow(PawnController active)
{
    // Sab band + stop
    StopPulse(redGlow);
    StopPulse(greenGlow);
    StopPulse(blueGlow);
    StopPulse(yellowGlow);

    // Sab band + stop (Profile Masks)
    StopPulse(redMask);   redMask.SetActive(false);
    StopPulse(greenMask); greenMask.SetActive(false);
    StopPulse(blueMask);  blueMask.SetActive(false);
    StopPulse(yellowMask);yellowMask.SetActive(false);

    redGlow.SetActive(false);
    greenGlow.SetActive(false);
    blueGlow.SetActive(false);
    yellowGlow.SetActive(false);

    // Sirf active wala on + pulse
    if (active == greenPawn)
    {
        greenGlow.SetActive(true);
        StartPulse(greenGlow);


        greenMask.SetActive(true); 
        StartPulse(greenMask);
    }
    else if (active == yellowPawn)
    {
        yellowGlow.SetActive(true);
        StartPulse(yellowGlow);


        yellowMask.SetActive(true);
        StartPulse(yellowMask);
    }
    else if (active == bluePawn)
    {
        blueGlow.SetActive(true);
        StartPulse(blueGlow);

        blueMask.SetActive(true);
        StartPulse(blueMask);
    }
    else if (active == redPawn)
    {
        redGlow.SetActive(true);
        StartPulse(redGlow);

        redMask.SetActive(true);
        StartPulse(redMask);
    }
}


    }
    
    
    }