using UnityEngine;
using DG.Tweening;
using Core.Events;
using UnityEngine.UI;
using Core.Variables;
using System.Collections;
using Core.GamePlay.Coloring;

namespace Core.GamePlay.WaterSort
{
    public class WaterSortTutorialThree : MonoBehaviour
    {
        [SerializeField] SOEvents SwipeColorsModeEvent, StartColoringEvent;
        [SerializeField] SOInterger CanPlay, LevelCompleteStateIndex;
        [SerializeField] SOIntegerEvents ChangeStateEvent;
        [SerializeField] Button ExtraTubeBtn;
        [SerializeField] TubeHandler FirstTube, SecondTube, ThirdTube, ForthTube, ExtraTube;
        [SerializeField] GameObject HandObj, InfoText, InfoTextObj;
        [SerializeField] Color[] CurrentColors;
        [SerializeField] BowlColorHandler BowlObj;

        Vector3 _bowlScale = new Vector3(1.5f, 0.1f, 1.5f);
        Vector2 _firstBowlPos = new Vector2(1, -1), _secondBowlPos = new Vector2(-1, -1), _thirdBowlPos = new Vector2(-2, -1), _forthBowlPos = new Vector2(0, -1),
                _extraBowlPos = new Vector2(2, -1);
        int _colorIndex = 0;

        private void OnEnable()
        {
            StartColoringEvent.EventHandler += ColoringPreparation;
            ChangeStateEvent.EventHandler += HideInfoText;
            ExtraTubeBtn.onClick.AddListener(AddTubbe);
        }

        private void OnDisable()
        {
            StartColoringEvent.EventHandler -= ColoringPreparation;
            ChangeStateEvent.EventHandler -= HideInfoText;
            ExtraTubeBtn.onClick.RemoveListener(AddTubbe);
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
            SwipeColorsModeEvent.InvokeSOEvent();
            CanPlay.Value = 1;
            HandObj.SetActive(true);
        }

        void AddTubbe()
        {
            if (CanPlay.Value == 1)
            {
                ExtraTube.gameObject.SetActive(true);
                HandObj.SetActive(false);
                InfoText.SetActive(true);
                Destroy(ExtraTubeBtn.gameObject);
            }
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
            if (FirstTube.WaterColors.Count > 0)
            {
                FirstTube.TubeCap.gameObject.SetActive(false);
                FirstTube.transform.DOScale(_bowlScale, tweenTime);
                FirstTube.transform.DOLocalMove(_firstBowlPos, tweenTime).OnComplete(() =>
                    {
                        BowlColorHandler colorBowl = Instantiate(BowlObj, transform.parent);
                        colorBowl.transform.localPosition = FirstTube.transform.localPosition;
                        colorBowl.SetColor(FirstTube.CurrentColor);
                        Destroy(FirstTube.gameObject);
                    });
            }
            else
            {
                FirstTube.transform.DOLocalMove(_firstBowlPos, tweenTime).OnComplete(() =>
                    {
                        Destroy(FirstTube.gameObject);
                    });
            }

            if (SecondTube.WaterColors.Count > 0)
            {
                SecondTube.TubeCap.gameObject.SetActive(false);
                SecondTube.transform.DOScale(_bowlScale, tweenTime);
                SecondTube.transform.DOLocalMove(_secondBowlPos, tweenTime).OnComplete(() =>
                    {
                        BowlColorHandler colorBowl = Instantiate(BowlObj, transform.parent);
                        colorBowl.transform.localPosition = SecondTube.transform.localPosition;
                        colorBowl.SetColor(SecondTube.CurrentColor);
                        Destroy(SecondTube.gameObject);
                    });
            }
            else
            {
                SecondTube.transform.DOLocalMove(_secondBowlPos, tweenTime).OnComplete(() =>
                    {
                        Destroy(SecondTube.gameObject);
                    });
            }

            if (ThirdTube.WaterColors.Count > 0)
            {
                ThirdTube.TubeCap.gameObject.SetActive(false);
                ThirdTube.transform.DOScale(_bowlScale, tweenTime);
                ThirdTube.transform.DOLocalMove(_thirdBowlPos, tweenTime).OnComplete(() =>
                    {
                        BowlColorHandler colorBowl = Instantiate(BowlObj, transform.parent);
                        colorBowl.transform.localPosition = ThirdTube.transform.localPosition;
                        colorBowl.SetColor(ThirdTube.CurrentColor);
                        Destroy(ThirdTube.gameObject);
                    });
            }
            else
            {
                ThirdTube.transform.DOLocalMove(_thirdBowlPos, tweenTime).OnComplete(() =>
                    {
                        Destroy(ThirdTube.gameObject);
                    });
            }

            if (ForthTube.WaterColors.Count > 0)
            {
                ForthTube.TubeCap.gameObject.SetActive(false);
                ForthTube.transform.DOScale(_bowlScale, tweenTime);
                ForthTube.transform.DOLocalMove(_forthBowlPos, tweenTime).OnComplete(() =>
                    {
                        BowlColorHandler colorBowl = Instantiate(BowlObj, transform.parent);
                        colorBowl.transform.localPosition = ForthTube.transform.localPosition;
                        colorBowl.SetColor(ForthTube.CurrentColor);
                        Destroy(ForthTube.gameObject);
                    });
            }
            else
            {
                ForthTube.transform.DOLocalMove(_forthBowlPos, tweenTime).OnComplete(() =>
                    {
                        Destroy(ForthTube.gameObject);
                    });
            }

            if (ExtraTube.WaterColors.Count > 0)
            {
                ExtraTube.TubeCap.gameObject.SetActive(false);
                ExtraTube.transform.DOScale(_bowlScale, tweenTime);
                ExtraTube.transform.DOLocalMove(_extraBowlPos, tweenTime).OnComplete(() =>
                    {
                        BowlColorHandler colorBowl = Instantiate(BowlObj, transform.parent);
                        colorBowl.transform.localPosition = ExtraTube.transform.localPosition;
                        colorBowl.SetColor(ExtraTube.CurrentColor);
                        Destroy(ExtraTube.gameObject);
                    });
            }
            else
            {
                ExtraTube.transform.DOLocalMove(_extraBowlPos, tweenTime).OnComplete(() =>
                    {
                        Destroy(ExtraTube.gameObject);
                    });
            }
        }
    }
}
