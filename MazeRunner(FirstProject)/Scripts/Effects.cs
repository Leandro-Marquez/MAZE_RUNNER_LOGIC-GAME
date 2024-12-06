using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{
    public static int tommyenfriando;
    public static int gallyEnfriando;
    public static int terezaEnfriando;
    public static int sartenEnfriando;
    public static int minhoEnfriando;
    public static int newtEnfriando;
    public void OnApplyEffectButtonClicked()
    {
        ActivateEffect(GameManager.instancia.clickedHero.GetComponent<HeroVisual>().hero);
    }
    private void ActivateEffect(Hero currentHero)
    {
        if(currentHero.name == "Tommy" && tommyenfriando == 0)
        {
            tommyenfriando = currentHero.coolingTime;
            ActivateTommyEffect();
        }
        else if(currentHero.name == "Gally" && gallyEnfriando == 0)
        {
            gallyEnfriando = currentHero.coolingTime;
            ActivateTerezaEffect();
        }
        else if(currentHero.name == "Sarten" && sartenEnfriando == 0)
        {
            sartenEnfriando = currentHero.coolingTime;
            ActivateSartenEffect();
        }
        else if(currentHero.name == "Newt" && newtEnfriando == 0)
        {
            newtEnfriando = currentHero.coolingTime;
            ActivateNewtEffect();
        }
        else if(currentHero.name == "Minho" && minhoEnfriando == 0)
        {
            minhoEnfriando = currentHero.coolingTime;
            ActivateMinhoEffect();
        }
        else if(currentHero.name == "Tereza" && terezaEnfriando == 0)
        {
            terezaEnfriando = currentHero.coolingTime;
            ActivateTerezaEffect();
        }
    }
    public static void RestTime()
    {
        if(tommyenfriando > 0) tommyenfriando -= 1;
        if(gallyEnfriando > 0) gallyEnfriando -= 1;
        if(sartenEnfriando > 0) sartenEnfriando -= 1;
        if(minhoEnfriando > 0) minhoEnfriando -= 1;
        if(terezaEnfriando > 0) terezaEnfriando -= 1;
        if(newtEnfriando > 0 ) newtEnfriando -= 1;
    }
    private void ActivateTerezaEffect()
    {
        //implentar 
    } 
    private void ActivateMinhoEffect()
    {
        //implementar
    }
    private void ActivateNewtEffect()
    {
        //implementar
    }
    private void ActivateSartenEffect()
    {
        //implementar 
    }
    private void ActivateTommyEffect()
    {
        GameManager.instancia.tomySound.Play();//reproducir el audio de teletransportacion
        int x = NPCMove.x;
        int y = NPCMove.y;
        if(!GameManager.instancia.currentPlayer)//verificar si es el jugador 1 quien aplica ele fecto
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
        else
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
