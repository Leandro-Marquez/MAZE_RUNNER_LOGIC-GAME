using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class PlayerData
{
    public static string Player1Name { get; set; }
    public static string Player2Name { get; set; }
}

public class PlayersName : MonoBehaviour
{
    public TMP_InputField inputField1; // El componente InputField
    public TMP_InputField inputField2; // El componente InputField
    public Button LetsFightButton; // El botón para crear la carta

    public void OnLetsFightButtonClicked()
    {
        // Guardar los nombres en la clase estática
        PlayerData.Player1Name = inputField1.text;
        PlayerData.Player2Name = inputField2.text;

        StartCoroutine(LoadSceneAndModifyText());
    }

    private IEnumerator LoadSceneAndModifyText()
    {
        // Cargar la escena de manera aditiva
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);

        // Esperar a que la escena se cargue completamente
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Esperar un frame adicional para asegurarse de que los objetos estén disponibles
        yield return null;

        // Modificar los textos después de que la escena se haya cargado
        TextMeshPro pointsPlayer2 = GameObject.Find("PointsPlayer2")?.GetComponent<TextMeshPro>();
        TextMeshPro shadowPointsPlayer2 = GameObject.Find("ShadowPointsPlayer2")?.GetComponent<TextMeshPro>();
        TextMeshPro pointsPlayer1 = GameObject.Find("PointsPlayer1")?.GetComponent<TextMeshPro>();
        TextMeshPro shadowPointsPlayer1 = GameObject.Find("ShadowPointsPlayer1")?.GetComponent<TextMeshPro>();

        if (pointsPlayer2 != null) pointsPlayer2.text = PlayerData.Player2Name;
        if (shadowPointsPlayer2 != null) shadowPointsPlayer2.text = PlayerData.Player2Name;
        if (pointsPlayer1 != null) pointsPlayer1.text = PlayerData.Player1Name;
        if (shadowPointsPlayer1 != null) shadowPointsPlayer1.text = PlayerData.Player1Name;

        // Cargar la siguiente escena si es necesario
        SceneManager.LoadScene(2);
    }
}

