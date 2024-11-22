using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayersInfo : MonoBehaviour
{
    public Button acceptButton;
    public Button startButton;
    public TextMeshProUGUI selectedHero;
    public TMP_InputField player1;
    public TMP_InputField player2;
    public string player1Name;
    public string player2Name;
    public string player1Hero;
    public string player2Hero;

    public void AcceptButton()
    {
        player1Name = player1.text.ToString();
        player1Hero = selectedHero.text.ToString().Substring(5);
        player1.gameObject.SetActive(false);
        player2.gameObject.SetActive(true);
        acceptButton.gameObject.transform.parent.gameObject.SetActive(false);
        startButton.gameObject.transform.parent.gameObject.SetActive(true);
    }
    public void StartButton()
    {
        player2Name = player2.text.ToString();
        player2Hero = selectedHero.text.ToString().Substring(5);
        CreateGame();
    }
    public void CreateGame()
    {
        GameManager.player1Name = player1Name;
        GameManager.player1Hero = player1Hero;
        GameManager.player2Name = player2Name;
        GameManager.player2Hero = player2Hero;
        SceneManager.LoadScene(3);
    }
}
