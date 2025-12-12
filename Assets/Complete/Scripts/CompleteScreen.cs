using TMPro;
using System.IO;
using UnityEngine;
using DG.Tweening;
using Core.Events;
using Core.Economy;
using Core.GamePlay;
using Core.Variables;
using UnityEngine.UI;
using Core.DB.Variables;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.Screen
{
    public class CompleteScreen : UiScreens
    {
        [SerializeField] Currency CashCurrency;
        [SerializeField] DBInt LevelIndex, LvlNum;
        [SerializeField] SOInterger LevelStars, CanPlay, MainMenuStateIndex, LevelMoves, DetailsApplied, GamePlayState, LevelCompleteStateIndex,
                                     MinLvlCount, MaxLvlCount, TempLvlIndex, CurrentMultiplayer,SortingCompleted;
        [SerializeField] SO2IntergerEvent TaskEvent;
        [SerializeField] SOIntegerEvents SoundEffectEvent, ActiveStateEvent, DestroyStatEvent;
        [SerializeField] SOEvents DestroyLevelEvent, InitLvlEvent, MultiplayRewardEvent;
        [SerializeField] TextMeshProUGUI LevelBonusText, StarsBonusText, DetailsBonusText, MovesBonusText, TotalBonusText;
        [SerializeField] Camera ScreenshotCamera;
        [SerializeField] RenderTexture TargetTexture;
        [SerializeField] RawImage DisplayImage;
        [SerializeField] Image[] StarsImg;
        [SerializeField] RectTransform StarsObj, NextBtn;
        [SerializeField] GameObject MultiplayerBar, RvBtn;


        float _starsPos = 100, _durationTweeing = 1f;
        bool _onceClicked = true;
        int _levelBonus = 15, _starsBonus = 10, _detailsBonus = 0, _totalBonus = 0; 
        Coroutine _screenShotRotine;
        string _starsDataPath;
        Vector2 _nextBtnPosition = new Vector2(-135, -430);
        string textFolder = "Appreation/Text/";
        GameObject _textObj;
        AsyncOperationHandle _txtHandle;

        private void OnEnable()
        {
            MultiplayRewardEvent.EventHandler += OnMultiplyReward;
        }

        private async void OnDisable()
        {
            MultiplayRewardEvent.EventHandler -= OnMultiplyReward;
            if (_screenShotRotine != null)
            {
                StopCoroutine(_screenShotRotine);
            }
            await ReleaseHandler();
        }

        void Start()
        {
            int textNum = 0;
            if (LevelStars.Value > 2)
            { 
                textNum = Random.Range(1,9);
            }
            LoadAppreationText(textFolder + textNum);
            SortingCompleted.Value = 0;
            _starsDataPath = Path.Combine(Application.persistentDataPath, "starsData.json");
            _screenShotRotine = StartCoroutine(CaptureColoredArea());
            TaskEvent.InvokeSOEvent(1,1);
        }


        async void LoadAppreationText(string path)
        {
            _txtHandle = Addressables.LoadAssetAsync<GameObject>(path);
            await _txtHandle.Task;

            if (_txtHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to load Addressable prefab at: {path}");
                return;
            }

            _textObj = null;
            _textObj = Instantiate(_txtHandle.Result as GameObject, transform);

            await Task.Yield();
            await Task.Yield();

            while (_textObj == null)
                await Task.Yield();
        }

        IEnumerator CaptureColoredArea()
        {
            ScreenshotCamera.targetTexture = TargetTexture;
            ScreenshotCamera.Render(); 
            int currentLvl = TempLvlIndex.Value != -1 ? TempLvlIndex.Value : LvlNum.Value;
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

            DisplayImage.texture = screenshot;
            string directoryPath = Path.Combine(Application.persistentDataPath, "Paintings");
            if (!Directory.Exists(directoryPath))
            { Directory.CreateDirectory(directoryPath); }

            string filePath = Path.Combine(directoryPath, $"Painting_{currentLvl}.png");
            File.WriteAllBytes(filePath, screenshot.EncodeToPNG());
            yield return new WaitForSeconds(2.5f);

            DestroyLevelEvent.InvokeSOEvent();
            OnOpen();
            Invoke(nameof(OnPanelVisible), _durationTweeing);
            for (int s = 0; s < LevelStars.Value; s++)
            {
                StarsImg[s].color = Color.white;
            }
            yield return new WaitForSeconds(0.1f);

            GameData starsData = LoadStars();
            yield return new WaitForSeconds(0.1f);
            LevelData existingLevel = starsData.Levels.Find(l => l.LevelNumber == currentLvl);
            if (existingLevel != null)
            {
                existingLevel.Stars = LevelStars.Value; // Save max stars
            }
            else
            {
                starsData.Levels.Add(new LevelData { LevelNumber = currentLvl, Stars = LevelStars.Value });
            }

            string json = JsonUtility.ToJson(starsData, true);
            File.WriteAllText(_starsDataPath, json);
            yield return new WaitForSeconds(0.1f); 
            _onceClicked = false;
        }

        void OnPanelVisible()
        {
            DestroyStatEvent.InvokeSOEvent(GamePlayState.Value);
            if (LvlNum.Value >= MinLvlCount.Value)
            {
                NextBtn.DOAnchorPos(_nextBtnPosition, _durationTweeing).SetEase(Ease.InOutBack).OnComplete(() =>
                {
                    MultiplayerBar.SetActive(true);
                    RvBtn.SetActive(true);
                });
            }
            StarsObj.DOAnchorPosY(_starsPos, _durationTweeing).SetEase(Ease.OutBack);
            StarsObj.DOScale(1, _durationTweeing).SetEase(Ease.OutBack).OnComplete(() => {
                DisplayImage.gameObject.SetActive(true);
                SoundEffectEvent.InvokeSOEvent(7);
                DisplayImage.transform.DOScale(1, _durationTweeing / 2).SetEase(Ease.OutBack);

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

        GameData LoadStars()
        {
            if (File.Exists(_starsDataPath))
            {
                string json = File.ReadAllText(_starsDataPath);
                return JsonUtility.FromJson<GameData>(json);
            }
            return new GameData();
        }

        void OnMultiplyReward()
        {
            if (!_onceClicked)
            {
                _onceClicked = true;
                CanPlay.Value = 0;
                CashCurrency.Amount += (_totalBonus * CurrentMultiplayer.Value);
                if (TempLvlIndex.Value == -1)
                {
                    LvlNum.Value++;
                    LevelIndex.Value++;
                }
                Invoke(nameof(OnClose), 2);
            }
        }

        public void OnClickNext()
        {
            if (!_onceClicked)
            {
                _onceClicked = true;
                CanPlay.Value = 0;
                CashCurrency.Amount += _totalBonus;
                if (TempLvlIndex.Value == -1)
                {
                    LvlNum.Value++;
                    LevelIndex.Value++;
                }
                Invoke(nameof(OnClose), 2);
            }
        }

        public override void OnOpen()
        {
            if (_textObj != null)
                Destroy(_textObj);

           SoundEffectEvent.InvokeSOEvent(3);
           Body.DOAnchorPosX(0, _transitionDuration).SetEase(Ease.OutBack);
        }

        public override void OnClose()
        {
            SoundEffectEvent.InvokeSOEvent(2);
            Body.DOAnchorPosX(1500, _transitionDuration/2).SetEase(Ease.InBack).OnComplete(() => {
                if (LevelIndex.Value > MaxLvlCount.Value)
                {
                    LevelIndex.Value = MinLvlCount.Value;
                }

                if (LvlNum.Value <= MinLvlCount.Value)
                {
                    InitLvlEvent.InvokeSOEvent();
                    ActiveStateEvent.InvokeSOEvent(GamePlayState.Value);
                }
                else
                {
                    ActiveStateEvent.InvokeSOEvent(MainMenuStateIndex.Value);
                }
                DestroyStatEvent.InvokeSOEvent(LevelCompleteStateIndex.Value);
            });
        }

        async Task ReleaseHandler()
        {
            if(!_txtHandle.IsValid())
                return;

            Addressables.Release(_txtHandle);
            while (_txtHandle.IsValid())
                await Task.Yield();
        }
    }
}
