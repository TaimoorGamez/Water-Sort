using TMPro;
using DG.Tweening;
using UnityEngine;
using Core.Events;
using Core.Plugins;
using Core.Variables;
using Core.DB.Variables;

namespace Core.Screen
{
    public class MainMenuScreen : MonoBehaviour
    {
        [SerializeField] DBInt LvlNum;
        [SerializeField] Initialization FirebaseInit, AdmobInit;
        [SerializeField] SOEvents InitLevelEvent;
        [SerializeField] SOInterger MainMenuStateIndex, GamePlayStateIndex, SettingStateIndex, TempLvlIndex, IsFirebaseInit;
        [SerializeField] SOIntegerEvents ActiveStateEvent, DestroyStateEvent;
        [SerializeField] TextMeshProUGUI[] Lvls;
        [SerializeField] Transform LevelView;
        [SerializeField] RectTransform LevelsHolder;

        int _activeLvl = 4;

        public void OnclickSettingBtn()
        {
            ActiveStateEvent.InvokeSOEvent(SettingStateIndex.Value);
        }

        private void Start()
        {
            TempLvlIndex.Value = -1;
            if(IsFirebaseInit.Value == 1)
            {
                AdmobInit.InitPlugin();
            }
            else
            {
                FirebaseInit.InitPlugin();
            }

            for (int l = 0; l < Lvls.Length; l++)
            {
                if (l < _activeLvl)
                {
                    Lvls[l].text = (LvlNum.Value - (_activeLvl - l)).ToString();
                }
                else if (l > _activeLvl)
                {
                    Lvls[l].text = (LvlNum.Value + (l- _activeLvl)).ToString();
                }
                else
                {
                    Lvls[l].text = LvlNum.Value.ToString();
                }
            }

            LevelView.DOScale(1, 1).SetEase(Ease.OutBack).OnComplete(()=> LevelsHolder.DOAnchorPosY(150,1).SetEase(Ease.OutBack));
        }

        public void OnClickPlayButton()
        {
            InitLevelEvent.InvokeSOEvent();
            ActiveStateEvent.InvokeSOEvent(GamePlayStateIndex.Value);
            DestroyStateEvent.InvokeSOEvent(MainMenuStateIndex.Value);
        }
    }
}
