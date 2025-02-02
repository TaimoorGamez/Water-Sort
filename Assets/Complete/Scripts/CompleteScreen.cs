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
        [SerializeField] SOIntegerEvents SoundEffectEvent, ChangeStateEvent, DeActiveStatEvent;
        [SerializeField] SOEvents DestroyLevelEvent;
        [SerializeField] SOAnChorMove ShowPanel;
        [SerializeField] GameObject Body;
        [SerializeField] RectTransform StarsObj, Painting;
        [SerializeField] Image[] StarsImg;
        [SerializeField] RawImage PaintingImg;
        [SerializeField] Button NextButton;
        [SerializeField] TextMeshProUGUI LevelBonusText, StarsBonusText, DetailsBonusText, MovesBonusText, TotalBonusText;
        [SerializeField] Camera ScreenshotCamera;
        [SerializeField] RenderTexture TargetTexture;

        float _starsPos = 100, _durationTweeing = 1f;
        bool _onceClicked = false; 
        int _levelBonus = 50, _starsBonus = 100, _detailsBonus = 0, _totalBonus = 0, _tutorialLevels = 5;
        Coroutine _screenShotRotine;

        private void OnEnable()
        {
            NextButton.onClick.AddListener(OnClickNext);
        }

        private void Start()
        {
            _screenShotRotine = StartCoroutine(CaptureColoredArea());
        }

        private void OnDisable()
        {
            NextButton.onClick.RemoveListener(OnClickNext);
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
            yield return new WaitForSeconds(0.25f);
            // Reset the RenderTexture
            RenderTexture.active = null;

            string directoryPath = Path.Combine(Application.dataPath, folderPath);
            string filePath = Path.Combine(directoryPath, "Painting " + LvlNum.Value + ".png");
            // Optional: Save the screenshot as a PNG
            byte[] bytes = screenshot.EncodeToPNG();
            File.WriteAllBytes(filePath, bytes);
            yield return new WaitForSeconds(2.5f);
            DeActiveStatEvent.InvokeSOEvent(GamePlayState.Value);
            DestroyLevelEvent.InvokeSOEvent();
            ScreenshotCamera.gameObject.SetActive(true);
            ShowPanel.TargetObj = Body;
            ShowPanel.PlayAnimation();
            SoundEffectEvent.InvokeSOEvent(3);
            Invoke(nameof(OnPanelVisible), _durationTweeing);
            for (int s = 0; s < LevelStars.Value; s++)
            {
                StarsImg[s].material = null;
            }
            PaintingImg.texture = screenshot;
            if (_screenShotRotine != null)
            {
                StopCoroutine(_screenShotRotine);
            }

#if UNITY_EDITOR
            // Refresh the AssetDatabase to show the file in the editor
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        void OnPanelVisible()
        {
            StarsObj.DOAnchorPosY(_starsPos, _durationTweeing).SetEase(Ease.OutBack);
            StarsObj.DOScale(1, _durationTweeing).SetEase(Ease.OutBack);
            SoundEffectEvent.InvokeSOEvent(7);
            Painting.gameObject.SetActive(true);
            Painting.DOScale(1, _durationTweeing).SetEase(Ease.OutBack);
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


        void OnClickNext()
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
