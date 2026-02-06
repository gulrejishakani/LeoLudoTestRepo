using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

namespace BEKStudio{
    public class PhotonController : MonoBehaviourPunCallbacks {
        public static PhotonController Instance;
        public int roomEntryPice = 0;
        public int botAvatar;
        public string botName;


        void Awake() {
            if (Instance == null) {
                Instance = this;
            }
        }

        public string gameMode() {
            return PlayerPrefs.GetString("mode");
        }

        public int gameEntryPrice() {
            if (gameMode() == "online") {
                return Constants.ONLINE_ENTRY_PRICE;
            } else {
                return Constants.COMPUTER_ENTRY_PRICE;
            }
        }

        public void Connect() {
            MenuController.Instance.OnlineInfoMsg("Connecting to server...");

            PhotonNetwork.KeepAliveInBackground = 60;
            if (!PhotonNetwork.IsConnectedAndReady) {
                PhotonNetwork.ConnectUsingSettings();
            } else if (PhotonNetwork.InLobby) {
                FindRoom();
            } else {
                JoinLobby();
            }
        }

        public override void OnConnectedToMaster() {
            if (PlayerPrefs.HasKey("username")){
                PhotonNetwork.NickName = PlayerPrefs.GetString("username");
            }

            if (SceneManager.GetActiveScene().name == "Menu") {
                if (!MenuController.Instance.onlineCancelButton.interactable) {
                    PhotonNetwork.Disconnect();
                    return;
                }
            }
            
            JoinLobby();
        }

        public void JoinLobby() {
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby(){
            FindRoom();
        }

        public override void OnLeftLobby(){
            
        }

        public void FindRoom() {
            MenuController.Instance.OnlineInfoMsg("Searching room...");

            int count = PlayerPrefs.GetInt("playerCount");
            ExitGames.Client.Photon.Hashtable roomHastable = new ExitGames.Client.Photon.Hashtable {
                { "playerCount", count }
            };

            PhotonNetwork.JoinRandomRoom(roomHastable, 0);
        }

        public override void OnJoinRandomFailed(short returnCode, string message) {
            int count = PlayerPrefs.GetInt("playerCount");
            ExitGames.Client.Photon.Hashtable roomHastable = new ExitGames.Client.Photon.Hashtable {
                { "playerCount",  count}
            };

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.CustomRoomProperties = roomHastable;
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "playerCount" };
            roomOptions.IsOpen = true;
            roomOptions.MaxPlayers = PlayerPrefs.GetInt("playerCount");
            roomOptions.IsVisible = true;
            PhotonNetwork.CreateRoom(null, roomOptions);
        }

        public override void OnJoinedRoom() {
            PhotonNetwork.AutomaticallySyncScene = true;

            ExitGames.Client.Photon.Hashtable userHastable = new ExitGames.Client.Photon.Hashtable();
            userHastable.Add("avatar", PlayerPrefs.GetInt("avatar"));
            userHastable.Add("colorID", getMyOrder());
            PhotonNetwork.SetPlayerCustomProperties(userHastable);

            MenuController.Instance.OnlineInfoMsg("Joined Room \n" + PhotonNetwork.PlayerList.Length + "/" + (int)PhotonNetwork.CurrentRoom.CustomProperties["playerCount"]);
            if (PhotonNetwork.PlayerList.Length == (int)PhotonNetwork.CurrentRoom.CustomProperties["playerCount"]) {
                MenuController.Instance.OnlineInfoMsg("Starting...");

                if (PhotonNetwork.IsMasterClient) {
                    PhotonNetwork.CurrentRoom.IsVisible = false;
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                    StartCoroutine(DelayForCustomProperties());
                }
            }
        }

        IEnumerator DelayForCustomProperties() {
            MenuController.Instance.OnlineInfoMsg("Scene loading...");
            yield return new WaitForSeconds(1.5f);
            PhotonNetwork.LoadLevel("Game");
        }

        public override void OnLeftRoom() {
            PhotonNetwork.AutomaticallySyncScene = false;

            if (SceneManager.GetActiveScene().name == "Game") {
                SceneManager.LoadScene("Menu");
            }
        }

        int getMyOrder() {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) {
                if (PhotonNetwork.PlayerList[i] == PhotonNetwork.LocalPlayer) {
                    return i;
                }
            }
            return -1;
        }

        public override void OnCreatedRoom() {
            
        }

        public override void OnPlayerEnteredRoom(Player newPlayer) {
            MenuController.Instance.OnlineInfoMsg("Joined Room \n" + PhotonNetwork.PlayerList.Length + "/" + PlayerPrefs.GetInt("playerCount"));

            if (PhotonNetwork.PlayerList.Length == (int)PhotonNetwork.CurrentRoom.CustomProperties["playerCount"]) {
                MenuController.Instance.OnlineInfoMsg("Starting...");

                if (PhotonNetwork.IsMasterClient) {
                    StartCoroutine(DelayForCustomProperties());
                }
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer) {
            if (SceneManager.GetActiveScene().name == "Menu") {
                MenuController.Instance.OnlineInfoMsg("Joined Room \n" + PhotonNetwork.PlayerList.Length + "/" + PlayerPrefs.GetInt("playerCount"));

                ExitGames.Client.Photon.Hashtable userHastable = new ExitGames.Client.Photon.Hashtable();
                userHastable.Add("avatar", PlayerPrefs.GetInt("avatar"));
                userHastable.Add("colorID", getMyOrder());
                PhotonNetwork.SetPlayerCustomProperties(userHastable);
            } else if (SceneManager.GetActiveScene().name == "Game") {
                GameController.Instance.CheckRoomPlayers(otherPlayer);
            }
        }

        public override void OnMasterClientSwitched(Player newMasterClient){
            if (SceneManager.GetActiveScene().name == "Game") {
                GameController.Instance.MasterClientChanged();
            }
        }

        public override void OnDisconnected(DisconnectCause cause) {
            if (SceneManager.GetActiveScene().name == "Menu") {
                MenuController.Instance.OnlineClose();
            }
        }

    }
}