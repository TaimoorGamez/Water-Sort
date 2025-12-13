using Core.Events;
using DG.Tweening;
using UnityEngine;
using Core.Variables;
using System.Collections;
using Core.GamePlay.Coloring;

namespace Core.GamePlay.WaterSort
{
    public class WaterSortTutorialFour : MonoBehaviour
    {
        [SerializeField] SOInterger CanPlay, LevelCompleteStateIndex;
        [SerializeField] CapsuleCollider ColliderOne, ColliderTwo, ColliderThree, ColliderFour, ColliderFive;
        [SerializeField] TubeHandler FirstTube, SecondTube, ThirdTube, ForthTube, FifthTube, SixthTube;
        [SerializeField] GameObject InfoTextObj;
        [SerializeField] Color[] CurrentColors;
        [SerializeField] bool IsSwipButton, IsFirstTube, IsLastTube;
        [SerializeField] BowlColorHandler BowlObj;
        [SerializeField] Transform BowlParent, Circle, HandObj;

        Vector3 _bowlScale = new Vector3(1.5f, 0.1f, 1.5f);
        int _colorIndex = 0;
        Vector3 _bowlYPos = new Vector3(-2, 4.5f, 0);
        bool _isFirstClick = true;
        float _tutorialAnimationTime = 1;

        private void OnEnable()
        {
            if (IsSwipButton)
            {
                SimpleEventsHolder.StartColoringEvent += ColoringPreparation;
            }
        }

        private void OnDisable()
        {
            if (IsSwipButton)
            {
                SimpleEventsHolder.StartColoringEvent -= ColoringPreparation;
            }
        }

        private void Start()
        {
            if (IsSwipButton)
            { 
                StartCoroutine(SetColors());
            }
        }

        IEnumerator SetColors()
        {
            yield return new WaitForSeconds(0.5f);
            for (int c = 0; c < 4; c++)
            {
                FirstTube.SetColor(CurrentColors[_colorIndex]);
                yield return new WaitForSeconds(0.1f);
                if (_colorIndex == 3)
                {
                    _colorIndex = 0;
                }
                else
                {
                    _colorIndex++;
                }
                SecondTube.SetColor(CurrentColors[_colorIndex]);
                yield return new WaitForSeconds(0.1f); 
                if (_colorIndex == 3)
                {
                    _colorIndex = 0;
                }
                else
                {
                    _colorIndex++;
                }
                ThirdTube.SetColor(CurrentColors[_colorIndex]);
                yield return new WaitForSeconds(0.1f); 
                if (_colorIndex == 3)
                {
                    _colorIndex = 0;
                }
                else
                {
                    _colorIndex++;
                }
                ForthTube.SetColor(CurrentColors[_colorIndex]);
            }
            SingleIntegerEventsHolder.SwitchProtectorEvent?.Invoke(0);
            CanPlay.Value = 1;
            Circle.gameObject.SetActive(true);
            Circle.DOScale(1, _tutorialAnimationTime).SetEase(Ease.Linear).OnComplete(() => {
                HandObj.gameObject.SetActive(true);
                HandObj.DOLocalMoveY(-510, _tutorialAnimationTime).SetEase(Ease.InBack);
            });
        }

        private void OnMouseDown()
        {
            if (CanPlay.Value == 1)
            {
                if (IsFirstTube)
                {
                    if (_isFirstClick)
                    {
                        _isFirstClick = false;
                        HandObj.DOLocalMoveX(-45f, _tutorialAnimationTime).SetEase(Ease.InOutBack);
                        ColliderThree.enabled = false;
                        ColliderTwo.enabled = true;
                        enabled = false;
                    }
                }
                else if (IsLastTube)
                {
                    if (_isFirstClick)
                    {
                        _isFirstClick = false;
                        Destroy(HandObj.gameObject);
                        ColliderOne.enabled = true;
                        ColliderTwo.enabled = true;
                        ColliderThree.enabled = true;
                        ColliderFour.enabled = true;
                        ColliderFive.enabled = true;
                    }
                }
            }
        }

        public void SwapeColor()
        {
            if (_isFirstClick)
            {
                _isFirstClick = false;
                HandObj.DOLocalMove(new Vector3(225, 0, 0), _tutorialAnimationTime).SetEase(Ease.InOutBack);
                ColliderThree.enabled = true;
            }
            SimpleEventsHolder.SwapColorsEvent?.Invoke();
        }

        void ColoringPreparation()
        {
            float tweenTime = 1;
            InfoTextObj.SetActive(false);
            if (FirstTube.WaterColors.Count > 0)
            {
                FirstTube.TubeCap.gameObject.SetActive(false);
                FirstTube.transform.DOKill();
                FirstTube.transform.DOScale(_bowlScale, tweenTime);
                FirstTube.transform.DOLocalMove(_bowlYPos, tweenTime).OnKill(() =>
                {
                    BowlColorHandler colorBowl = Instantiate(BowlObj, BowlParent);
                    colorBowl.transform.localPosition = FirstTube.transform.localPosition;
                    colorBowl.SetColor(FirstTube.CurrentColor);
                    Destroy(FirstTube.gameObject, 0.15f);
                });
                _bowlYPos += new Vector3(1, 0, 0);
            }
            else
            {
                Destroy(FirstTube.gameObject);
            }

            if (SecondTube.WaterColors.Count > 0)
            {
                SecondTube.TubeCap.gameObject.SetActive(false);
                SecondTube.transform.DOKill();
                SecondTube.transform.DOScale(_bowlScale, tweenTime);
                SecondTube.transform.DOLocalMove(_bowlYPos, tweenTime).OnKill(() =>
                {
                    BowlColorHandler colorBowl = Instantiate(BowlObj, BowlParent);
                    colorBowl.transform.localPosition = SecondTube.transform.localPosition;
                    colorBowl.SetColor(SecondTube.CurrentColor);
                    Destroy(SecondTube.gameObject, 0.15f);
                });
                _bowlYPos += new Vector3(1, 0, 0);
            }
            else
            {
                Destroy(SecondTube.gameObject);
            }

            if (ThirdTube.WaterColors.Count > 0)
            {
                ThirdTube.TubeCap.gameObject.SetActive(false);
                ThirdTube.transform.DOKill();
                ThirdTube.transform.DOScale(_bowlScale, tweenTime);
                ThirdTube.transform.DOLocalMove(_bowlYPos, tweenTime).OnKill(() =>
                {
                    BowlColorHandler colorBowl = Instantiate(BowlObj, BowlParent);
                        colorBowl.transform.localPosition = ThirdTube.transform.localPosition;
                        colorBowl.SetColor(ThirdTube.CurrentColor);
                        Destroy(ThirdTube.gameObject, 0.15f);
                });
                _bowlYPos += new Vector3(1, 0, 0);
            }
            else
            {
                Destroy(ThirdTube.gameObject);
            }

            if (ForthTube.WaterColors.Count > 0)
            {
                ForthTube.TubeCap.gameObject.SetActive(false);
                ForthTube.transform.DOKill();
                ForthTube.transform.DOScale(_bowlScale, tweenTime);
                ForthTube.transform.DOLocalMove(_bowlYPos, tweenTime).OnKill(() =>
                {
                    BowlColorHandler colorBowl = Instantiate(BowlObj, BowlParent);
                    colorBowl.transform.localPosition = ForthTube.transform.localPosition;
                    colorBowl.SetColor(ForthTube.CurrentColor);
                    Destroy(ForthTube.gameObject, 0.15f);
                });
                _bowlYPos += new Vector3(1, 0, 0);
            }
            else
            {
                Destroy(ForthTube.gameObject);
            }

            if (FifthTube.WaterColors.Count > 0)
            {
                FifthTube.TubeCap.gameObject.SetActive(false);
                FifthTube.transform.DOKill();
                FifthTube.transform.DOScale(_bowlScale, tweenTime);
                FifthTube.transform.DOLocalMove(_bowlYPos, tweenTime).OnKill(() =>
                {
                    BowlColorHandler colorBowl = Instantiate(BowlObj, BowlParent);
                    colorBowl.transform.localPosition = FifthTube.transform.localPosition;
                    colorBowl.SetColor(FifthTube.CurrentColor);
                    Destroy(FifthTube.gameObject, 0.15f);

                });
                _bowlYPos += new Vector3(1, 0, 0);
            }
            else
            {
                Destroy(FifthTube.gameObject);
            }

            if (SixthTube.WaterColors.Count > 0)
            {
                SixthTube.TubeCap.gameObject.SetActive(false);
                SixthTube.transform.DOKill();
                SixthTube.transform.DOScale(_bowlScale, tweenTime);
                SixthTube.transform.DOLocalMove(_bowlYPos, tweenTime).OnKill(() =>
                {
                    BowlColorHandler colorBowl = Instantiate(BowlObj, BowlParent);
                    colorBowl.transform.localPosition = SixthTube.transform.localPosition;
                    colorBowl.SetColor(SixthTube.CurrentColor);
                    Destroy(SixthTube.gameObject, 0.15f);
                });
                _bowlYPos += new Vector3(1, 0, 0);
            }
            else
            {
                Destroy(SixthTube.gameObject);
            }
        }

    }
}
