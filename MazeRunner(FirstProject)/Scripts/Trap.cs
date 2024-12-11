using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newTrap", menuName = "Trap", order = 0)]
public class Trap : ScriptableObject //escriptable object para tener trampas fisicas en unity 
{
    public string Name;
    public int Penalty;
    public Sprite TrapPhoto;
    public string Description;
    public AudioClip audioClip1;
    public AudioClip audioClip2;
}