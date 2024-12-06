using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newTrap", menuName = "Trap", order = 0)]
public class Trap : ScriptableObject
{
    public string Name;
    public int Penalty;
    public Sprite TrapPhoto;
    public string Description;
}