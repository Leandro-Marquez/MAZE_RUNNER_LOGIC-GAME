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
    public Button acceptButton; //guardar el boton de aceptar para apagarlo 
    public Button startButton; //guardar el boton de iniciar para encenderlo
    public TextMeshProUGUI selectedHero; //texto en la escena del heroe seleccionado
    public TMP_InputField player1; //entrada del nombre del player 1
    public TMP_InputField player2; //entrada del nombre del player 2
    public string player1Name; //guardar el nombre del player 1
    public string player2Name; //guardar el nombre del player 2
    public List<string> player1Heros; //lista de heroes con los que jugara el jugador 1
    public List<string> player2Heros; //lista de heroes con los que jugara el jugador 2
    public int currentPlayer; //entero para saber quien es el jugador que esta seleccionando heroes
    public void Start() //inicializar las listas y el current player 
    {
        player1Heros = new List<string>(); //inicializar la lista de heroes del jugador 1
        player1Heros = new List<string>(); //inicializar la lista de heroes del jugador 2
        currentPlayer = 1; //actualizar el jugador actual con el primero 
    }
    public void AddButton() //agregar el heroe seleccionada al player correspondiente
    {
        string aux = selectedHero.text.ToString();
        string limpio = aux.Trim();
        if(limpio.Length == 6) return;
        //verificar q no contenga el mismo heroe y se le agrega al correspondiente jugador 
        if(currentPlayer == 1 && !player1Heros.Contains(selectedHero.text.ToString().Substring(5)) && !player2Heros.Contains(selectedHero.text.ToString().Substring(5)) && player1Heros.Count < 3)
        {
            player1Heros.Add(selectedHero.text.ToString().Substring(5));//agregar el heroe seleccionado a su lista correspondiente
            string name = selectedHero.text.ToString().Substring(6);
            string clearname = name.Trim();
            GameObject.Find(clearname).SetActive(false);
        }
        //verificar que aun se le puedan agregar heroes al segundo jugador a partir de la primera cantidad de jugadores instanciados 
        else if(currentPlayer == 2 && player2Heros.Count < player1Heros.Count && !player2Heros.Contains(selectedHero.text.ToString().Substring(5)) && !player1Heros.Contains(selectedHero.text.ToString().Substring(5)) && player2Heros.Count < 3) 
        {
            player2Heros.Add(selectedHero.text.ToString().Substring(5));//agregar el heore seleccionador a su lista correspondiente
            string name = selectedHero.text.ToString().Substring(6);
            string clearname = name.Trim();
            GameObject.Find(clearname).SetActive(false);
        }
        
    }
    public void AcceptButton() //on accept button is clicked 
    {
        player1Name = player1.text.ToString(); //guardar el valor del jugador 1 
        player1.gameObject.SetActive(false); //apagar la entrada de texto de jugador 1
        player2.gameObject.SetActive(true); //enceder la entrada de texto del jugador 2
        acceptButton.gameObject.transform.parent.gameObject.SetActive(false); //apagar el boton de aceptar
        startButton.gameObject.transform.parent.gameObject.SetActive(true); //encender el boton de iniciar el juego 
        currentPlayer = 2; //actualizar el current player a player 2
    }
    public void StartButton() //on start button is clicked 
    {
        if(player1Heros.Count == player2Heros.Count) //verificar que ambos jugadores tengan la misma cantidad de heroes 
        {
            player2Name = player2.text.ToString(); //guardar el nombre del jugador 2 
            CreateGame(); //llevar las cosas a la siguiente escena
        }
    }
    public void CreateGame() //preparar la siguiente escena en visperas de lo seleccionado 
    {
        GameManager.player1Name = player1Name;//.llevar el nombre de player 1 al game manager para la siguiente escena
        GameManager.player2Name = player2Name;//.llevar el nombre de player 2 al game manager para la siguiente escena
        GameManager.player1Heros = this.player1Heros;//llevar los heroes del player 1 al GameManager para su posterior instanciacion 
        GameManager.player2Heros = this.player2Heros;//llevar los heroes del player 2 al GameManager para su posterior instanciacion 
        ScenesController.LoadGameScene();//cargar la escena del juego desde el controlador de escenas 
    }
}