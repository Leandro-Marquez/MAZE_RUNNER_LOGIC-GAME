using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DataGame : MonoBehaviour
{
    public static DataGame Instance { get; private set; }

    public List<Card> DarkCards = new List<Card>();
    public List<Card> ElementalsCards = new List<Card>();
    public Card Tork;
    public Card Ilai;
    
    

    public List<Card> specialCards = new List<Card>();
    public List<Card> createdCards = new List<Card>();

    public List<Card> p1Cards = new List<Card>();
    public List<Card> p2Cards = new List<Card>();
    public Card p1Leader;
    public Card p2Leader;
    void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
}