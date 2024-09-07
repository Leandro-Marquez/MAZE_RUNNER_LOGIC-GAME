using System;
using System.Collections.Generic;
using System.Linq;

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
        int x = 1;
        List<ASTNode> ToReturn = new List<ASTNode>();
        while(CurrentToken is not null)
        {
            if(CurrentToken != null && CurrentToken.TokenValue == "effect")
            {
                ToReturn.Add(Effect_Parse());
            }
            else if(CurrentToken != null && CurrentToken.TokenValue == "card")
            {
                ToReturn.Add(Cards_Parse());
            }
            else
            {
                throw new Exception("Unexpected Token value, token have been expected (card) or (effect)" 
                + $"  {CurrentToken.TokenValue} " + $" {tokens.IndexOf(CurrentToken)}");
            }
            x++;
        }
        return ToReturn;
    }
    public EffectNode Effect_Parse()
    {
        Expect("PalabrasReservadas"); //effect
        Expect("Delimitadores"); //{
        // crear el nodo de efecto 
        EffectNode effect = new EffectNode();
        // effect.Action.Hijos = new List<ASTNode>();
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
                string variableType = "";
                Expect("Identificadores");
                Expect("OperadoresDeAsignacion");
                if(CurrentToken.TokenValue == "Number" || CurrentToken.TokenValue == "Bool" || CurrentToken.TokenValue == "String")
                {
                    variableType = CurrentToken.TokenValue;
                    if(CurrentToken.TokenValue == "Number")
                    {
                        effectContext.DefineVariable(paramsName,0);
                        context.DefineVariable(paramsName,0);
                        effect.Params [paramsName] = 0;
                    }
                    else if(CurrentToken.TokenValue == "Bool")
                    {
                        effectContext.DefineVariable(paramsName,false);
                        context.DefineVariable(paramsName,false);
                        effect.Params [paramsName] = false;
                    }
                    else if(CurrentToken.TokenValue == "String")
                    {
                        effectContext.DefineVariable(paramsName,"");
                        context.DefineVariable(paramsName,"");
                        effect.Params [paramsName] = "";
                    }
                }
                else if(CurrentToken.TokenType == "Booleano" || CurrentToken.TokenType == "PatronDeNumero" || CurrentToken.TokenType == "Identificadorestring")
                {
                    effectContext.DefineVariable(paramsName,CurrentToken.TokenValue);
                    context.DefineVariable(paramsName,CurrentToken.TokenValue);
                }
                Expect("Identificadores");

                if(CurrentToken.TokenType == "Coma") Expect("Coma");
            }
            Expect("Delimitadores");
            Expect("Coma");
        }
        // parsear el action
        Expect("Identificadores"); // action
        Expect("OperadoresDeAsignacion");// :
        Expect("Delimitadores"); //(
        Expect("Identificadores");//targets
        Expect("Coma");
        Expect("Identificadores");//context
        Expect("Delimitadores"); // )
        Expect("OperadorLanda"); // =>
        Expect("Delimitadores"); // { empieza el cuerpo del metodo
        // parsear el cuerpo del action
        while(CurrentToken.TokenValue != "}")
        {
            effect.Action.Hijos = ParseAction(effectContext);
        }
        Expect("Delimitadores");
        // Expect("Delimitadores");

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
            else if(CurrentToken.TokenType == "Identificadores" && PeekNexToken().TokenValue == ".")
            {
                aSTNodes.Add(ParseMemberAccess(effectcontext));
            }
            else if(CurrentToken.TokenType == "Identificadores" && PeekNexToken().TokenType == "OperadoresDeAsignacion")
            {
                aSTNodes.Add(ParseAssignment(null , effectcontext));
            }
            
            else throw new Exception($"Current expression is not valid in the current context {CurrentToken.TokenValue}"  + $" {tokens.IndexOf(CurrentToken)}");
        }
        Expect("Delimitadores");
        // Expect("Delimitadores");
        return aSTNodes;
    }
    private AssignmentNode ParseAssignment(List<string> accessChain , Context effectcontext)
    {
        AssignmentNode assignmentNode = new AssignmentNode();
        // asignar el nombre de la variable
        string variableName = CurrentToken.TokenValue;
        assignmentNode.VariableName = CurrentToken.TokenValue;
        Expect("Identificadores");
        assignmentNode.Operator = CurrentToken.TokenValue;
        if(CurrentToken.TokenType == "OperadoresDeAsignacion") Expect("OperadoresDeAsignacion");
        else if(CurrentToken.TokenType == "OperadoresDeIncDec") Expect("OperadoresDeIncDec");

        if(PeekNexToken().TokenValue == ".")
        {
            var valueExpression = ParseMemberAccess(effectcontext);
            assignmentNode.ValueExpression = valueExpression;
            assignmentNode.CadenaDeAcceso = accessChain;
            // Expect("PuntoComa");

            return assignmentNode;
        }
        else
        {
            var valueExpression = ParseExpressions(ExpressionsTokens(),false,effectcontext);
            Expect("PuntoComa");
            assignmentNode.ValueExpression = valueExpression;
            assignmentNode.CadenaDeAcceso = accessChain;//cambioooooooooooooooooo
            if (effectcontext.Variables.ContainsKey(variableName))
            {
                effectcontext.SetVariable(variableName, valueExpression.Evaluate(effectcontext));
            }
            else
            {
                effectcontext.DefineVariable(variableName, valueExpression.Evaluate(effectcontext));
            }
            return assignmentNode;
        }
    }

    private ASTNode ParseMemberAccess(Context currentContext)
    {
        string objectName = CurrentToken.TokenValue;
        List<string> accessChain = new List<string>{objectName};

        //Procesar los accesos anidados
        while (PeekNexToken().TokenValue == ".")
        {
            Expect("Identificadores"); //Se espera el siguiente identificador
            Expect("Punto"); //Se espera el punto
            string memberName = CurrentToken.TokenValue;
            accessChain.Add(memberName);
        }

        //Verifica si es una propiedad o un metodo
        bool isProperty = false;
        List<ExpressionNode> arguments = new List<ExpressionNode>();
        if (PeekNexToken().TokenValue == ";")
        {
            isProperty = true;
            Expect("Identificadores");
            Expect("PuntoComa");
        }
        else if (PeekNexToken().TokenValue == "(")
        {
            isProperty = false;
            Expect("Identificadores");
            Expect("Delimitadores");
            arguments = ParseArguments();
            Expect("Delimitadores");
            Expect("PuntoComa");
        }
        else if (PeekNexToken().TokenType == "OperadoresDeAsignacion")
        {
            var assignmentNode = ParseAssignment(accessChain, currentContext);
            return assignmentNode ;
        }

        return new MemberAccessNode { AccessChain = accessChain, Arguments = arguments , IsProperty = isProperty };
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
    public ASTNode ParseFor(Context effectcontext)
    {

        ForNode forNode = new ForNode();

        Expect("PalabrasReservadas"); // for
        // asignar la variable del for
        forNode.Item = CurrentToken.TokenValue;
        Expect("Identificadores"); // target // item u algo asi
        Expect("PalabrasReservadas"); //in 

        forNode.Collection = new VariableReferenceNode {Name = CurrentToken.TokenValue};
        Expect("Identificadores"); //Coolection
        Expect("Delimitadores"); //{
        //parsear el cuerpo del for
        while(CurrentToken.TokenValue != "}")
        {
            if(CurrentToken.TokenType == "Identificadores" && (PeekNexToken().TokenType == "OperadoresDeAsignacion" || PeekNexToken().TokenType == "OperadoresDeIncDec"))
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
            else throw new Exception($"{CurrentToken.TokenValue} is not implemented yet");
        }
        Expect("Delimitadores"); // } se cierra el cuerpo del ciclo
        Expect("PuntoComa");
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
        Expect("Delimitadores"); // {
        while(CurrentToken.TokenValue != "}")
        {
            if(CurrentToken.TokenType == "Identificadores" && (PeekNexToken().TokenType == "OperadoresDeAsignacion" || PeekNexToken().TokenType == "OperadoresDeIncDec"))
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
            else throw new Exception($"{CurrentToken.TokenValue} is not implemented");
        }
        Expect("Delimitadores");
        return whileNode;
    }
    public ExpressionNode ParseExpressions(List<Token> analizar , bool isCondition, Context effectcontext)
    {
        List<Token> PostFijo = ConvertPostFijo(analizar);
        var astnodes = ParsePostFijo(PostFijo,effectcontext);

        if (isCondition && astnodes.Evaluate(effectcontext).GetType() != typeof(bool))
        {   
           throw new Exception("La expresion de condicion debe evaluar a un booleano.");
        }

        return astnodes;
    }
    private ExpressionNode ParsePostFijo(List<Token> postfixTokens, Context currentContext)
    {
        Stack<ExpressionNode> stack = new Stack<ExpressionNode>();

        foreach (var token in postfixTokens)
        {
            if (token.TokenType == "PatronDeNumero")
            {
                stack.Push(new NumberNode { Value = int.Parse(token.TokenValue) });
            }
            else if (token.TokenType == "Booleano")
            {
                stack.Push(new BooleanNode { Value = bool.Parse(token.TokenValue)});
            }
            else if (token.TokenType == "Identificadores")
            {
                if (currentContext.Variables.ContainsKey(token.TokenValue))
                {
                    var variableValue = currentContext.GetVariable(token.TokenValue);
                    stack.Push(new VariableReferenceNode { Name = token.TokenValue, Value = variableValue });
                }
                else 
                {
                    throw new Exception ($"La varible '{token.TokenValue}' no esta definida");
                }
            }
            else if (Operadores.ContainsKey(token.TokenValue))
            {
                var right = stack.Pop();
                var left = stack.Pop();
                stack.Push(new BinaryOperationNode { MiembroIzq = left, Operator = token.TokenValue, MiembroDer = right });
            }
        }

        return stack.Pop();
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
    private List<Token> ConvertPostFijo(List<Token> tokens)
    {
        Stack<Token> operatorStack = new Stack<Token>();
        List<Token> output = new List<Token>();

        foreach (var token in tokens)
        {
            if (token.TokenType == "PatronDeNumero" || token.TokenType == "Identificadores" || token.TokenType == "Booleano")
            {
                output.Add(token);
            }
            else if (Operadores.ContainsKey(token.TokenValue))
            {
                while (operatorStack.Any() && Operadores.ContainsKey(operatorStack.Peek().TokenValue) &&
                       ((Operadores[token.TokenValue].associaDerecha && Operadores[token.TokenValue].procedencia < Operadores[operatorStack.Peek().TokenValue].procedencia) ||
                        (!Operadores[token.TokenValue].associaDerecha && Operadores[token.TokenValue].procedencia <= Operadores[operatorStack.Peek().TokenValue].procedencia)))
                {
                    output.Add(operatorStack.Pop());
                }
                operatorStack.Push(token);
            }
            else if (token.TokenValue == "(")
            {
                operatorStack.Push(token);
            }
            else if (token.TokenValue == ")")
            {
                while (operatorStack.Peek().TokenValue != "(")
                {
                    output.Add(operatorStack.Pop());
                }
                operatorStack.Pop();
            }
        }

        while (operatorStack.Any())
        {
            output.Add(operatorStack.Pop());
        }

        return output;
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