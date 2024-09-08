using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImageTransfer : MonoBehaviour, IPointerEnterHandler
{
    public Image targetImage; // La imagen del objeto destino
    public TextMeshProUGUI CardInformation;
    public TextMeshProUGUI CardInformationShadow;
    private Card CurrentCard;

    public void Start()
    {
        targetImage = GameObject.Find("Photo").GetComponent<Image>();
        CardInformation = GameObject.Find("Info").GetComponent<TextMeshProUGUI>();
        CardInformationShadow = GameObject.Find("ShadowInfo").GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GetComponent<VisualCard>() is not null)
        {
            CurrentCard = GetComponent<VisualCard>().card;
            Sprite TemporalImage = GetComponent<VisualCard>().card.CardPhoto;
            if (TemporalImage is not null)
            {
                Sprite sourceImage = GetComponent<VisualCard>().card.CardPhoto;
                if (sourceImage != null)
                {
                    GameObject.Find("Photo").GetComponent<Image>().sprite = sourceImage;
                }
            }

            // Mostrar información de la carta
            string cardInfo = $"Name: {CurrentCard.Name}\nFaction: {CurrentCard.Faction}\nCardType: {CurrentCard.Type}";
            if (CurrentCard.Type == CardType.oro || CurrentCard.Type == CardType.plata)
            {
                string attackTypes = string.Join(", ", CurrentCard.Range.Select(r => "\"" + r.ToString() + "\""));
                cardInfo += $"\nPower: {CurrentCard.Power}\nAttackType: {attackTypes}\nEffect: {CurrentCard.EffectType}";
            }
            else if (CurrentCard.Type == CardType.aumento || CurrentCard.Type == CardType.senuelo || CurrentCard.Type == CardType.despeje || CurrentCard.Type == CardType.clima)
            {
                cardInfo += $"\nEffect: {CurrentCard.EffectType}";
            }
            else if (CurrentCard.Type == CardType.lider)
            {
                cardInfo += $"\nAbility: {CurrentCard.EffectLeader}";
            }
            
            // Añadir el nombre del tipo de OnActivation si no es null
            if (CurrentCard.OnActivation != null)
            {
                string mengano = "";
                int fulano = CurrentCard.OnActivation.Count;
                for(int x=0 ; x<fulano ; x++)
                {
                    mengano += CurrentCard.OnActivation[x].Name;
                    mengano += "  ,";
                }
                cardInfo += $"\nOn Activation Type: {mengano}";
            }

            CardInformation.text = cardInfo;
            CardInformationShadow.text = cardInfo;
        }
    }
}