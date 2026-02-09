using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BEKStudio
{
    [System.Serializable]
    public struct BotMovements
    {
        public Pawn pawn;
        public int moveCount;

        public BotMovements(Pawn target, int count)
        {
            pawn = target;
            moveCount = count;
        }
    }

    public class PawnController : MonoBehaviour, IPunObservable
    {
        public Pawn[] pawns;
        public GameObject pawnParent;
        public GameObject profileParent;
        public Image profileDiceImg;
        public Image profileTimeImg;
        public Sprite[] diceSprites;
        public string pawnColor;
        public bool isBot;
        public bool canPlayAgain;
        public List<BotMovements> botMovements;
        public float time;
        public TextMeshPro usernameText;
        public Image avatarImg;

        public bool hasKilledOpponent;

        void Start()
        {
            botMovements = new List<BotMovements>();
        }

        void Update()
        {
            if (!GameController.Instance.isLocal && !GameController.Instance.photonView.IsMine) return;

            if (GameController.Instance.currentPawnController == this)
            {
                if (GameController.Instance.gameState != GameController.GameState.FINISHED && GameController.Instance.gameState != GameController.GameState.MOVING && GameController.Instance.gameState != GameController.GameState.WAIT)
                {
                    if (time > 0)
                    {
                        time -= 1 * Time.deltaTime;
                        profileTimeImg.fillAmount = time / 10;
                    }
                    else
                    {
                        canPlayAgain = false;
                        profileTimeImg.fillAmount = 0;
                        HighlightDices(false);
                        GameController.Instance.CheckGameStatus();
                    }
                }
                else
                {
                    profileTimeImg.fillAmount = 0;
                }
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(time);
            }
            else
            {
                profileTimeImg.fillAmount = (float)stream.ReceiveNext() / 10;
            }
        }

        public void SetUserInfo(string name, int avatarID)
        {
            usernameText.text = name;
            avatarImg.sprite = MenuController.Instance.avatars[avatarID];
        }

        public void SetUserInfo()
        {
            if (GameController.Instance.myPawnController != this)
            {
                usernameText.text = "Guest" + Random.Range(0, 99999);
                //avatarImg.sprite = GameController.Instance.avatars[Random.Range(0, GameController.Instance.avatars.Length)];
                avatarImg.sprite = MenuController.Instance.avatars[Random.Range(0, MenuController.Instance.avatars.Length)];
            }
            else
            {
                usernameText.text = PlayerPrefs.GetString("username");
                avatarImg.sprite = MenuController.Instance.avatars[PlayerPrefs.GetInt("avatar")];
            }
        }

        public void DisableColliders()
        {
            for (int i = 0; i < pawns.Length; i++)
            {
                pawns[i].GetComponent<CircleCollider2D>().enabled = false;
            }
        }

        /* public void HighlightDices(bool active) {
             for (int i = 0; i < pawns.Length; i++) {
                 if (active) {
                     LeanTween.scale(pawns[i].gameObject, pawns[i].transform.localScale * 1.1f, 0.2f).setLoopPingPong();
                 } else {
                     LeanTween.cancel(pawns[i].gameObject);
                     pawns[i].SetScaleToDefault();
                 }
             }
         }*/

        public void HighlightDices(bool active)
        {
            for (int i = 0; i < pawns.Length; i++)
            {
                Pawn p = pawns[i];

                // Cancel any previous tween on this pawn
                LeanTween.cancel(p.gameObject);

                if (active)
                {
                    // Always reset to original scale first
                    p.SetScaleToDefault();

                    Vector3 baseScale = p.transform.localScale;

                    // Animate between baseScale and 1.1x of it
                    LeanTween.scale(p.gameObject, baseScale * 1.1f, 0.2f)
                             .setLoopPingPong();
                }
                else
                {
                    // Stop animation and reset scale
                    p.SetScaleToDefault();
                }
            }
        }


        /*  public void CheckAvailableMovements(bool playAgain) {
              botMovements.Clear();
              canPlayAgain = playAgain;

              Pawn[] basePawns = pawns.Where(x => x.inBase).ToArray();
              int availablePawnCount = 0;

              if (basePawns.Length == 4 && GameController.Instance.currentDice != 5) {
                  GameController.Instance.ChangePlayer();
                  return;
              }

              for (int i = 0; i < pawns.Length; i++) {
                  Pawn pawn = pawns[i];

                  if (!pawn.isCollected) {
                      if (pawn.inBase) {
                          if (GameController.Instance.currentDice == 5) {
                              availablePawnCount++;
                              botMovements.Add(new BotMovements(pawn, GameController.Instance.currentDice + 1));
                              LeanTween.scale(pawn.gameObject, pawn.transform.localScale * 1.1f, 0.2f).setLoopPingPong();
                          }
                      } else {
                          if (pawn.moveCount < 56 && pawn.moveCount + GameController.Instance.currentDice + 1 <= 56) {
                              availablePawnCount++;
                              botMovements.Add(new BotMovements(pawn, GameController.Instance.currentDice + 1));
                              LeanTween.scale(pawn.gameObject, pawn.transform.localScale * 1.1f, 0.2f).setLoopPingPong();
                          }
                      }
                  }
              }

              if (availablePawnCount > 0) {
                  GameController.Instance.ChangeGameState(GameController.GameState.MOVE);

                  if (isBot && PlayerPrefs.GetInt("IsOfflineMultiplayer") != 1) {
                      GameController.Instance.ChangeGameState(GameController.GameState.MOVING);
                      HighlightDices(false);
                      BotMovements rand = botMovements[Random.Range(0, botMovements.Count)];
                      rand.pawn.Move(rand.moveCount);
                      botMovements.Clear();
                  }
              } else {
                  GameController.Instance.ChangePlayer();
              }

          }*/



        public void CheckAvailableMovements(bool playAgain)
        {

            //  Sirf current player apni pawns highlight kare 5-2-2026
            if (GameController.Instance.currentPawnController != this)
            {
                // : apni pawns ka highlight band rakho
                HighlightDices(false);
                return;
            }



            botMovements.Clear();
            canPlayAgain = playAgain;

            Pawn[] basePawns = pawns.Where(x => x.inBase).ToArray();
            int availablePawnCount = 0;

            if (basePawns.Length == 4 && GameController.Instance.currentDice != 5)
            {
                GameController.Instance.ChangePlayer();
                return;
            }

            for (int i = 0; i < pawns.Length; i++)
            {
                Pawn pawn = pawns[i];

                if (!pawn.isCollected)
                {

                    if (pawn.inBase)
                    {
                        if (GameController.Instance.currentDice == 5)
                        {
                            availablePawnCount++;
                            botMovements.Add(new BotMovements(pawn, GameController.Instance.currentDice + 1));
                            LeanTween.scale(pawn.gameObject, pawn.transform.localScale * 1.1f, 0.2f).setLoopPingPong();
                        }
                    }
                    else
                    {
                        if (pawn.moveCount < 56 && pawn.moveCount + GameController.Instance.currentDice + 1 <= 56)
                        {
                            availablePawnCount++;
                            botMovements.Add(new BotMovements(pawn, GameController.Instance.currentDice + 1));
                            LeanTween.scale(pawn.gameObject, pawn.transform.localScale * 1.1f, 0.2f).setLoopPingPong();
                        }
                    }
                }
            }

            if (availablePawnCount > 0)
            {
                GameController.Instance.ChangeGameState(GameController.GameState.MOVE);

                // BOT logic (as before)
                if (isBot && PlayerPrefs.GetInt("IsOfflineMultiplayer") != 1)
                {
                    GameController.Instance.ChangeGameState(GameController.GameState.MOVING);
                    HighlightDices(false);
                    BotMovements rand = botMovements[Random.Range(0, botMovements.Count)];
                    rand.pawn.Move(rand.moveCount);
                    botMovements.Clear();
                    return;
                }

                //  AUTO MOVE for HUMAN  only when EXACTLY 1 move is possible
                if (botMovements.Count == 1 && GameController.Instance.currentPawnController == this)
                {
                    Debug.Log("Only one pawn can move -> Auto moving");

                    GameController.Instance.ChangeGameState(GameController.GameState.MOVING);
                    HighlightDices(false);

                    BotMovements onlyMove = botMovements[0];
                    onlyMove.pawn.Move(onlyMove.moveCount);
                    botMovements.Clear();
                    return;
                }

                // Else: 2 ya zyada options → player manually click karega
            }
            else
            {
                GameController.Instance.ChangePlayer();
            }
        }





        public void DisablePawn()
        {
            profileParent.SetActive(false);
            pawnParent.SetActive(false);
            gameObject.SetActive(false);
        }

        public void StartTimer(bool animation = false)
        {
            if (animation)
            {
                profileParent.LeanScale(Vector3.one * 1.05f, 0.6f).setLoopPingPong();
            }
            time = 10;
        }

        public void StopAnimation()
        {
            if (profileParent.LeanIsTweening())
            {
                profileParent.LeanCancel();
            }
            profileParent.transform.localScale = Vector3.one;
        }

        public void Play()
        {
            GameController.Instance.GameDiceBtn(pawnColor);
        }

        public void PlayDiceAnimation()
        {
            StartCoroutine(DiceAnimation());
        }

        IEnumerator DiceAnimation()
        {
            int rand = Random.Range(0, diceSprites.Length);
            int oldRand = 0;
            float t = 0;

            while (t < 0.45f)
            {
                t += Time.deltaTime;

                if (rand == oldRand)
                {
                    rand = Random.Range(0, diceSprites.Length);
                }

                profileDiceImg.sprite = diceSprites[rand];

                oldRand = rand;
                yield return null;
            }

            profileDiceImg.sprite = diceSprites[GameController.Instance.currentDice];

        }
    }
}