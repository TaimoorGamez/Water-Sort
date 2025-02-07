using TMPro;
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
        [SerializeField] SOInterger LevelStars, CanPlay, MainMenuStateIndex, LevelMoves, DetailsApplied, GamePlayState;
        [SerializeField] SOIntegerEvents SoundEffectEvent, ChangeStateEvent, DestroyStatEvent;
        [SerializeField] SOEvents DestroyLevelEvent;
        [SerializeField] SOAnChorMove ShowPanel;
        [SerializeField] GameObject Body;
        [SerializeField] RectTransform StarsObj;
        [SerializeField] Image[] StarsImg;
        [SerializeField] TextMeshProUGUI LevelBonusText, StarsBonusText, DetailsBonusText, MovesBonusText, TotalBonusText;

        float _starsPos = 100, _durationTweeing = 1f;
        bool _onceClicked = false; 
        int _levelBonus = 50, _starsBonus = 100, _detailsBonus = 0, _totalBonus = 0, _tutorialLevels = 5;

        void Start()
        {
            ShowPanel.TargetObj = Body;
            ShowPanel.PlayAnimation();
            SoundEffectEvent.InvokeSOEvent(3);
            Invoke(nameof(OnPanelVisible), _durationTweeing);
            for (int s = 0; s < LevelStars.Value; s++)
            {
                StarsImg[s].material = null;
            }
            DestroyLevelEvent.InvokeSOEvent();
        }

        void OnPanelVisible()
        {
            DestroyStatEvent.InvokeSOEvent(GamePlayState.Value);
            StarsObj.DOAnchorPosY(_starsPos, _durationTweeing).SetEase(Ease.OutBack);
            StarsObj.DOScale(1, _durationTweeing).SetEase(Ease.OutBack);
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
