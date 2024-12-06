using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public enum Hability{Detruction , HighSpeed , HighVision , BigStrengh , Carefull}

[CreateAssetMenu(fileName = "New Hero", menuName = "Hero")]
public class Hero : ScriptableObject 
{
    public new string name;
    public Sprite heroPhoto;
    public Hability hability;
    public int coolingTime;
    public int speed;
    public int life;
    public string habilityDescription;
}
