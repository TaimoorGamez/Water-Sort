using UnityEngine;
using DG.Tweening;
using Core.Events;
using UnityEngine.UI;
using Core.Variables;
using System.Collections;

namespace Core.GamePlay.WaterSort
{
    public class WaterSortTutorialTwo : MonoBehaviour
    {
        [SerializeField] SOInterger CanPlay, LevelCompleteStateIndex;
        [SerializeField] SOEvents UndoEvent, SwipeColorsModeEvent;
        [SerializeField] SOIntegerEvents ChangeStateEvent;
        [SerializeField] Button UndoBtn;
        [SerializeField] CapsuleCollider MyCollider, SecondCollider, ThirdCollider;
        [SerializeField] Liquid MyLiquid, OtherLiquid;
        [SerializeField] GameObject HandObj, InfoTextObj;
        [SerializeField] Color[] CurrentColors;
        [SerializeField] bool IsUndoBtn, ExtraTube;

        int _colorIndex = 0;
        bool _isFirstClick = true;

        private void OnEnable()
        {
            if (IsUndoBtn)
            {
                ChangeStateEvent.EventHandler += HideInfoText;
                UndoBtn.onClick.AddListener(TutorialUndoButton);
            }
        }

        private void OnDisable()
        {
            if (IsUndoBtn)
            {
                ChangeStateEvent.EventHandler -= HideInfoText;
                UndoBtn.onClick.RemoveListener(TutorialUndoButton);
            }
        }

        private void Start()
        {
            if (!IsUndoBtn && !ExtraTube)
            { StartCoroutine(SetColors()); }
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
            SwipeColorsModeEvent.InvokeSOEvent();
            MyCollider.enabled = true;
            CanPlay.Value = 1;
            HandObj.SetActive(true);
        }

        void OnMouseDown()
        {
            if (CanPlay.Value == 1)
            {
                if (ExtraTube && _isFirstClick)
                {
                    _isFirstClick = false;
                    MyCollider.enabled = false;
                    Invoke(nameof(ShowUndoBtn), 1.5f);
                }
                else if (_isFirstClick)
                {
                    _isFirstClick = false;
                    MyCollider.enabled = false;
                    HandObj.transform.DOLocalMoveX(135f, 0.35f).SetEase(Ease.InOutBack);
                    ThirdCollider.enabled = true;
                }
            }
        }

        void TutorialUndoButton()
        {
            UndoEvent.InvokeSOEvent();
            if (_isFirstClick)
            {
                _isFirstClick = false;
                HandObj.SetActive(false);
                MyCollider.enabled = true;
                SecondCollider.enabled = true;
                ThirdCollider.enabled = true;
            }
        }

        void ShowUndoBtn()
        {
            InfoTextObj.SetActive(true);
            HandObj.transform.DOLocalMove(new Vector3(175f, -530f, 0), 0.35f).SetEase(Ease.InOutBack);
            UndoBtn.gameObject.SetActive(true);
        }

        void HideInfoText(int stateNum)
        {
            if (LevelCompleteStateIndex.Value == stateNum)
                InfoTextObj.SetActive(false);
        }
    }
}
