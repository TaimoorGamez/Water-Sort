using UnityEngine;
using Core.Events;
using Core.Variables;
using Core.DB.Variables;
using Core.Animations.DT;
using System.Collections.Generic;

namespace Core.GamePlay.WaterSort
{
    [CreateAssetMenu(fileName = "SwapingManager", menuName = "ScriptableObjects/WaterSort/SwapingManager")]
    public class ColorSwiper : ScriptableObject
    {
        [SerializeField] DBInt LvlNum;
        [SerializeField] SO2IntergerEvent TaskEvent;
        [SerializeField] SOIntegerEvents ToastMsgEvent, SwitchProtectorEvent;
        [SerializeField] SOEvents SwapColorsModeEvent, ChangeSwipeStateEvent;
        [SerializeField] SOInterger IsSwaping, UsingAnyFeature, MinLvlIndex;
        [SerializeField] SODOTween TubeScaleUpTween, TubeScaleDownTween;

        Dictionary<int,TubeHandler> SwapingTubes = new Dictionary<int, TubeHandler>();

        private void OnEnable()
        {
            SwapColorsModeEvent.EventHandler += OnClickSwapBtn;
        }

        private void OnDisable()
        {
            SwapColorsModeEvent.EventHandler -= OnClickSwapBtn;
        }

        void OnClickSwapBtn()
        {
            if (UsingAnyFeature.Value != 1 && IsSwaping.Value != 1)
            {
                UsingAnyFeature.Value = 1;
                IsSwaping.Value = 1;
                SwitchProtectorEvent.InvokeSOEvent(1);
                TaskEvent.InvokeSOEvent(4, 1);
            }
            else
            {
                ToastMsgEvent.InvokeSOEvent(4);
            }
        }


        public void AddTubeForSwaping(TubeHandler tube)
        {
            if (SwapingTubes.Count < 2)
            {
                if (tube.WaterColors.Count > 0)
                {
                    if (SwapingTubes.Count < 1)
                    {
                        TubeScaleUpTween.TargetObj = tube.gameObject;
                        TubeScaleUpTween.PlayAnimation();
                    }
                    SwapingTubes.Add(SwapingTubes.Count, tube);
                }
                else
                {
                    ToastMsgEvent.InvokeSOEvent(0);
                }
            }
            
            if(SwapingTubes.Count == 2)
            {
                SwapColorsNow();
            }
        }

        void SwapColorsNow()
        {
            TubeScaleDownTween.TargetObj = SwapingTubes[0].gameObject;
            TubeScaleDownTween.PlayAnimation();

            if (SwapingTubes[0].CurrentColor != SwapingTubes[1].CurrentColor)
            {
                Color oneColor = SwapingTubes[0].CurrentColor;
                SwapingTubes[0].SwapeColor(SwapingTubes[1].CurrentColor);
                SwapingTubes[1].SwapeColor(oneColor);
                if (LvlNum.Value >= MinLvlIndex.Value)
                {
                    ChangeSwipeStateEvent.InvokeSOEvent();
                }
            }
            else
            {
                ToastMsgEvent.InvokeSOEvent(1);
            }
            SwitchProtectorEvent.InvokeSOEvent(0);
            SwapingTubes.Clear();
            IsSwaping.Value = 0;
            UsingAnyFeature.Value = 0;
        }
    }
}
