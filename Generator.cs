using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GwentPlus
{
    public class CodeGenerator
    {
        private List<ASTNode> _nodes;
        public List<Card> _cards = new List<Card>(); // Almacena las cartas creadas
        public Context context = new Context(null!); //para llevar las variables

        public CodeGenerator(List<ASTNode> nodes)
        {
            _nodes = nodes;
        }

        public void GenerateCode(string outputPath)
        {
            using (StreamWriter writer = new StreamWriter(outputPath))
            {
                writer.WriteLine("namespace GwentPlus");
                writer.WriteLine("{");
                // Escribir la definición de la clase EffectCreated
                writer.WriteLine("public class EffectCreated");
                writer.WriteLine("{");

                foreach (var node in _nodes)
                {
                    if (node is EffectNode effectNode)
                    {
                        GenerateEffectMethod(writer, effectNode);
                    }
                    else if (node is CardNode cardNode)
                    {
                        CreateCardInstance(cardNode);
                    }
                }

                writer.WriteLine("}");
                writer.WriteLine("}");

            }
        }

        private void GenerateEffectMethod(StreamWriter writer, EffectNode effectNode)
        {
            string parametersString;
            // Genera una lista de parámetros basada en el diccionario Params del EffectNode
            if(effectNode.Params.Count != 0)
            {
                var parameters = new List<string>();
                foreach (var param in effectNode.Params)
                {
                    var type = param.Value;
                    if (type == "Number")
                    {
                        parameters.Add($"int {param.Key}");
                    }
                    else if (type == "String")
                    {
                        parameters.Add($"string {param.Key}");
                    }
                    else if (type == "Bool")
                    {
                        parameters.Add($"bool {param.Key}");
                    }

                }

                parametersString = ", " + string.Join(", ", parameters);
            }
            else
            {
                parametersString = "";
            }

            writer.WriteLine($"    public void {effectNode.Name.Substring(1, effectNode.Name.Length - 2)}Effect(CardList targets, GameContext context {parametersString})");
            writer.WriteLine("    {");

            foreach (var action in effectNode.Action.Hijos)
            {
                GenerateActionCode(writer, action);
            }

            writer.WriteLine("    }");
            writer.WriteLine();
        }

        private void GenerateActionCode(StreamWriter writer, ASTNode action)
        {
            if (action is AssignmentNode assignmentNode)
            {
                string variableDeclaration = context.Variables.ContainsKey(assignmentNode.VariableName) ? "" : "var";
                string access = "";
                if (assignmentNode.CadenaDeAcceso != null)
                {
                    for (int i = 0; i < assignmentNode.CadenaDeAcceso.Count; i++)
                    {
                        if (i < assignmentNode.CadenaDeAcceso.Count - 1)
                        access += assignmentNode.CadenaDeAcceso[i] + ".";
                        else
                        access += assignmentNode.CadenaDeAcceso[i];
                    }
                    variableDeclaration = "";
                }
                else access = assignmentNode.VariableName;

                writer.WriteLine($"        {variableDeclaration} {access} {assignmentNode.Operator} {GenerateValueExpressionCode(assignmentNode.ValueExpression)};");
                if (!context.Variables.ContainsKey(assignmentNode.VariableName))
                {
                    //asumir que el valor se asignará más adelante
                    context.DefineVariable(assignmentNode.VariableName, null);
                }
            }
            else if (action is IfNode ifNode)
            {
                writer.WriteLine($"        if ({GenerateValueExpressionCode(ifNode.Condition)})");
                writer.WriteLine("        {");
                foreach (var statement in ifNode.Body)
                {
                    GenerateActionCode(writer, statement);
                }
                writer.WriteLine("        }");
            }
            else if (action is WhileNode whileNode)
            {
                writer.WriteLine($"        while ({GenerateValueExpressionCode(whileNode.Condition)})");
                writer.WriteLine("        {");
                foreach (var statement in whileNode.Body)
                {
                    GenerateActionCode(writer, statement);
                }
                writer.WriteLine("        }");
            }
            else if (action is ForNode forNode)
            {
                writer.WriteLine($"        foreach (Card {forNode.Item} in {GenerateValueExpressionCode(forNode.Collection)})");
                writer.WriteLine("        {");
                foreach (var statement in forNode.Body)
                {
                    GenerateActionCode(writer, statement);
                }
                writer.WriteLine("        }");
            }
            else if (action is MemberAccessNode memberAccessNode)
            {
                //miembros de acceso, a metdos y propiedades
                if (memberAccessNode.IsProperty)
                {
                    //acceder a la propiedad
                    writer.WriteLine($"        {string.Join(".", memberAccessNode.AccessChain)}");
                }
                else
                {
                    string arguments = string.Join(", ", memberAccessNode.Arguments.Select(arg => GenerateValueExpressionCode(arg)));
                    writer.WriteLine($"        {string.Join(".", memberAccessNode.AccessChain)}({arguments});");
                }
            }
        }

        private string GenerateValueExpressionCode(ASTNode valueExpression)
        {
            if (valueExpression is NumberNode numberLiteral)
            {
                return numberLiteral.Value.ToString(); // Asumiendo un número
            }
            else if (valueExpression is BooleanNode booleanLiteral)
            {
                return booleanLiteral.Value.ToString().ToLower(); // Asumir que es true or false
            }
            else if (valueExpression is VariableReferenceNode variableReferenceNode)
            {
                return variableReferenceNode.Name;
            }
            else if (valueExpression is BinaryOperationNode binaryOperationNode)
            {
                return $"{GenerateValueExpressionCode(binaryOperationNode.MiembroIzq)} {binaryOperationNode.Operator} {GenerateValueExpressionCode(binaryOperationNode.MiembroDer)}";
            }
            else if (valueExpression is MemberAccessNode memberAccessNode)
            {
                if (memberAccessNode.IsProperty)
                {
                    // Si es una propiedad, simplemente accede a la propiedad
                    return $"{string.Join(".", memberAccessNode.AccessChain)}";
                }
                else
                {
                    // Si no es una propiedad y tiene argumentos, es una llamada a método
                    string arguments = string.Join(", ", memberAccessNode.Arguments.Select(arg => GenerateValueExpressionCode(arg)));
                    return $"{string.Join(".", memberAccessNode.AccessChain)}({arguments})";
                }
            }
            // Agrega más casos según sea necesario

            // Si no se reconoce el tipo, simplemente devuelve una cadena vacía o un valor predeterminado
            return "";
        }

        private void CreateCardInstance(CardNode cardNode)
        {
            // Crea una nueva instancia de CardData
            Card cardData = ScriptableObject.CreateInstance<Card>();

            // Asigna las propiedades
            cardData.Name = cardNode.Name.Substring(1, cardNode.Name.Length - 2);
            cardData.Type = (CardType)Enum.Parse(typeof(CardType), cardNode.Type.Substring(1, cardNode.Type.Length - 2));
            cardData.Faction = (Faction)Enum.Parse(typeof(Faction), cardNode.Faction.Substring(1, cardNode.Faction.Length - 2));
            cardData.Power = cardNode.Power;
            cardData.Range = Array.ConvertAll(cardNode.Range.ToArray(), r => (Range)Enum.Parse(typeof(Range), r.Substring(1, r.Length - 2)));
            cardData.OnActivation = new List<Effects>();
            cardData.EffectCreated = new EffectCreated();

            foreach (var activation in cardNode.Effects)
            {
                cardData.OnActivation.Add(CreateEffect(activation));
            }

            _cards.Add(cardData);
        }

        private Effects CreateEffect(OnActivationNode activation)
        {
            Effects effect = new Effects
            {
                Name = activation.effect?.Name ?? "DefaultEffectName", // Provide a default value or handle null differently
                Params = activation.effect != null ? activation.effect.Params : new List<object>(),
                Source = activation.selector?.Source ?? "DefaultSource",
                Single = activation.selector?.Single ?? false,
                Predicate = new Predicate // Aquí se crea una nueva instancia de Predicate y se asignan sus propiedades
                {
                    LeftMember = activation.selector?.Predicate?.MiembroIzq ?? "DefaultLeftMember",
                    Operator = activation.selector?.Predicate?.Operador ?? "DefaultOperator",
                    RightMember = activation.selector?.Predicate?.MiembroDer ?? "DefaultValue"
                },                    
            };
            
            return effect;
        }
    }
}