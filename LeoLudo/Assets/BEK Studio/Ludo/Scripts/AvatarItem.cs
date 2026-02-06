using UnityEngine;

public class AvatarItem : MonoBehaviour
{
    public int avatarIndex;
    public GameObject ringImage;

    public void OnSelect()
    {
        // Sirf selection (temporary)
        AvatarSelectionManager.Instance.SelectTempAvatar(avatarIndex);
    }

    public void SetSelected(bool selected)
    {
        ringImage.SetActive(selected);
    }
}
