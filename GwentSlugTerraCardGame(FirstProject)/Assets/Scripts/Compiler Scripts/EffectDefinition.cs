using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;

//definir un tipo de efecto para las cartass posteriormente 
public class EffectsDefinition 
{
    public string Name { get; set; } 
    public List<object> Params { get; set; } 
    public string Source { get; set; } 
    public bool Single { get; set; } 
    public Predicate Predicate { get; set; } 

    // CardList tipo definido de listas de carta para manejar con el contexto en cada actualizacion
    public CardList Targets => CreateTargets();
    private CardList CreateTargets()
    {
        CardList cards = new CardList();
        
        if (Source == "\"board\"")
        {
            foreach (Card unit in context.Instance.Board)
            {
                if (Predicate == null || EvaluatePredicate(unit))
                {
                    cards.Add(unit);
                }
            }
        }
        else if (Source == "\"hand\"")
        {
            foreach (Card unit in context.Instance.Hand)
            {
                if (Predicate == null || EvaluatePredicate(unit))
                {
                    cards.Add(unit);
                }
            }
        }
        else if (Source == "\"deck\"")
        {
            foreach (Card unit in context.Instance.Deck)
            {
                if (Predicate == null || EvaluatePredicate(unit))
                {
                    cards.Add(unit);
                }
            }
        }
        else if (Source == "\"cementery\"")
        {
            foreach (Card unit in context.Instance.Graveyard)
            {
                if (Predicate == null || EvaluatePredicate(unit))
                {
                    cards.Add(unit);
                }
            }
        }
        return cards;
    }
    //verifacar tipo de operacion y evaluar atendiendo a la operacion indicada
    private bool EvaluatePredicate(Card unit)
    {
        switch (Predicate.Operator)
        {
            case "==":
                return EvaluateEqualPredicate(unit);
            case "!=":
                return EvaluateNotEqualPredicate(unit);
            case "<=":
                return EvaluateLessThanOrEqualPredicate(unit);
            case ">=":
                return EvaluateGreaterThanOrEqualPredicate(unit);
            case "<":
                return EvaluateLessThanPredicate(unit);
            case ">":
                return EvaluateGreaterThanPredicate(unit);
            default:
                throw new Exception($"Operador no reconocido: {Predicate.Operator}");
        }
    }
    // evaluar el ==
    private bool EvaluateEqualPredicate(Card unit)
    {
        switch (Predicate.LeftMember)
        {
            case "Type":
                return unit.Type.ToString() == Predicate.RightMember.ToString();
            case "Faction":
                return "\"" + unit.Faction.ToString() + "\"" == Predicate.RightMember.ToString();
            case "Power":
                return unit.Power.ToString() == Predicate.RightMember.ToString();
            case "Range":
                return unit.Range.Contains(Enum.Parse<Range>(Predicate.RightMember.ToString()));
            default:
                throw new Exception($"Miembro izquierdo no reconocido: {Predicate.LeftMember}");
        }
    }
    // evaluar el !=
    private bool EvaluateNotEqualPredicate(Card unit)
    {
        switch (Predicate.LeftMember)
        {
            case "Type":
                return unit.Type.ToString() != Predicate.RightMember.ToString();
            case "Faction":
                return "\"" + unit.Faction.ToString() + "\"" != Predicate.RightMember.ToString();
            case "Power":
                return unit.Power.ToString() != Predicate.RightMember.ToString();
            case "Range":
                return !unit.Range.Contains(Enum.Parse<Range>(Predicate.RightMember.ToString()));
            default:
                throw new Exception($"Miembro izquierdo no reconocido: {Predicate.LeftMember}");
        }
    }
    //evaluar <= 
    private bool EvaluateLessThanOrEqualPredicate(Card unit)
    {
        switch (Predicate.LeftMember)
        {
            case "Power":
                return int.Parse(unit.Power.ToString()) <= int.Parse(Predicate.RightMember.ToString());
            default:
                throw new Exception($"Operador '<=' solo se puede usar con 'Power': {Predicate.LeftMember}");
        }
    }
    //evaluar >=
    private bool EvaluateGreaterThanOrEqualPredicate(Card unit)
    {
        switch (Predicate.LeftMember)
        {
            case "Power":
                return int.Parse(unit.Power.ToString()) >= int.Parse(Predicate.RightMember.ToString());
            default:
                throw new Exception($"Operador '>=' solo se puede usar con 'Power': {Predicate.LeftMember}");
        }
    }
    //evaluar <
    private bool EvaluateLessThanPredicate(Card unit)
    {
        switch (Predicate.LeftMember)
        {
            case "Power":
                return int.Parse(unit.Power.ToString()) < int.Parse(Predicate.RightMember.ToString());
            default:
                throw new Exception($"Operador '<' solo se puede usar con 'Power': {Predicate.LeftMember}");
        }
    }
    //evaluar >
    private bool EvaluateGreaterThanPredicate(Card unit)
    {
        switch (Predicate.LeftMember)
        {
            case "Power":
                return int.Parse(unit.Power.ToString()) > int.Parse(Predicate.RightMember.ToString());
            default:
                throw new Exception($"Operador '>' solo se puede usar con 'Power': {Predicate.LeftMember}");
        }
    }
}
//clase predicate para determinar source
public class Predicate
{
    public string LeftMember { get; set; }
    public string Operator { get; set; }
    public object RightMember { get; set; }
}