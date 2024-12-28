using UnityEngine;
using DG.Tweening;
using System.Collections;

namespace Core.GamePlay.Coloring
{
    public class ScreenSaver : MonoBehaviour
    {
        [SerializeField] Camera ScreenshotCamera;
        [SerializeField] RectTransform ColoringImage;
        [SerializeField] ParticleSystem StarParticles;
        [SerializeField] RenderTexture renderTexture; // Assign your RenderTexture here
        //public RawImage displayImage; // Assign the RawImage in your complete panel

        float _preparationTime = 1, _finalScale = 1.5f, _finalPos = 200;
        Coroutine _screenShotRotine;

        private void OnEnable()
        {
                ColoringImage.DOScale(_finalScale, _preparationTime);
                ColoringImage.DOAnchorPosY(_finalPos, _preparationTime).OnComplete(() =>
                {
                    StarParticles.Play();
                    _screenShotRotine = StartCoroutine(CaptureColoredArea());
                    //ChangeStateEvent.InvokeSOEvent(CompleteStateIndex.Value);
                });
        }

        IEnumerator CaptureColoredArea()
        {
            // Set the camera to render the colored area
            ScreenshotCamera.targetTexture = renderTexture;
            ScreenshotCamera.Render();
            yield return new WaitForSeconds(0.01f);
            // Create a new Texture2D
            Texture2D screenshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            yield return new WaitForSeconds(0.01f);
            // Read the pixels from the RenderTexture
            RenderTexture.active = renderTexture;
            screenshot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            screenshot.Apply();
            yield return new WaitForSeconds(0.5f);
            // Reset the RenderTexture
            RenderTexture.active = null;
            ScreenshotCamera.targetTexture = null;

            // Display the screenshot in the complete panel
            //displayImage.texture = screenshot;

            // Optional: Save the screenshot as a PNG
            byte[] bytes = screenshot.EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.persistentDataPath + "/ColoredArea.png", bytes);
            Debug.Log("Screenshot saved to: " + Application.persistentDataPath + "/ColoredArea.png");

            yield return new WaitForSeconds(0.5f);
        }
    }
}
