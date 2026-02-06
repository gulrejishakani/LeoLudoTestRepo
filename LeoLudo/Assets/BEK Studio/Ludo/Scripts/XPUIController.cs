using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XPUIController : MonoBehaviour
{
    public static XPUIController Instance;

    public Scrollbar scrollbar;
    public TMP_Text xpText;
    public TMP_Text levelText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        RefreshUI();
    }

    public static void RefreshUI()
    {
        if (Instance == null || XPSystem.Instance == null) return;

        int level = XPSystem.Instance.level;
        int currentXP = XPSystem.Instance.currentXP;
        int maxXP = XPSystem.Instance.GetMaxXPForLevel(level);

        //Instance.scrollbar.value = (float)currentXP / maxXP;
        Instance.scrollbar.size = (float)currentXP / maxXP;
        Instance.xpText.text = currentXP + "/" + maxXP;
        Instance.levelText.text = "Level " + level;
    }
}