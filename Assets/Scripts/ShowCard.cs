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
        if(GetComponent<VisualCard>() is not null) CurrentCard = GetComponent<VisualCard>().card;
        if(GetComponent<VisualCard>() is not null)
        {
            Sprite TemporalImage = GetComponent<VisualCard>().card.CardPhoto;
            if(TemporalImage is not null)
            {
                Sprite sourceImage = GetComponent<VisualCard>().card.CardPhoto;
                if (sourceImage != null)
                {
                    GameObject.Find("Photo").GetComponent<Image>().sprite = sourceImage;
                } 
            }
        }
        if(CurrentCard.Type == CardType.oro || CurrentCard.Type == CardType.plata)
        {
            string attackTypes = string.Join(", ", CurrentCard.Range.Select(r => "\"" + r.ToString() + "\""));
            CardInformation.text = $"Name: {CurrentCard.Name}\nPower: {CurrentCard.Power}\nFaction: {CurrentCard.Faction}\nCardType: {CurrentCard.Type}\nAttackType: {attackTypes}\nEffect: {CurrentCard.EffectType}";
            CardInformationShadow.text = $"Name: {CurrentCard.Name}\nPower: {CurrentCard.Power}\nFaction: {CurrentCard.Faction}\nCardType: {CurrentCard.Type}\nAttackType: {attackTypes}\nEffect: {CurrentCard.EffectType}";
        }
        else if (CurrentCard.Type == CardType.aumento || CurrentCard.Type == CardType.senuelo || CurrentCard.Type == CardType.despeje || CurrentCard.Type == CardType.clima)
        {
            CardInformation.text = $"Name: {CurrentCard.Name}\nFaction: {CurrentCard.Faction}\nCardType: {CurrentCard.Type}\nEffect: {CurrentCard.EffectType}";
            CardInformationShadow.text = $"Name: {CurrentCard.Name}\nFaction: {CurrentCard.Faction}\nCardType: {CurrentCard.Type}\nEffect: {CurrentCard.EffectType}";
        }
        else if (CurrentCard.Type == CardType.lider)
        {
            CardInformation.text = $"Name: {CurrentCard.Name}\nFaction: {CurrentCard.Faction}\nCardType: {CurrentCard.Type}\nAbility: {CurrentCard.EffectLeader}";
            CardInformationShadow.text = $"Name: {CurrentCard.Name}\nFaction: {CurrentCard.Faction}\nCardType: {CurrentCard.Type}\nAbility: {CurrentCard.EffectLeader}";
        }
    }
}