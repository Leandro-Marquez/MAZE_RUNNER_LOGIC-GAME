using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Effects : MonoBehaviour , IPointerDownHandler
{
    private static bool [,] aux; //matriz auxiliar para veirifcar acorde el efecto que se teletranportara o destruira un objeto en su rango de alcance
    public void Start() //instanciar el valor del objeto clikeado para con minho o tommy
    {
        aux = new bool[17,19];
    }
    public void OnApplyEffectButtonClicked() //cuando se hace click en el boton de activar efecto 
    {
        ActivateEffect(GameManager.instancia.clickedHero.GetComponent<HeroVisual>().hero);
    }
    public void OnPointerDown(PointerEventData eventData) //cuando se hace click actualizar el objeto objetivo
    {
        GameObject objetoClickeado = eventData.pointerCurrentRaycast.gameObject; //guardar el objeto clickeado
        if(objetoClickeado.tag == "boton") return; //si es el propio boton de activar efecto no actualizar el target object
        GameManager.instancia.currentObjectClickedForMinhoEffect = objetoClickeado;
    }
    private void ActivateEffect(Hero currentHero) //metodo de activar efecto acorde al heroe desde el cual se quiera activar el efecto 
    {
        if(currentHero.name == "Tommy" && GameManager.tommyenfriando == 0) //el caso en que es tommy
        {
            GameManager.tommyenfriando = currentHero.coolingTime; //reiniciar el valor de enfriamiento
            ActivateTommyEffect();//llamar a metodo especifico para el
        }
        else if(currentHero.name == "Gally" && GameManager.gallyEnfriando == 0)
        {
            GameManager.gallyEnfriando = currentHero.coolingTime;//reiniciar el valor de enfriamiento
            ActivateGallyEffect();//llamar a metodo especifico para el
        }
        else if(currentHero.name == "Sarten" && GameManager.sartenEnfriando == 0)
        {
            GameManager.sartenEnfriando = currentHero.coolingTime;//reiniciar el valor de enfriamiento
            ActivateSartenEffect();//llamar a metodo especifico para el
        }
        else if(currentHero.name == "Minho" && GameManager.minhoEnfriando == 0)
        {
            GameManager.minhoEnfriando = currentHero.coolingTime;//reiniciar el valor de enfriamiento
            ActivateMinhoEffect();//llamar a metodo especifico para el
        }
        else if(currentHero.name == "Tereza" && GameManager.terezaEnfriando == 0)
        {
            GameManager.terezaEnfriando = currentHero.coolingTime;//reiniciar el valor de enfriamiento
            ActivateTerezaEffect();//llamar a metodo especifico para ella
        }
    }
    public static void RestTime() //restar el tiempo de enfriamiento de todo heroe una vez se pase de turno
    {
        if(GameManager.tommyenfriando > 0) GameManager.tommyenfriando -= 1; //restar una unidad de tiempo al tiempo de enfriamiento correspondiente
        if(GameManager.gallyEnfriando > 0) GameManager.gallyEnfriando -= 1; //...
        if(GameManager.sartenEnfriando > 0) GameManager.sartenEnfriando -= 1; //...
        if(GameManager.minhoEnfriando > 0) GameManager.minhoEnfriando -= 1; //...
        if(GameManager.terezaEnfriando > 0) GameManager.terezaEnfriando -= 1; //...
        if(GameManager.newtEnfriando > 0 ) GameManager.newtEnfriando -= 1; //...
    }
    private void ActivateTerezaEffect() //activar el efecto de tereza
    {
        //implentar 
    } 
    public static void ActivateMinhoEffect() //activar el efecto de minho
    {
        //el caso de que el objeto objetivo sea nulo o sea e suelo retornar 
        if(GameManager.instancia.currentObjectClickedForMinhoEffect is null || GameManager.instancia.currentObjectClickedForMinhoEffect.tag == "floor") return;
        
        AudioSource audio = GameManager.instancia.colectedSound.GetComponent<AudioSource>();//crear un componente audio source para reproducir el audio correspondiente con el objeto u trampa colectada 
        AudioClip leo = GameManager.instancia.clickedHero.GetComponent<HeroVisual>().hero.audioClip; //obtener al audio clip del escriptable
        audio.clip = leo; //asiganr el audio del escriptable al objeto de audio creado
        audio.Play();//reproducir el audio 
        
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
    public static void ActivateGallyEffect() //activar el efecto de Gally
    {
        //implementar
    }
    public static void ActivateNewtEffect() //activar el efecto de Newt
    {
        //verificar que no este en una misma casilla que un companero
        if( GameManager.instancia.maze.transform.GetChild(NPCMove.x).transform.GetChild(NPCMove.y).transform.GetChild(1).GetComponent<HeroVisual>().owner ==  GameManager.instancia.maze.transform.GetChild(NPCMove.x).transform.GetChild(NPCMove.y).transform.GetChild(2).GetComponent<HeroVisual>().owner ) return;
        if(GameManager.newtEnfriando != 0) return;//manejar el caso de que no este ennfriado totalmente
        else
        {
            AudioSource audio = GameManager.instancia.colectedSound.GetComponent<AudioSource>();//crear un componente audio source para reproducir el audio correspondiente con el objeto u trampa colectada 
            AudioClip leo = GameManager.instancia.clickedHero.GetComponent<HeroVisual>().hero.audioClip; //obtener al audio clip del escriptable
            audio.clip = leo; //asiganr el audio del escriptable al objeto de audio creado
            audio.Play();//reproducir el audio 
            GameObject toMove = GameManager.instancia.maze.transform.GetChild(NPCMove.x).transform.GetChild(NPCMove.y).transform.GetChild(1).gameObject;//heroe enemigo a mover
            if(GameManager.instancia.clickedHero.transform.GetComponent<HeroVisual>().owner == Owner.Player1)//verificar acorde a de quien sea a donde ponerlo estrategicamente 
            {
                for (int i = 17 ; i >= 1; i--)//verificar acorde a de quien sea a donde ponerlo estrategicamente 
                {
                    //si la casilla tiene solo un hijo y no tiene tarjeta de pared entonces sirve
                    if(GameManager.instancia.maze.transform.GetChild(15).transform.GetChild(i).transform.childCount == 1 && GameManager.instancia.maze.transform.GetChild(15).transform.GetChild(i).tag != "wall") 
                    {
                        toMove.transform.SetParent(GameManager.instancia.maze.transform.GetChild(15).transform.GetChild(i).transform);//darle su padre correspondiente en la gerarquia 
                        toMove.transform.localPosition = Vector3.zero;//colocarle lasc coordenadas 0,0,0 para evitar troques
                    }
                }
            }
            else 
            {
                for (int i = 1; i < 18; i++)//verificar acorde a de quien sea a donde ponerlo estrategicamente 
                {
                    //si la casilla tiene solo un hijo y no tiene tarjeta de pared entonces sirve
                    if(GameManager.instancia.maze.transform.GetChild(1).transform.GetChild(i).transform.childCount == 1)
                    {
                        toMove.transform.SetParent(GameManager.instancia.maze.transform.GetChild(1).transform.GetChild(i).transform);//darle su padre correspondiente en la gerarquia 
                        toMove.transform.localPosition = Vector3.zero;//colocarle lasc coordenadas 0,0,0 para evitar troques
                    }
                }
            }
        }
        //actualizar el booleano y terminar la ronda con la aplicacion del efecto 
        if(GameManager.instancia.currentPlayer) GameManager.instancia.currentPlayer = false;
        else GameManager.instancia.currentPlayer = true;
        GameManager.instancia.PrepareGame(); //volver a preparar la escena
    }
    public static void ActivateSartenEffect() //activar el efecto de Sarten
    {
        AudioSource audio = GameManager.instancia.colectedSound.GetComponent<AudioSource>();//crear un componente audio source para reproducir el audio correspondiente con el objeto u trampa colectada 
        AudioClip leo = GameManager.instancia.clickedHero.GetComponent<HeroVisual>().hero.audioClip; //obtener al audio clip del escriptable
        audio.clip = leo; //asiganr el audio del escriptable al objeto de audio creado
        audio.Play();//reproducir el audio 
        
        //actualizar el booleano y terminar la ronda con la aplicacion del efecto 
        if(GameManager.instancia.currentPlayer) GameManager.instancia.currentPlayer = false;
        else GameManager.instancia.currentPlayer = true;
        GameManager.instancia.PrepareGame(); //volver a preparar la escena

        //actualizar el booleano y terminar la ronda con la aplicacion del efecto 
        if(GameManager.instancia.currentPlayer) GameManager.instancia.currentPlayer = false;
        else GameManager.instancia.currentPlayer = true;
        GameManager.instancia.PrepareGame(); //volver a preparar la escena
    }
    public static void ActivateTommyEffect() //activar el efecto de tommy
    {
        AudioSource audio = GameManager.instancia.colectedSound.GetComponent<AudioSource>();//crear un componente audio source para reproducir el audio correspondiente con el objeto u trampa colectada 
        AudioClip leo = GameManager.instancia.clickedHero.GetComponent<HeroVisual>().hero.audioClip; //obtener al audio clip del escriptable
        audio.clip = leo; //asiganr el audio del escriptable al objeto de audio creado
        audio.Play();//reproducir el audio 

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
    public static void ColectObjects(int xpos , int ypos) //colectar objetos, sumar y restar la energia correspondiente 
    {

        if(!GameManager.instancia.currentPlayer) // si es el caso del jugador 1
        {
            if(GameManager.instancia.maze.transform.GetChild(xpos).transform.GetChild(ypos).childCount == 2 && GameManager.instancia.maze.transform.GetChild(xpos).transform.GetChild(ypos).GetChild(0).tag != "floor")
            {
                Debug.Log("no es una trampa");
                if(GameManager.instancia.maze.transform.GetChild(xpos).transform.GetChild(ypos).GetChild(0).GetComponent<TeleportOwner>() != null)
                {
                    Debug.Log("es un teleport");
                    if(GameManager.instancia.maze.transform.GetChild(xpos).transform.GetChild(ypos).GetChild(0).GetComponent<TeleportOwner>().owner == Owner.Player2)
                    {
                        Debug.Log("es del jugador 2");
                        int aux = int.Parse(GameManager.instancia.player1Energy.text.ToString());
                        if(aux >= GameManager.winConditionForPLayers)
                        {
                            ScenesController.LoadPlayer1VictoryScene();
                            return;
                        }
                    }
                }
                return;
            }
            if(GameManager.instancia.maze.transform.GetChild(xpos).transform.GetChild(ypos).childCount == 2) return;
            int finalEnergy = 0; //entero para guardar la energia final 
            finalEnergy += GameManager.instancia.maze.transform.GetChild(xpos).transform.GetChild(ypos).GetChild(1).GetComponent<TrapVisual>().trap.Penalty;//sumar la energia del objeto colectado 
            
            AudioSource audio = GameManager.instancia.colectedSound.GetComponent<AudioSource>();//crear un componente audio source para reproducir el audio correspondiente con el objeto u trampa colectada 
            AudioClip leo = GameManager.instancia.maze.transform.GetChild(xpos).transform.GetChild(ypos).GetChild(1).GetComponent<TrapVisual>().trap.audioClip1; //obtener al audio clip del escriptable
            audio.clip = leo; //asiganr el audio del escriptable al objeto de audio creado
            audio.Play();//reproducir el audio 

            // Esperar a que termine el primer audio
            if (GameManager.instancia.maze.transform.GetChild(xpos).transform.GetChild(ypos).GetChild(1).GetComponent<TrapVisual>().trap.audioClip2 != null) 
            {
                // Usar una Coroutine para esperar la reproducción del primer audio
                CoroutineRunner.instance.StartCoroutine(PlaySecondAudioAfterFirst(audio, GameManager.instancia.maze.transform.GetChild(xpos).transform.GetChild(ypos).GetChild(1).GetComponent<TrapVisual>().trap.audioClip2));
            }
            if(GameManager.instancia.maze.transform.GetChild(xpos).transform.GetChild(ypos).GetChild(1).GetComponent<TrapVisual>().trap.name == "hollow") //en caso de que sea un hueco se acaba inmediatamente la capacidad de movimiento del equipo 
            {
                //actualizar el booleano y terminar la ronda con la aplicacion del efecto 
                if(GameManager.instancia.currentPlayer) GameManager.instancia.currentPlayer = false;
                else GameManager.instancia.currentPlayer = true;
                GameManager.instancia.PrepareGame(); //volver a preparar la escena
            }
            if(GameManager.instancia.maze.transform.GetChild(xpos).transform.GetChild(ypos).GetChild(1).GetComponent<TrapVisual>().trap.name == "teleport") //en caso de que sea un teleport
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
            }
            int actualEnergy = int.Parse(GameManager.instancia.player1Energy.text.ToString()); //tenrr la energia guardada en el texto en escena 
            finalEnergy += actualEnergy; //calcular la energia final 
            GameManager.instancia.player1Energy.text = finalEnergy.ToString();//modificar el texto en escena 
            GameManager.instancia.player1Energys.text = finalEnergy.ToString();//sombra ...
            GameObject.Destroy(GameManager.instancia.maze.transform.GetChild(xpos).transform.GetChild(ypos).GetChild(1).gameObject); //destruir el objeto coleccionado 

        }
        else // si es el caso del jugador 2
        {
            if(GameManager.instancia.maze.transform.GetChild(xpos).transform.GetChild(ypos).childCount == 2 && GameManager.instancia.maze.transform.GetChild(xpos).transform.GetChild(ypos).GetChild(0).tag != "floor")
            {
                    Debug.Log("es un teleport");

                if(GameManager.instancia.maze.transform.GetChild(xpos).transform.GetChild(ypos).GetChild(0).GetComponent<TeleportOwner>() != null)
                {
                        Debug.Log("es del jugador 1");
                    if(GameManager.instancia.maze.transform.GetChild(xpos).transform.GetChild(ypos).GetChild(0).GetComponent<TeleportOwner>().owner == Owner.Player2)
                    {
                        int aux = int.Parse(GameManager.instancia.player2Energy.text.ToString());
                        if(aux >= GameManager.winConditionForPLayers)
                        {
                            ScenesController.LoadPlayer2VictoryScene();
                            return;
                        }
                    }
                }
                return;
            }
            if(GameManager.instancia.maze.transform.GetChild(xpos).transform.GetChild(ypos).childCount == 2) return;
            int finalEnergy = 0;//entero para guardar la energia final 
            finalEnergy += GameManager.instancia.maze.transform.GetChild(xpos).transform.GetChild(ypos).GetChild(1).GetComponent<TrapVisual>().trap.Penalty;//sumar la energia del objeto colectado 
            
            AudioSource audio = GameManager.instancia.colectedSound.GetComponent<AudioSource>();//crear un componente audio source para reproducir el audio correspondiente con el objeto u trampa colectada 
            AudioClip leo= GameManager.instancia.maze.transform.GetChild(xpos).transform.GetChild(ypos).GetChild(1).GetComponent<TrapVisual>().trap.audioClip1;//obtener al audio clip del escriptable
            audio.clip = leo; //asiganr el audio del escriptable al objeto de audio creado
            audio.Play();//reproducir el audio 

            // Esperar a que termine el primer audio
            if (GameManager.instancia.maze.transform.GetChild(xpos).transform.GetChild(ypos).GetChild(1).GetComponent<TrapVisual>().trap.audioClip2 != null)
            {
                // Usar una Coroutine para esperar la reproducción del primer audio
                CoroutineRunner.instance.StartCoroutine(PlaySecondAudioAfterFirst(audio, GameManager.instancia.maze.transform.GetChild(xpos).transform.GetChild(ypos).GetChild(1).GetComponent<TrapVisual>().trap.audioClip2));
            }
            if(GameManager.instancia.maze.transform.GetChild(xpos).transform.GetChild(ypos).GetChild(1).GetComponent<TrapVisual>().trap.name == "hollow") //en caso de que sea un hueco se acaba inmediatamente la capacidad de movimiento del equipo 
            {
                //actualizar el booleano y terminar la ronda con la aplicacion del efecto 
                if(GameManager.instancia.currentPlayer) GameManager.instancia.currentPlayer = false;
                else GameManager.instancia.currentPlayer = true;
                GameManager.instancia.PrepareGame(); //volver a preparar la escena
            }
            if(GameManager.instancia.maze.transform.GetChild(xpos).transform.GetChild(ypos).GetChild(1).GetComponent<TrapVisual>().trap.name == "teleport") //en caso de que sea un teleport
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
            }
            int actualEnergy = int.Parse(GameManager.instancia.player2Energy.text.ToString());//tenrr la energia guardada en el texto en escena 
            finalEnergy += actualEnergy;//calcular la energia final 
            GameManager.instancia.player2Energy.text = finalEnergy.ToString();//modificar el texto en escena 
            GameManager.instancia.player2Energys.text = finalEnergy.ToString();//sombra...
            GameObject.Destroy(GameManager.instancia.maze.transform.GetChild(xpos).transform.GetChild(ypos).GetChild(1).gameObject);//destruir el objeto coleccionado 
        }
    }
    private static IEnumerator PlaySecondAudioAfterFirst(AudioSource audio, AudioClip clip2) //verificar si aun se reproduce el primer audio 
    {
        // Esperar hasta que termine de reproducir el primer clip
        while (audio.isPlaying)
        {
            yield return null; // Espera un frame
        }
        audio.clip = clip2; // Cambiar al segundo clip
        audio.Play(); // Reproducir el segundo clip
    }
}
