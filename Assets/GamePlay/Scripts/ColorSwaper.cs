using UnityEngine;
using Core.Events;
using Core.Variables;
using Core.DB.Variables;
using Core.Animations.DT;
using System.Collections.Generic;

namespace Core.GamePlay.WaterSort
{
    public class ColorSwaper : MonoBehaviour
    {
        [SerializeField] DBInt LvlNum;
        [SerializeField] SO2IntergerEvent TaskEvent;
        [SerializeField] SOInterger IsSwaping, UsingAnyFeature, MinLvlIndex, CompletedTubes, CurrrentLvl, SortingCompleted;
        [SerializeField] SODOTween TubeScaleUpTween, TubeScaleDownTween;

        Dictionary<int,TubeHandler> SwapingTubes = new Dictionary<int, TubeHandler>();

        private void OnEnable()
        {
            SimpleEventsHolder.SwapColorsEvent += OnClickSwapBtn;
            SimpleEventsHolder.StartColoringEvent += StopSwapping;
        }

        private void OnDisable()
        {
            SimpleEventsHolder.SwapColorsEvent -= OnClickSwapBtn;
            SimpleEventsHolder.StartColoringEvent -= StopSwapping;
        }

        void OnClickSwapBtn()
        {
            if (SortingCompleted.Value == 0 && CompletedTubes.Value < CurrrentLvl.Value-1)
            {
                if (UsingAnyFeature.Value != 1 && IsSwaping.Value != 1)
                {
                    UsingAnyFeature.Value = 1;
                    IsSwaping.Value = 1;
                    SingleIntegerEventsHolder.SwitchProtectorEvent?.Invoke(1);
                    SingleIntegerEventsHolder.ShowToastEvent?.Invoke(8);
                }
                else
                {
                    SingleIntegerEventsHolder.ShowToastEvent?.Invoke(4);
                }
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
                        SingleIntegerEventsHolder.ShowToastEvent?.Invoke(9);
                        TubeScaleUpTween.TargetObj = tube.gameObject;
                        TubeScaleUpTween.PlayAnimation();
                    }
                    SwapingTubes.Add(SwapingTubes.Count, tube);
                }
                else
                {
                    SingleIntegerEventsHolder.ShowToastEvent?.Invoke(0);
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
                    SimpleEventsHolder.UpdateSwapStateEvent?.Invoke();
                }
                TaskEvent.InvokeSOEvent(4, 1);
            }
            else
            {
                SingleIntegerEventsHolder.ShowToastEvent?.Invoke(1);
            }
            SingleIntegerEventsHolder.SwitchProtectorEvent?.Invoke(0);
            SwapingTubes.Clear();
            IsSwaping.Value = 0;
            UsingAnyFeature.Value = 0;
        }

        void StopSwapping()
        {
            SingleIntegerEventsHolder.SwitchProtectorEvent?.Invoke(0);
            SwapingTubes.Clear();
            IsSwaping.Value = 0;
            UsingAnyFeature.Value = 0;
        }
    }
}
