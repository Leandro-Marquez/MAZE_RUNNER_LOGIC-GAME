using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CardCreationHandler : MonoBehaviour
{
    public GameObject inputFieldObject; // El cuadro de texto
    public TMP_InputField inputField; // El componente InputField
    public Button createCardButton; // El botón para crear la carta
    public Button processButton; // El botón para procesar el texto
    public void Start()
    {
        // Inicialmente, ocultar el cuadro de texto y el botón de procesar
        inputFieldObject.SetActive(false);
        processButton.gameObject.SetActive(false);

        // Asignar funciones a los botones
        createCardButton.onClick.AddListener(ShowInputField);
        processButton.onClick.AddListener(ProcessText);
    }

    public void ShowInputField()
    {
        // Mostrar el cuadro de texto y el botón de procesar
        inputFieldObject.SetActive(true);
        processButton.gameObject.SetActive(true);

    }

    public void ProcessText()
    {
        // Obtener el texto del cuadro de texto
        string userInput = inputField.text;

        Lexer lexer = new Lexer(userInput);
        // Call the Tokenizar method to tokenize the input
        List<Token> tokens = lexer.Tokenizar();

        // Debug.Log(userInput);

        // imprimir cada token de momento 
        // foreach (var item in tokens)
        // {
        //     Debug.Log(item.ToString());
        // }

        Parser parser = new Parser(tokens);
        List<ASTNode> aSTNodes = parser.Parse();

        CodeGenerator codeGenerator = new CodeGenerator(aSTNodes);
        codeGenerator.GenerateCode("Assets/Scripts/EffectCreated.cs");

        // Ocultar el cuadro de texto y el botón de procesar después de procesar el texto
        inputFieldObject.SetActive(false);
        processButton.gameObject.SetActive(false);
    }
}
// @"
//         effect
//         {
//             Name: "Damage",
//             Params:
//             {
//                 Amount: Number,
//             },
//              Action: (targets, context) =>
//              {
//                 for target in targets
//                 {
//                     i = 0;
//                     while (i < Amount)
//                     {
//                         target.Power -= 1;
//                         i+=1;
//                     }
//                 };
//             }
//         }

//         effect
//         {
//              Name: "Draw",
//              Action: (targets, context) =>
//              {
//                 topCard = context.Deck.Pop();
//                 context.Hand.Add(topCard);
//                 context.Hand.Shuffle();
//              }
//         }

//          effect
//         {
//             Name: "ReturnToDeck",
//             Action: (targets, context) =>
//             {
//                  for target in targets
//                  {
//                     owner = target.Owner;
//                     deck = context.DeckOfPlayer(owner);
//                     deck.Push(target);
//                     deck.Shuffle();
//                     context.Board.Remove(target);
//                 };
//             }
//         }

//         card
//         {
//             Type: "Oro",
//             Name: "Beluga",
//             Faction: "Northern Realms",
//             Power: 10,
//             Range: ["Melee", "Ranged"],
//             OnActivation: [
//                 {
//                     Effect:
//                     {
//                         Name: "Damage",
//                         Amount: 5,
//                     },
//                     Selector:
//                      {
//                         Source: "board",
//                         Single: false,
//                         Predicate: (unit) => unit.Faction == "Northern Realms"
//                     },
//                     PostAction:
//                     {
//                       Effect:
//                       {
//                         Name: "ReturnToDeck",
//                         },
//                         Selector:
//                         {
//                             Source: "parent",
//                             Single: false,
//                             Predicate: (unit) => unit.Power < 1
//                         },
//                         PostAction:
//                         {
//                             Effect:
//                             {
//                                 Name: "Draw",
//                             },
//                         },
//                     },
//                  },
//             ]
//         }"
