using System.Collections;
using System.Collections.Generic;
// using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoController : MonoBehaviour 
{
   public VideoPlayer videoPlayer;
   public RawImage rawImage;
   void Start()
   {
        // Configura el VideoPlayer para usar la Render Texture
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = new RenderTexture(1920, 1080, 0);//ajustar la resolucion de pantalla adecuada
        // Configura el RawImage para mostrar la Render Texture
        rawImage.texture = videoPlayer.targetTexture;
        // Reproduce el video
        videoPlayer.Play();
        videoPlayer.loopPointReached += EndReached; //para cuando se termine 
    }
    private void EndReached(VideoPlayer f) //cuando se acabe el video 
    {
        SceneManager.LoadScene(1); //cargar la escena del menu del juego 
    }
}