using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainEngine : MonoBehaviour
{
    public static int Level;

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

    public void PlayLevel(int level)
    {
        MainEngine.Level = level;
        SceneManager.LoadScene("PlayALevel");
    }
}
