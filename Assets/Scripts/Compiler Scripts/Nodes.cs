using System.Dynamic;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using System.Linq;

public abstract class ASTNode
{
    public abstract void Print(int index);
    public abstract object Evaluate(Context context);
}
//nodo de efecto general 
public class EffectNode : ASTNode
{
    public string Name{get; set;}
    //guardar nombres y valores por default de los parametros definidos para con el metodo
    public Dictionary<string,object> Params{get; set;} = new Dictionary<string, object>();

    public ActionNode Action{get; set;} = new ActionNode(); //cuerpo de accion del efecto osea el metodo!!!

    public override object Evaluate(Context context)
    {
        throw new NotImplementedException();
    }

        public override void Print(int indent = 0)
        {
            throw new NotImplementedException();
        }
}
// nodo de accion del efecto
public class ActionNode : ASTNode 
{
    public List<ASTNode> Hijos { get; set; } = new List<ASTNode>();

    public override object Evaluate(Context context)
    {
        throw new NotImplementedException();
    }

        public override void Print(int indent = 0)
        {
           throw new NotImplementedException();
        }

}
//nodo carta 
public class CardNode : ASTNode
{
    public string Type{get; set;}
    public string Name{get; set;}
    public string Faction{get; set;}
    public int Power{get; set;}
    public List<string> Range{get; set;}
    public List<OnActivationNode> Effects{get; set;}
    public CardNode()
    {
        Range = new List<string>();
        Effects = new List<OnActivationNode>();
    }
    public override void Print(int indent = 0)
    {
       throw new NotImplementedException();
    }

    public override object Evaluate(Context context)
    {
        throw new NotImplementedException();
    }
}
//nodo on activation
public class OnActivationNode : ASTNode
{
    public CardEffectNode effect{get; set;} = new CardEffectNode();
    public SelectorNode selector{get; set;} = new SelectorNode();

    public override object Evaluate(Context context)
    {
        throw new NotImplementedException();
    }

    public override void Print(int indent = 0)
    {
        throw new NotImplementedException();
    }
}
//nodo de efecto de la carta 
public class CardEffectNode : ASTNode
{
    public string Name{get; set;}
    public List<object> Params{get; set;} = new List<object>();

    public override object Evaluate(Context context)
    {
        throw new NotImplementedException();
    }

    public override void Print(int indent = 0)
    {
        throw new NotFiniteNumberException();
    }
}
// nodo selector de la carta para con el efecto
public class SelectorNode : ASTNode
{
    public string Source{get; set;}
    public bool Single{get; set;}
    public PredicateNode Predicate{get; set;} = new PredicateNode();

    public override object Evaluate(Context context)
    {
        throw new NotImplementedException();
    }

        public override void Print(int indent = 0)
        {
            string indentation = new string(' ', indent);
            Console.WriteLine($"{indentation}Selector:");
            Console.WriteLine($"{indentation}  Source: {Source}");
            Console.WriteLine($"{indentation}  Single: {Single}");
            Predicate.Print(indent);
        }
}
// nodo predicate del selector
public class PredicateNode : ASTNode
{
    public string MiembroIzq{get; set;}

    public string Operador{get; set;}
    public object MiembroDer{get; set;}
    public override void Print(int indent = 0)
    {
        throw new NotImplementedException();
    }

    public override object Evaluate(Context context)
    {
        throw new NotImplementedException();
    }
}

// nodo abstracto de expresiones
public abstract class ExpressionNode : ASTNode
{
    public static implicit operator ExpressionNode(MemberAccessNode v)
    {
        throw new NotImplementedException();
    }
}
// nodo literal para numero y su evaluate
public class NumberNode : ExpressionNode
{
    public int Value{get; set;}

    public override object Evaluate(Context context)
    {
        return Value;
    }
    public override void Print(int indent = 0)
    {
        throw new NotImplementedException();
    }
}
//nodo literal para booleano y su evaluate
public class BooleanNode : ExpressionNode
{        
    public bool Value{get; set;}

    public override object Evaluate(Context context)
    {
        return Value;
    }

    public override void Print(int indent = 0)
    {
        throw new NotImplementedException();
    }
}
//nodo de refrencia a variable y su evaluate
public class VariableReferenceNode : ExpressionNode
{
    public string Name{get; set;}
    public object Value{get; set;}

    public override object Evaluate(Context context)
    {
         return context.GetVariable(Name);
    }

    public override void Print(int indent = 0)
    {
        throw new NotImplementedException();
    }
}
//nodo de operacion binaria y su evaluate
public class BinaryOperationNode : ExpressionNode
{
    public ExpressionNode MiembroIzq{get; set;}
    public ExpressionNode MiembroDer{get; set;}
    public string Operator{get; set;}
    public override void Print(int indent = 0)
    {
        throw new NotImplementedException();
    }

    public override object Evaluate(Context context)
    {
         var leftValue = MiembroIzq.Evaluate(context);
        var rightValue = MiembroDer.Evaluate(context);

        switch (Operator)
        {
            case "+":
                return (int)leftValue + (int)rightValue;
            case "-":
                return (int)leftValue - (int)rightValue;
            case "*":
                return (int)leftValue * (int)rightValue;
            case "/":
                return (int)leftValue / (int)rightValue;
            case "&&":
                return (bool)leftValue && (bool)rightValue;
            case "||":
                return (bool)leftValue || (bool)rightValue;
            case "!":
                return !(bool)leftValue;
            case "==":
                return (int)leftValue == (int)rightValue;
            case "!=":
                return (int)leftValue != (int)rightValue;
            case ">":
                return (int)leftValue > (int)rightValue;
            case "<":
                return (int)leftValue < (int)rightValue;
            case ">=":
                return (int)leftValue >= (int)rightValue;
            case "<=":
                return (int)leftValue <= (int)rightValue;  
            default:
                throw new InvalidOperationException($"Operador desconocido: {Operator}");
        }
    }
}
//nodo foreach
public class ForNode : ASTNode
{
    public string Item { get; set; }
    public VariableReferenceNode Collection { get; set; } // coleccion para el foreach
    public List<ASTNode> Body { get; set; } = new List<ASTNode>();

    public override object Evaluate(Context context)
    {
        throw new NotImplementedException();
    }

    public override void Print(int indent = 0)
    {
        throw new NotImplementedException();
    }
}
// nodo while
public class WhileNode : ASTNode
{
    public ExpressionNode Condition { get; set; }
    public List<ASTNode> Body { get; set; } = new List<ASTNode>();

    public override object Evaluate(Context context)
    {
        throw new NotImplementedException();
    }

    public override void Print(int indent = 0)
    {
         throw new NotImplementedException();
    }
}
// nodo de assiganacion a variable y su evaluate
public class AssignmentNode : ASTNode
{
    public string VariableName { get; set; }
    public ASTNode ValueExpression { get; set; }
    public List<string> CadenaDeAcceso { get; set; } = new List<string>();
    public string Operator { get; set; }

    public override object Evaluate(Context context)
    {
        var value = ValueExpression.Evaluate(context);

        if (context.Variables.ContainsKey(VariableName))
        {
            context.SetVariable(VariableName, value);
        }
        else 
        {
            context.DefineVariable(VariableName, value);
        }

        return value;
    }

    public override void Print(int indent = 0)
    {
        throw new NotImplementedException();
    }
}
// nodo miembro de acceso
public class MemberAccessNode : ASTNode 
{
    public List<string> AccessChain { get; set; } = new List<string>();
    public List<ExpressionNode> Arguments { get; set; } =  new List<ExpressionNode>();
    public bool IsProperty { get; set; }

    public override object Evaluate(Context context)
    {
        throw new NotImplementedException();
    }

    public override void Print(int indent = 0)
    {
        throw new NotImplementedException();
    }
}
