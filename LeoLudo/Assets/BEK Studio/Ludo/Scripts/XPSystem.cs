using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BEKStudio;

public class XPSystem : MonoBehaviour
{
    public static XPSystem Instance;
    
    public int level = 1;
    public int currentXP = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadXP();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddXP(string result)
    {
        int xp = 5;
        if (result == "Win") xp += 10;
        else if (result == "Second") xp += 6;
        else if (result == "Third") xp += 3;

        currentXP += xp;

        while (currentXP >= GetMaxXPForLevel(level))
        {
            currentXP -= GetMaxXPForLevel(level);
            level++;
        }

        SaveXP();

        //  UI ko batao
        XPUIController.RefreshUI();
    }

    void SaveXP()
    {
        PlayerPrefs.SetInt("XP_Level", level);
        PlayerPrefs.SetInt("XP_Current", currentXP);
        PlayerPrefs.Save();

        MenuController.Instance.playerLevelText.text = level.ToString();
    }

    void LoadXP()
    {
        level = PlayerPrefs.GetInt("XP_Level", 1);
        currentXP = PlayerPrefs.GetInt("XP_Current", 0);
    }

    public int GetMaxXPForLevel(int level)
    {
        return level <= 10 ? 100 * level : 3300 + (level - 10) * 500;
    }
}