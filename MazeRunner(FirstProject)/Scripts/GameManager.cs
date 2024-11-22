using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject hierba,bloque;
    public GameObject Tommy,Tereza,Sarten,Newt,Gally,Minho;
    public TextMeshProUGUI player1NameT,player1NameTs;
    public TextMeshProUGUI player2NameT,player2NameTs;
    public static string player1Name;
    public static string player2Name;
    public static string player1Hero;
    public static string player2Hero;
    public List<Hero> heros;
    public GameObject maze; //guardar el objeto padre de todos los objetos(Padre de la matriz)
    public static GameManager instancia;//tener una instancia estatica para poder tener acceso a ella desde cualquier script
    
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
    }
    //ejecutar en el inicio de la escena 
    void Start()
    {
        MazeGenerator.Starting();
        player1NameT.text = player1Name;
        player1NameTs.text = player1Name;
        player2NameT.text = player2Name;
        player2NameTs.text = player2Name;
        MazeGenerator.GenerateHeros();
        // Debug.Log(player1Name + " " + player1Hero);
        // Debug.Log(player2Name + " " + player2Hero);
    }
    
}
