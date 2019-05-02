using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainEngine : MonoBehaviour
{
    Dropdown ddLevel;

    public void Start()
    {
        ddLevel = GameObject.Find("ddLevel").GetComponent<Dropdown>();
    }

    public void Play()
    {
        int level = ddLevel.value;
        PlayLevel(level);
    }

    public static void PlayLevel(int level)
    {
        LevelInfo.SetLevelData(level);
        SceneManager.LoadScene("PlayALevel");
    }

}

public static class LevelInfo
{
    public static int Level;
    public static string[] Squares, Facings; // for the level that will be played

    public static void SetLevelData(int level)
    {
        LevelInfo.Level = level;

        if (level == 1)
        {
            Squares = new string[]
            {
            "000111000",
            "401000011",
            "000000004",
            "000000000",
            "000000000",
            "300000001",
            "000011021",
            "000015000",
            "400010140"
            };
            Facings = new string[]
            {
            "000000000",
            "300000000",
            "000000004",
            "000000000",
            "000000000",
            "300000000",
            "000000000",
            "000000000",
            "300000010"
            };
        }
        else // default: level 0
        {
            Squares = new string[]
            {
            "000000005",
            "000000000",
            "000000010",
            "000000100",
            "000000004",
            "401000000",
            "000000000",
            "000020000",
            "000030000"
            };
            Facings = new string[]
            {
            "000000000",
            "000000000",
            "000000000",
            "000000000",
            "000000002",
            "300000000",
            "000000000",
            "000000000",
            "000010000"
            };
        }
    }
}
