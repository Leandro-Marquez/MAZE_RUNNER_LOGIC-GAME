using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroVisual : MonoBehaviour
{
    public Hero hero;
    public Image heroImage;
    public void Start ()
    {
        //..
    }
    public void InitializeHero()
    {
        heroImage.sprite = hero.heroPhoto;
    }
}
