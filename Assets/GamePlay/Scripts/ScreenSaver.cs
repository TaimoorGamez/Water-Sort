using System.IO;
using UnityEngine;
using DG.Tweening;
using Core.Events;
using Core.Variables;
using Core.DB.Variables;
using System.Collections;

namespace Core.GamePlay.Coloring
{
    public class ScreenSaver : MonoBehaviour
    {
        [SerializeField] DBInt LvlNum;
        [SerializeField] SOIntegerEvents ChangeStateEvent;
        [SerializeField] SOInterger CompleteStateIndex;
        [SerializeField] RectTransform ColoringImage;
        [SerializeField] ParticleSystem StarParticles;
        [SerializeField] ColorFilling[] ColoringPart;
        [SerializeField] Texture2D Details;
        [SerializeField] Camera ScreenshotCamera;
        [SerializeField] RenderTexture TargetTexture;

        float _preparationTime = 1, _finalScale = 1.5f, _finalPos = 200;
        Coroutine _screenShotRotine;

        private void OnEnable()
        {
            StarParticles.Play();
            ColoringImage.DOScale(_finalScale, _preparationTime);
            ColoringImage.DOAnchorPosY(_finalPos, _preparationTime).OnComplete(() =>
            {
                _screenShotRotine = StartCoroutine(CaptureColoredArea());
            });
        }

        IEnumerator CaptureColoredArea()
        {
            string folderPath = "GamePlay/Resources/Paintings";
            ScreenshotCamera.targetTexture = TargetTexture;
            ScreenshotCamera.Render();
            yield return new WaitForSeconds(0.01f);
            // Create a new Texture2D
            Texture2D screenshot = new Texture2D(TargetTexture.width, TargetTexture.height, TextureFormat.RGB24, false);
            yield return new WaitForSeconds(0.01f);
            // Read the pixels from the RenderTexture
            RenderTexture.active = TargetTexture;
            screenshot.ReadPixels(new Rect(0, 0, TargetTexture.width, TargetTexture.height), 0, 0);
            screenshot.Apply();
            yield return new WaitForSeconds(0.5f);
            // Reset the RenderTexture
            RenderTexture.active = null;
            //ScreenshotCamera.targetTexture = null;

            // Display the screenshot in the complete panel
            //displayImage.texture = screenshot;

            string directoryPath = Path.Combine(Application.dataPath, folderPath);
            string filePath = Path.Combine(directoryPath, "Painting " + LvlNum.Value + ".png");
            // Optional: Save the screenshot as a PNG
            byte[] bytes = screenshot.EncodeToPNG();
            File.WriteAllBytes(filePath, bytes);
            yield return new WaitForSeconds(0.5f);
            ChangeStateEvent.InvokeSOEvent(CompleteStateIndex.Value);

#if UNITY_EDITOR
            // Refresh the AssetDatabase to show the file in the editor
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        Color32 BlendDetailPixel(Color32 basePixel, Color32 detailPixel)
        {
            float alpha = detailPixel.a / 255f; // Normalize alpha to [0, 1]

            // Darken the base pixel by the amount of black in the detail pixel
            float detailIntensity = 1f - (detailPixel.r / 255f); // Assuming grayscale, use red channel
            detailIntensity *= alpha; // Modulate by alpha of the detail pixel

            return new Color32(
                (byte)(basePixel.r * (1f - detailIntensity)),
                (byte)(basePixel.g * (1f - detailIntensity)),
                (byte)(basePixel.b * (1f - detailIntensity)),
                basePixel.a // Preserve original alpha
            );
        }

        private void OnDisable()
        {
            if (_screenShotRotine != null)
            {
                StopCoroutine(_screenShotRotine);
            }
        }
    }
}
