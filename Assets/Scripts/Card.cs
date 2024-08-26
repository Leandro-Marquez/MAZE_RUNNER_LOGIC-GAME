using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum CardType {oro,plata,clima,aumento,despeje,senuelo,lider}
public enum CardFaction {Elementales , Oscuridad}
public enum Range {M,R,S}
public enum CardEffects{oro,plata,senuelo,despeje,clima,aumento,No_Effect}
public enum LiderEffects{Destruccion,Recuperacion}

[CreateAssetMenu(fileName = "New Card" , menuName = "Card")]
public class Card : ScriptableObject
{
    public string Name;
    public int Power;
    public Sprite CardPhoto;
    public int Owner;
    public CardType Type;
    public CardFaction Faction;
    public Range [] Range;
    public CardEffects EffectType;
    public LiderEffects EffectLeader;
    public List<int> OnActivation; // cambiar a lista de efectos 
    public void ActivateEffect(GameObject DroppedCard)
    {
        if(EffectType == CardEffects.aumento)
        {
            Effects.Aumento(DroppedCard.transform);
        }
        else if(EffectType == CardEffects.clima)
        {
            Effects.ClimaResta(DroppedCard.transform.parent);
        }
        else if(EffectType == CardEffects.senuelo)
        {
            Effects.Senuelo(DroppedCard.transform.parent);
        }
        else if(EffectType == CardEffects.despeje)
        {
            Effects.Despeje(DroppedCard.transform.parent);
        }
        else if(EffectType == CardEffects.oro)
        {
            Effects.Oro(DroppedCard.transform);
        }
        else if(EffectType == CardEffects.plata)
        {
            Effects.Plata(DroppedCard.transform);
        }
    }
}

