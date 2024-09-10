public class EffectCreated
{
    public void DamageEffect(CardList targets, context context , int Amount)
    {
         UnityEngine.Debug.Log("EffectoEjecutado");
         UnityEngine.Debug.Log("Current:" + GameManager.Instancia.CurrentPlayer);
        foreach (Card target in targets)
        {
        var i = 0;
        while (i < Amount)
        {
         target.Power -= 1;
         i += 1;
        }
        }
    }

    public void AumentoEffect(CardList targets, context context , int Amount)
    {
         UnityEngine.Debug.Log("EffectoEjecutado");
         UnityEngine.Debug.Log("Current:" + GameManager.Instancia.CurrentPlayer);
        foreach (Card target in targets)
        {
        var j = 0;
        while (j < Amount)
        {
         target.Power += 1;
         j += 1;
        }
        }
    }

    public void DrawEffect(CardList targets, context context )
    {
         UnityEngine.Debug.Log("EffectoEjecutado");
         UnityEngine.Debug.Log("Current:" + GameManager.Instancia.CurrentPlayer);
        var topCard = context.Deck.Pop();
        context.Hand.Add(topCard);
        context.Hand.Shuffle();
    }

    public void ReturnToDeckEffect(CardList targets, context context )
    {
         UnityEngine.Debug.Log("EffectoEjecutado");
         UnityEngine.Debug.Log("Current:" + GameManager.Instancia.CurrentPlayer);
        foreach (Card target in targets)
        {
        var deck = context.Deck;
        deck.Push(target);
        deck.Shuffle();
        context.RemoveCard(target);
        }
    }

}
