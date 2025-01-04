using DG.Tweening;
using UnityEngine;
using Core.Events;
using Core.Variables;
using UnityEngine.UI;
using Core.DB.Variables;
using Core.Animations.DT;

namespace Core.Screen
{
    public class CompleteScreen : MonoBehaviour
    {
        [SerializeField] DBInt LevelIndex, LvlNum;
        [SerializeField] SOInterger LevelStars, CanPlay, MainMenuStateIndex;
        [SerializeField] SOIntegerEvents SoundEffectEvent, ChangeStateEvent;
        [SerializeField] SOEvents DestroyLevelEvent;
        [SerializeField] SOAnChorMove ShowPanel;
        [SerializeField] GameObject Body;
        [SerializeField] RectTransform StarsObj, Painting;
        [SerializeField] Image[] StarsImg;
        [SerializeField] Image PaintingImg;
        [SerializeField] Button NextButton;

        float _starsPos = 100, _durationTweeing = 1f;
        string _paintingPath = "Paintings/Painting ";
        bool _onceClicked = false;

        private void OnEnable()
        {
            DestroyLevelEvent.InvokeSOEvent();
            NextButton.onClick.AddListener(OnClickNext);
            ShowPanel.TargetObj = Body;
            ShowPanel.PlayAnimation();
            SoundEffectEvent.InvokeSOEvent(3);
            Invoke(nameof(OnPanelVisible), 0.5f);
            for (int s = 0; s < LevelStars.Value; s++)
            {
                StarsImg[s].material = null;
            }
            PaintingImg.sprite = Resources.Load<Sprite>(_paintingPath + LevelIndex.Value);
        }

        private void OnDisable()
        {
            NextButton.onClick.RemoveListener(OnClickNext);
        }

        void OnPanelVisible()
        {
            StarsObj.DOAnchorPosY(_starsPos, _durationTweeing).SetEase(Ease.OutBack);
            StarsObj.DOScale(1, _durationTweeing).SetEase(Ease.OutBack);
            Painting.gameObject.SetActive(true);
            Painting.DOScale(1, _durationTweeing).SetEase(Ease.OutBack);
        }


        void OnClickNext()
        {
            if (!_onceClicked)
            {
                _onceClicked = true;
                CanPlay.Value = 0;
                ChangeStateEvent.InvokeSOEvent(MainMenuStateIndex.Value);
                LvlNum.Value++;
                LevelIndex.Value++;
            }
        }
    }
}
