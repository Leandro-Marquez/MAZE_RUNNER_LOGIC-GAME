using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroVisual : MonoBehaviour //clase monobehavior para tratar los heroes de manera visual
{
    public Hero hero;
    public Image heroImage;
    public Owner owner;
    public void Start ()
    {
        //..
    }
    public void InitializeHero() //inicializar la foto del heroe en el 
    {
        heroImage.sprite = hero.heroPhoto;
    }
}
