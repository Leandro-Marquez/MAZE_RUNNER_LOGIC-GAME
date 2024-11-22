using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum CardType {oro,plata,clima,aumento,despeje,senuelo,lider}
public enum Faction {Elementales , Oscuridad}
public enum Range {M,R,S}
public enum CardEffects{oro,plata,senuelo,despeje,clima,aumento,No_Effect, Created}
public enum LiderEffects{Destruccion,Recuperacion}

[CreateAssetMenu(fileName = "New Card" , menuName = "Card")]
public class Card : ScriptableObject
{
    public string Name;
    public int Power;
    public bool IsCreated;
    public Sprite CardPhoto;
    public int Owner;
    public CardType Type;
    public Faction Faction;
    public Range [] Range;
    public CardEffects EffectType;
    public LiderEffects EffectLeader;
    public List<EffectsDefinition> OnActivation;
    public EffectCreated EffectCreated;
    public void ActivateEffect(GameObject DroppedCard)
    {
        if (DroppedCard.GetComponent<VisualCard>().card.IsCreated)
        {
            foreach (var effect in OnActivation)
            {
                Debug.Log("Owner: " + Owner);
                ActivateSpecificEffect(effect, effect.Params);
            }
        }
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
        private void ActivateSpecificEffect(EffectsDefinition effect, List<object> prms)
        {
            var effectMethod = typeof(EffectCreated).GetMethod(effect.Name.Substring(1, effect.Name.Length - 2) + "Effect");
            if (effectMethod != null)
            {

                if(prms.Count == 0 || prms == null)
                {
                    var targetList = effect.Targets ; 
                    effectMethod.Invoke(EffectCreated, new object[] { targetList, context.Instance }); 
                }
                else if(prms.Count == 1)
                {
                    var targetList = effect.Targets; 
                    effectMethod.Invoke(EffectCreated, new object[] { targetList, context.Instance, int.Parse(prms[0].ToString()!) }); 
                }
                else if(prms.Count == 2)
                {
                    var targetList = effect.Targets; 
                    effectMethod.Invoke(EffectCreated, new object[] { targetList, context.Instance, int.Parse(prms[0].ToString()!), int.Parse(prms[1].ToString()!) }); 
                }
                else if(prms.Count == 3)
                {
                    var targetList = effect.Targets; 
                    effectMethod.Invoke(EffectCreated, new object[] { targetList, context.Instance, int.Parse(prms[0].ToString()!), int.Parse(prms[1].ToString()!), int.Parse(prms[2].ToString()!) }); 
                }
                else
                {
                    var targetList = effect.Targets; 
                    effectMethod.Invoke(EffectCreated, new object[] { targetList, context.Instance, int.Parse(prms[0].ToString()!), int.Parse(prms[1].ToString()!), int.Parse(prms[2].ToString()!), int.Parse(prms[3].ToString()!) }); 
                }
            }
            else
            {
                Console.WriteLine($"Efecto no encontrado: {effect.Name}");
            }
        }
}