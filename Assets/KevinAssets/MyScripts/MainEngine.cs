using UnityEngine;
using UnityEngine.SceneManagement;

public class MainEngine : MonoBehaviour
{
    public static int Level;

    public void PlayLevel(int level)
    {
        MainEngine.Level = level;
        SceneManager.LoadScene("PlayALevel");
    }
}
