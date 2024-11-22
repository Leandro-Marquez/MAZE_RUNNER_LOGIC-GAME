using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroVisual : MonoBehaviour
{
    public Hero hero;
    public Image heroPhoto;
    public void Start ()
    {
        if(hero is not null ) InitializeHero();
    }
    public void InitializeHero()
    {
        heroPhoto.sprite = hero.heroPhoto;
    }
}
