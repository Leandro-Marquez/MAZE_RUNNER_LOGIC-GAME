using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Effects : MonoBehaviour , IPointerDownHandler
{
    public static int tommyenfriando; //entero para controlar el tiempo que lleva enfriandose el heroe
    public static int gallyEnfriando; // ...
    public static int terezaEnfriando; // ... 
    public static int sartenEnfriando; //...
    public static int minhoEnfriando; //...
    public static int newtEnfriando; //...
    public void Start() //instanciar el valor del objeto clikeado para con minho o tommy
    {
        GameManager.instancia.currentObjectClickedForMinhoEffect = null;
    }
    public void OnApplyEffectButtonClicked() //cuando se hace click en el boton de activar efecto 
    {
        ActivateEffect(GameManager.instancia.clickedHero.GetComponent<HeroVisual>().hero);
    }
    public void OnPointerDown(PointerEventData eventData) //cuando se hace click actualizar el objeto objetivo
    {
        GameObject objetoClickeado = eventData.pointerCurrentRaycast.gameObject; //guardar el objeto clickeado
        if(objetoClickeado.tag == "boton") return; //si es el propio boton de activar efecto no actualizar el target object
        GameManager.instancia.currentObjectClickedForMinhoEffect = objetoClickeado; //en lugar de ser un posible objetivo actualizar el objeto clickeado en el game manager
    }
    private void ActivateEffect(Hero currentHero) //metodo de activar efecto acorde al heroe desde el cual se quiera activar el efecto 
    {
        if(currentHero.name == "Tommy" && tommyenfriando == 0) //el caso en que es tommy
        {
            tommyenfriando = currentHero.coolingTime; //reiniciar el valor de enfriamiento
            ActivateTommyEffect();//llamar a metodo especifico para el
        }
        else if(currentHero.name == "Gally" && gallyEnfriando == 0)
        {
            gallyEnfriando = currentHero.coolingTime;//reiniciar el valor de enfriamiento
            ActivateTerezaEffect();//llamar a metodo especifico para el
        }
        else if(currentHero.name == "Sarten" && sartenEnfriando == 0)
        {
            sartenEnfriando = currentHero.coolingTime;
            ActivateSartenEffect();//llamar a metodo especifico para el
        }
        else if(currentHero.name == "Newt" && newtEnfriando == 0)
        {
            newtEnfriando = currentHero.coolingTime;//reiniciar el valor de enfriamiento
            ActivateNewtEffect();//llamar a metodo especifico para el
        }
        else if(currentHero.name == "Minho" && minhoEnfriando == 0)
        {
            minhoEnfriando = currentHero.coolingTime;//reiniciar el valor de enfriamiento
            ActivateMinhoEffect();//llamar a metodo especifico para el
        }
        else if(currentHero.name == "Tereza" && terezaEnfriando == 0)
        {
            terezaEnfriando = currentHero.coolingTime;//reiniciar el valor de enfriamiento
            ActivateTerezaEffect();//llamar a metodo especifico para ella
        }
    }
    public static void RestTime() //restar el tiempo de enfriamiento de todo heroe una vez se pase de turno
    {
        if(tommyenfriando > 0) tommyenfriando -= 1; //restar una unidad de tiempo al tiempo de enfriamiento correspondiente
        if(gallyEnfriando > 0) gallyEnfriando -= 1;//...
        if(sartenEnfriando > 0) sartenEnfriando -= 1;//...
        if(minhoEnfriando > 0) minhoEnfriando -= 1;//...
        if(terezaEnfriando > 0) terezaEnfriando -= 1;//...
        if(newtEnfriando > 0 ) newtEnfriando -= 1;//...
    }
    private void ActivateTerezaEffect() //activar el efecto de tereza
    {
        //implentar 
    } 
    private void ActivateMinhoEffect() //activar el efecto de minho
    {
        //el caso de que el objeto objetivo sea nulo retornar 
        if(GameManager.instancia.currentObjectClickedForMinhoEffect is null || GameManager.instancia.currentObjectClickedForMinhoEffect.tag == "floor") return;
        if(GameManager.instancia.currentObjectClickedForMinhoEffect.tag == "extras") //si es un extra es una trampa
        {
            GameObject.Destroy(GameManager.instancia.currentObjectClickedForMinhoEffect); // destruir la trampa correspondiente
        }
        else if(GameManager.instancia.currentObjectClickedForMinhoEffect.tag == "wall") // si es una pared manejar de manera diferente
        {
            GameObject.Destroy(GameManager.instancia.currentObjectClickedForMinhoEffect); //detruir la pared
            //guardar en un objeto una instancia del prefab correspondiente guardado en el GameManager(hierba sucia para cuando se destruye el objeto)
            GameObject pared = GameObject.Instantiate(GameManager.instancia.sueloAuxForMinho,GameManager.instancia.currentObjectClickedForMinhoEffect.transform.parent);
            pared.transform.localPosition = new Vector3(0,0,0);//reescribir la posicion del objeto al centro de la escena
        }
        //actualizar el booleano y terminar la ronda con la aplicacion del efecto 
        if(GameManager.instancia.currentPlayer) GameManager.instancia.currentPlayer = false;
        else GameManager.instancia.currentPlayer = true;
        GameManager.instancia.PrepareGame(); //volver a preparar la escena
    }
    private void ActivateNewtEffect() //activar el efecto de Newt
    {
        //implementar
    }
    private void ActivateSartenEffect() //activar el efecto de Sarten
    {
        //implementar 
    }
    private void ActivateTommyEffect() //activar el efecto de tommy
    {
        GameManager.instancia.tomySound.Play();//reproducir el audio de teletransportacion
        int x = NPCMove.x; //guardar la posicion del heroe actual guardada en NPCMove
        int y = NPCMove.y; // ...
        if(!GameManager.instancia.currentPlayer)//verificar si es el jugador 1 quien aplica el efecto
        {
            // si esta dentro de los rangos y si es posible moverse en esa direccion
            if(x+3 < 17 && GameManager.instancia.maze.transform.GetChild(x+3).GetChild(y).childCount == 1 && GameManager.instancia.maze.transform.GetChild(x+3).GetChild(y).GetChild(0).tag != "wall") 
            {
                GameObject aux = GameManager.instancia.maze.transform.GetChild(x).GetChild(y).GetChild(GameManager.instancia.maze.transform.GetChild(x).GetChild(y).childCount-1).gameObject;//guardar el heroe para moverlo despues
                aux.transform.SetParent(GameManager.instancia.maze.transform.GetChild(x+3).transform.GetChild(y).transform);//darle su padre correspondiente en la gerarquia 
                aux.transform.localPosition = Vector3.zero;//colocarle lasc coordenadas 0,0,0 para evitar troques
            }
            // si esta dentro de los rangos y si es posible moverse en esa direccion
            else if(y+3 < 19 && GameManager.instancia.maze.transform.GetChild(x).GetChild(y+3).childCount == 1 && GameManager.instancia.maze.transform.GetChild(x).GetChild(y+3).GetChild(0).tag != "wall")
            {
                GameObject aux = GameManager.instancia.maze.transform.GetChild(x).GetChild(y).GetChild(GameManager.instancia.maze.transform.GetChild(x).GetChild(y).childCount-1).gameObject;//guardar el heroe para moverlo despues
                aux.transform.SetParent(GameManager.instancia.maze.transform.GetChild(x).transform.GetChild(y+3).transform);//darle su padre correspondiente en la gerarquia 
                aux.transform.localPosition = Vector3.zero;//colocarle lasc coordenadas 0,0,0 para evitar troques
            }
        }
        else //verificar si es el jugador 2 quien aplica el efecto
        {
            // si esta dentro de los rangos y si es posible moverse en esa direccion
            if(x-3 >= 0 && GameManager.instancia.maze.transform.GetChild(x-3).GetChild(y).childCount == 1 && GameManager.instancia.maze.transform.GetChild(x-3).GetChild(y).GetChild(0).tag != "wall") 
            {
                GameObject aux = GameManager.instancia.maze.transform.GetChild(x).GetChild(y).GetChild(GameManager.instancia.maze.transform.GetChild(x).GetChild(y).childCount-1).gameObject;//guardar el heroe para moverlo despues
                aux.transform.SetParent(GameManager.instancia.maze.transform.GetChild(x-3).transform.GetChild(y).transform);//darle su padre correspondiente en la gerarquia 
                aux.transform.localPosition = Vector3.zero;//colocarle lasc coordenadas 0,0,0 para evitar troques
            }
            // si esta dentro de los rangos y si es posible moverse en esa direccion
            else if(y-3 >= 0 && GameManager.instancia.maze.transform.GetChild(x).GetChild(y-3).childCount == 1 && GameManager.instancia.maze.transform.GetChild(x).GetChild(y-3).GetChild(0).tag != "wall")
            {
                GameObject aux = GameManager.instancia.maze.transform.GetChild(x).GetChild(y).GetChild(GameManager.instancia.maze.transform.GetChild(x).GetChild(y).childCount-1).gameObject;//guardar el heroe para moverlo despues
                aux.transform.SetParent(GameManager.instancia.maze.transform.GetChild(x).transform.GetChild(y-3).transform);//darle su padre correspondiente en la gerarquia 
                aux.transform.localPosition = Vector3.zero;//colocarle lasc coordenadas 0,0,0 para evitar troques
            }
        }
        //actualizar el booleano y terminar la ronda con la aplicacion del efecto 
        if(GameManager.instancia.currentPlayer) GameManager.instancia.currentPlayer = false;
        else GameManager.instancia.currentPlayer = true;
        GameManager.instancia.PrepareGame(); //volver a preparar la escena
    }
}
