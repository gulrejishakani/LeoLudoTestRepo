using UnityEngine;
using System.Collections.Generic;

public class AvatarSelectionManager : MonoBehaviour
{
    public static AvatarSelectionManager Instance;

    public List<AvatarItem> avatarItems;
    private int tempSelectedIndex = -1;

    void Awake()
    {
        Instance = this;
    }

    public void SelectTempAvatar(int index)
    {
        tempSelectedIndex = index;

        foreach (var item in avatarItems)
        {
            item.SetSelected(item.avatarIndex == index);
        }
    }

    //  SAVE BUTTON 
    public void SaveAvatar()
    {
        if (tempSelectedIndex < 0) return;

        PlayerPrefs.SetInt("avatar", tempSelectedIndex);
        PlayerPrefs.Save();

        // 3 jagah avatar refresh
        FindObjectOfType<OnClickProfileButton>()?.RefreshAvatarFromServer();

        // Close avatar selection panel
        gameObject.SetActive(false);
    }
}
