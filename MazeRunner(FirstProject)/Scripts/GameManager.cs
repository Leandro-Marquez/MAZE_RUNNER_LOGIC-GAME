using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //guardar los prefabricados en la escena para su intanciacion desde codigo 
    public GameObject wallPrefab,floorPrefab,teleportPrefab,heroPrefab;//... prefabricados principales
    public GameObject doorPrefab,teleporPrefab,chestPrefab,keyPrefab,deadPrefab;//...prefabricados especiales
    public GameObject trapPrefab; // prefabricado principal de las trampas
    public TextMeshProUGUI player1NameT,player1NameTs; //nombre del jugador 1, texto y sombra
    public TextMeshProUGUI player2NameT,player2NameTs; //nombre del jugador 2, texto y sombra
    public TextMeshProUGUI player1Energy,player1Energys; //energia del jugador 1, texto y sombra
    public TextMeshProUGUI player2Energy,player2Energys; //energia del jugador 1, texto y sombra
    public static Sprite clikedObjectFija; // imagen de objeto clickeado fija para cuando se pase de turno
    public static string player1Name; //nombre del player 1 a montar en la escena 
    public static string player2Name; //nombre del player 2 a montar en la escena 
    public static List<string> player1Heros; //lista de heroes del player 1 
    public static List<string> player2Heros; //lista de heores del player 2
    public List<Trap> traps; //guardar los scriptables para su puesta en escena(trampas)
    public List<Hero> heros; //guardar los scriptables para su puesta en escena(heroes)
    public GameObject maze; //guardar el objeto padre de todos los objetos(Padre de la matriz)
    public UnityEngine.UI.Image currentPlayer1Image; //para mayor legibilidad a la hora de saber a quien le toca jugar 
    public UnityEngine.UI.Image currentPlayer2Image; // ...
    public UnityEngine.UI.Image auxImageForDestruction; // imagen auxiliar 
    public GameObject sueloAuxForMinho; // prefabricado auxiliar de suelo para cuando Minho destruya la tierra
    public Button applyEffectPlayer1; //boton de aplicar el efecto del jugador 1
    public Button applyEffectPlayer2; //boton de aplicar el efecto del jugador 2
    public GameObject currentObjectClickedForMinhoEffect;//guardar el objeto clickeado para el efecto de minho
    public GameObject clickedHero; //objeto clickeado en la escena por si se activa el efecto
    public static GameManager instancia;//tener una instancia estatica para poder tener acceso a la clase desde cualquier script
    public bool currentPlayer;//valor booleano para representar los juadore(false para player 1) y (true para player 2)
    public List<GameObject> herosPlayer1; //rellenar una vez instanciados los heroes en la escena para el sistema de turnos
    public List<GameObject> herosPlayer2; //rellenar una vez instanciados los heroes en la escena para el sistema de turnos
    public AudioSource tomySound;//guardar el audio source de tomy cuando aplique su habilidad 
    public AudioSource minhoSound;//guardar el audio source de minho cuando aplique su habilidad 
    public AudioSource colectedSound;//guardar el audio source de objeto coleccionado para cuando se coleccione algo 
    public static int tommyenfriando; //entero para controlar el tiempo que lleva enfriandose el heroe
    public static int gallyEnfriando; // ...
    public static int terezaEnfriando; // ... 
    public static int sartenEnfriando; //...
    public static int minhoEnfriando; //...
    public static int newtEnfriando; //...

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
        herosPlayer1 = new List<GameObject>();//inicializar las listas para evitar errores de referencia
        herosPlayer2 = new List<GameObject>();//...
    }
   
    void Start()  //ejecutar en el inicio de la escena 
    {
        clikedObjectFija = null;
        currentPlayer = false; //inicia el primer jugador
        currentObjectClickedForMinhoEffect = null;
        MazeGenerator.Starting();//inicializar el laberinto una vez se cargue la escena
        player1NameT.text = player1Name;//llevar el nombre del player 1 a la escena
        player1NameTs.text = player1Name;//..sombra
        player2NameT.text = player2Name;//llevar el nombre del player 2 a la escena 
        player2NameTs.text = player2Name;//..sombra

        MazeGenerator.GenerateHeros();//inicializar los heroes correspondientes a cada jugador 
        MazeGenerator.GenerateTeleports(0,1);//inicializar los teletransportadores al incio y final de laberinto 
        MazeGenerator.GenerateTeleports(16,17);//inicializar los teletransportadores al incio y final de laberinto 
        ObtainHeros(); //guardar los heroes en sus listas correspondientes para el sistema de turnos 
        PrepareGame(); //prepar el laberinto para el jugador 1
        
        MazeGenerator.PrepareTraps(herosPlayer1.Count*12); //instanciar las trampas de manera random en el laberinto 
        
        //iniciar los valores de enfriamiento de los respectivos heroes
        gallyEnfriando = 0;
        tommyenfriando = 0;
        newtEnfriando = 0;
        terezaEnfriando = 0;
        minhoEnfriando = 0;
        sartenEnfriando = 0;
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
        Effects.RestTime();//restar el tiempo de enfriamiento de las habilidades de los heroes 
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
            currentPlayer1Image.enabled = true; //activar el indicador de luz verde del jugador 1
            applyEffectPlayer1.gameObject.SetActive(true);//activar el boton de aplicar efecto del jugador 1
            currentPlayer2Image.enabled = false;//desactivar el indicador de luz verde del jugador 2
            applyEffectPlayer2.gameObject.SetActive(false);//descativar el boton de aplicar efecto del jugador 2
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
            currentPlayer1Image.enabled = false;//desactivar el indicador de luz verde del jugador 1
            applyEffectPlayer1.gameObject.SetActive(false);//desactivar el boton de aplicar efecto del juador 1
            currentPlayer2Image.enabled = true;//activar el indicador de luz verde del jugador 2
            applyEffectPlayer2.gameObject.SetActive(true);//activar el boton de aplicar efecto del jugador 2
        }
        NPCMove.n = 0;//restablecer el valor a cero para que el otro jugador tambien se pueda mover 
        if(NPCMove.clikedObjectImage is null) return; //evitar errores de referencia con la imagen de clicked objet de la escena
        NPCMove.clikedObjectImage.sprite = GameManager.clikedObjectFija; //cambiar la imagen a la imagen por default
    }
}