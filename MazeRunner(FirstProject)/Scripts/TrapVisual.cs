using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrapVisual : MonoBehaviour
{
    public Trap trap;
    public new string name;
    public Image trapPhoto;
    public string description;
    private void Start()
    {
        //..
    }
    public void InitializeTrap()
    {
        trapPhoto.sprite = trap.TrapPhoto;
        name = trap.Name;
        this.description = trap.Description;
    }
}
