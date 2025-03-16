using UnityEngine;
using DG.Tweening;
using Core.Events;
using Core.Variables;
using System.Collections;
using Core.GamePlay.Coloring;

namespace Core.GamePlay.WaterSort
{
    public class WaterSortTutorialTwo : MonoBehaviour
    {
        [SerializeField] SOInterger CanPlay, LevelCompleteStateIndex;
        [SerializeField] SOEvents StartColoringEvent;
        [SerializeField] SOIntegerEvents SwitchProtectorEvent;
        [SerializeField] CapsuleCollider MyCollider, SecondCollider, ThirdCollider;
        [SerializeField] TubeHandler MyLiquid, OtherLiquid, ThirdLiquid;
        [SerializeField] GameObject HandObj, InfoTextObj, UndoBtn;
        [SerializeField] Color[] CurrentColors;
        [SerializeField] bool IsUndoBtn, ExtraTube;
        [SerializeField] BowlColorHandler BowlObj;

        int _colorIndex = 0;
        bool _isFirstClick = true; 
        Vector3 _bowlScale = new Vector3(1.5f, 0.1f, 1.5f), _firstBowlPos = new Vector3(0, -1.5f, -2), _otherBowlPos = new Vector3(-1, -1.5f, -2), 
                _thirdBowlPos = new Vector3(1, -1.5f, -2);

        private void OnEnable()
        {

            if (!IsUndoBtn && !ExtraTube)
            {
                StartColoringEvent.EventHandler += ColoringPreparation;
            }
        }

        private void OnDisable()
        {
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
            SwitchProtectorEvent.InvokeSOEvent(0);
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

        public void TutorialUndoButton()
        {
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
            UndoBtn.SetActive(true);
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
                    Destroy(ThirdLiquid.gameObject);
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
                    Destroy(OtherLiquid.gameObject);
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
                    Destroy(MyLiquid.gameObject);
                }
            }
        }
    }
}
