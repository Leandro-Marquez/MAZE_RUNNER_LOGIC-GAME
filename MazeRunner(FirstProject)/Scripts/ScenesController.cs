using System.Collections;
using System.Collections.Generic;
// using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesController : MonoBehaviour
{
    public void LoadNextScene()
    {
        SceneManager.LoadScene(2); //escena de seleccion de heroes
    }
    public static void LoadGameScene()
    {
        SceneManager.LoadScene(3); //escena del juego
    }
    public static void LoadPlayer1VictoryScene()
    {
        SceneManager.LoadScene(4); //escena de victoria del jugador 1
    }
    public static void LoadPlayer2VictoryScene()
    {
        SceneManager.LoadScene(5); //escena de viscotoria del jugador 2
    }
    public void ExitGame()// Esto cerrará el juego cuando se ejecute el ejecutable
    {
        Application.Quit();
    }
}
