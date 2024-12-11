using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NPCMove : MonoBehaviour , IPointerDownHandler
{
    public Hero currentHero; //guardar el heroe actual
    public static int n; //entero para controlar que una vez que se mueva un objeto no se pueda mover ningun otro 
    public static int x;//cordenada x del heroe actual
    public static int y;//coordenada y del heroe actual
    public static bool seMovio;//booleano para verificar si se movio un objeto o no 
    public static Image clikedObjectImage; //imagen del objeto clickeado en la escena 
    public static bool [,] maze; //guardar el laberinto completo de la escena
    private bool [,] posibleMoves; //guardar las posiciones accesibles acorde a cada heroe;
    public void Start() // inicializar los objetos en el primer momento del juego 
    { 
        n = 0;//inicializar
        maze = new bool[17,19]; //...
        posibleMoves = new bool[17,19];//...
        currentHero = null;//...
        seMovio = false;//...
        UpdateMatrix();//actualizar el laberinto de la escena en la mascara booleana para tenerlo a nivel de codigo 
        clikedObjectImage = GameObject.Find("CliskedObjectImage").GetComponent<Image>(); //asignar la imagen que le corresponde en la escena ya que no tengo objeto alguno con este script hasta que se inicialicen los heroes
        GameManager.clikedObjectFija = clikedObjectImage.sprite;//inicializar la imagen fija con la imagen asignada desde el inspector 
    }
    public void OnPointerDown(PointerEventData eventData) //cuando se hace click 
    {
        if(n != 0) return; // si ya se movio no se puede seleccionar ningun otro heroe
        GameObject objetoClickeado = eventData.pointerCurrentRaycast.gameObject; //guardar el objeto clickeado
        Hero clickedHero = objetoClickeado.GetComponent<HeroVisual>().hero; //guardar el componente hero 
        if(clickedHero is not null ) //verificar si el componente hero no es nulo osea que es un heroe
        {
            currentHero = clickedHero;//actualizar el heroe actual con el heroe clickeado
            GameManager.instancia.clickedHero = objetoClickeado; //guardar una instancia del heroe clickeado a nivel de game anager 
            clikedObjectImage.sprite = currentHero.heroPhoto; //actualizar la imagen del clicked object de la escena!!!
            seMovio = false; //restablecer el valor ya que np ha habido movimiento 
            n += 1; //aumentar el valor una vez se hace click 
            UpdateMatrix();//actualizar la el laberinto por si se destruyo algun objeto(pared_roca)
            UpdatePosition();//actualizar la posicion con la posicion del heroe clickeado
            UpdatePosibleMoves(); // marcar las celdas accesibles 
            InvalidOperationsWithOthers();//invalidar el la habilidad de movimiento a los otros Heroes
        }
    }
    private void UpdatePosition() //actualizar la posicion del heroe clickeado
    {
        //actualir matriz a partir del laberinto en la escena
        for (int i = 0; i < 17; i++)
        {
            for (int j = 0; j < 19; j++)
            {
                //verificar que sea una celda con al menos un objeto que no sea hierba o pared 
                if(GameManager.instancia.maze.transform.GetChild(i).GetChild(j).childCount > 1)
                {
                    //en caso de que dicho objeto no tenga el componente hero visual seria una trampa u algo asi  
                    if(GameManager.instancia.maze.transform.GetChild(i).GetChild(j).GetChild(1).GetComponent<HeroVisual>() is null) continue;
                    //en el caso de que lo tenga verificar que tenga el nombre del current hero
                    else if(GameManager.instancia.maze.transform.GetChild(i).GetChild(j).GetChild(1).GetComponent<HeroVisual>().hero.name == currentHero.name)
                    {
                        //actualizar las coordenadas con sus respectivas
                        x = i;
                        y = j;
                        return; //una vez se encontro, por eficiencia retornar
                    }
                }
            }
        }
    }
    private void DetectPressedKeys()//detectar que tecla se presiona en cada frame
    {
        if (Input.GetKeyDown(KeyCode.W)) MoveW(); //Detectar si se presiona la tecla W
        if (Input.GetKeyDown(KeyCode.A)) MoveA(); // Detectar si se presiona la tecla A
        if (Input.GetKeyDown(KeyCode.S)) MoveS(); // Detectar si se presiona la tecla S
        if (Input.GetKeyDown(KeyCode.D)) MoveD(); // Detectar si se presiona la tecla D
    }
    private void MoveW() //mover hacia arriba 
    {
        if(currentHero is null) return;//verificar que no se haga nada si no hay heroe alguno seleccionado
        if(!seMovio) //si no ha habido movimiento necesita actualizarce el laberinto e invalidad el componente a los demas heroes 
        {
            UpdateMatrix();//actualizar el laberinto de la escena en la mascara booleana para tenerlo a nivel de codigo 
            seMovio = true;//si llamo al metodo es que hubo movimiento
        }
        if(currentHero is null) return;//verificar que no se haga nada si no hay heroe alguno seleccionado
        int currentIndex = GameManager.instancia.maze.transform.GetChild(x).GetChild(y).childCount-1;//inidice del heroe actual en la gerarquia
        if(x-1 >= 0 && !maze[x-1,y] && posibleMoves[x-1,y])//si esta en los rangos de la matriz y si se puede mover hacia alli
        {
            GameObject aux = GameManager.instancia.maze.transform.GetChild(x).GetChild(y).GetChild(currentIndex).gameObject;//guardar el heroe para moverlo despues
            aux.transform.SetParent(GameManager.instancia.maze.transform.GetChild(x-1).transform.GetChild(y).transform);//darle su padre correspondiente en la gerarquia 
            aux.transform.localPosition = Vector3.zero;//colocarle lasc coordenadas 0,0,0 para evitar troques
            x-=1;//actualizar la posicion
            //manejar el caso de que el heroe actual sea Gally
            if(GameManager.instancia.maze.transform.GetChild(x).transform.GetChild(y).transform.childCount == 3)
            {
                if(GameManager.instancia.maze.transform.GetChild(x).transform.GetChild(y).transform.GetChild(2).transform.GetComponent<HeroVisual>().hero.name == "Newt")//si es newt 
                {
                    //si hay otro en la casilla de Newt
                    if(GameManager.instancia.maze.transform.GetChild(x).transform.GetChild(y).transform.GetChild(1).transform.GetComponent<HeroVisual>() != null)
                    {
                        //activar el efecto
                        Effects.ActivateNewtEffect();
                        return;
                    }
                }
            }
            //manejar el caso de que el heroe haya caida en una casilla trampa u item
            if(GameManager.instancia.maze.transform.GetChild(x).transform.GetChild(y).transform.childCount == 3) Effects.ColectObjects(x,y);
        }
    }
    private void MoveA()//mover a la izquierda
    {
        if(currentHero is null) return;//verificar que no se haga nada si no hay heroe alguno seleccionado
        if(!seMovio)//si no ha habido movimiento necesita actualizarce el laberinto e invalidad el componente a los demas heroes 
        {
            UpdateMatrix();//actualizar el laberinto de la escena en la mascara booleana para tenerlo a nivel de codigo 
            seMovio = true;//si llamo al metodo es que hubo movimiento
        }
        int currentIndex = GameManager.instancia.maze.transform.GetChild(x).GetChild(y).childCount-1;//inidice del heroe actual en la gerarquia
        if(y-1 >= 0 && !maze[x,y-1] && posibleMoves[x,y-1])//si esta en los rangos de la matriz y si se puede mover hacia alli
        {
            GameObject aux = GameManager.instancia.maze.transform.GetChild(x).GetChild(y).GetChild(currentIndex).gameObject;//guardar el heroe para moverlo despues
            aux.transform.SetParent(GameManager.instancia.maze.transform.GetChild(x).transform.GetChild(y-1).transform);//darle su padre correspondiente en la gerarquia 
            aux.transform.localPosition = Vector3.zero;//colocarle lasc coordenadas 0,0,0 para evitar troques
            y-=1;//actualizar la posicion

            if(GameManager.instancia.maze.transform.GetChild(x).transform.GetChild(y).transform.childCount == 3)
            {
                if(GameManager.instancia.maze.transform.GetChild(x).transform.GetChild(y).transform.GetChild(2).transform.GetComponent<HeroVisual>().hero.name == "Newt")//si es newt 
                {
                    //si hay otro en la casilla de Newt
                    if(GameManager.instancia.maze.transform.GetChild(x).transform.GetChild(y).transform.GetChild(1).transform.GetComponent<HeroVisual>() != null)
                    {
                        //activar el efecto
                        Effects.ActivateNewtEffect();
                        return;
                    }
                }
            }
            //manejar el caso de que el heroe haya caida en una casilla trampa u item
            if(GameManager.instancia.maze.transform.GetChild(x).transform.GetChild(y).transform.childCount == 3) Effects.ColectObjects(x,y);

        }
    }
    private void MoveS()//mover hacia abajo
    {
        if(currentHero is null) return;//verificar que no se haga nada si no hay heroe alguno seleccionado
        if(!seMovio)//si no ha habido movimiento necesita actualizarce el laberinto e invalidad el componente a los demas heroes 
        {
            UpdateMatrix();//actualizar el laberinto de la escena en la mascara booleana para tenerlo a nivel de codigo 
            seMovio = true;//si llamo al metodo es que hubo movimiento
        }
        int currentIndex = GameManager.instancia.maze.transform.GetChild(x).GetChild(y).childCount-1;//inidice del heroe actual en la gerarquia
        if(x+1 < 17 && !maze[x+1,y] && posibleMoves[x+1,y]) //si esta en los rangos de la matriz y si se puede mover hacia alli
        {
            GameObject aux = GameManager.instancia.maze.transform.GetChild(x).GetChild(y).GetChild(currentIndex).gameObject;//guardar el heroe para moverlo despues
            aux.transform.SetParent(GameManager.instancia.maze.transform.GetChild(x+1).transform.GetChild(y).transform);//darle su padre correspondiente en la gerarquia 
            aux.transform.localPosition = Vector3.zero;//colocarle lasc coordenadas 0,0,0 para evitar troques
            x+=1;//actualizar la posicion

            if(GameManager.instancia.maze.transform.GetChild(x).transform.GetChild(y).transform.childCount == 3)
            {
                if(GameManager.instancia.maze.transform.GetChild(x).transform.GetChild(y).transform.GetChild(2).transform.GetComponent<HeroVisual>().hero.name == "Newt")//si es newt 
                {
                    //si hay otro en la casilla de Newtly
                    if(GameManager.instancia.maze.transform.GetChild(x).transform.GetChild(y).transform.GetChild(1).transform.GetComponent<HeroVisual>() != null)
                    {
                        //activar el efecto
                        Effects.ActivateNewtEffect();
                        return;
                    }
                }
            }
            //manejar el caso de que el heroe haya caida en una casilla trampa u item
            if(GameManager.instancia.maze.transform.GetChild(x).transform.GetChild(y).transform.childCount == 3) Effects.ColectObjects(x,y);
        }
    }
    private void MoveD() //mover a la derecha
    {
        if(currentHero is null) return; //verificar que no se haga nada si no hay heroe alguno seleccionado
        if(!seMovio) //si no ha habido movimiento necesita actualizarce el laberinto e invalidad el componente a los demas heroes 
        {
            UpdateMatrix();//actualizar el laberinto de la escena en la mascara booleana para tenerlo a nivel de codigo 
            seMovio = true;//si llamo al metodo es que hubo movimiento
        }
        int currentIndex = GameManager.instancia.maze.transform.GetChild(x).GetChild(y).childCount-1; //inidice del heroe actual en la gerarquia
        if(y+1 < 19 && !maze[x,y+1] && posibleMoves[x,y+1]) //si esta en los rangos de la matriz y si se puede mover hacia alli
        {
            GameObject aux = GameManager.instancia.maze.transform.GetChild(x).GetChild(y).GetChild(currentIndex).gameObject; //guardar el heroe para moverlo despues
            aux.transform.SetParent(GameManager.instancia.maze.transform.GetChild(x).transform.GetChild(y+1).transform); //darle su padre correspondiente en la gerarquia 
            aux.transform.localPosition = Vector3.zero; //colocarle lasc coordenadas 0,0,0 para evitar troques
            y += 1;//actualizar la posicion

            if(GameManager.instancia.maze.transform.GetChild(x).transform.GetChild(y).transform.childCount == 3)
            {
                if(GameManager.instancia.maze.transform.GetChild(x).transform.GetChild(y).transform.GetChild(2).transform.GetComponent<HeroVisual>().hero.name == "Newt")//si es newt 
                {
                    //si hay otro en la casilla de Newt
                    if(GameManager.instancia.maze.transform.GetChild(x).transform.GetChild(y).transform.GetChild(1).transform.GetComponent<HeroVisual>() != null)
                    {
                        //activar el efecto
                        Effects.ActivateNewtEffect();
                        return;
                    }
                }
            }
            //manejar el caso de que el heroe haya caida en una casilla trampa u item
            if(GameManager.instancia.maze.transform.GetChild(x).transform.GetChild(y).transform.childCount == 3) Effects.ColectObjects(x,y);
        }
    }
    private void UpdatePosibleMoves() //marcar toda celda accesible para el heroe
    {
        posibleMoves = new bool[17,19]; //reiniciar los valores de la matriz para evitar confusiones
        int xpos = x; //guardar la posicion incial para evitar trabajar con los campos de la clase
        int ypos = y; // ...
        DFS(xpos,ypos,currentHero.speed); //lamar a marcar cada ceda alcanzable por el current hero
    }
    private void DFS(int xpos , int ypos , int moves) //visitar todas las casillas accesibles desde la posicion del heroe
    {
        if(moves == 0) return; //el caso de que no pueda seguir caminando debido a la velocidad
        posibleMoves[xpos,ypos] = true; //marcar la posicion pertinente
        //direcciones    der izq  arr abj
        int [] dfilas = { 0 , 0 , -1 , 1 };
        int [] dcolus = { 1 ,-1 ,  0 , 0 };
        for (int i = 0; i < dfilas.Length ; i++) //iterar por las direcciones
        {
            int nuevafila = xpos + dfilas[i]; //nueva fila 
            int nuevacolumna = ypos + dcolus[i]; //nuevacolumna columna
            if(nuevafila >= 0 && nuevafila < 17 && nuevacolumna >= 0 && nuevacolumna < 19) //verificar que este en los rangos de la matriz
            {
                if(!maze[nuevafila,nuevacolumna])//verificar que no sea una pared y no se haya visitado
                {
                    posibleMoves[nuevafila,nuevacolumna] = true; //marcar los valores
                    DFS(nuevafila,nuevacolumna, moves - 1);//llamar con la nueva posicion 
                }
            }
        }
    }
    private void UpdateMatrix()//guardar en maze el laberinto de la escena en el frame exacto!!
    {
        //inicializr ambas matrices 
        maze = new bool[17,19];
        for (int i = 0; i < 17; i++) //iterar por las filas
        {
            for (int j = 0; j < 19; j++) //iterar por las columnas 
            {
                //si tiene un solo hijo o bien es una hierba o es una pared y si el hijo tiene la tarjeta entonces solamente podria ser una pared 
                if(GameManager.instancia.maze.transform.GetChild(i).transform.GetChild(j).transform.GetChild(0).transform.tag == "wall")
                {
                    maze[i,j] = true; //marcar la celda correspondiente en true en el laberinto booleano 
                }
                else if(GameManager.instancia.maze.transform.GetChild(i).transform.GetChild(j).transform.childCount == 2) //si tiene exactamente dos hijos verificar si uno de ellos es una roca para evitar el paso 
                {
                    int index = GameManager.instancia.maze.transform.GetChild(i).transform.GetChild(j).childCount-1;//buscar el inidice del ultimo hijo 
                    if(GameManager.instancia.maze.transform.GetChild(i).transform.GetChild(j).transform.GetChild(index).tag == "extras") //verificar que tenga la tarjeta de extras
                    {
                        //verificar que sea una roca 
                        if(GameManager.instancia.maze.transform.GetChild(i).transform.GetChild(j).transform.GetChild(index).GetComponent<TrapVisual>().name == "Rock")
                        {
                            maze[i,j] = true; //en el caso de que sea una roca marcarla como obstaculo (true)
                        }
                    }
                }
            }
        }
    }
    private void InvalidOperationsWithOthers() //una vez se presione cualquier tecla, osea que te muevas con un heroe se invalidan los movimientos con los demas 
    {
        if(!GameManager.instancia.currentPlayer) // en caso de que sea el jugador 1
        {
            for (int i = 0; i < GameManager.instancia.herosPlayer1.Count ; i++)//iterar por los heroes del jugador 1
            {
                if(GameManager.instancia.herosPlayer1[i].GetComponent<HeroVisual>().hero.name != currentHero.name) // si tiene nombre distinto al current hero se le desactiva el componente
                {
                    GameManager.instancia.herosPlayer1[i].GetComponent<NPCMove>().enabled = false;
                }
            }
        }
        else // en caso de que sea el jugador 2
        {
            for (int i = 0; i < GameManager.instancia.herosPlayer2.Count ; i++) //iterar por los heroes del jugador 2
            {
                if(GameManager.instancia.herosPlayer2[i].GetComponent<HeroVisual>().hero.name != currentHero.name) // si tiene nombre distinto al current hero se le desactiva el componente
                {
                    GameManager.instancia.herosPlayer2[i].GetComponent<NPCMove>().enabled = false;
                }
            }
        }
    }
    public void OnPassButtonPressed()//cuando se presiona el boton de pasar turno
    {
        clikedObjectImage.sprite = GameManager.clikedObjectFija; //cambiar la imagen a la imagen por default
        if(GameManager.instancia.currentPlayer) GameManager.instancia.currentPlayer = false; //cambiar el valor de current player 
        else GameManager.instancia.currentPlayer = true; //cambiar el valor de current player 
        n = 0;
        GameManager.instancia.PrepareGame(); // se llama a preparar el laberinto respecto al jugador actual, osea apagar y encender los componentes de movimiento
        currentHero = null; //restablecer el current hero 
        seMovio = false; //restablecer el valor de si ha habido movimiento o no ya que se va a cambiar de jugador
    }
    void Update() //detectar teclas presionadas en cada frame
    {
       DetectPressedKeys(); //detectar las teclas presionadas en cada frame 
    }
}