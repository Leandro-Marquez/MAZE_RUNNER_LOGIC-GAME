using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoAux : MonoBehaviour
{
    public TextMeshProUGUI heroName,heroNames;//nombre del heroe en la Info(sombra)
    public TextMeshProUGUI heroHability,heroHabilitys;//habilidad del heroe en la Info(sombra)
    public TextMeshProUGUI heroSpeed,heroSpeeds;//velocidad del heroe en la Info(sombra)
    public TextMeshProUGUI heroCoolingTime,heroCoolingTimes;//enfriamiento del heroe en la Info(sombra)
    public TextMeshProUGUI heroLife,heroLifes;//vida del heroe en la Info(sombra)
    public List<Hero> heros; //lista de heroes para facilitar el trabajo 
   public GameObject showHeroHabilityDescription; // para mostrar la habilidad del lider 
    public TextMeshProUGUI heroHabilityDescriptionText;//texto de la descripcion del heroe
    public TextMeshProUGUI heroHabilityDescriptionTexts;//... sombra
    public List<GameObject> lights; //lista de objetos contenedores con las imagenes verdes para indicar cual fue seleccionado
    public void OnClicked(GameObject clickedObject)
    {
        //reiniciar los valores de los textos para cada heroe
        heroName.text = "";
        heroNames.text = "";
        heroHability.text = "";
        heroHabilitys.text = "";
        heroSpeed.text = "";
        heroSpeeds.text = "";
        heroCoolingTime.text = "";
        heroCoolingTimes.text = "";
        heroLife.text = "";
        heroLifes.text = "";
        for (int i = 0; i < heros.Count ; i++)//buscar un heroe que coincida con el que se le hizo click para mostrar su info
        {
            if(heros[i].name == clickedObject.tag)
            {
                heroName.text += $"NAME: {heros[i].name}";
                heroNames.text += $"NAME: {heros[i].name}";
                heroHability.text += $"HABILITY: {heros[i].hability}";
                heroHabilitys.text += $"HABILITY: {heros[i].hability}";
                heroSpeed.text += $"SPEED: {heros[i].speed}";
                heroSpeeds.text += $"SPEED: {heros[i].speed}";
                heroCoolingTime.text += $"COOLING-TIME: {heros[i].coolingTime}";
                heroCoolingTimes.text += $"COOLING-TIME: {heros[i].coolingTime}";
                heroLife.text += $"LIFE: {heros[i].life}";
                heroLifes.text += $"LIFE: {heros[i].life}";
                clickedObject.gameObject.SetActive(true);
                PowerOnLights(clickedObject.tag);
                showHeroHabilityDescription.SetActive(true);
                heroHabilityDescriptionText.text = heros[i].habilityDescription;
                heroHabilityDescriptionTexts.text = heros[i].habilityDescription;
                return;
            }
        }
    }

    //apagar las luces verdes de los demas objetos 
    public void PowerOnLights(string tag) //recibir la targeta actual
    {
        for (int i = 0; i < lights.Count ; i++) //buscar entre todas las luces la correspondiente a dicha tarjeta 
        {
            if(lights[i].tag != tag) //verificar q los objetos tenga la tarjeta distinta al q ya se instancio 
            {
                lights[i].SetActive(false); //activar la luz 
            }
        }
    }
}