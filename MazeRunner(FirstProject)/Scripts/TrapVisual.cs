using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrapVisual : MonoBehaviour //clase mono behavior para poder enlazar el visual con el codigo 
{
    public Trap trap;
    public new string name;
    public Image trapPhoto;
    public string description;
    private void Start()
    {
        //..
    }
    public void InitializeTrap() //enlazar el objeto fisico de unity con la parte visual
    {
        trapPhoto.sprite = trap.TrapPhoto; //asignar la imagen
        name = trap.Name;//asignar el nombre para su facil acceso 
        this.description = trap.Description; //asignar la descripcion para su facil acceso
    }
}
