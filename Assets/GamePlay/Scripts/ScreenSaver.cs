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
        //public RawImage displayImage; // Assign the RawImage in your complete panel

        float _preparationTime = 1, _finalScale = 1.5f, _finalPos = 200;
        Coroutine _screenShotRotine;

        private void OnEnable()
        {
            StarParticles.Play();
            _screenShotRotine = StartCoroutine(CaptureColoredArea());
        }

        IEnumerator CaptureColoredArea()
        {
            Texture2D partTexture;
            int textureSixe = 128;
            string folderPath = "GamePlay/Resources/Paintings";
            Texture2D paintingTexture = new Texture2D(textureSixe, textureSixe, TextureFormat.RGBA32, false);
            Color defaultColor = Color.clear;
            Color32[] partPixles;

            for (int p = 0; p < ColoringPart.Length; p++)
            {
                partTexture = ColoringPart[p].GetCurrenTexture();
                yield return new WaitForSeconds(0.1f);
                partPixles = partTexture.GetPixels32();
                yield return new WaitForSeconds(0.1f);
                paintingTexture.SetPixels32(partPixles);
                yield return new WaitForSeconds(0.1f);
                paintingTexture.Apply();
            }

            partPixles = Details.GetPixels32();
            Color32[] paintingPixels = paintingTexture.GetPixels32();
            yield return new WaitForSeconds(0.1f);
            for (int i = 0; i < partPixles.Length; i++)
            {
                paintingPixels[i] = BlendDetailPixel(paintingPixels[i], partPixles[i]);
            }
                paintingTexture.SetPixels32(partPixles);
            yield return new WaitForSeconds(0.1f);
            paintingTexture.Apply();
            yield return new WaitForSeconds(0.1f);

            // Convert texture to PNG
            byte[] pngData = paintingTexture.EncodeToPNG();

            // Ensure Resources folder exists
            string directoryPath = Path.Combine(Application.dataPath, folderPath);
            if (!Directory.Exists(directoryPath))
            {
                Debug.Log("error");
            }

            // Save the PNG file
            string filePath = Path.Combine(directoryPath, "Painting " + LvlNum.Value + ".png");
            File.WriteAllBytes(filePath, pngData);

            //Debug.Log($"Texture saved to: {filePath}");

#if UNITY_EDITOR
            // Refresh the AssetDatabase to show the file in the editor
            UnityEditor.AssetDatabase.Refresh();
#endif
            yield return new WaitForSeconds(1f); 
            ColoringImage.DOScale(_finalScale, _preparationTime);
            ColoringImage.DOAnchorPosY(_finalPos, _preparationTime).OnComplete(() =>
            {
                //ChangeStateEvent.InvokeSOEvent(CompleteStateIndex.Value);
            });
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
