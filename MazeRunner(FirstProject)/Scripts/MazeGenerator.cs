using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MazeGenerator
{
    //guardar las posiciones donde se colocaran los bloques del laberinto 
    public static List<(int a , int b)> values;
    
    //metodo principal q se encarga de controlar la creacion del laberinto
    public static void Starting()
    {
        values = new List<(int a, int b)>(); // instanciar la lista para evitar null references 
        GenerateMaze(); //llamar a generar el laberinto
        while(!IsValid()) //mientras q la disposicion de obstaculos actuales no conformen un laberinto valido se genera otro
        {
            GenerateMaze(); //volver a generar otro laberinto 
        }
        bool [,] imprimir = new bool [16,16]; //matriz axiliar donde se guradara la posicion de cada obstaculo 
        for (int i = 0; i < values.Count ; i++)
        {
            if(values[i] != (0,1) && values[i] != (15,14))
            {
                imprimir[values[i].a,values[i].b] = true;
            }
        }
        bool[,] mapa = new bool [16,16];
        MarcarIslas(imprimir,mapa);//marcar todas las casillas alcanzadas en el mapa 
        //transcribir en una sola matriz la posterior representacion visual del laberinto 
        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                if(!mapa[i,j] && !imprimir[i,j]) imprimir[i,j] = true; //corregir los islotes en el tablero origninal antes de instanciar 
            }
        }
        InstantiateMaze(imprimir); //llamado a instanciar los objetos en el visual 

    }
    //rellenar los islotes de manera q cualquier laberinto con un recorrido valido no tenga islas 
    private static void MarcarIslas(bool [,] imprimir , bool [,] mapa)
    {
        Marcar(imprimir,mapa,0,1);
    }
    //metodo recursivo encargado de encontrar toda casilla q se visite 
    private static void Marcar(bool [,] imprimir ,bool [,] mapa , int x , int y)
    {
        //verificar q este dentro de los rangos de la matriz 
        if(x>=0 && x<mapa.GetLength(0) && y>=0 && y<mapa.GetLength(1))
        {
            if(!imprimir[x,y] && !mapa[x,y]) //verificar q no es un obstaculo y no se ha visitado
            {
                mapa[x,y]=true;
                Marcar(imprimir,mapa,x+1,y); //abajo
                Marcar(imprimir,mapa,x,y+1); //derecha
                Marcar(imprimir,mapa,x-1,y); //arriba
                Marcar(imprimir,mapa,x,y-1); //izquierda 
            }
        }
    }
    
    //verificar si se tiene un laberinto valido osea uno donde haya salida
    public static bool IsValid()
    {
        //crear la matriz para pasarle como parametro al metodo Maze 
        bool [,] bools = new bool[16,16];
        //  iniciar la variable de ciclo del for en 2 para no tomar los dos primero valores q serian la primera casilla y la ultima 
        for (int i = 0; i < values.Count ; i++)
        {
            if(values[i] != (0,1) && values[i] != (15,14))
            {
                bools[values[i].a , values[i].b] = true;
            }
        }
        return Lee(bools , 0 , 1);
    }

    //algoritmo de lee, eficiente para veificar si el laberinto tiene salida o no 
    public static bool Lee (bool [,] a , int fila, int columna)
    {
        int [,] costos = new int [a.GetLength(0),a.GetLength(1)];
        costos[fila,columna]=1;
        //             der abaj izq arribita
        int [] filas = { 0 , 1 , 0 , -1 };
        int [] columnas = { 1 , 0 , -1 , 0 };
        bool marlon;
        do
        {
            marlon=false;
            for(int x=0 ; x<a.GetLength(0) ; x++)
            {
                for(int y=0 ; y<a.GetLength(1) ; y++)
                {
                    if(a[x,y]==true) continue;
                    if(costos[x,y]==0) continue;
                    for(int d=0 ; d<filas.Length ; d++)
                    {
                        int Factual = x+filas[d];
                        int Cactual = y+columnas[d];
                        if(Posicion(a.GetLength(0),a.GetLength(1),Factual,Cactual) && !a[Factual,Cactual] && costos[Factual,Cactual]==0)
                        {
                            costos[Factual,Cactual] = costos[x,y]+1;
                            marlon=true;
                        }
                    }
                }
            }
        }
        while(marlon);
        if(costos[15,14]!=0) return true; //el caso de q tenga un costo llegar a dicha casilla es q se pudo llegar 
        return false;
    }

    //verifcar si la posicion esta dentro de los rangos de la matriz 
    public static bool Posicion (int totalfilas , int totalcolumnas , int h , int k)
    {
        return totalfilas > h && totalcolumnas > k && h >= 0 && k >= 0;
    }

    // instanciar los objetos en sus respectivas posiciones
    private static void InstantiateMaze(bool [,] imprimir )
    {
        // se inicializa el ciclo desde la posicion 2 paa evitar instanciar el inicio y la salida, la salida y el incio respectivamente para ambos miebros 
        for (int i = 0; i < imprimir.GetLength(0) ; i++)
        {
            for (int j = 0; j < imprimir.GetLength(1) ; j++)
            {
                if((i == 0 && j == 1 ) || (i == 15 && j == 14)) continue;
                if(imprimir[i,j])
                {
                    GameObject pared = GameObject.Instantiate(GameManager.instancia.bloque,GameManager.instancia.maze.transform.GetChild(i).GetChild(j).transform);
                    pared.transform.localPosition = new Vector3(0,0,0);
                    pared.transform.parent = GameManager.instancia.maze.transform.GetChild(i).GetChild(j).transform;
                }
                else
                {
                    GameObject pared = GameObject.Instantiate(GameManager.instancia.hierba,GameManager.instancia.maze.transform.GetChild(i).GetChild(j).transform);
                    pared.transform.localPosition = new Vector3(0,0,0);
                    pared.transform.parent = GameManager.instancia.maze.transform.GetChild(i).GetChild(j).transform;
                }
            }
        }
        // instanciar la casilla incial
        GameObject inicio = GameObject.Instantiate(GameManager.instancia.hierba,GameManager.instancia.maze.transform.GetChild(0).GetChild(1).transform);
        inicio.transform.localPosition = new Vector3(0,0,0);
        inicio.transform.parent = GameManager.instancia.maze.transform.GetChild(0).GetChild(1).transform;
        // instanciar la casilla final
        GameObject final = GameObject.Instantiate(GameManager.instancia.hierba,GameManager.instancia.maze.transform.GetChild(15).GetChild(14).transform);
        final.transform.localPosition = new Vector3(0,0,0);
        final.transform.parent = GameManager.instancia.maze.transform.GetChild(15).GetChild(14).transform;
    }
    
    //generar posiciones randoms en el tablero para los obstaculos del laberinto
    public static void GenerateMaze()
    {
        int n = 0; // variable para contabilizar la cantidad de elementos q se le agregaran al laberinto
        System.Random random = new System.Random();
        values.Clear();
        //agregar elementos de las paredes verticales y horizontales del laberinto 
        for (int i = 0; i < 16; i++)
        {
            values.Add((i,0));
            values.Add((i,15));
            if(!values.Contains((0,i))) values.Add((0,i));
            if(!values.Contains((15,i))) values.Add((15,i));
        }

        //generar 65 posiciones random para un posible laberinto
        while(n < 65)
        {
            int x = random.Next(0,16);
            int y = random.Next(0,16);
            // se verifica si ya la posicion no ha sido tomada anteriormente
            if (!values.Contains((x,y)))
            {
                values.Add((x,y));//se agrega una nueva posicion a la lista 
                n++;//se incrementa la cantidad de elementos agregados 
                continue;
            }
        }
    }
    public static void GenerateHeros()
    {
        GameObject hero1 = GameObject.Instantiate(GameManager.instancia.Tommy,GameManager.instancia.maze.transform);//guardar una instancia del heroe
        hero1.transform.localPosition = new Vector3(86,0,0);//reasignar la posicion del heroe en su casilla correspondiente 
        // GameManager.instancia.Tommy.transform.GetComponent<NPCMove>().enabled = false;//desactivar la capacidad de movimiento una vez lograda la instancia 
        // GameManager.instancia.Tommy.transform.localPosition = new Vector3(86,0,0);
    }

}