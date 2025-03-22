using TMPro;
using System.IO;
using DG.Tweening;
using UnityEngine;
using Core.Events;
using Core.Economy;
using Core.GamePlay;
using Core.Variables;
using UnityEngine.UI;
using Core.DB.Variables;
using Core.Animations.DT;
using System.Collections;

namespace Core.Screen
{
    public class CompleteScreen : MonoBehaviour
    {
        [SerializeField] Currency Coins;
        [SerializeField] DBInt LevelIndex, LvlNum;
        [SerializeField] SOInterger LevelStars, CanPlay, MainMenuStateIndex, LevelMoves, DetailsApplied, GamePlayState, LevelCompleteStateIndex,
                                     MinLvlCount, MaxLvlCount;
        [SerializeField] SOIntegerEvents SoundEffectEvent, ActiveStateEvent, DestroyStatEvent;
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
        bool _onceClicked = true;
        int _levelBonus = 15, _starsBonus = 10, _detailsBonus = 0, _totalBonus = 0, _tutorialLevels = 5; 
        Coroutine _screenShotRotine;
        string _starsDataPath;

        void Start()
        {
            _starsDataPath = Path.Combine(Application.persistentDataPath, "starsData.json");
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
                _detailsBonus = 20;
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
            yield return new WaitForSeconds(0.1f);
            GameData starsData = LoadStars();
            yield return new WaitForSeconds(0.1f);
            LevelData existingLevel = starsData.Levels.Find(l => l.LevelNumber == LevelIndex.Value);
            if (existingLevel != null)
            {
                existingLevel.Stars = LevelStars.Value; // Save max stars
            }
            else
            {
                starsData.Levels.Add(new LevelData { LevelNumber = LevelIndex.Value, Stars = LevelStars.Value });
            }

            string json = JsonUtility.ToJson(starsData, true);
            File.WriteAllText(_starsDataPath, json);
            yield return new WaitForSeconds(0.1f); 
            _onceClicked = false;
        }

        GameData LoadStars()
        {
            if (File.Exists(_starsDataPath))
            {
                string json = File.ReadAllText(_starsDataPath);
                return JsonUtility.FromJson<GameData>(json);
            }
            return new GameData();
        }

        public void OnClickNext()
        {
            if (!_onceClicked)
            {
                _onceClicked = true;
                CanPlay.Value = 0;
                Coins.Amount += _totalBonus;
                LvlNum.Value++;
                LevelIndex.Value++;
                Invoke(nameof(GoNext), 2);
            }
        }

        void GoNext()
        {
            if (LevelIndex.Value > MaxLvlCount.Value)
            {
                LevelIndex.Value = MinLvlCount.Value;
            }

            if (LevelIndex.Value < _tutorialLevels)
            {
                ActiveStateEvent.InvokeSOEvent(GamePlayState.Value);
            }
            else
            {
                ActiveStateEvent.InvokeSOEvent(MainMenuStateIndex.Value);
            }
            DestroyStatEvent.InvokeSOEvent(LevelCompleteStateIndex.Value);
        }
    }
}
