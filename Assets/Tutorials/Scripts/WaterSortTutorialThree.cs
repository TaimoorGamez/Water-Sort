using UnityEngine;
using DG.Tweening;
using Core.Events;
using Core.Variables;
using System.Collections;
using Core.GamePlay.Coloring;

namespace Core.GamePlay.WaterSort
{
    public class WaterSortTutorialThree : MonoBehaviour
    {
        [SerializeField] SOEvents StartColoringEvent;
        [SerializeField] SOIntegerEvents SwitchProtectorEvent;
        [SerializeField] SOInterger CanPlay, LevelCompleteStateIndex;
        [SerializeField] TubeHandler FirstTube, SecondTube, ThirdTube, ForthTube, ExtraTube;
        [SerializeField] GameObject HandObj, InfoTextObj;
        [SerializeField] Color[] CurrentColors;
        [SerializeField] BowlColorHandler BowlObj;

        Vector3 _bowlScale = new Vector3(1.5f, 0.1f, 1.5f), _firstBowlPos = new Vector3(1, -2.3f, -2), _secondBowlPos = new Vector3(-1, -2.3f, -2), 
                _thirdBowlPos = new Vector3(-2, -2.3f, -2), _forthBowlPos = new Vector3(0, -2.3f, -2), _extraBowlPos = new Vector3(2, -2.3f, -2);
        int _colorIndex = 0;

        private void OnEnable()
        {
            StartColoringEvent.EventHandler += ColoringPreparation;
        }

        private void OnDisable()
        {
            StartColoringEvent.EventHandler -= ColoringPreparation;
        }

        private void Start()
        {
            StartCoroutine(SetColors());
        }

        IEnumerator SetColors()
        {
            yield return new WaitForSeconds(0.5f);
            for (int c = 0; c < 4; c++)
            {
                FirstTube.SetColor(CurrentColors[_colorIndex]);
                yield return new WaitForSeconds(0.1f);
                if (_colorIndex == 2)
                {
                    _colorIndex = 0;
                }
                else
                {
                    _colorIndex++;
                }
                SecondTube.SetColor(CurrentColors[_colorIndex]);
                yield return new WaitForSeconds(0.1f); 
                if (_colorIndex == 2)
                {
                    _colorIndex = 0;
                }
                else
                {
                    _colorIndex++;
                }
                ThirdTube.SetColor(CurrentColors[_colorIndex]);
                yield return new WaitForSeconds(0.1f);
            }
            SwitchProtectorEvent.InvokeSOEvent(0);
            CanPlay.Value = 1;
            HandObj.SetActive(true);
        }

        void ColoringPreparation()
        {
            float tweenTime = 1;
            InfoTextObj.SetActive(false);
            if (FirstTube.WaterColors.Count > 0)
            {
                FirstTube.TubeCap.gameObject.SetActive(false);
                FirstTube.transform.DOKill();
                Sequence firstSeq = DOTween.Sequence();
                firstSeq.Join(FirstTube.transform.DOScale(_bowlScale, tweenTime));
                firstSeq.Join(FirstTube.transform.DOLocalMove(_firstBowlPos, tweenTime));
                firstSeq.OnComplete(() =>
                {
                    BowlColorHandler colorBowl = Instantiate(BowlObj, transform.parent);
                    colorBowl.transform.localPosition = FirstTube.transform.localPosition;
                    colorBowl.SetColor(FirstTube.CurrentColor);
                    Destroy(FirstTube.gameObject, 0.15f);

                });
            }
            else
            {
                Destroy(FirstTube.gameObject);
            }

            if (SecondTube.WaterColors.Count > 0)
            {
                SecondTube.TubeCap.gameObject.SetActive(false);
                SecondTube.transform.DOKill();
                Sequence secondSeq = DOTween.Sequence();
                secondSeq.Join(SecondTube.transform.DOScale(_bowlScale, tweenTime));
                secondSeq.Join(SecondTube.transform.DOLocalMove(_secondBowlPos, tweenTime));
                secondSeq.OnComplete(() =>
                {
                    BowlColorHandler colorBowl = Instantiate(BowlObj, transform.parent);
                    colorBowl.transform.localPosition = SecondTube.transform.localPosition;
                    colorBowl.SetColor(SecondTube.CurrentColor);
                    Destroy(SecondTube.gameObject, 0.15f);
                });
            }
            else
            {
                Destroy(SecondTube.gameObject);
            }

            if (ThirdTube.WaterColors.Count > 0)
            {
                ThirdTube.TubeCap.gameObject.SetActive(false);
                ThirdTube.transform.DOKill();
                Sequence thirdSeq = DOTween.Sequence();
                thirdSeq.Join(ThirdTube.transform.DOScale(_bowlScale, tweenTime));
                thirdSeq.Join(ThirdTube.transform.DOLocalMove(_thirdBowlPos, tweenTime));
                thirdSeq.OnComplete(() =>
                {
                    BowlColorHandler colorBowl = Instantiate(BowlObj, transform.parent);
                    colorBowl.transform.localPosition = ThirdTube.transform.localPosition;
                    colorBowl.SetColor(ThirdTube.CurrentColor);
                    Destroy(ThirdTube.gameObject, 0.15f);
                });
            }
            else
            {
                Destroy(ThirdTube.gameObject);
            }

            if (ForthTube.WaterColors.Count > 0)
            {
                ForthTube.TubeCap.gameObject.SetActive(false);
                ForthTube.transform.DOKill();
                Sequence forthSeq = DOTween.Sequence();
                forthSeq.Join(ForthTube.transform.DOScale(_bowlScale, tweenTime));
                forthSeq.Join(ForthTube.transform.DOLocalMove(_forthBowlPos, tweenTime));
                forthSeq.OnComplete(() =>
                {
                    BowlColorHandler colorBowl = Instantiate(BowlObj, transform.parent);
                    colorBowl.transform.localPosition = ForthTube.transform.localPosition;
                    colorBowl.SetColor(ForthTube.CurrentColor);
                    Destroy(ForthTube.gameObject, 0.15f);
                });
            }
            else
            {
                Destroy(ForthTube.gameObject);
            }

            if (ExtraTube.WaterColors.Count > 0)
            {
                ExtraTube.TubeCap.gameObject.SetActive(false);
                ExtraTube.transform.DOKill();
                Sequence extraSeq = DOTween.Sequence();
                extraSeq.Join(ExtraTube.transform.DOScale(_bowlScale, tweenTime));
                extraSeq.Join(ExtraTube.transform.DOLocalMove(_extraBowlPos, tweenTime));
                extraSeq.OnComplete(() =>
                {
                    BowlColorHandler colorBowl = Instantiate(BowlObj, transform.parent);
                    colorBowl.transform.localPosition = ExtraTube.transform.localPosition;
                    colorBowl.SetColor(ExtraTube.CurrentColor);
                    Destroy(ExtraTube.gameObject, 0.15f);
                });
            }
            else
            {
                Destroy(ExtraTube.gameObject);
            }
        }
    }
}
