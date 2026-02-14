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
            PlayerPrefs.SetInt("joinByLink", 1); // Flag to indicate joining via deep link G
            PlayerPrefs.SetString("joinRoomName", roomCode); // Store the room code for later use G
            PlayerPrefs.Save();

            StartCoroutine(JoinFriendRoomAuto());
        }




//Pranav Changes: Commented out auto join logic for now, will revisit after testing link generation and manual joining works fine
        /*IEnumerator JoinFriendRoomAuto()
        {
            while (PhotonController.Instance == null)
                yield return null;

            PhotonController.Instance.Connect();

            while (!PhotonNetwork.IsConnectedAndReady)
                yield return null;



            PhotonNetwork.JoinRoom(roomCode);

            Debug.Log("Attempting to join room: " + PhotonNetwork.CurrentRoom.MaxPlayers);

            MenuController.Instance.LinkTextAnimation();
        }*/

//Gulrej Changes: Updated auto join logic to set flags and let PhotonController decide whether to join or create room based on flags

        IEnumerator JoinFriendRoomAuto()
{
    while (PhotonController.Instance == null)
        yield return null;

    // Flag set: batao ki hum JOIN kar rahe hain, CREATE nahi
    PlayerPrefs.SetInt("joinByLink", 1);
    PlayerPrefs.SetString("joinRoomName", roomCode);
    PlayerPrefs.Save();

    PhotonController.Instance.Connect();

    // Yahan direct JoinRoom mat karo
    // PhotonController.OnJoinedLobby() decide karega join ya create

    MenuController.Instance.LinkTextAnimation();
}

    }
}
