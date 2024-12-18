using System.Collections;
using System.Collections.Generic;
// using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopManager : MonoBehaviour , IPointerDownHandler
{
    public GameObject shopShop; //guardar el objeto que contiene todo sobre la tienda
    public static GameObject clickedObject; //obtener el objeto clickeado en la tienda;
    public List<GameObject> luces; //lista de objetos para apagar las luces que de los objetos que no se hayan seleccionado
    public static int actualMoney; //alamacenar el dinero actual 
    void Start()
    {
        actualMoney = 0;
        clickedObject = null; //inicializar el objeto para evitar erroes de referencia
    }
    public void InitTheShopping() //una vez se entra en la tienda
    {
        if(!GameManager.instancia.currentPlayer) actualMoney = int.Parse(GameManager.instancia.player1Money.text.ToString()); //actualizar el valor del dinero actual con el dinero del correspodniente jugador (caso del jugador 1)
        else actualMoney = int.Parse(GameManager.instancia.player2Money.text.ToString()); //caso del jugador 2
        shopShop.SetActive(true); //activar el objeto contenedor de la tienda
    }
    public void OnPointerDown(PointerEventData eventData) //cada vez que se haga click
    {
        GameObject aux = eventData.pointerCurrentRaycast.gameObject; //obtener el objeto clickeado(objeto bajo el raycast)
        if(aux.transform.childCount != 4) return; //si no tiene 4 hijos no nos intereza 
        else 
        {
            clickedObject = aux; //actualizar el estatico con el objeto clickeado en dicho frame
            clickedObject.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().enabled = true; //encender su respectivo indicador verde
            PowerOfTheLights(clickedObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text.ToString()); //lamar a apagar las luces de los demas objetos
        }
    }
    public void OnBuyButtonPressed() //cuando se presione el botn de comprar
    {
        //verificar el caso de que el objeto clickeado sea null o que el dinero no sea suficiente para hacer ninguna compra
        if(clickedObject == null || int.Parse(clickedObject.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text.ToString().Substring(0,1)) > actualMoney) return;
        else //en caso contrario 
        {
            //reproducir el audio de cajero automatico
            shopShop.GetComponent<AudioSource>().Play();
            int energy = int.Parse(clickedObject.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text.ToString().Substring(0,1)); //guardar la energia que sera aumentada
            actualMoney -= int.Parse(clickedObject.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text.ToString().Substring(0,1)); //restar el precio al dinero actual 
            if(!GameManager.instancia.currentPlayer) //el caso de que sea el jugador 1
            {
                if(clickedObject.tag == "puerta" && GameManager.instancia.clickedHero is not null)
                {
                    System.Random random = new System.Random(); //crear una instancia random para generar posiciones aleatorias
                    int x = 0;
                    int y = 0;
                    while(NPCMove.maze[x,y] || GameManager.instancia.maze.transform.GetChild(x).transform.GetChild(y).childCount > 1) //en caso de que sea un obstaculo o haya alguna trampa generar otra 
                    {
                        x = random.Next(1,16);
                        y = random.Next(1,18);
                    }
                    GameObject aux = GameManager.instancia.clickedHero; //guardar el Heroe con el que se juega
                    aux.transform.SetParent(GameManager.instancia.maze.transform.GetChild(x).GetChild(y).transform); //darle el padre de que le corresponde generado de manera random 
                    aux.transform.localPosition = Vector3.zero; //situar al centro de la gerarquia para evitar troyes 

                    //actualizar el booleano y terminar la ronda con la aplicacion del efecto 
                    if(GameManager.instancia.currentPlayer) GameManager.instancia.currentPlayer = false;
                    else GameManager.instancia.currentPlayer = true;
                    GameManager.instancia.PrepareGame(); //volver a preparar la escena
                    //restar el dinero por la puerta
                    GameManager.instancia.player1Money.text = actualMoney.ToString();
                    GameManager.instancia.player1Moneys.text = actualMoney.ToString();
                }
                else if(clickedObject.tag == "puerta") return;

                int actualEnergy = int.Parse(GameManager.instancia.player1Energy.text.ToString()); //guardar la energia que posee el jugador 
                actualEnergy += energy; //modificar la energia final del jugador
                GameManager.instancia.player1Money.text = actualMoney.ToString();
                GameManager.instancia.player1Moneys.text = actualMoney.ToString();
                GameManager.instancia.player1Energy.text = actualEnergy.ToString();
                GameManager.instancia.player1Energys.text = actualEnergy.ToString();
                if(actualMoney == 0) //si no se tiene mas dinero desactivar el icono de item colected
                {
                    UnityEngine.UI.Image aux = GameObject.Find("Money1").GetComponent<UnityEngine.UI.Image>();//obtener el objeto(imagen) que le corresponde a dinero del jugador
                    aux.enabled = false; //apagar
                }
            }
            else
            {
                if(clickedObject.tag == "puerta" && GameManager.instancia.clickedHero is not null)
                {
                    System.Random random = new System.Random(); //crear una instancia random para generar posiciones aleatorias
                    int x = 0;
                    int y = 0;
                    while(NPCMove.maze[x,y] || GameManager.instancia.maze.transform.GetChild(x).transform.GetChild(y).childCount > 1) //en caso de que sea un obstaculo o haya alguna trampa generar otra 
                    {
                        x = random.Next(1,16);
                        y = random.Next(1,18);
                    }
                    GameObject aux = GameManager.instancia.clickedHero; //guardar el Heroe con el que se juega
                    aux.transform.SetParent(GameManager.instancia.maze.transform.GetChild(x).GetChild(y).transform); //darle el padre de que le corresponde generado de manera random 
                    aux.transform.localPosition = Vector3.zero; //situar al centro de la gerarquia para evitar troyes 

                    //actualizar el booleano y terminar la ronda con la aplicacion del efecto 
                    if(GameManager.instancia.currentPlayer) GameManager.instancia.currentPlayer = false;
                    else GameManager.instancia.currentPlayer = true;
                    GameManager.instancia.PrepareGame(); //volver a preparar la escena
                    //resyar el dinero por la puerta
                    GameManager.instancia.player2Money.text = actualMoney.ToString();
                    GameManager.instancia.player2Moneys.text = actualMoney.ToString();
                }
                else if(clickedObject.tag == "puerta") return;

                int actualEnergy = int.Parse(GameManager.instancia.player2Energy.text.ToString()); //guardar la energia que posee el jugador 
                actualEnergy += energy;
                GameManager.instancia.player2Money.text = actualMoney.ToString();
                GameManager.instancia.player2Moneys.text = actualMoney.ToString();
                GameManager.instancia.player2Energy.text = actualEnergy.ToString();
                GameManager.instancia.player2Energys.text = actualEnergy.ToString();
                if(actualMoney == 0) //si no se tiene mas dinero desactivar el icono de item colected
                {
                    UnityEngine.UI.Image aux = GameObject.Find("Money2").GetComponent<UnityEngine.UI.Image>(); //obtener el objeto(imagen) que le corresponde a dinero del jugador
                    aux.enabled = false; //apagar
                }
            }
        }
    }
    private void PowerOfTheLights(string name) //apagar las luces verdes de los demas objetos para que saber que objeto ha sido clickeado
    {
        for (int i = 0 ; i < luces.Count ; i++) //iterar por las luces
        {
            if(luces[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text.ToString() == name) continue; //si es el objeto en el que estamos seguir iterando
            else //de lo contrario apagar la luz verde
            {
                luces[i].transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().enabled = false; //aceder al componente imagen(luz verde) y apagar
            } 
        }
    }
    public void OnExitButtonPressed() //cuando se presione el boton de salir de la tienda
    {
        shopShop.SetActive(false); //apagar el objeto contenedor de la tienda
    }
}
