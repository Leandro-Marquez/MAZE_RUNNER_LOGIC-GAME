using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesController : MonoBehaviour
{
    // Start is called before the first frame update
    public void LoadNextScene()
    {
        SceneManager.LoadScene(2);
    }
    public static void LoadGameScene()
    {
        SceneManager.LoadScene(3);
    }
}
