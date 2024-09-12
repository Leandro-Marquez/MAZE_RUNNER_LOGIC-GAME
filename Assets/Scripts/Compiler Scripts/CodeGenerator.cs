using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
    public class CodeGenerator
    {
        private List<ASTNode> _nodes; // lista de nodos devuelta por el parser
        public static List<Card> _cards = new List<Card>(); // Almacena las cartas creadas
        public Context context = new Context(null!);

        public CodeGenerator(List<ASTNode> nodes)
        {
            _nodes = nodes;
        }

        //generar codigo de dos maneras cartas o efectos 
        public void GenerateCode(string outputPath)
        {
            //definir la direccion del archivo a sobreescribir 
            using (StreamWriter writer = new StreamWriter(outputPath))
            {

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
                    var fulanito = param.Value;
                    if (fulanito is int)
                    {
                        parameters.Add($"int {param.Key}");
                    }
                    else if (fulanito is string)
                    {
                        parameters.Add($"string {param.Key}");
                    }
                    else if (fulanito is bool)
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
            //escribir el metodo con sus correspondientes parametros
            writer.WriteLine($"    public void {effectNode.Name.Substring(1, effectNode.Name.Length - 2)}Effect(CardList targets, context context {parametersString})");
            writer.WriteLine("    {");
            writer.WriteLine("         UnityEngine.Debug.Log(\"EffectoEjecutado\");");
            writer.WriteLine("         UnityEngine.Debug.Log(\"Current:\" + GameManager.Instancia.CurrentPlayer);");

            foreach (var action in effectNode.Action.Hijos)
            {
                GenerateActionCode(writer, action);
            }

            writer.WriteLine("    }");
            writer.WriteLine();
        }
        // escribir el cuerpo de accion del metodo
        private void GenerateActionCode(StreamWriter writer, ASTNode action)
        {
            //verificar cada tipo de nodo en el primer momento del cuerpo de accion 
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
                    context.DefineVariable(assignmentNode.VariableName, null); // Asumir el valor se asignará más adelante o es irrelevante en este contexto
                }
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
                //manejar el acceso a miembros y llamadas a métodos
                if (memberAccessNode.IsProperty)
                {
                    // Si es una propiedad, simplemente accede a la propiedad
                    writer.WriteLine($"        {string.Join(".", memberAccessNode.AccessChain)}");
                }
                else
                {
                    /// Si no es una propiedad y tiene argumentos, es una llamada a método
                    string arguments = string.Join(", ", memberAccessNode.Arguments.Select(arg => GenerateValueExpressionCode(arg)));
                    writer.WriteLine($"        {string.Join(".", memberAccessNode.AccessChain)}({arguments});");
                }
            }
        }
        //generar el valor de la expresion que vaya a generar de ser posible 
        private string GenerateValueExpressionCode(ASTNode valueExpression)
        {
            if (valueExpression is NumberNode numberLiteral)
            {
                return numberLiteral.Value.ToString(); // Asumir que Value es un número
            }
            else if (valueExpression is BooleanNode booleanLiteral)
            {
                return booleanLiteral.Value.ToString().ToLower(); // Asumir que Value es un bool
            }
            else if (valueExpression is VariableReferenceNode variableReferenceNode)
            {
                return variableReferenceNode.Name; // Asumir que Value es un string osea una variable 
            }
            else if (valueExpression is BinaryOperationNode binaryOperationNode)
            {
                return $"{GenerateValueExpressionCode(binaryOperationNode.MiembroIzq)} {binaryOperationNode.Operator} {GenerateValueExpressionCode(binaryOperationNode.MiembroDer)}";
            }
            else if (valueExpression is MemberAccessNode memberAccessNode)
            {
                // Aquí puedes manejar el acceso a miembros y llamadas a métodos
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
            // Agregar más casos según sea necesario

            // Si no se reconoce el tipo,devolver una cadena vacía o un valor predeterminado
            return "";
        }

        //crear instancias de scriptable objects (cartas)
        private void CreateCardInstance(CardNode cardNode)
        {
            // Crear una nueva instancia Data
            Card cardData = ScriptableObject.CreateInstance<Card>();

            // Asignar las propiedades
            cardData.IsCreated = true;
            cardData.Name = cardNode.Name.Substring(1, cardNode.Name.Length - 2);
            cardData.Type = (CardType)Enum.Parse(typeof(CardType), cardNode.Type.Substring(1, cardNode.Type.Length - 2).ToLower());
            cardData.Faction = (Faction)Enum.Parse(typeof(Faction), cardNode.Faction.Substring(1, cardNode.Faction.Length - 2));
            cardData.Power = cardNode.Power;
            cardData.Owner = 1;
            cardData.Range = Array.ConvertAll(cardNode.Range.ToArray(), r => (Range)Enum.Parse(typeof(Range), r.Substring(1, r.Length - 2)));
            cardData.OnActivation = new List<EffectsDefinition>();
            cardData.EffectCreated = new EffectCreated();
            cardData.EffectType = CardEffects.Created;

            //manejar los efectos de activación si es necesario
            foreach (var activation in cardNode.Effects)
            {
                cardData.OnActivation.Add(CreateEffect(activation));
            }
            cardData.EffectType = CardEffects.Created;
            _cards.Add(cardData);
        }
        
        //crear la definicion de efecto para con la carta 
        private EffectsDefinition CreateEffect(OnActivationNode activation)
        {
            EffectsDefinition effect = new EffectsDefinition()
            {
                Name = activation.effect?.Name ?? "DefaultEffectName", // Provide a default value or handle null differently
                Params = activation.effect != null ? activation.effect.Params : new List<object>(),
                Source = activation.selector?.Source ?? "DefaultSource",
                Single = activation.selector?.Single ?? false,
                Predicate = new Predicate //crear una nueva instancia de Predicate y se asignan sus propiedades
                {
                    LeftMember = activation.selector?.Predicate?.MiembroIzq ?? "DefaultLeftMember",
                    Operator = activation.selector?.Predicate?.Operador ?? "DefaultOperator",
                    RightMember = activation.selector?.Predicate?.MiembroDer ?? "DefaultValue"
                },
            };
            return effect;
        }
    }
