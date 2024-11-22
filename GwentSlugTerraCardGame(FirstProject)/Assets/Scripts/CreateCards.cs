using System;
using System.Collections.Generic;
using System.IO;
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

        Parser parser = new Parser(tokens);
        List<ASTNode> aSTNodes = parser.Parse();

        // Definir la ruta completa del archivo en la carpeta "Compiler Scripts"
        string filePath = Path.Combine(Application.dataPath, "Scripts/Compiler Scripts/EffectCreatedRef.cs");

        // Crear una instancia de CodeGenerator y generar el código
        CodeGenerator codeGenerator = new CodeGenerator(aSTNodes);
        codeGenerator.GenerateCode(filePath);

        // Ocultar el cuadro de texto y el botón de procesar después de procesar el texto
        inputFieldObject.SetActive(false);
        processButton.gameObject.SetActive(false);
    }
}
