using TMPro;
using System.IO;
using DG.Tweening;
using UnityEngine;
using Core.Events;
using Core.Variables;
using UnityEngine.UI;
using Core.DB.Variables;
using Core.Animations.DT;
using System.Collections;

namespace Core.Screen
{
    public class CompleteScreen : MonoBehaviour
    {
        [SerializeField] DBInt LevelIndex, LvlNum;
        [SerializeField] SOInterger LevelStars, CanPlay, MainMenuStateIndex, LevelMoves, DetailsApplied, GamePlayState;
        [SerializeField] SOIntegerEvents SoundEffectEvent, ChangeStateEvent, DestroyStatEvent;
        [SerializeField] SOEvents DestroyLevelEvent;
        [SerializeField] SOAnChorMove ShowPanel;
        [SerializeField] GameObject Body;
        [SerializeField] RectTransform StarsObj;
        [SerializeField] Image[] StarsImg;
        [SerializeField] TextMeshProUGUI LevelBonusText, StarsBonusText, DetailsBonusText, MovesBonusText, TotalBonusText;
        [SerializeField] Camera ScreenshotCamera;
        [SerializeField] RenderTexture TargetTexture;
        [SerializeField] RawImage DisplayImage;

        float _starsPos = 100, _durationTweeing = 1f;
        bool _onceClicked = false; 
        int _levelBonus = 50, _starsBonus = 100, _detailsBonus = 0, _totalBonus = 0, _tutorialLevels = 5; 
        Coroutine _screenShotRotine;

        void Start()
        {
            _screenShotRotine = StartCoroutine(CaptureColoredArea());
        }

        void OnPanelVisible()
        {
            DestroyStatEvent.InvokeSOEvent(GamePlayState.Value);
            StarsObj.DOAnchorPosY(_starsPos, _durationTweeing).SetEase(Ease.OutBack);
            StarsObj.DOScale(1, _durationTweeing).SetEase(Ease.OutBack).OnComplete(() => {
                DisplayImage.gameObject.SetActive(true);
                SoundEffectEvent.InvokeSOEvent(7);
                DisplayImage.transform.DOScale(1, _durationTweeing/2).SetEase(Ease.OutBack);

            });
            DOTween.To(() => 0, x => LevelBonusText.text = x.ToString(), _levelBonus, _durationTweeing);
            if (LevelMoves.Value > 0)
            { DOTween.To(() => 0, x => MovesBonusText.text = x.ToString(), LevelMoves.Value, _durationTweeing); }
            if (DetailsApplied.Value == 1)
            {
                _detailsBonus = 100;
                DOTween.To(() => 0, x => DetailsBonusText.text = x.ToString(), _detailsBonus, _durationTweeing); 
            }
            _starsBonus *= LevelStars.Value;
            DOTween.To(() => 0, x => StarsBonusText.text = x.ToString(), _starsBonus, _durationTweeing);
            _totalBonus = (_levelBonus + LevelMoves.Value + _detailsBonus + _starsBonus);
            DOTween.To(() => 0, x => TotalBonusText.text = x.ToString(), _totalBonus, _durationTweeing).SetDelay(_durationTweeing);
            TotalBonusText.rectTransform.DOScale(1, _durationTweeing).SetEase(Ease.OutBack).SetDelay(_durationTweeing);
        }

        IEnumerator CaptureColoredArea()
        {
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
            yield return new WaitForSeconds(0.25f);
            // Reset the RenderTexture
            RenderTexture.active = null;
            //ScreenshotCamera.targetTexture = null;

            // Display the screenshot in the complete panel
            DisplayImage.texture = screenshot;
            string directoryPath = Path.Combine(Application.persistentDataPath, "Paintings");
            if (!Directory.Exists(directoryPath))
            { Directory.CreateDirectory(directoryPath); }

            string filePath = Path.Combine(directoryPath, $"Painting_{LvlNum.Value}.png");
            File.WriteAllBytes(filePath, screenshot.EncodeToPNG());
            yield return new WaitForSeconds(2.5f);

            DestroyLevelEvent.InvokeSOEvent();
            ShowPanel.TargetObj = Body;
            ShowPanel.PlayAnimation();
            SoundEffectEvent.InvokeSOEvent(3);
            Invoke(nameof(OnPanelVisible), _durationTweeing);
            for (int s = 0; s < LevelStars.Value; s++)
            {
                StarsImg[s].material = null;
            }

            //if (File.Exists(filePath))
            //{
            //    byte[] fileData = File.ReadAllBytes(filePath);
            //    Texture2D texture = new Texture2D(2, 2);
            //    if (texture.LoadImage(fileData)) // Load PNG into Texture2D
            //    {
            //        PaintingImg.texture = texture; // Set to UI RawImage
            //    }
            //}
            //else
            //{
            //    Debug.LogWarning("Image not found: " + filePath);
            //}
        }

        public void OnClickNext()
        {
            if (!_onceClicked)
            {
                _onceClicked = true;
                CanPlay.Value = 0;
                LvlNum.Value++;
                LevelIndex.Value++;
                if (LvlNum.Value < _tutorialLevels)
                {
                    ChangeStateEvent.InvokeSOEvent(GamePlayState.Value);
                }
                else
                {
                    ChangeStateEvent.InvokeSOEvent(MainMenuStateIndex.Value);
                }
            }
        }
    }
}
