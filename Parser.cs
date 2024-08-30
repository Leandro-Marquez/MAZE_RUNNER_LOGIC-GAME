using System.Collections.Generic;

public class Parser
{
    private List<Token> tokens; // lista de tokens a analizar 
    int position;
    private Token CurrentToken => position < tokens.Count ? tokens[position] : null!;
    private Context context = new Context();
    public static readonly Dictionary<string,(int procedencia,bool associaDerecha)> Operadores = new Dictionary<string,(int , bool associaDerecha)>{
    {"+",(4,false)},
    {"-",(4,false)},
    {"*",(5,false)},
    {"/",(5,false)},
    {"&&",(1,false)},
    {"||",(2,false)},
    {"==",(3,false)},
    {"!=",(3,false)},
    {"<=",(3,false)},
    {">=",(3,false)},
    {">",(3,false)},
    {"<",(3,false)},
    {"!",(1,true)},
    };
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
        // crear el nodo de efecto 
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
        // parsear el cuerpo del action
        while(CurrentToken.TokenValue != "}")
        {
            effect.Action.Hijos = ParseAction(effectContext);
        }

        return effect;
    }
    private List<ASTNode> ParseAction(Context effectcontext)
    {
        List<ASTNode> aSTNodes = new List<ASTNode>();

        while(CurrentToken.TokenValue != "}")
        {
            if(CurrentToken.TokenValue == "for")
            {
                aSTNodes.Add(ParseFor(effectcontext));
            }
            else if(CurrentToken.TokenValue == "while")
            {
                aSTNodes.Add(ParseWhile(effectcontext));
            }
            else if(CurrentToken.TokenValue == "if")
            {
                aSTNodes.Add(ParseIf(effectcontext));
            }
            else if(CurrentToken.TokenValue == "Identificadores" && PeekNexToken().TokenValue == ".")
            {
                aSTNodes.Add(ParseMemberAccess(effectcontext));
            }
            else if(CurrentToken.TokenType == "Identificadores" && PeekNexToken().TokenType == "OperadoresDeAsignacion")
            {
                aSTNodes.Add(ParseAssignment(null , effectcontext));
            }
            else throw new Exception("Current expression is not valid in the current context");
        }
        Expect("Delimitadores");
        return aSTNodes;
    }
    private AssignmentNode ParseAssignment(List<string> accessChain , Context effectontext)
    {
        AssignmentNode assignmentNode = new AssignmentNode();
        // asignar el nombre de la variable 
        string variableName = CurrentToken.TokenValue;
        assignmentNode.VariableName = CurrentToken.TokenValue;
        Expect("Identificadores");
        assignmentNode.Operator = CurrentToken.TokenValue;
        Expect("OperadoresDeAsignacion");

        if(PeekNexToken().TokenValue == ".")
        {
            var valueExpression = ParseMemberAccess(effectontext);
            assignmentNode.ValueExpression = valueExpression;
            assignmentNode.CadenaDeAcceso = accessChain;
            return assignmentNode;
        }
        else
        {
            var valueExpression = ParseExpressions(ExpressionsTokens(),false,effectontext);
            Expect("PuntoComa");
            AssignmentNode assignmentNode1 = new AssignmentNode{ValueExpression = valueExpression , CadenaDeAcceso = accessChain};
            if (effectontext.Variables.ContainsKey(variableName))
            {
                effectontext.SetVariable(variableName, valueExpression);
            }
            else
            {
                effectontext.DefineVariable(variableName, valueExpression);
            }
            return assignmentNode;
        }

    }
    private ASTNode ParseMemberAccess(Context context)
    {
        string objectName = CurrentToken.TokenValue;
        List<string> accessChain = new List<string>{objectName};
        
        // procesar los accesos anidados 
        while(PeekNexToken().TokenValue != ";")
        {
            Expect("Identificadores");
            Expect("Punto");
            string accessMember = CurrentToken.TokenValue;
            accessChain.Add(accessMember);
            
        }
        bool is_property = false;
        
        List<ExpressionNode> expressions = new List<ExpressionNode>();
        if(PeekNexToken().TokenValue == ";")
        {
            is_property = true;
            Expect("Identificadores");
            Expect("PuntoComa");
        }
        else if(PeekNexToken().TokenValue == "(")
        {
            is_property = false;
            Expect("Identificadores");
            Expect("Delimitadores");
            expressions = ParseArguments();
            Expect("Delimitadores");
            Expect("PuntoComa");
        }
        else if(PeekNexToken().TokenType == "OperadoresDeAsignacion")
        {
            var assignmentNode = ParseAssignment(accessChain,context);
            return assignmentNode;
        }
        return new MemberAccessNode{AccessChain = accessChain, Arguments = expressions , IsProperty = is_property};
        
    }
    private List<ExpressionNode> ParseArguments()
    {
        List<ExpressionNode> expressions = new List<ExpressionNode>();

        while(CurrentToken.TokenValue != ")")
        {
            if(CurrentToken.TokenType == "PatronDeNumero")
            {
                // agregar a la lista un nodo de numero
                expressions.Add(new NumberNode{Value = int.Parse(CurrentToken.TokenValue)});
                NextToken(); // avanzar en tokens
            }
            else if(CurrentToken.TokenType == "Booleano")
            {
                // agregar a la lista un nodo de booleano
                expressions.Add(new BooleanNode{Value = bool.Parse(CurrentToken.TokenValue)});
                NextToken();
            }
            else if(CurrentToken.TokenType == "Delimitadores")
            {
                NextToken();
            }
            else if(CurrentToken.TokenType == "Identificadores")
            {
                //agregar a la lista un nodo de referencia a variable
                expressions.Add(new VariableReferenceNode{Name = CurrentToken.TokenValue});
                NextToken();
            }
            else throw new Exception("Not implemented exception");
        }
        return expressions;
    }
    // arreglarrrrrrrrrrrrrrrrrrrrrrrrrrrrrr
    public ASTNode ParseFor(Context effectcontext)
    {
        ForNode forNode = new ForNode();

        Expect("PalabrasReservadas"); // for
        Expect("Delimitadores"); // (
        // asignar la variable del for
        forNode.Item = CurrentToken.TokenValue;
        Expect("Identificadores"); // target // item u algo asi
        Expect("PalabrasReservadas"); //in 

        forNode.Collection = new VariableReferenceNode {Name = CurrentToken.TokenValue};
        Expect("Identificadores"); //targets
        Expect("Delimitadores");// )
        Expect("Delimitadores"); //{
        //parsear el cuerpo del for
        while(CurrentToken.TokenValue != "}")
        {
            if(CurrentToken.TokenType == "Identificadores" && PeekNexToken().TokenType == "OperadoresDeAsignacion")
            {
                forNode.Body.Add(ParseAssignment(null,effectcontext));
            }
            else if(CurrentToken.TokenType == "Identificadores" && PeekNexToken().TokenValue == ".")
            {
                forNode.Body.Add(ParseMemberAccess(effectcontext));
            }
            else if(CurrentToken.TokenValue == "for")
            {
                forNode.Body.Add(ParseFor(effectcontext));
            }
            else if(CurrentToken.TokenValue == "while")
            {
                forNode.Body.Add(ParseWhile(effectcontext));
            }
            else if(CurrentToken.TokenValue == "if")
            {
                forNode.Body.Add(ParseIf(effectcontext));
            }
            else throw new Exception($"{CurrentToken.TokenValue} is not implemented");
        }
        Expect("Delimitadores"); // } se cierra el cuerpo del ciclo
        return forNode;
    }
    private WhileNode ParseWhile(Context effectcontext)
    {
        WhileNode whileNode = new WhileNode();
        Expect("PalabrasReservadas"); //while
        Expect("Delimitadores"); // (
        //parsear la condicion del while
        whileNode.Condition = ParseExpressions(ExpressionsTokens(), true, effectcontext);
        Expect("Delimitadores"); // ) se acaba la condicion de ejecucion y comienza el cuerpo
        Expect("Delimittadores"); // {
        while(CurrentToken.TokenValue != "}")
        {
            if(CurrentToken.TokenType == "Identificadores" && PeekNexToken().TokenType == "OperadoresDeAsignacion")
            {
                whileNode.Body.Add(ParseAssignment(null,effectcontext));
            }
            else if(CurrentToken.TokenType == "Identificadores" && PeekNexToken().TokenValue == ".")
            {
                whileNode.Body.Add(ParseMemberAccess(effectcontext));
            }
            else if(CurrentToken.TokenValue == "for")
            {
                whileNode.Body.Add(ParseFor(effectcontext));
            }
            else if(CurrentToken.TokenValue == "while")
            {
                whileNode.Body.Add(ParseWhile(effectcontext));
            }
            else if(CurrentToken.TokenValue == "if")
            {
                whileNode.Body.Add(ParseIf(effectcontext));
            }
            else throw new Exception($"{CurrentToken.TokenValue} is not implemented");
        }
        Expect("Delimitadores");
        return whileNode;
    }
    private IfNode ParseIf(Context effectcontext)
    {
        IfNode ifNode = new IfNode();
        
        Expect("PalabrasReservadas"); //if
        Expect("Delimitadores"); //comienza el cuerpo de la condicional 
        //parsear la condicion
        ifNode.Condition = ParseExpressions(ExpressionsTokens(),true,effectcontext);
        Expect("Delimitadores");

        while(CurrentToken.TokenValue != "}" && CurrentToken.TokenValue != "else")
        {
            if(CurrentToken.TokenValue == "for")
            {
                ifNode.Body.Add(ParseFor(effectcontext));
            }
            else if(CurrentToken.TokenValue == "while")
            {
                ifNode.Body.Add(ParseWhile(effectcontext));
            }
            else if(CurrentToken.TokenValue == "if")
            {
                ifNode.Body.Add(ParseIf(effectcontext));
            }
            else if(CurrentToken.TokenValue == "Identificadores" && PeekNexToken().TokenValue == ".")
            {
                ifNode.Body.Add(ParseMemberAccess(effectcontext));
            }
            else if(CurrentToken.TokenType == "Identificadores" && PeekNexToken().TokenType == "OperadoresDeAsignacion")
            {
                ifNode.Body.Add(ParseAssignment(null , effectcontext));
            }
            else throw new Exception("Current expression is not valid in the current context");
        }

        Expect("Delimitadores"); // } se acaba el cuerpo del if 
        if(CurrentToken.TokenValue == "else")
        {
            Expect("Delimitadores"); // {
            while(CurrentToken.TokenValue != "}")
            {
                if(CurrentToken.TokenValue == "for")
                {
                    ifNode.ElseBody.Add(ParseFor(effectcontext));
                }
                else if(CurrentToken.TokenValue == "while")
                {
                    ifNode.ElseBody.Add(ParseWhile(effectcontext));
                }
                else if(CurrentToken.TokenValue == "if")
                {
                    ifNode.ElseBody.Add(ParseIf(effectcontext));
                }
                else if(CurrentToken.TokenValue == "Identificadores" && PeekNexToken().TokenValue == ".")
                {
                    ifNode.ElseBody.Add(ParseMemberAccess(effectcontext));
                }
                else if(CurrentToken.TokenType == "Identificadores" && PeekNexToken().TokenType == "OperadoresDeAsignacion")
                {
                    ifNode.ElseBody.Add(ParseAssignment(null , effectcontext));
                }
                else throw new Exception("Current expression is not valid in the current context");
            }
            Expect("Delimitadores"); // }
        }
        return ifNode;
    }
 
    public ExpressionNode ParseExpressions(List<Token> analizar , bool isCondition, Context currentcontext)
    {
        List<Token> PostFijo = ConvertPostFijo(analizar);
        var astnodes = ParsePostFijo(PostFijo,currentcontext);

        return astnodes;
    }
    public ExpressionNode ParsePostFijo(List<Token> PostFijo , Context currentcontext)
    {
        Stack<ExpressionNode> expressionNode = new Stack<ExpressionNode>();
        foreach (var item in PostFijo)
        {
            if(item.TokenValue == "PatronDeNumero")
            {
                expressionNode.Push(new NumberNode{Value = int.Parse(item.TokenValue)});
            }
            else if(item.TokenValue == "Booleano")
            {
                expressionNode.Push(new BooleanNode{ Value = bool.Parse(item.TokenValue)});
            }
            else if(item.TokenValue == "Identificadores")
            {
                if(currentcontext.Variables.ContainsKey(item.TokenValue))
                {
                    var valuenow = currentcontext.GetVariable(item.TokenValue);
                    expressionNode.Push(new VariableReferenceNode{Name = item.TokenValue , Value = valuenow});
                }
                else throw new Exception ("There is not an instance of current variable");
            }
            else if(Operadores.ContainsKey(item.TokenValue))
            {
                var right = expressionNode.Pop();
                var left = expressionNode.Pop();
                expressionNode.Push(new BinaryOperationNode {MiembroIzq = left , MiembroDer = right , Operator = CurrentToken.TokenValue}); 
            }
        }
        return expressionNode.Pop();
    }
    public List<Token> ExpressionsTokens()
    {
        List<Token> retornar = new List<Token>();
        while(CurrentToken.TokenValue != ";" && CurrentToken.TokenValue != ")" && CurrentToken.TokenValue != "}" )
        {
            retornar.Add(CurrentToken);
            NextToken();
        }
        return retornar;
    }
    public List<Token> ConvertPostFijo(List<Token> convertir)
    {
        List<Token> retorn = new List<Token>();
        Stack<Token> tokens = new Stack<Token>();
        foreach(var item in convertir)
        {
            if(item.TokenValue == "PatronDeNumero" || item.TokenValue == "Identificadores" || item.TokenValue == "Booleano")
            {
                retorn.Add(item);
            }
            else if(Operadores.ContainsKey(item.TokenValue))
            {
                while(tokens.Any() && Operadores.ContainsKey(tokens.Peek().TokenValue) && ((Operadores[item.TokenValue].associaDerecha && Operadores[item.TokenValue].procedencia 
                < Operadores[tokens.Peek().TokenValue].procedencia) 
                || ((!Operadores[item.TokenValue].associaDerecha && Operadores[item.TokenValue].procedencia <= Operadores[tokens.Peek().TokenValue].procedencia)) ))
                {
                    retorn.Add(tokens.Pop());
                }
                tokens.Push(item);
            }
            else if(item.TokenValue == "(")
            {
                tokens.Push(item);
            }
            else if(item.TokenValue == ")" )
            {
                while(tokens.Peek().TokenValue != "(")
                {
                    retorn.Add(tokens.Pop());
                }
                tokens.Pop();
            } 
        }
        while(tokens.Any())
        {
            retorn.Add(tokens.Pop());
        }
        return retorn;
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
                Expect("Identificadores");
                Expect("OperadoresDeAsignacion");
                Expect("Delimitadores");
                //Source
                Expect("Identificadores");
                Expect("OperadoresDeAsignacion");
                onActivationNode.selector.Source = CurrentToken.TokenValue;
                Expect("Identificadorestring");
                Expect("Coma");
                //Single
                Expect("Identificadores");
                Expect("OperadoresDeAsignacion");
                if(CurrentToken.TokenValue == "true") onActivationNode.selector.Single = true;
                else if(CurrentToken.TokenValue == "false") onActivationNode.selector.Single = false;
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
                onActivationNode.selector.Predicate.MiembroIzq = mIzq;
                Expect("Identificadores");
                operador = CurrentToken.TokenValue;
                onActivationNode.selector.Predicate.Operador = operador;
                Expect("OperadoresDeComparacion");
                Object mDer = CurrentToken.TokenValue;
                onActivationNode.selector.Predicate.MiembroDer = mDer;
                if(CurrentToken.TokenType == "PatronDeNumero") Expect("PatronDeNumero");
                else if(CurrentToken.TokenType == "Booleano") Expect("Booleano");
                else if(CurrentToken.TokenType == "Identificadorestring") Expect("Identificadorestring");
                Expect("Delimitadores");
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