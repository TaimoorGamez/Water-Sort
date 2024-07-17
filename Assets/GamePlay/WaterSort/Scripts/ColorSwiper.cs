using UnityEngine;
using Core.Events;
using Core.Variables;
using Core.Animations.LT;
using System.Collections.Generic;

namespace Core.GamePlay.WaterSort
{
    [CreateAssetMenu(fileName = "SwapingManager", menuName = "ScriptableObjects/WaterSort/SwapingManager")]
    public class ColorSwiper : ScriptableObject
    {
        [SerializeField] SOEvents SwipeColorsModeEvent;
        [SerializeField] SOInterger IsSwaping, UsingAnyFeature;
        [SerializeField] SOIntegerEvents ToastMsgEvent;
        [SerializeField] SOLeanTween TubeScaleUpTween, TubeScaleDownTween;

        Dictionary<int,TubeHandler> SwapingTubes = new Dictionary<int, TubeHandler>();

        public void AddTubeForSwaping(TubeHandler tube)
        {
            if (SwapingTubes.Count < 2)
            {
                if (tube.MyLiquid.WaterColors.Count > 0)
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
                    //Debug.Log("Here34");
                    ToastMsgEvent.InvokeEvent(0);
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

            if (SwapingTubes[0].MyLiquid.CurrentTopColor != SwapingTubes[1].MyLiquid.CurrentTopColor)
            {
                Color oneColor = SwapingTubes[0].MyLiquid.CurrentTopColor;
                SwapingTubes[0].MyLiquid.SwapeColor(SwapingTubes[1].MyLiquid.CurrentTopColor);
                SwapingTubes[1].MyLiquid.SwapeColor(oneColor);
            }
            else
            {
                ToastMsgEvent.InvokeEvent(1);
            }

            SwapingTubes.Clear();
            SwipeColorsModeEvent.InvokeSOEvent();
            IsSwaping.Value = 0;
            UsingAnyFeature.Value = 0;
        }
    }
}
