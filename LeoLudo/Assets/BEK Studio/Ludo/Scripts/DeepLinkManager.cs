using UnityEngine;
using Photon.Pun;
using System.Collections;

namespace BEKStudio
{
    public class DeepLinkManager : MonoBehaviour
    {
        string roomCode;
        int playerCount = 2; // default fallback

        void Awake()
        {
            DontDestroyOnLoad(gameObject);

            Application.deepLinkActivated += OnDeepLinkActivated;

            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                OnDeepLinkActivated(Application.absoluteURL);
            }
        }

        void OnDeepLinkActivated(string url)
        {
            Debug.Log("Deep Link Received: " + url);

            System.Uri uri = new System.Uri(url);
            string query = uri.Query;   // ?room=ROOM123&count=4

            if (string.IsNullOrEmpty(query))
                return;

            // Remove "?"
            query = query.TrimStart('?');

            string[] parameters = query.Split('&');

            foreach (string param in parameters)
            {
                string[] keyValue = param.Split('=');

                if (keyValue.Length != 2)
                    continue;

                if (keyValue[0] == "room")
                {
                    roomCode = keyValue[1];
                }
                else if (keyValue[0] == "count")
                {
                    int.TryParse(keyValue[1], out playerCount);
                }
            }

            if (string.IsNullOrEmpty(roomCode))
                return;

            PlayerPrefs.SetString("mode", "friend");
            PlayerPrefs.SetInt("playerCount", playerCount);
            PlayerPrefs.Save();

            StartCoroutine(JoinFriendRoomAuto());
        }

        IEnumerator JoinFriendRoomAuto()
        {
            while (PhotonController.Instance == null)
                yield return null;

            PhotonController.Instance.Connect();

            while (!PhotonNetwork.IsConnectedAndReady)
                yield return null;



            PhotonNetwork.JoinRoom(roomCode);

            Debug.Log("Attempting to join room: " + PhotonNetwork.CurrentRoom.MaxPlayers);

            MenuController.Instance.LinkTextAnimation();
        }
    }
}
