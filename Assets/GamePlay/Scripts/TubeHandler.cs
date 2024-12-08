using UnityEngine;
using DG.Tweening;
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

        [SerializeField] SOIntegerEvents SoundEffectEvent;
        [SerializeField] SOInterger DoingUndo, IsHiddenLevel, CompletedTubes, IsSwaping, CanPlay, UsingAnyFeature;
        [SerializeField] UndoManager UndoManager;
        [SerializeField] ColorSwiper SwapingManager;
        [SerializeField] SOWaterTube OpenTube;
        [SerializeField] SOEvents CheckCompleteEvent;
        [SerializeField] ParticleSystem WaveParticle, DropsParticle;
        [SerializeField] LineRenderer WaterLine;
        [SerializeField] Transform AnchorPos1, AnchorPos2;
        [SerializeField] Vector3[] ParticlePos;
        [SerializeField] CapsuleCollider TubeCollider;
        [SerializeField] CapHandler TubeCap;
        [SerializeField] Liquid[] MyLiquid;
        [SerializeField] GameObject[] HidenMarks;
        [SerializeField] Renderer WaveRenderer, DropsRenderer;
        [SerializeField] Animation TubeAnimation;

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
            WaveParticle.transform.localPosition = ParticlePos[WaterColors.Count];
            DropsParticle.transform.localPosition = ParticlePos[WaterColors.Count];
            WaterColors.Add(currentColor);
            CurrentColor = currentColor;
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
            Transform anchorPos = AnchorPos1;
            if (randomDirection == 0)
            {
                anchorPos = AnchorPos2;
            }
            int colorsToAdd = 1;
            senderTube.transform.DOMove(anchorPos.position, 0.1f);
            senderTube.transform.DORotate(anchorPos.eulerAngles, 0.1f).OnComplete(() =>
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
            yield return new WaitForSeconds(2.5f);
            CompletedTubes.Value ++;
            //CheckCompleteEvent.InvokeSOEvent();
            if (_celebrationRotine != null)
            {
                StopCoroutine(_celebrationRotine);
            }
        }

        public void CheckHiddenColor()
        {
            RevelColour();
        }

        public void RemoveFromCompleted()
        {
            if (_tubeCompleted && _alreadyAddedToCompleted)
            {
                _tubeCompleted = false;
                CompletedTubes.Value--;
                _alreadyAddedToCompleted = false;
                TubeCollider.enabled = true;
                TubeCap.HideCap();
            }
        }
        public void RemoveColor(int layers)
        {
            _removingRotine = StartCoroutine(ColorRemovingCorotine(layers));
        }

        IEnumerator ColorRemovingCorotine(int layers)
        {
            WaveParticle.Stop();
            TubeAnimation.Play("Liquid "+WaterColors.Count+""+layers);
            for (int c = 1; c <= layers; c++)
            {
                yield return new WaitForSeconds((float)1 / layers);
                MyLiquid[WaterColors.Count-1].HideColor();
                WaterColors.RemoveAt(WaterColors.Count - 1);
            }

            if (WaterColors.Count > 0)
            { DropsParticle.transform.localPosition = ParticlePos[WaterColors.Count - 1]; }

            if (_removingRotine != null)
            {
                StopCoroutine(_removingRotine);
            }
            yield return new WaitForSeconds(0.1f);
            for (int c = 1; c <= layers; c++)
            {
                MyLiquid[WaterColors.Count - 1].transform.localScale = Vector3.one;
            }
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
                SoundEffectEvent.InvokeSOEvent(0);
                MyLiquid[WaterColors.Count-1].SetGlow(true); 
                for (int c = 1; c < WaterColors.Count; c++)
                {
                    if (WaterColors[c] == WaterColors[c-1])
                    {
                        MyLiquid[c - 1].SetGlow(true);
                    }
                }
                _propBlock.SetColor("_BaseColor", CurrentColor);
                WaveRenderer.SetPropertyBlock(_propBlock);
                WaveParticle.Play();
                transform.DOLocalMoveY(_orignalPos.y + 0.5f, 0.1f);
            }
            else
            {
                MyLiquid[WaterColors.Count-1].SetGlow(false);
                WaveParticle.Stop();
                for (int c = 1; c < WaterColors.Count; c++)
                {
                    if (WaterColors[c] == WaterColors[c - 1])
                    {
                        MyLiquid[c- 1].SetGlow(false);
                    }
                }
                transform.DOLocalMove(_orignalPos, 0.05f);
                transform.DOLocalRotate(Vector3.zero, 0.05f).OnComplete(() => transform.position = _orignalPos);
            }
        }

        void AddColor(Color currentColor, int layers)
        {
            SoundEffectEvent.InvokeSOEvent(1);
            _propBlock.SetColor("_BaseColor", currentColor);
            WaterLine.SetPropertyBlock(_propBlock);
            WaveRenderer.SetPropertyBlock(_propBlock);
            DropsRenderer.SetPropertyBlock(_propBlock);
            WaterLine.gameObject.SetActive(true);
            CurrentColor = currentColor;
            _addingRotine = StartCoroutine(ColorAdddingCorotine(currentColor,layers));
        }

        IEnumerator ColorAdddingCorotine(Color currentColor, int layers)
        {
            float elapsedTime = 0f, duration = (float)1 / layers, smoothTimer = 0.01f;
            Vector3 startPosition = DropsParticle.transform.localPosition;
            DropsParticle.Play();
            for (int c = 1; c <= layers; c++)
            {
                StartCoroutine(MyLiquid[WaterColors.Count].SmoothlyAddColor(currentColor, (float)1 / layers));
                while (elapsedTime < duration)
                {
                    // Interpolate between the start and destination positions
                    DropsParticle.transform.localPosition = Vector3.Lerp(startPosition, ParticlePos[WaterColors.Count], elapsedTime / duration);

                    // Increase elapsed time
                    elapsedTime += Time.deltaTime;

                    // Wait for the next frame
                    yield return new WaitForSeconds(smoothTimer);
                }
                WaterColors.Add(currentColor);
            }
            DropsParticle.Stop();
            DropsParticle.transform.localPosition = ParticlePos[WaterColors.Count-1];

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
            if (_removingRotine != null)
            {
                StopCoroutine(_removingRotine);
            }
        }
    }
}
