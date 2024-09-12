using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

    public class CardList : IEnumerable<Card>
    {
        protected List<Card> cards = new List<Card>();

        public IEnumerator GetEnumerator()
        {
            return cards.GetEnumerator();
        }

        IEnumerator<Card> IEnumerable<Card>.GetEnumerator() // This line is redundant and can be removed
        {
            return cards.GetEnumerator();
        }

        //Devuelve todas las cartas que cumplen con un predicado
        public Card Find(Func<Card, bool> predicate)
        {
            return cards.FirstOrDefault(predicate);
        }

        //Agrega una carta al tope de la lista 
        public void Push(Card card)
        {
            cards.Add(card);
        }

        //Agrega una carta al fondo de la lista 
        public void SendBottom(Card card)
        {
            cards.Insert(0, card);
        }

        //Quita la carta que esta al tope y la devuelve 
        public Card Pop()
        {
            if (cards.Count == 0) return null;
            Card topCard = cards[cards.Count - 1];
            cards.RemoveAt(cards.Count - 1);
            return topCard;
        }

        //Remueve una carta de la lista 
        public void Remove(Card card)
        {
            cards.Remove(card);
        }

        //Mezcla la lista
        public void Shuffle()
        {
            System.Random rng = new System.Random();
            int n = cards.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Card value = cards[k];
                cards[k] = cards[n];
                cards[n] = value;
            }
        }

        //Agregar una card a la lista
        public void Add(Card card)
        {
            cards.Add(card);
        }
        //elimina todos los elementos de la lista
        public void Clear()
        {
            cards.Clear();
        } 

        public void AddRange(CardList cards)
        {
            this.cards.AddRange(cards.cards);
        }
        
        //devolver el total de elementos de la lista
        public int Count()
        {
            return cards.Count;
        }

        //colocar una carta en un lugar especifico
        public void Instert(int index, Card card)
        {
            cards.Insert(index, card);
        }
        //verificar si pertenece el elemento
        public bool Contains(Card card)
        {
            return cards.Contains(card);
        }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}