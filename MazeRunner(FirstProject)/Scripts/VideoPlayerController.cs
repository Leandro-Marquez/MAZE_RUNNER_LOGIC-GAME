using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
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
        videoPlayer.targetTexture = new RenderTexture(1920, 1080, 0);
        // Configura el RawImage para mostrar la Render Texture
        rawImage.texture = videoPlayer.targetTexture;
        // Reproduce el video
        videoPlayer.Play();
        videoPlayer.loopPointReached += EndReached;
    }
    private void EndReached(VideoPlayer f)
    {
        SceneManager.LoadScene(1);
    }
}