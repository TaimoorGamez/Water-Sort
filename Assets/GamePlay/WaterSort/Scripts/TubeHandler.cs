using UnityEngine;
using Core.Events;
using Core.Variables;
using System.Collections;
using System.Collections.Generic;

namespace Core.GamePlay.WaterSort
{
    public class TubeHandler : MonoBehaviour
    {
        public WaterColor MyLiquid;
        public bool IsBussy = false;

        [SerializeField] Transform OutPosOne, OutPosTwo;
        [SerializeField] Vector3 LeftRotation, RightRotation;
        [SerializeField] CapsuleCollider TubeCollider;
        [SerializeField] CapHandler TubeCap;
        [SerializeField] SOInterger DoingUndo, IsHiddenLevel, CompletedTubes, IsSwaping, CanPlay, UsingAnyFeature;
        [SerializeField] UndoManager UndoManager;
        [SerializeField] ColorSwiper SwapingManager;
        [SerializeField] SOWaterTube OpenTube;
        [SerializeField] SOEvents CheckCompleteEvent;

        List<Coroutine> _drinkingRotine = new List<Coroutine>();
        TubeHandler _senderTube;
        Vector3 _orignalPos;
        int _totalLiquidLayers = 4, _colorsToUndo = 1;
        bool _isDrinkingWater = false, _tubeCompleted = false, _alreadyAddedToCompleted = false;
        Coroutine _celebrationRotine;

        private void Start()
        {
            _orignalPos = transform.position;
        }

        private void OnMouseDown()
        {
            if (DoingUndo.Value == 0 && IsSwaping.Value == 0 && CanPlay.Value == 1)
            {
                if (OpenTube.Tube == null && !IsBussy && !_isDrinkingWater && !MyLiquid.IsEmpty())
                {
                    TubeState(true);
                    OpenTube.Tube = this;
                }
                else
                {
                    if (OpenTube.Tube == this && !IsBussy)
                    {
                        OpenTube.Tube = null;
                        MoveBackIn();
                    }
                    else
                    {
                        if (!IsBussy && OpenTube.Tube != null)
                        {
                            _senderTube = OpenTube.Tube;
                            if (MyLiquid.WaterColors.Count < 1)
                            {
                                DrinkWater();
                            }
                            else if (MyLiquid.WaterColors.Count < _totalLiquidLayers && _senderTube.MyLiquid.CurrentTopColor == MyLiquid.CurrentTopColor)
                            {
                                DrinkWater();
                            }
                            else
                            {
                                OpenTube.Tube.MoveBackIn();
                                OpenTube.Tube = null;
                            }
                        }
                    }
                }
            }
            else if (IsSwaping.Value == 1 && CanPlay.Value == 1)
            {
                SwapingManager.AddTubeForSwaping(this);
            }
        }

        public void MoveBackIn()
        {
            IsBussy = false;
            TubeState(false);
        }

        public void DrinkWater()
        {
            IsBussy = true;
            _isDrinkingWater = true;
            _senderTube.IsBussy = true;
            OpenTube.Tube = null;
            _drinkingRotine.Add(StartCoroutine(ChangingWater(_senderTube)));
        }

        public void UndoWater(TubeHandler senderTube, int liquidLayers)
        {
            //Debug.Log("Here97");
            //Debug.Log(senderTube);
            IsBussy = true;
            _isDrinkingWater = true;
            senderTube.IsBussy = true;
            _colorsToUndo = liquidLayers;
            OpenTube.Tube = null;
            _drinkingRotine.Add(StartCoroutine(ChangingWater(senderTube)));
        }

        private IEnumerator ChangingWater(TubeHandler senderTube)
        {
            int randomDirection = Random.Range(0, 2);
            Vector3 tubePosition = OutPosOne.position;
            Vector3 rotationDirection = LeftRotation;
            if (randomDirection == 0)
            {
                tubePosition = OutPosTwo.position;
                rotationDirection = RightRotation;
            }
            int colorsToAdd = 1;
            //Debug.Log("Here109");
            LeanTween.move(senderTube.gameObject, tubePosition, 0.1f);
            LeanTween.rotate(senderTube.gameObject, rotationDirection, 0.1f).setOnComplete(() =>
            {
                if (MyLiquid.WaterColors.Count < _totalLiquidLayers - 1 && DoingUndo.Value == 0)
                {
                    int sameColors = 1;
                    for (int i = senderTube.MyLiquid.WaterColors.Count - 1; i > 0; i--)
                    {
                        if (senderTube.MyLiquid.WaterColors[i] == senderTube.MyLiquid.WaterColors[i - 1])
                        {
                            if (IsHiddenLevel.Value == 1)
                            {
                                if (!senderTube.MyLiquid.GetHidenColor(i - 1))
                                    sameColors++;
                            }
                            else
                            {
                                sameColors++;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (sameColors > 1)
                    {
                        int remaingLayers = _totalLiquidLayers - MyLiquid.WaterColors.Count;
                        if (sameColors <= remaingLayers)
                        {
                            colorsToAdd = sameColors;
                        }
                        else
                        {
                            colorsToAdd = remaingLayers;
                        }
                    }
                }

                if (DoingUndo.Value == 1)
                {
                    colorsToAdd = _colorsToUndo;
                }
                //Debug.Log("Here162");
                //Debug.Log(colorsToAdd);
                MyLiquid.AddColor(senderTube.MyLiquid.CurrentTopColor, colorsToAdd);
                senderTube.MyLiquid.RemoveColor(colorsToAdd);
                if (DoingUndo.Value == 0)
                {
                    UndoManager.AddUndo(senderTube, this, colorsToAdd);
                }
                IsBussy = false;
            });
            yield return new WaitForSeconds(1f);
            senderTube.TubeState(false);
            if (IsHiddenLevel.Value == 1)
            {
                senderTube.CheckHiddenColor();
            }
            WaterAdded();
            yield return new WaitForSeconds(0.5f);
            senderTube.IsBussy = false;
            if (DoingUndo.Value == 1)
            {
                yield return new WaitForSeconds(0.5f);
                DoingUndo.Value = 0;
                UsingAnyFeature.Value = 0;
            }
        }

        public void WaterAdded()
        {
            _isDrinkingWater = false;
            if (MyLiquid.WaterColors.Count == _totalLiquidLayers)
            {
                _tubeCompleted = true;
                for (int i = _totalLiquidLayers - 1; i > 0; i--)
                {
                    if (MyLiquid.WaterColors[i] != MyLiquid.WaterColors[i - 1])
                    {
                        _tubeCompleted = false;
                        break;
                    }
                }
                if (!_alreadyAddedToCompleted && _tubeCompleted)
                {
                    _alreadyAddedToCompleted = true;
                    TubeCollider.enabled = false;
                    if (IsHiddenLevel.Value == 1)
                    {
                        MyLiquid.RevelFullTube();
                    }
                    _celebrationRotine = StartCoroutine(CelebrationOnComplete());
                }
            }
        }

        IEnumerator CelebrationOnComplete()
        {
            yield return new WaitForSeconds(0.5f);
            TubeCap.PlayCelebration(MyLiquid.CurrentTopColor);
            yield return new WaitForSeconds(1);
            CompletedTubes.Value ++;
            CheckCompleteEvent.InvokeSOEvent();
            if (_celebrationRotine != null)
            {
                StopCoroutine(_celebrationRotine);
            }
            //Debug.Log("Here223");
        }

        public void CheckHiddenColor()
        {
            MyLiquid.RevelColour();
        }

        public void RemoveFromCompleted()
        {
            if (_tubeCompleted && _alreadyAddedToCompleted)
            {
                //Debug.Log("Here239");
                _tubeCompleted = false;
                CompletedTubes.Value--;
                _alreadyAddedToCompleted = false;
                TubeCollider.enabled = true;
                TubeCap.HideCap();
            }
        }

        void TubeState(bool state)
        {
            if (state)
            {
                LeanTween.moveLocalY(gameObject, _orignalPos.y + 0.5f, 0.1f);
            }
            else
            {
                LeanTween.moveLocal(gameObject, _orignalPos, 0.05f);
                LeanTween.rotateLocal(gameObject, Vector3.zero, 0.05f).setOnComplete(()=> transform.position = _orignalPos );
            }
        }

        private void OnDisable()
        {
            foreach (Coroutine rotine in _drinkingRotine)
            {
                if (rotine != null)
                {
                    StopCoroutine(rotine);
                }
            }
            if (_celebrationRotine != null)
            {
                StopCoroutine(_celebrationRotine);
            }
        }
    }
}
