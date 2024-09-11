using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instancia {get; private set;}
    public GameObject Prefab;
    public GameObject Hand1;
    public GameObject Hand2;
    public GameObject RowM1,RowR1,RowS1;
    public GameObject RowM2,RowR2,RowS2;
    public GameObject ClimaM1,ClimaR1,ClimaS1;

    public GameObject ClimaM2,ClimaR2,ClimaS2;
    public GameObject Deck1;
    public GameObject Deck2;
    public GameObject Cementery1;
    public GameObject Cementery2;
    public GameObject Deck1Back;
    public GameObject Deck2Back;
    public GameObject lidersqr1;
    public GameObject lidersqr2;
    public List<Card> CardsPlayer1 = new List<Card>();
    public List<Card> CardsPlayer2 = new List<Card>();
    public Card LiderIlai;
    public Card LiderTork;

    public bool CurrentPlayer = false;
    public static int playedcard;

    void Awake()
    {
        if(Instancia is null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
        playedcard = 0;
    }
    void Start()
    {
        // agregar cartas a los deck de los jugadores
        if(CodeGenerator._cards.Count != 0)
        {
            for( int x = 0 ; x < CodeGenerator._cards.Count ; x+=2 )
            {
                CardsPlayer1.Add(CodeGenerator._cards[x]);
                CardsPlayer1.Add(CodeGenerator._cards[x+1]);
            }
        }
        context.Instance.Hands[1] = new CardList();
        context.Instance.Hands[2] = new CardList();
        context.Instance.Decks[1] = new CardList();
        context.Instance.Decks[2] = new CardList();
        context.Instance.Graveyards[1] = new CardList();
        context.Instance.Graveyards[2] = new CardList();
        context.Instance.Fields[1] = new CardList();
        context.Instance.Fields[2] = new CardList();

        PrepareGame(CardsPlayer1,Deck1);
        PrepareGame(CardsPlayer2,Deck2);
        InstanciarLideres();
        MixCards(Deck1.transform);
        MixCards(Deck2.transform);
        Stole(Deck1.transform,10,false);
        Stole(Deck2.transform,10,true);
        StarGame(CurrentPlayer);
        CurrentPlayer = false;

    }
    public void InstanciarLideres()
    {
        GameObject game = GameObject.Instantiate(Prefab, lidersqr1.transform);
        VisualCard Scriptable = game.GetComponent<VisualCard>();
        Scriptable.card = LiderIlai;
        Scriptable.InicializaCarta();
        lidersqr1.transform.GetChild(0).AddComponent<Lideres>();

        GameObject game1 = GameObject.Instantiate(Prefab, lidersqr2.transform);
        VisualCard Scriptable1 = game1.GetComponent<VisualCard>();
        Scriptable1.card = LiderTork;
        Scriptable1.InicializaCarta();
        lidersqr2.transform.GetChild(0).AddComponent<Lideres>();

    }
    public void PrepareGame(List<Card> PlayerCards , GameObject CustomDeck)
    {
        for(int item = 0 ; item < PlayerCards.Count ; item ++)
        {
            GameObject CardInstance = GameObject.Instantiate(Prefab,CustomDeck.transform);
            VisualCard Scriptable = CardInstance.GetComponent<VisualCard>();
            Scriptable.card = PlayerCards[item];
            Scriptable.InicializaCarta();
        }
    }
    public void MixCards(Transform Deck1)
    {
        Transform [] transforms = new Transform [Deck1.childCount];
        for(int x=0 ; x<transforms.Length ; x++)
        {
            transforms [x] = Deck1.GetChild(x);
        }
        System.Random Random= new System.Random();
        for(int x=0 ; x<transforms.Length-1 ; x++)
        {
            int n = Random.Next(0,transforms.Length-1);
            Transform Temporal = transforms[n];
            transforms[n] = transforms[x];
            transforms[x] = Temporal;
        }
        for(int x=0 ; x<transforms.Length ; x++)
        {
            transforms[x].SetParent(null);
            transforms[x].SetParent(Deck1);
        }
    }
    public void Stole(Transform Father, int n, bool playertostole)
    {
        Vector2 nuevaescala = new Vector2(1,1);
        
        if(!playertostole)
        {
            Transform [] hijos = new Transform[n];
            for(int x=0 ; x<n ; x++)
            {
                hijos[x] = Father.GetChild(x).transform;
                GameObject hijo = hijos[x].gameObject;
                hijo.SetActive(true);
                hijo.GetComponent<DragDrop>().enabled = true ;
            }
            for(int x=0 ; x<n ; x++)
            {
                hijos[x].SetParent(Hand1.transform);
                MoverObjeto(hijos[x] , Hand1.transform , nuevaescala);
            }
        }
        else
        {
            Transform [] hijos = new Transform[n];
            for(int x = 0 ; x < n ; x ++)
            {
                hijos[x] = Father.GetChild(x).transform;
                GameObject hijo = hijos[x].gameObject;
                hijo.SetActive(true);
                hijo.GetComponent<DragDrop>().enabled = true ;
            }
            for(int x = 0 ; x < n ; x ++)
            {
                hijos[x].SetParent(Hand2.transform);
                MoverObjeto(hijos[x] , Hand2.transform , nuevaescala);
            }
        }
        
    }
    void MoverObjeto(Transform objeto, Transform nuevaPosicion, Vector2 nuevaEscala)
    {
        objeto.transform.SetParent(nuevaPosicion);
        objeto.transform.localScale = nuevaEscala;
    }
    public void StarGame(bool CurrentPlayer)
    {
        playedcard = 0;
        if(!CurrentPlayer)
        {

            Hand2.SetActive(false);
            Deck2.SetActive(false);
            Deck2Back.SetActive(false);
            lidersqr2.SetActive(false);

            Hand1.SetActive(true);
            Deck1.SetActive(true);
            Deck1Back.SetActive(true);
            lidersqr1.SetActive(true);

            CurrentPlayer = true;
        }
        else 
        {
            Hand1.SetActive(false);
            Deck1.SetActive(false);
            Deck1Back.SetActive(false);
            lidersqr1.SetActive(false);

            Hand2.SetActive(true);
            Deck2.SetActive(true);
            Deck2Back.SetActive(true);
            lidersqr2.SetActive(true);

            CurrentPlayer = false;
        }
    }
    void Update()
    {
        Actualizacion.Actualizar();
    }
     public void ActualiceContext()
    {
        context.Instance.ActualiceContext(1, Hand1, Deck1, Cementery1, RowM1, RowR1, RowS1, ClimaM1, ClimaR1, ClimaS1);
        context.Instance.ActualiceContext(2, Hand2, Deck2, Cementery2, RowM2, RowR2, RowS2, ClimaM2, ClimaR2, ClimaS2);
    }

    public void ActualiceVisual()
    {
        context.Instance.ActualiceVisual(1,Hand1, Deck1, Cementery1, RowM1, RowR1, RowS1, ClimaM1, ClimaR1, ClimaS1, Prefab);
        context.Instance.ActualiceVisual(2, Hand2, Deck2, Cementery2, RowM2, RowR2, RowS2, ClimaM2, ClimaR2, ClimaS2, Prefab);
    }
}
