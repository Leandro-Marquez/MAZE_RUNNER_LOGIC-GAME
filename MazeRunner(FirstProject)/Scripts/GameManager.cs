using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject wallPrefab,floorPrefab,teleportPrefab,heroPrefab;//guardar los prefabricados en la escena para su intanciacion desde codigo 
    public TextMeshProUGUI player1NameT,player1NameTs; //nombre del jugador 1, texto y sombra
    public TextMeshProUGUI player2NameT,player2NameTs; //nombre del jugador 2, texto y sombra
    public static string player1Name; //nombre del player 1 a montar en la escena 
    public static string player2Name; //nombre del player 2 a montar en la escena 
    public static List<string> player1Heros; //lista de heroes del player 1 
    public static List<string> player2Heros; //lista de heores del player 2
    public List<Hero> heros; //guardar los scriptables para su puesta en escena
    public GameObject maze; //guardar el objeto padre de todos los objetos(Padre de la matriz)
    public UnityEngine.UI.Image currentPlayer1Image; //para mayor legibilidad a la hora de saber a quien le toca jugar 
    public UnityEngine.UI.Image currentPlayer2Image; // ...
    public static GameManager instancia;//tener una instancia estatica para poder tener acceso a la clase desde cualquier script
    public bool currentPlayer;//valor booleano para representar los juadore(false para player 1) y (true para player 2)
    public List<GameObject> herosPlayer1; //rellenar una vez instanciados los heroes en la escena para el sistema de turnos
    public List<GameObject> herosPlayer2; //rellenar una vez instanciados los heroes en la escena para el sistema de turnos
    
    //ejecutar antes de cualquier frame en el juego 
    private void Awake()
    {
        // Verificar si ya existe una instancia
        if (instancia == null)
        {
            instancia = this; // Asignar la instancia
            DontDestroyOnLoad(gameObject); //no destruir en nuevas escenas
        }
        else
        {
            Destroy(gameObject); // Destruir el duplicado
        }
        herosPlayer1 = new List<GameObject>();
        herosPlayer2 = new List<GameObject>();
    }
    //ejecutar en el inicio de la escena 
    void Start()
    {
        currentPlayer = false; //inicia el primer jugador
        MazeGenerator.Starting();//inicializar el laberinto una vez se cargue la escena
        player1NameT.text = player1Name;//llevar el nombre del player 1 a la escena
        player1NameTs.text = player1Name;//..sombra
        player2NameT.text = player2Name;//llevar el nombre del player 2 a la escena 
        player2NameTs.text = player2Name;//..sombra

        MazeGenerator.GenerateHeros();//inicializar los heroes correspondientes a cada jugador 
        MazeGenerator.GenerateTeleports(0,1);//inicializar los teletransportadores al incio y final de laberinto 
        MazeGenerator.GenerateTeleports(16,17);//inicializar los teletransportadores al incio y final de laberinto 
        ObtainHeros(); //guardar los heroes en sus listas correspondientes para el sistema de turnos 
        PrepareGame(); //prepar el primer turno para el jugador 1
    
    }
    private void ObtainHeros() //rellenar los objetos instanciados en la escena en el primer momento
    {
        for (int i = 1; i < 18 ; i++) //iterar por la fila 1
        {
            if(GameManager.instancia.maze.transform.GetChild(1).GetChild(i).childCount > 1) //obtener los objetos donde se instanciaron los clones
            {
                //agregar los objetos a la respectiva lista
                herosPlayer1.Add(GameManager.instancia.maze.transform.GetChild(1).GetChild(i).GetChild(1).gameObject);
            }
        }
        for (int i = 1; i < 18 ; i++) //iterar por la fila 15
        {
            if(GameManager.instancia.maze.transform.GetChild(15).GetChild(i).childCount > 1) //obtener los objetos donde se instanciaron los clones
            {
                //agregar los objetos a la respectiva lista
                herosPlayer2.Add(GameManager.instancia.maze.transform.GetChild(15).GetChild(i).GetChild(1).gameObject);
            }
        }
    }
    public void PrepareGame() //preparar el sistema de turnos al inicio del juego 
    {
        if(!currentPlayer) //el caso de que le toca al jugador 1
        {
            for (int i = 0; i < herosPlayer2.Count ; i++) //desactivar la propiedad NPCMove de los objetos del jugador 2
            {
                herosPlayer2[i].GetComponent<NPCMove>().enabled = false;
            }
            for (int i = 0; i < herosPlayer1.Count ; i++) //activar la propiedad NPCMove de los objetos del jugador 1
            {
                herosPlayer1[i].GetComponent<NPCMove>().enabled = true;
            }
            NPCMove.seMovio = false; //restablecer el valor de movimiento para que cada jugador se pueda seguir moviendo
            currentPlayer1Image.enabled = true;
            currentPlayer2Image.enabled = false;
        }
        else //el caso de que le toca al jugador 2
        {
            for (int i = 0; i < herosPlayer1.Count ; i++) //desactivar la propiedad NPCMove de los objetos del jugador 1
            {
                herosPlayer1[i].GetComponent<NPCMove>().enabled = false;
            }
            for (int i = 0; i < herosPlayer2.Count ; i++) //activar la propiedad NPCMove de los objetos del jugador 2
            {
                herosPlayer2[i].GetComponent<NPCMove>().enabled = true;
            }
            NPCMove.seMovio = false; //restablecer el valor de movimiento para que cada jugador se pueda seguir moviendo
            currentPlayer1Image.enabled = false;
            currentPlayer2Image.enabled = true;
        }
    }
}
