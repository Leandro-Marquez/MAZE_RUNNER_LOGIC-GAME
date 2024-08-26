using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuGame : MonoBehaviour
{
    public void OnStartGameClikedButton()
    {
        SceneManager.LoadScene(1);
    }

    public void OnCreateCardClikedButton()
    {
        SceneManager.LoadScene(2);
    }

    public void OnExitClikedButton()
    {
        Application.Quit();
    }
}
