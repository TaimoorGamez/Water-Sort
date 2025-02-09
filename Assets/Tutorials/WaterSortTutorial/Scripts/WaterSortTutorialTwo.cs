using UnityEngine;
using DG.Tweening;
using Core.Events;
using Core.Variables;
using UnityEngine.UI;
using System.Collections;
using Core.GamePlay.Coloring;

namespace Core.GamePlay.WaterSort
{
    public class WaterSortTutorialTwo : MonoBehaviour
    {
        [SerializeField] SOInterger CanPlay, LevelCompleteStateIndex;
        [SerializeField] SOEvents UndoEvent, SwipeColorsModeEvent, StartColoringEvent;
        [SerializeField] SOIntegerEvents ChangeStateEvent;
        [SerializeField] Button UndoBtn;
        [SerializeField] CapsuleCollider MyCollider, SecondCollider, ThirdCollider;
        [SerializeField] TubeHandler MyLiquid, OtherLiquid, ThirdLiquid;
        [SerializeField] GameObject HandObj, InfoTextObj;
        [SerializeField] Color[] CurrentColors;
        [SerializeField] bool IsUndoBtn, ExtraTube;
        [SerializeField] BowlColorHandler BowlObj;

        int _colorIndex = 0;
        bool _isFirstClick = true; 
        Vector3 _bowlScale = new Vector3(1.5f, 0.1f, 1.5f);
        Vector2 _firstBowlPos = new Vector2(0, -1), _otherBowlPos = new Vector2(-1, -1), _thirdBowlPos = new Vector2(1, -1);

        private void OnEnable()
        {
            if (IsUndoBtn)
            {
                ChangeStateEvent.EventHandler += HideInfoText;
                UndoBtn.onClick.AddListener(TutorialUndoButton);
            }

            if (!IsUndoBtn && !ExtraTube)
            {
                StartColoringEvent.EventHandler += ColoringPreparation;
            }
        }

        private void OnDisable()
        {
            if (IsUndoBtn)
            {
                ChangeStateEvent.EventHandler -= HideInfoText;
                UndoBtn.onClick.RemoveListener(TutorialUndoButton);
            }

            if (!IsUndoBtn && !ExtraTube)
            {
                StartColoringEvent.EventHandler -= ColoringPreparation;
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

        void ColoringPreparation()
        {
            float tweenTime = 1;
            InfoTextObj.SetActive(false);
            if (!IsUndoBtn && !ExtraTube)
            {
                if (ThirdLiquid.WaterColors.Count > 0)
                {
                    ThirdLiquid.TubeCap.gameObject.SetActive(false);
                    ThirdLiquid.transform.DOScale(_bowlScale, tweenTime);
                    ThirdLiquid.transform.DOLocalMove(_thirdBowlPos, tweenTime).OnComplete(() =>
                    {
                        BowlColorHandler colorBowl = Instantiate(BowlObj, transform.parent);
                        colorBowl.transform.localPosition = ThirdLiquid.transform.localPosition;
                        colorBowl.SetColor(ThirdLiquid.CurrentColor);
                        Destroy(ThirdLiquid.gameObject);
                    });
                }
                else
                {
                    ThirdLiquid.transform.DOLocalMove(_thirdBowlPos, tweenTime).OnComplete(() =>
                    {
                        Destroy(ThirdLiquid.gameObject);
                    });
                }

                if (OtherLiquid.WaterColors.Count > 0)
                {
                    OtherLiquid.TubeCap.gameObject.SetActive(false);
                    OtherLiquid.transform.DOScale(_bowlScale, tweenTime);
                    OtherLiquid.transform.DOLocalMove(_otherBowlPos, tweenTime).OnComplete(() =>
                    {
                        BowlColorHandler colorBowl = Instantiate(BowlObj, transform.parent);
                        colorBowl.transform.localPosition = OtherLiquid.transform.localPosition;
                        colorBowl.SetColor(OtherLiquid.CurrentColor);
                        Destroy(OtherLiquid.gameObject);
                    });
                }
                else
                {
                    OtherLiquid.transform.DOLocalMove(_otherBowlPos, tweenTime).OnComplete(() =>
                    {
                        Destroy(OtherLiquid.gameObject);
                    });
                }

                if (MyLiquid.WaterColors.Count > 0)
                {
                    MyLiquid.TubeCap.gameObject.SetActive(false);
                    MyLiquid.transform.DOScale(_bowlScale, tweenTime);
                    MyLiquid.transform.DOLocalMove(_firstBowlPos, tweenTime).OnComplete(() =>
                    {
                        BowlColorHandler colorBowl = Instantiate(BowlObj, transform.parent);
                        colorBowl.transform.localPosition = MyLiquid.transform.localPosition;
                        colorBowl.SetColor(MyLiquid.CurrentColor);
                        Destroy(MyLiquid.gameObject);
                    });
                }
                else
                {
                    MyLiquid.transform.DOLocalMove(_firstBowlPos, tweenTime).OnComplete(() =>
                    {
                        Destroy(MyLiquid.gameObject);
                    });
                }
            }
        }
    }
}
