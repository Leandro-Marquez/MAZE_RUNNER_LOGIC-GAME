using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
    public class CodeGenerator
    {
        private List<ASTNode> _nodes;
        public static List<Card> _cards = new List<Card>(); // Almacena las cartas creadas
        public Context context = new Context(null!);

        public CodeGenerator(List<ASTNode> nodes)
        {
            _nodes = nodes;
        }

        public void GenerateCode(string outputPath)
        {
            using (StreamWriter writer = new StreamWriter(outputPath))
            {
                // writer.WriteLine("{");


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
                // writer.WriteLine("}");

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
                    context.DefineVariable(assignmentNode.VariableName, null); // Asumiendo que el valor se asignará más adelante o es irrelevante en este contexto
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
                // Aquí puedes manejar el acceso a miembros y llamadas a métodos
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

        private string GenerateValueExpressionCode(ASTNode valueExpression)
        {
            if (valueExpression is NumberNode numberLiteral)
            {
                return numberLiteral.Value.ToString(); // Asumiendo que Value es un número
            }
            else if (valueExpression is BooleanNode booleanLiteral)
            {
                return booleanLiteral.Value.ToString().ToLower(); // Asumiendo que Value es un bool
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
            // Agrega más casos según sea necesario

            // Si no se reconoce el tipo, simplemente devuelve una cadena vacía o un valor predeterminado
            return "";
        }

        private void CreateCardInstance(CardNode cardNode)
        {
            // Crear una nueva instancia de CardData
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

            //manejar los efectos de activación si es necesario
            foreach (var activation in cardNode.Effects)
            {
                cardData.OnActivation.Add(CreateEffect(activation));
            }
            cardData.EffectType = CardEffects.Created;
            _cards.Add(cardData);
        }

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
        // effect
        // {
        //     Name: "Damage",
        //     Params:
        //     {
        //         Amount: Number,
        //     },
        //     Action: (targets, context) =>
        //     {
        //         for target in targets
        //         {
        //             i = 0;
        //             while (i < Amount)
        //             {
        //                 target.Power -= 1;
        //                 i+=1;
        //             }
        //         };
        //     }
        // }

        // effect
        // {
        //     Name: "Draw",
        //     Action: (targets, context) =>
        //     {
        //         topCard = context.Deck.Pop();
        //         context.Hand.Add(topCard);
        //         context.Hand.Shuffle();
        //     }
        // }

        // effect
        // {
        //     Name: "ReturnToDeck",
        //     Action: (targets, context) =>
        //     {
        //         for target in targets
        //         {
        //             owner = target.Owner;
        //             deck = context.DeckOfPlayer(owner);
        //             deck.Push(target);
        //             deck.Shuffle();
        //             context.Board.Remove(target);
        //         };
        //     }
        // }

        // card {
        //     Type: "oro",
        //     Name: "Beluga",
        //     Faction: "Elementales",
        //     Power: 10,
        //     Range: ["M", "R"],
        //     OnActivation: [
        //         {
        //             Effect: {
        //                 Name: "Damage",
        //                 Amount: 5,
        //             },
        //             Selector: {
        //                 Source: "board",
        //                 Single: false,
        //                 Predicate: (unit) => unit.Faction == "Northern Realms"
        //             },
        //             PostAction: {
        //                 Effect: {
        //                     Name: "ReturnToDeck",
        //                 },
        //                 Selector: {
        //                     Source: "parent",
        //                     Single: false,
        //                     Predicate: (unit) => unit.Power < 1
        //                 },
        //                 PostAction: {
        //                     Effect: {
        //                         Name: "Draw",
        //                     },
        //                 },
        //             },
        //         },
        //     ]
        // }