using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public GameObject shopShop; //guardar el objeto que contiene todo sobre la tienda
    void Start()
    {
        
    }
    public void InitTheShopping()
    {
        shopShop.SetActive(true);
    }
    public void OnBuyButtonPressed()
    {

    }
    public void OnExitButtonPressed()
    {
        shopShop.SetActive(false);
    }
}
