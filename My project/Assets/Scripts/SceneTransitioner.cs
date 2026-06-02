using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class SceneTransitioner : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("TutorialScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("OpeningScreen");
    }

    [YarnCommand("loadScene")]
    public static void LoadGameplay()
    {
        SceneManager.LoadScene("SampleScene");
    }

    [YarnCommand("loadEnding")]
    public static void LoadEnding()
    {
        SceneManager.LoadScene("ClosingScreen");
    }
}
