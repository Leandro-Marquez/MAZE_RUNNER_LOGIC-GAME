using System;
using System.Collections.Generic;
    public class Context
    {
        //tener refrencia a todas las variables creadas en el codigo 
        public Dictionary<string, object> Variables { get; } = new Dictionary<string, object>();
        //guardar los nodos efectos para verificar en la creacion de cartas si dichos efectos existen
        public Dictionary<string, EffectNode> Effects { get; } = new Dictionary<string, EffectNode>();
        //guardar los nodos de carta creados
        public Dictionary<string, CardNode> Cards { get; } = new Dictionary<string, CardNode>();

        private Context _parent;

        public Context(Context parent = null!)
        {
            _parent = parent;
        }
        //definir variable en el diccionario
        public void DefineVariable(string name, object value)
        {
            if (Variables.ContainsKey(name))
            {
                SetVariable(name, value);
            }
            Variables[name] = value;
        }
        //obtener valor de la variable en el diccionario
        public object GetVariable(string name)
        {
            if (Variables.ContainsKey(name))
            {
                return Variables[name];
            }
            else if (_parent != null)
            {
                return _parent.GetVariable(name);
            }

            throw new Exception($"Variable '{name}' no definida.");
        }
        //modificar valor de la variable 
        public void SetVariable(string name, object value)
        {
            if (!Variables.ContainsKey(name))
            {
                throw new Exception($"Variable '{name}' no definida.");
            }

            var currentValue = Variables[name];

            if (currentValue.GetType() != value.GetType())
            {
                throw new Exception($"No se puede asignar un valor de tipo '{value.GetType().Name}' a la variable '{name}' de tipo '{currentValue.GetType().Name}'.");
            }

            Variables[name] = value;
        }
        //definir un efecto en el diccionario con su respectivo nodo
        public void DeffineEffect(string name, EffectNode effect)
        {
            if (Effects.ContainsKey(name))
            {
                throw new Exception($"El efecto '{name}' ya esta definido en este contexto.");
            }
            Effects[name] = effect;
        }
        //obtener un nodo efecto del diccionario a traves del nombre 
        public EffectNode GetEffect(string name)
        {
            if (Effects.ContainsKey(name))
            {
                return Effects[name];
            }
            else if (_parent != null)
            {
                return _parent.GetEffect(name);
            }
            throw new Exception($"El efecto '{name}' no esta definido.");
        }
        //definir una carta con su respectivo nodo
        public void DeffineCard(string name, CardNode card)
        {
            if (Cards.ContainsKey(name))
            {
                throw new Exception($"La carta '{name}' ya esta definida en este contexto.");
            }
            Cards[name] = card;
        }
        //obtener un nodo carta a traves de su nombre 
        public CardNode GetCard(string name)
        {
            if (Cards.ContainsKey(name))
            {
                return Cards[name];
            }
            else if (_parent != null)
            {
                return _parent.GetCard(name);
            }
            throw new Exception($"La carta '{name}' no esta definida.");
        }
        
        public void PrintVariables()
        {
            Console.WriteLine("Variables:");
            foreach (var variable in Variables)
            {
                Console.WriteLine($"Name: {variable.Key}, Value: {variable.Value}");
            }
        }
        
        public void PrintEffects()
        {
            Console.WriteLine("Effects:");
            foreach (var effect in Effects)
            {
                Console.WriteLine($"Name: {effect.Key}, Effect: {effect.Value}");
            }
        }
        
        public void PrintCards()
        {
            Console.WriteLine("Cards:");
            foreach (var card in Cards)
            {
                Console.WriteLine($"Name: {card.Key}, Card: {card.Value}");
            }
        }
    }