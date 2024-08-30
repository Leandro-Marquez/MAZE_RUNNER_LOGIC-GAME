using System.Dynamic;
using System;
using System.Security.Cryptography.X509Certificates;

public abstract class ASTNode
{
    public abstract void Print(int index);
}

public class EffectNode : ASTNode
{
    public string Name{get; set;}

    public Dictionary<string,object> Params{get; set;} = new Dictionary<string, object>();

    public ActionNode Action{get; set;} // arreglarrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr 
    public override void Print(int indent = 0)
        {
            string indentation = new string(' ', indent);
            Console.WriteLine($"{indentation}Effect: {Name}");
            foreach (var param in Params)
            {
                Console.WriteLine($"{indentation}  Param: {param.Key} = {param.Value}");
            }
            Console.WriteLine($"{indentation}  Action: {Action}");

            // foreach (var action in Actions.Children)
            // {
            //     action.Print(indent + 3); 
            // }
        }
}

public class ActionNode
{
    public List<ASTNode> Hijos { get; set; } = new List<ASTNode>();

}

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
        string indentation = new string(' ', indent);
        Console.WriteLine($"{indentation}Card: {Name}");
        Console.WriteLine($"{indentation}  Type: {Type}");
        Console.WriteLine($"{indentation}  Faction: {Faction}");
        Console.WriteLine($"{indentation}  Power: {Power}");
        Console.WriteLine($"{indentation}  Range: [{string.Join(", ", Range)}]");
        Console.WriteLine($"{indentation}  OnActivation:");
        foreach (var activation in Effects)
        {
            activation.Print(indent + 2);
        }
    }
}
public class OnActivationNode : ASTNode
{
    public CardEffectNode effect{get; set;} = new CardEffectNode();
    public SelectorNode selector{get; set;} = new SelectorNode();
    public override void Print(int indent = 0)
    {
        string indentation = new string(' ', indent);
        Console.WriteLine($"{indentation}Activation:");
        effect.Print(indent + 2);
        selector.Print(indent + 2);
    }
}
public class CardEffectNode : ASTNode
{
    public string Name{get; set;}
    public List<object> Params{get; set;} = new List<object>();

    public override void Print(int indent = 0)
    {
        string indentation = new string(' ', indent);
        Console.WriteLine($"{indentation}Effect:");
        Console.WriteLine($"{indentation}  Name: {Name}");
        foreach (var item in Params)
        {
            Console.WriteLine($"{indentation}  Amount: {item}");
        }
    }
}
public class SelectorNode : ASTNode
{
    public string Source{get; set;}
    public bool Single{get; set;}
    public PredicateNode Predicate{get; set;} = new PredicateNode();

    public override void Print(int indent = 0)
        {
            string indentation = new string(' ', indent);
            Console.WriteLine($"{indentation}Selector:");
            Console.WriteLine($"{indentation}  Source: {Source}");
            Console.WriteLine($"{indentation}  Single: {Single}");
            Predicate.Print(indent);
        }
}
public class PredicateNode : ASTNode
{
    public string MiembroIzq{get; set;}

    public void Imprimir()
    {
        System.Console.WriteLine(MiembroIzq);
    }
    public string Operador{get; set;}
    public object MiembroDer{get; set;}
    public override void Print(int indent = 0)
        {
            Imprimir();
            string indentation = new string(' ', indent);
            Console.WriteLine($"{indentation}Predicate: {MiembroIzq} {Operador} {MiembroDer}");
        }
}
public abstract class ExpressionNode : ASTNode
{

}
public class NumberNode : ExpressionNode
{
    public int Value{get; set;}

    public override void Print(int index)
    {
        throw new NotImplementedException();
    }
}
public class BooleanNode : ExpressionNode
{
    public bool Value{get; set;}

    public override void Print(int index)
    {
        throw new NotImplementedException();
    }
}
public class VariableReferenceNode : ExpressionNode
{
    public string Name{get; set;}
    public object Value{get; set;}

    public override void Print(int index)
    {
        throw new NotImplementedException();
    }
}
public class BinaryOperationNode : ExpressionNode
{
    public ExpressionNode MiembroIzq{get; set;}
    public ExpressionNode MiembroDer{get; set;}
    public string Operator{get; set;}
    public override void Print(int index)
    {
        // throw new NotImplementedException();
    }
}
public class ForNode : ASTNode
{
    public string Item { get; set; }
    public VariableReferenceNode Collection { get; set; }
    public List<ASTNode> Body { get; set; } = new List<ASTNode>();

    public override void Print(int index)
    {
        throw new NotImplementedException();
    }
}
public class WhileNode : ASTNode
{
    public ExpressionNode Condition { get; set; }
    public List<ASTNode> Body { get; set; } = new List<ASTNode>();

    public override void Print(int indent = 0)
    {

    }
}
public class AssignmentNode : ASTNode
{
    public string VariableName { get; set; }
    public ASTNode ValueExpression { get; set; }
    public List<string> CadenaDeAcceso { get; set; } = new List<string>();
    public string Operator { get; set; }

    public override void Print(int indent = 0)
    {

    }

}
public class MemberAccessNode : ASTNode 
{
    public List<string> AccessChain { get; set; } = new List<string>();
    public List<ExpressionNode> Arguments { get; set; } =  new List<ExpressionNode>();
    public bool IsProperty { get; set; }

    public override void Print(int index)
    {
        throw new NotImplementedException();
    }
}
public class IfNode : ASTNode
{
    public ExpressionNode Condition { get; set; }
    public List<ASTNode> Body { get; set; } = new List<ASTNode>();
    public List<ASTNode> ElseBody { get; set; } = new List<ASTNode>();
    
    public override void Print(int indent = 0)
    {

    } 
}

