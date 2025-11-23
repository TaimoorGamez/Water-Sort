using System.IO;
using DG.Tweening;
using UnityEngine;
using Core.Events;
using Core.GamePlay;
using Core.Variables;
using UnityEngine.UI;
using System.Collections;

namespace Core.Screen
{
    public class GalleryScreen : UiScreens
    {
        [SerializeField] SOIntegerEvents SoundEffectEvent;
        [SerializeField] SOInterger MinLvlNum;
        [SerializeField] PaintingView PaintingObj;
        [SerializeField] Transform Body;
        [SerializeField] RectTransform Content;
        [SerializeField] ScrollRect GalaryScroll;

        float _tweenTime = 0.5f;
        string _starsDataPath, _directoryPath;
        Coroutine _paintingRotine;

        private void OnEnable()
        {
            Body.DOScale(1, _tweenTime).SetEase(Ease.OutBack);
            SoundEffectEvent.InvokeSOEvent(2);
        }

        private void OnDisable()
        {
            if (_paintingRotine != null)
                StopCoroutine(_paintingRotine);
        }

        private void Start()
        {
            _starsDataPath = Path.Combine(Application.persistentDataPath, "starsData.json");
            _directoryPath = Path.Combine(Application.persistentDataPath, "Paintings");
            _paintingRotine = StartCoroutine(PaintingsCorotine());
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

        IEnumerator PaintingsCorotine()
        {
            GameData starsData = LoadStars();
            yield return new WaitForSeconds(0.01f);
            for(int s = MinLvlNum.Value-1; s < starsData.Levels.Count; s++)
            {
                string filePath = Path.Combine(_directoryPath, $"Painting_{starsData.Levels[s].LevelNumber}.png");
                if (File.Exists(filePath))
                {
                    byte[] fileData = File.ReadAllBytes(filePath);
                    Texture2D texture = new Texture2D(2, 2);
                    if (texture.LoadImage(fileData))
                    {
                        yield return new WaitForSeconds(0.01f);
                        PaintingView newPainting = Instantiate(PaintingObj, Content);
                        newPainting.InitPainting(texture, starsData.Levels[s].LevelNumber, starsData.Levels[s].Stars);
                    }
                }
            }
            GalaryScroll.DOVerticalNormalizedPos(0f, 1).SetEase(Ease.OutBack);
        }

        public override void OnClose()
        {
            Body.DOScale(0, _tweenTime).SetEase(Ease.InBack).OnComplete(() => gameObject.SetActive(false));
            SoundEffectEvent.InvokeSOEvent(2);
        }
    }
}
