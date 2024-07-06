using UnityEngine;
using Core.Events;
using Core.Variables;
using Core.Animations.LT;
using System.Collections;

namespace Core.GamePlay.WaterSort
{
    public class WaterSortTutorialTwo : MonoBehaviour
    {
        [SerializeField] SOLeanTween ScaleDownButton, ScaleUpButton;
        [SerializeField] SOInterger CanPlay;
        [SerializeField] CapsuleCollider MyCollider, SecondCollider, ThirdCollider;
        [SerializeField] WaterColor MyLiquid, OtherLiquid;
        [SerializeField] GameObject HandObj, UndoButton;
        [SerializeField] Color[] CurrentColors;
        [SerializeField] bool IsUndoBtn, ExtraTube;
        [SerializeField] SOEvents UndoEvent, SwipeColorsModeEvent;

        int _colorIndex = 0;
        bool _isFirstClick = true;

        private void Start()
        {
            if (!IsUndoBtn && !ExtraTube)
            { StartCoroutine(SetColors()); }
            else if (IsUndoBtn)
            {
                ScaleDownButton.TargetObj = this.gameObject;
                ScaleUpButton.TargetObj = this.gameObject;
            }
        }

        IEnumerator SetColors()
        {
            yield return new WaitForSeconds(0.5f);
            for (int c = 0; c < 4; c++)
            {
                MyLiquid.SetColor(CurrentColors[_colorIndex]);
                yield return new WaitForSeconds(0.1f);
                if (_colorIndex == 1)
                {
                    _colorIndex = 0;
                }
                else
                {
                    _colorIndex++;
                }
                OtherLiquid.SetColor(CurrentColors[_colorIndex]);
                yield return new WaitForSeconds(0.1f);
            }
            SwipeColorsModeEvent.InvokeEvent();
            MyCollider.enabled = true;
            CanPlay.Value = 1;
            HandObj.SetActive(true);
        }

        private void OnMouseDown()
        {
            if (CanPlay.Value == 1)
            {
                if (IsUndoBtn)
                {
                    ScaleUpButton.CancleAnimation();
                    ScaleDownButton.PlayAnimation();
                    UndoEvent.InvokeEvent();
                    if (_isFirstClick)
                    {
                        _isFirstClick = false;
                        HandObj.SetActive(false);
                        MyCollider.enabled = true;
                        SecondCollider.enabled = true;
                        ThirdCollider.enabled = true;
                    }
                }
                else if (ExtraTube && _isFirstClick)
                {
                    _isFirstClick = false;
                    MyCollider.enabled = false;
                    Invoke(nameof(ShowUndoBtn), 0.75f);
                }
                else if (_isFirstClick)
                {
                    _isFirstClick = false;
                    MyCollider.enabled = false;
                    LeanTween.moveX(HandObj, 1.6f, 0.35f).setEase(LeanTweenType.easeInOutBack);
                    ThirdCollider.enabled = true;
                }
            }
        }

        private void OnMouseUp()
        {
            if (IsUndoBtn)
            {
                ScaleDownButton.CancleAnimation();
                ScaleUpButton.PlayAnimation();
            }
        }

        void ShowUndoBtn()
        {
            LeanTween.move(HandObj, new Vector3(0.5f, -1.15f, 0), 0.35f).setEase(LeanTweenType.easeInOutBack);
            UndoButton.SetActive(true);
        }
    }
}
