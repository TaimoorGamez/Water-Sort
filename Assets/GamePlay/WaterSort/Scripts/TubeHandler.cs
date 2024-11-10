using UnityEngine;
using Core.Events;
using Core.Variables;
using System.Collections;
using System.Collections.Generic;

namespace Core.GamePlay.WaterSort
{
    public class TubeHandler : MonoBehaviour
    {
        public bool IsBussy = false;
        public List<Color> WaterColors = new List<Color>();
        public Color CurrentColor;

        [SerializeField] SOInterger DoingUndo, IsHiddenLevel, CompletedTubes, IsSwaping, CanPlay, UsingAnyFeature;
        [SerializeField] UndoManager UndoManager;
        [SerializeField] ColorSwiper SwapingManager;
        [SerializeField] SOWaterTube OpenTube;
        [SerializeField] SOEvents CheckCompleteEvent;
        [SerializeField] ParticleSystem WaveParticle, DropsParticle;
        [SerializeField] LineRenderer WaterLine;
        [SerializeField] Vector3 LeftPosition, LeftRotation, RightPosition, RightRotation;
        [SerializeField] CapsuleCollider TubeCollider;
        [SerializeField] CapHandler TubeCap;
        [SerializeField] Liquid[] MyLiquid;
        [SerializeField] GameObject[] HidenMarks;
        [SerializeField] ParticleSystemRenderer WaveRenderer, DropsRenderer;


        List<Coroutine> _drinkingRotine = new List<Coroutine>();
        TubeHandler _senderTube;
        Vector3 _orignalPos;
        int _totalLiquidLayers = 4, _colorsToUndo = 1;
        bool _isDrinkingWater = false, _tubeCompleted = false, _alreadyAddedToCompleted = false;
        Coroutine _celebrationRotine, _addingRotine, _removingRotine;
        MaterialPropertyBlock _propBlock;

        private void Start()
        {
            _orignalPos = transform.position;
            _propBlock = new MaterialPropertyBlock();
            //WaterLine.SetPosition(0, TubePos.TransformPoint(WaterStartPosition.localPosition));
            //WaterLine.SetPosition(1, TubePos.TransformPoint(WaterLineEndPos.localPosition));
        }

        public void SetHidenColour(Color currentColor)
        {
            if (WaterColors.Count < _totalLiquidLayers - 1)
            {
                MyLiquid[WaterColors.Count].SetHidenColour();
                HidenMarks[WaterColors.Count].gameObject.SetActive(true);
            }
            SetColor(currentColor);
        }

        public void SetColor(Color currentColor)
        {
            MyLiquid[WaterColors.Count].SetColor(currentColor);
            WaterColors.Add(currentColor);
            CurrentColor = currentColor;
            //WaterLineEndPos.localPosition += new Vector3(0, _waterPosIncrement, 0);
        }

        private void OnMouseDown()
        {
            if (DoingUndo.Value == 0 && IsSwaping.Value == 0 && CanPlay.Value == 1)
            {
                if (OpenTube.Tube == null && !IsBussy && !_isDrinkingWater && WaterColors.Count > 0)
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
                            if (WaterColors.Count < 1)
                            {
                                DrinkWater();
                            }
                            else if (WaterColors.Count < _totalLiquidLayers && _senderTube.CurrentColor == CurrentColor)
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
            Vector3 tubePosition = LeftPosition;
            Vector3 rotationDirection = LeftRotation;
            if (randomDirection == 0)
            {
                tubePosition = RightPosition;
                rotationDirection = RightRotation;
            }
            int colorsToAdd = 1;
            //Debug.Log("Here109");
            LeanTween.move(senderTube.gameObject, tubePosition, 0.1f);
            LeanTween.rotate(senderTube.gameObject, rotationDirection, 0.1f).setOnComplete(() =>
            {
                if (WaterColors.Count < _totalLiquidLayers - 1 && DoingUndo.Value == 0)
                {
                    int sameColors = 1;
                    for (int i = senderTube.WaterColors.Count - 1; i > 0; i--)
                    {
                        if (senderTube.WaterColors[i] == senderTube.WaterColors[i - 1])
                        {
                            if (IsHiddenLevel.Value == 1)
                            {
                                if (!senderTube.GetHidenColor(i - 1))
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
                        int remaingLayers = _totalLiquidLayers - WaterColors.Count;
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
                AddColor(senderTube.CurrentColor, colorsToAdd);
                senderTube.RemoveColor(colorsToAdd);
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
            WaterLine.gameObject.SetActive(false);
            _isDrinkingWater = false;
            if (WaterColors.Count == _totalLiquidLayers)
            {
                _tubeCompleted = true;
                for (int i = _totalLiquidLayers - 1; i > 0; i--)
                {
                    if (WaterColors[i] != WaterColors[i - 1])
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
                        RevelFullTube();
                    }
                    _celebrationRotine = StartCoroutine(CelebrationOnComplete());
                }
            }
        }

        IEnumerator CelebrationOnComplete()
        {
            yield return new WaitForSeconds(0.5f);
            TubeCap.PlayCelebration(CurrentColor);
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
            RevelColour();
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
        public void RemoveColor(int layers)
        {
            //for (int c = 0; c < layers; c++)
            //{
            //    WaterColors.RemoveAt(WaterColors.Count - 1);
            //    if (WaterColors.Count > 0)
            //    {
            //        CurrentColor = WaterColors[WaterColors.Count - 1];
            //    }
            //    else
            //    {
            //        CurrentColor = Color.black;
            //    }
            //}
            //StartCoroutine(SmoothlyChangeTransparency(_eachLiquidLayerHeight * WaterColors.Count));
            ////WaterLineEndPos.localPosition -= new Vector3(0, _waterPosIncrement, 0);
            //if (layers < 2)
            //{
            //    TubeAnimation.Play("FixDrop " + WaterColors.Count.ToString());
            //}
            //else
            //{
            //    TubeAnimation.Play("Dropping " + WaterColors.Count.ToString());
            //}
        }
        public void SwapeColor(Color currentColor)
        {
            //WaterColors.RemoveAt(WaterColors.Count - 1);
            //CurrentColor = currentColor;
            ////WaterColors.Add(currentColor);
            //_propBlock.SetColor("_Color" + WaterColors.Count.ToString(), currentColor);
            //// Apply the material property block to the renderer
            //MySkin.SetPropertyBlock(_propBlock);
            //// Set transparency property in the material property block
            //_propBlock.SetFloat("_TransparencyRange", _eachLiquidLayerHeight * WaterColors.Count);

            // Apply the material property block to the renderer
            //MySkin.SetPropertyBlock(_propBlock);
            //WaterLineEndPos.localPosition += new Vector3(0, _waterPosIncrement, 0);
        }

        void TubeState(bool state)
        {
            if (state)
            {
                MyLiquid[WaterColors.Count-1].SetGlow(true);
                for(int c = 1; c < WaterColors.Count; c++)
                {
                    if (WaterColors[c] == WaterColors[c-1])
                    {
                        MyLiquid[c - 1].SetGlow(true);
                    }
                }
                LeanTween.moveLocalY(gameObject, _orignalPos.y + 0.5f, 0.1f);
            }
            else
            {
                MyLiquid[WaterColors.Count-1].SetGlow(false);
                for (int c = 1; c < WaterColors.Count; c++)
                {
                    if (WaterColors[c] == WaterColors[c - 1])
                    {
                        MyLiquid[c- 1].SetGlow(false);
                    }
                }
                LeanTween.moveLocal(gameObject, _orignalPos, 0.05f);
                LeanTween.rotateLocal(gameObject, Vector3.zero, 0.05f).setOnComplete(()=> transform.position = _orignalPos );
            }
        }

        void AddColor(Color currentColor, int layers)
        {
            _propBlock.SetColor("_BaseColor", currentColor);
            WaterLine.SetPropertyBlock(_propBlock);
            WaveRenderer.SetPropertyBlock(_propBlock);
            DropsRenderer.SetPropertyBlock(_propBlock);
            WaterLine.gameObject.SetActive(true); 
            CurrentColor = currentColor;
            _addingRotine = StartCoroutine(ColorAdddingRotine(currentColor,layers));
        }

        IEnumerator ColorAdddingRotine(Color currentColor, int layers)
        {
            for (int c = 1; c <= layers; c++)
            {
                MyLiquid[WaterColors.Count].SmoothlyAddColor(currentColor, 1/layers);
                WaterColors.Add(currentColor);
                yield return new WaitForSeconds(1/layers);
            }

            if (_addingRotine != null)
            {
                StopCoroutine( _addingRotine );
            }
        }

        void RevelColour()
        {
            //if (WaterColors.Count > 0)
            //{
            //    HidenMarks[WaterColors.Count - 1].SetActive(false);
            //}

            //if (WaterColors.Count > 1)
            //{
            //    LeanTween.moveLocalZ(QustionMark, QustionMarkPositions[WaterColors.Count - 2], QustionMarkMovemenTime);
            //}
            //else
            //{
            //    QustionMark.SetActive(false);
            //}
        }

        void RevelFullTube()
        {
            //QustionMark.SetActive(false);
            //foreach (GameObject obj in HidenMarks)
            //{
            //    obj.SetActive(false);
            //}
        }

        bool GetHidenColor(int i)
        {
            return HidenMarks[i].gameObject.activeInHierarchy;
        }

        bool IsEmpty()
        {
            return (WaterColors.Count > 0) ? false : true;
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
