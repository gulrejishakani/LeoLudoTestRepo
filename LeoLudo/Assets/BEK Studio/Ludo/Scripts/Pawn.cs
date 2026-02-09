using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace BEKStudio
{
    public class Pawn : MonoBehaviourPun, IPunObservable
    {
        public PawnController pawnController;
        public int firstWayID;
        public bool inBase;
        public bool inColorWay;
        public bool isProtected;
        public bool isCollected;
        public int currentWayID;
        public int moveCount;
        Vector2 startScale;
        Vector2 startPosition;


        void Start()
        {
            inBase = true;
            startScale = transform.localScale;
            startPosition = transform.position;
        }

        public void SetScaleToDefault()
        {
            transform.localScale = startScale;
        }


        public void Move(int count)
        {
            if (isCollected) return;
            if (!GameController.Instance.isLocal && !photonView.IsMine) return;

            if (inBase)
            {
                inBase = false;
                isProtected = true;
                currentWayID = firstWayID;
                LeanTween.move(gameObject, GameController.Instance.waypointParent.GetChild(firstWayID).position, 0.3f).setOnComplete(() =>
                {
                    AudioController.Instance.PlayPawnMoveSound();
                    GameController.Instance.CheckGameStatus();
                    GameController.Instance.CheckGameStatus();
                    GameController.Instance.UpdateStackedPawns(currentWayID); //  ADD THIS
                });
                return;
            }

            StartCoroutine(MoveCoroutine(moveCount + count));
        }

        IEnumerator MoveCoroutine(int totalCount)
        {
            if (photonView.IsMine || GameController.Instance.isLocal)
            {
                bool canMove = false;
                while (moveCount != totalCount)
                {
                    if (!canMove)
                    {
                        canMove = true;
                        currentWayID = (currentWayID + 1) % GameController.Instance.waypointParent.childCount;
                        Debug.Log($"11111111111     StartPosition : {startPosition},    firstWayID : {firstWayID},  currentWayID : {currentWayID},   Movement : {moveCount},    TotalCount : {totalCount}, ");
                        if (moveCount < 50)
                        {
                            LeanTween.move(gameObject, GameController.Instance.waypointParent.GetChild(currentWayID).position, 0.1f).setDelay(0.05f).setOnComplete(() =>
                            {
                                AudioController.Instance.PlayPawnMoveSound();
                                moveCount++;
                                canMove = false;
                            });
                        }
                        else
                        {
                            //QUICK MODE RULE: must kill before entering color way
                            if (GameController.Instance.mustKillToWin && !pawnController.hasKilledOpponent)
                            {
                                //Continue looping on normal path
                                LeanTween.move(gameObject, GameController.Instance.waypointParent.GetChild(currentWayID).position, 0.1f).setDelay(0.05f).setOnComplete(() =>
                                {
                                    AudioController.Instance.PlayPawnMoveSound();
                                    moveCount++;
                                    canMove = false;
                                });
                                Debug.Log($"2222222222     StartPosition : {startPosition},    firstWayID : {firstWayID},  currentWayID : {currentWayID},   Movement : {moveCount},    TotalCount : {totalCount}, ");
                                yield return null;
                                continue;
                            }

                            inColorWay = true;

                            string[] parseName = gameObject.name.Split("-");
                            Transform colorWay = GameController.Instance.colorWayParent.Find(parseName[0]);
                            LeanTween.move(gameObject, colorWay.GetChild(moveCount - 50).position, 0.1f).setDelay(0.05f).setOnComplete(() =>
                            {
                                AudioController.Instance.PlayPawnMoveSound();
                                moveCount++;
                                canMove = false;
                                if (moveCount == 56)
                                {
                                    isCollected = true;
                                    GetComponent<CircleCollider2D>().enabled = false;
                                    //  EXTRA TURN when a pawn reaches home
                                    pawnController.canPlayAgain = true;
                                }
                            });
                            Debug.Log($"33333333     StartPosition : {startPosition},    firstWayID : {firstWayID},  currentWayID : {currentWayID},   Movement : {moveCount},    TotalCount : {totalCount}, ");
                        }
                    }

                    yield return null;
                }

                if (currentWayID == 2 || currentWayID == 10 || currentWayID == 15 || currentWayID == 23 || currentWayID == 28 || currentWayID == 36 || currentWayID == 41 || currentWayID == 49)
                {
                    isProtected = true;
                }
                else
                {
                    isProtected = false;
                }

                GameController.Instance.CheckGameStatus();
            }
        }

        public void ReturnToBase()
        {
            StartCoroutine(ReturnToBaseCoroutine());
        }

        /* IEnumerator ReturnToBaseCoroutine() {
             bool canMove = false;
             while (!inBase) {
                 if (!canMove) {
                     canMove = true;

                     if (currentWayID > firstWayID) {
                         currentWayID = (currentWayID - 1) % GameController.Instance.waypointParent.childCount;
                         LeanTween.move(gameObject, GameController.Instance.waypointParent.GetChild(currentWayID).position, 0.05f).setDelay(0.025f).setOnComplete(() => {
                             canMove = false;
                         });
                     } else if (currentWayID < firstWayID) {
                         currentWayID = (currentWayID + 1) % GameController.Instance.waypointParent.childCount;
                         LeanTween.move(gameObject, GameController.Instance.waypointParent.GetChild(currentWayID).position, 0.05f).setDelay(0.025f).setOnComplete(() => {
                             canMove = false;
                         });
                     } else {
                         LeanTween.move(gameObject, startPosition, 0.05f).setDelay(0.025f).setOnComplete(() => {
                             canMove = false;
                             inBase = true;
                             moveCount = 0;
                             GameController.Instance.CheckForFinish();
                         });
                     }
                 }

                 yield return null;
             }

         }*/

        IEnumerator ReturnToBaseCoroutine()
        {
            bool canMove = false;

            while (!inBase)
            {
                if (!canMove)
                {
                    canMove = true;

                    // Always move BACKWARD on main track
                    currentWayID--;

                    if (currentWayID < 0)
                        currentWayID = GameController.Instance.waypointParent.childCount - 1;

                    // Check if reached entry point
                    if (currentWayID == firstWayID)
                    {
                        // Go to base position
                        LeanTween.move(gameObject, startPosition, 0.05f)
                            .setDelay(0.025f)
                            .setOnComplete(() =>
                            {
                                inBase = true;
                                moveCount = 0;
                                inColorWay = false;
                                isProtected = false;
                                canMove = false;
                                Debug.Log("Return to base.");
                                GameController.Instance.CheckForFinish();
                            });
                    }
                    else
                    {
                        // Move step by step backward
                        LeanTween.move(
                            gameObject,
                            GameController.Instance.waypointParent.GetChild(currentWayID).position,
                            0.05f
                        ).setDelay(0.025f).setOnComplete(() =>
                        {
                            canMove = false;
                        });
                    }
                }

                yield return null;
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(inBase);
                stream.SendNext(inColorWay);
                stream.SendNext(isProtected);
                stream.SendNext(isCollected);
                stream.SendNext(currentWayID);
                stream.SendNext(moveCount);
            }
            else
            {
                inBase = (bool)stream.ReceiveNext();
                inColorWay = (bool)stream.ReceiveNext();
                isProtected = (bool)stream.ReceiveNext();
                isCollected = (bool)stream.ReceiveNext();
                currentWayID = (int)stream.ReceiveNext();
                moveCount = (int)stream.ReceiveNext();
            }
        }
    }
}