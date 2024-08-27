public class Parser
{
    private List<Token> tokens; // lista de tokens a analizar 
    int position;
    private Token CurrentToken => position < tokens.Count ? tokens[position] : null!;
    private Context context = new Context();
    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
        this.position = 0;
    }
    void NextToken() => position +=1;
    Token PeekNexToken()
    {
        if(position + 1 < tokens.Count) return tokens[position + 1];
        else return null;
    }
    void Expect(string TokenType)
    {
        if(CurrentToken.TokenType == TokenType) NextToken();
        else throw new Exception($"Not expected token ({CurrentToken.TokenType}), token have been expected is {TokenType}" + $"  {CurrentToken.TokenValue}" + $"{tokens.IndexOf(CurrentToken)}");
    }
    
    public List<ASTNode> Parse()
    {
        List<ASTNode> ToReturn = new List<ASTNode>();
        while(CurrentToken is not null)
        {
            if(CurrentToken.TokenValue == "effect") 
            {
                ToReturn.Add(Effect_Parse());
            }
            else if(CurrentToken.TokenValue == "card")
            {
                ToReturn.Add(Cards_Parse());
            }
            else throw new Exception("unexpected Token value, token have been expected (card) or (effect)" + $"  {CurrentToken.TokenValue}" + $"{tokens.IndexOf(CurrentToken)}");
        }
        return ToReturn;
    }
    public EffectNode Effect_Parse()
    {
        Expect("PalabrasReservadas"); //effect
        Expect("Delimitadores"); //{

        EffectNode effect = new EffectNode();
        Context effectContext = new Context();

        Expect("Identificadores"); // Name
        Expect("OperadoresDeAsignacion"); // : 
        //actualizo el valor del nombre
        effect.Name = CurrentToken.TokenValue;

        Expect("Identificadorestring");
        Expect("Coma"); // brinco
        context.DeffineEffect(effect.Name,effect); //definir el efecto en el contexto
        //parsear params 
        if(CurrentToken.TokenValue == "Params")
        {
            Expect("Identificadores"); // Params 
            Expect("OperadoresDeAsignacion"); // :
            Expect("Delimitadores"); // {
            while(CurrentToken.TokenValue != "}")
            {
                string paramsName = CurrentToken.TokenValue;
                Expect("Identificadores");
                Expect("OperadoresDeAsignacion");
                object paramsType = CurrentToken.TokenValue;
                Expect("Identificadores");
                effect.Params [paramsName] = paramsType;

                // effectContext.DefineVariable(paramsName,paramsType);
                context.DefineVariable(paramsName,paramsType);


                if(CurrentToken.TokenType == "Coma") Expect("Coma"); 
            }
            Expect("Delimitadores");
            Expect("Coma");
        }
        // parsear el action
        Expect("Identificadores");
        Expect("OperadoresDeAsignacion");
        Expect("Delimitadores");
        Expect("Identificadores");
        Expect("Coma");
        Expect("Identificadores");
        Expect("Delimitadores"); // se acaba el metodo
        Expect("OperadorLanda"); // =>
        Expect("Delimitadores"); // { empieza el cuerpo del metodo
        ParseAction();
        Expect("Delimitadores"); // se acaba el cuerpo de efecto

        return effect;
    }
    void ParseAction()
    {
        Expect("Delimitadores");
    }
    public CardNode Cards_Parse()
    {
        Expect("PalabrasReservadas"); //comenzar con el cuerpo de card
        Expect("Delimitadores");

        CardNode card = new CardNode();

        while(CurrentToken.TokenValue != "}")
        {
            if(CurrentToken.TokenValue == "Type")
            {
                // Type
                Expect("Identificadores");
                Expect("OperadoresDeAsignacion");
                card.Type = CurrentToken.TokenValue;
                Expect("Identificadorestring");
            }
            
            else if(CurrentToken.TokenValue == "Name")
            {
                //Name
                Expect("Identificadores");
                Expect("OperadoresDeAsignacion");
                card.Name = CurrentToken.TokenValue;
                Expect("Identificadorestring");
            }

            else if(CurrentToken.TokenValue == "Faction")
            {
                //Faction 
                Expect("Identificadores");
                Expect("OperadoresDeAsignacion");
                card.Faction = CurrentToken.TokenValue;
                Expect("Identificadorestring");
            }

            else if(CurrentToken.TokenValue == "Power")
            {
                //Power
                Expect("Identificadores");
                Expect("OperadoresDeAsignacion");
                card.Power = int.Parse(CurrentToken.TokenValue);
                Expect("PatronDeNumero");
            }

            else if(CurrentToken.TokenValue == "Range")
            {
                //Range
                Expect("Identificadores");
                Expect("OperadoresDeAsignacion");
                Expect("Delimitadores");
                while(CurrentToken.TokenValue != "]")
                {
                    card.Range.Add(CurrentToken.TokenValue);
                    Expect("Identificadorestring");
                    if(CurrentToken.TokenType == "Coma") Expect("Coma");
                }
                Expect("Delimitadores"); //se guardo correctamente el Range
            }

            else if(CurrentToken.TokenValue == "OnActivation")
            {
                //On Activation
                Expect("Identificadores");
                Expect("OperadoresDeAsignacion");
                if(CurrentToken.TokenValue == "[") Expect("Delimitadores");
                if(CurrentToken.TokenValue == "{") Expect("Delimitadores");
                
                bool isFirst =  true;
                while(CurrentToken.TokenValue != "]")
                {
                    card.Effects.Add(ParseActivation(isFirst));
                    isFirst = false;
                    while(CurrentToken.TokenValue == "}" || CurrentToken.TokenValue == ",")
                    {
                        NextToken();
                    }
                }
            }
            if(CurrentToken.TokenType == "Coma") Expect("Coma");
            else Expect("Delimitadores");
        }
        Expect("Delimitadores");
        return card;
    }
    public OnActivationNode ParseActivation(bool isFirst)
    {
        if (!isFirst)
        {
            Expect("Identificadores");
            Expect("OperadoresDeAsignacion");
            Expect("Delimitadores");
        }
        OnActivationNode onActivationNode = new OnActivationNode();


        while(CurrentToken.TokenValue != "}" && CurrentToken.TokenValue != "PostAction")
        {
            if(CurrentToken.TokenValue == "Effect")
            {

                Expect("Identificadores");
                Expect("OperadoresDeAsignacion");
                onActivationNode.effect = ParseCardEffect();
                Expect("Coma");
            }
            else if(CurrentToken.TokenValue == "Selector")
            {
                SelectorNode selectorNode = new SelectorNode();
                PredicateNode predicateNode = new PredicateNode();

                Expect("Identificadores");
                Expect("OperadoresDeAsignacion");
                Expect("Delimitadores");
                //Source
                Expect("Identificadores");
                Expect("OperadoresDeAsignacion");
                selectorNode.Source = CurrentToken.TokenValue;
                Expect("Identificadorestring");
                Expect("Coma");
                //Single
                Expect("Identificadores");
                Expect("OperadoresDeAsignacion");
                if(CurrentToken.TokenValue == "true") selectorNode.Single = true;
                else if(CurrentToken.TokenValue == "false") selectorNode.Single = false;
                Expect("Booleano");
                Expect("Coma");
                //Predicate
                Expect("Identificadores");
                Expect("OperadoresDeAsignacion");
                Expect("Delimitadores");
                Expect("Identificadores");
                Expect("Delimitadores");
                Expect("OperadorLanda");

                string mIzq = CurrentToken.TokenValue;
                string operador;
                Expect("Identificadores");
                mIzq += CurrentToken.TokenValue;
                Expect("Punto");
                mIzq += CurrentToken.TokenValue;
                // actualizar Miembro Izquierdo
                predicateNode.MiembroIzq = mIzq;
                System.Console.WriteLine(predicateNode.MiembroIzq);
                Expect("Identificadores");
                operador = CurrentToken.TokenValue;
                predicateNode.Operador = operador;
                Expect("OperadoresDeComparacion");
                Object mDer = CurrentToken.TokenValue;
                predicateNode.MiembroDer = mDer;
                if(CurrentToken.TokenType == "PatronDeNumero") Expect("PatronDeNumero");
                else if(CurrentToken.TokenType == "Booleano") Expect("Booleano");
                else if(CurrentToken.TokenType == "Identificadorestring") Expect("Identificadorestring");
                Expect("Delimitadores");
                onActivationNode.selector = selectorNode;
                Expect("Coma");
            }
        }
        return onActivationNode;
    }
    public CardEffectNode ParseCardEffect()
    {
        Expect("Delimitadores");
        CardEffectNode cardEffectNode = new CardEffectNode();

        while(CurrentToken.TokenValue != "}")
        {
            if(CurrentToken.TokenValue == "Name")
            {
                Expect("Identificadores"); //Name
                Expect("OperadoresDeAsignacion");
                string Metodo = CurrentToken.TokenValue;
                if(!context.Effects.ContainsKey(Metodo)) throw new Exception($"Current Effect {Metodo} does not exist in the current context");
                cardEffectNode.Name = Metodo;
                Expect("Identificadorestring");
                Expect("Coma");
            }
            else if(CurrentToken.TokenType == "Identificadores")
            {
                while(CurrentToken.TokenValue != "}")
                {
                    if(!context.Variables.ContainsKey(CurrentToken.TokenValue)) throw new Exception($"Current Variable is not instance yet {CurrentToken.TokenValue}");
                    Expect("Identificadores");
                    Expect("OperadoresDeAsignacion");
                    if(CurrentToken.TokenType == "PatronDeNumero") Expect("PatronDeNumero");
                    else if(CurrentToken.TokenType == "Identificadorestring") Expect("Identificadorestring");
                    else if(CurrentToken.TokenType == "Booleano") Expect("Booleano");
                    Expect("Coma");
                }
            }
        }
        Expect("Delimitadores");
        return cardEffectNode;
    }
}