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
        public CapHandler TubeCap;

        [SerializeField] SOIntegerEvents SoundEffectEvent, TaostMsgEvent;
        [SerializeField] SOInterger DoingUndo, IsHiddenLevel, CompletedTubes, IsSwaping, CanPlay, UsingAnyFeature;
        [SerializeField] UndoManager UndoManager;
        [SerializeField] ColorSwiper SwapingManager;
        [SerializeField] SOWaterTube OpenTube;
        [SerializeField] SOEvents CheckCompleteEvent;
        [SerializeField] ParticleSystem WaveParticle, DropsParticle, CompleteParticle;
        [SerializeField] LineRenderer WaterLine;
        [SerializeField] Transform AnchorPos1, AnchorPos2;
        [SerializeField] Vector3[] ParticlePos;
        [SerializeField] CapsuleCollider TubeCollider;
        [SerializeField] Liquid[] MyLiquid;
        [SerializeField] GameObject[] HidenMarks;
        [SerializeField] Animation TubeAnimation;

        List<Coroutine> _drinkingRotine = new List<Coroutine>();
        TubeHandler _senderTube;
        Vector3 _orignalPos;
        int _totalLiquidLayers = 4, _colorsToUndo = 1;
        bool _isDrinkingWater = false, _tubeCompleted = false, _alreadyAddedToCompleted = false;
        Coroutine _celebrationRotine, _addingRotine, _removingRotine;
        MaterialPropertyBlock _propertyBlock;
        ParticleSystem.MainModule _pM;

        private void Start()
        {
            _orignalPos = transform.position;
            _propertyBlock = new MaterialPropertyBlock();
            _pM = DropsParticle.main;
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
                if (OpenTube.Tube == null && !IsBussy && !_isDrinkingWater)
                {
                    if (WaterColors.Count > 0)
                    {
                        TubeState(true);
                        OpenTube.Tube = this;
                    }
                    else
                    {
                        TaostMsgEvent.InvokeSOEvent(0);
                    }
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
                            else if (WaterColors.Count < _totalLiquidLayers)
                            {
                                if (_senderTube.CurrentColor == CurrentColor)
                                {
                                    DrinkWater();
                                }
                                else
                                {
                                    TaostMsgEvent.InvokeSOEvent(6);
                                    OpenTube.Tube.MoveBackIn();
                                    OpenTube.Tube = null;
                                }
                            }
                            else
                            {
                                TaostMsgEvent.InvokeSOEvent(7);
                                OpenTube.Tube.MoveBackIn();
                                OpenTube.Tube = null;
                            }
                        }
                    }
                }
            }
            else if (IsSwaping.Value == 1 && CanPlay.Value == 1 && WaterColors.Count > 0)
            {
                SoundEffectEvent.InvokeSOEvent(0);
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
            WaterLine.gameObject.SetActive(false);
            senderTube.TubeState(false);
            yield return new WaitForSeconds(0.25f);
            if (IsHiddenLevel.Value == 1)
            {
                senderTube.RevelColour();
            }
            WaterAdded();
            yield return new WaitForSeconds(0.1f);
            senderTube.IsBussy = false;
            if (DoingUndo.Value == 1)
            {
                yield return new WaitForSeconds(0.05f);
                DoingUndo.Value = 0;
                UsingAnyFeature.Value = 0;
            }
        }

        public void WaterAdded()
        {
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
            SoundEffectEvent.InvokeSOEvent(4);
            CompleteParticle.Play();
            TubeCap.PlayCelebration(CurrentColor);
            yield return new WaitForSeconds(2f);
            CompletedTubes.Value ++;
            CheckCompleteEvent.InvokeSOEvent();
            if (_celebrationRotine != null)
            {
                StopCoroutine(_celebrationRotine);
            }
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
            TubeAnimation.Play("Liquid " + WaterColors.Count + "" + layers);
            for (int c = 1; c <= layers; c++)
            {
                yield return new WaitForSeconds((float)1 / layers);
                MyLiquid[WaterColors.Count - 1].HideColor();
                WaterColors.RemoveAt(WaterColors.Count - 1);
            }
            TubeAnimation.Play("LiquidDefault");
            if (WaterColors.Count > 0)
            {
                DropsParticle.transform.localPosition = ParticlePos[WaterColors.Count - 1];
                CurrentColor = WaterColors[WaterColors.Count - 1];
            }
            yield return new WaitForSeconds(0.1f);
            if (_removingRotine != null)
            {
                StopCoroutine(_removingRotine);
            }
        }

        public void SwapeColor(Color currentColor)
        {
            WaterColors[WaterColors.Count - 1] = currentColor;
            CurrentColor = currentColor;
            MyLiquid[WaterColors.Count-1].ChangeColor(currentColor);
        }

        void TubeState(bool state)
        {
            if (state)
            {
                SoundEffectEvent.InvokeSOEvent(0);
                MyLiquid[WaterColors.Count-1].SetGlow(true);
                if (IsHiddenLevel.Value != 1)
                {
                    for (int c = 1; c < WaterColors.Count; c++)
                    {
                        if (WaterColors[c] == WaterColors[c - 1])
                        {
                            MyLiquid[c - 1].SetGlow(true);
                        }
                    }
                }
                _propertyBlock.SetColor("_BaseColor", CurrentColor);
                _pM.startColor = CurrentColor;
                WaveParticle.Play();
                transform.DOLocalMoveY(_orignalPos.y + 0.5f, 0.1f);
            }
            else
            {
                WaveParticle.Stop();
                for (int c = 0; c < MyLiquid.Length; c++)
                {
                    MyLiquid[c].SetGlow(false);
                }
                transform.DOLocalMove(_orignalPos, 0.05f);
                transform.DOLocalRotate(Vector3.zero, 0.05f).OnComplete(() => transform.position = _orignalPos);
            }
        }

        void AddColor(Color currentColor, int layers)
        {
            SoundEffectEvent.InvokeSOEvent(1);
            _propertyBlock.SetColor("_BaseColor", currentColor);
            WaterLine.SetPropertyBlock(_propertyBlock);
            _pM.startColor = currentColor;
            CurrentColor = currentColor;
            _addingRotine = StartCoroutine(ColorAdddingCorotine(currentColor,layers));
        }

        IEnumerator ColorAdddingCorotine(Color currentColor, int layers)
        {
            float duration = (float)1 / layers;
            Vector3 startPosition = DropsParticle.transform.localPosition;
            WaterLine.gameObject.SetActive(true);
            DropsParticle.Play();
            for (int c = 1; c <= layers; c++)
            {
                MyLiquid[WaterColors.Count].SmoothlyAddColor(currentColor, duration);
                DropsParticle.transform.DOLocalMove(ParticlePos[WaterColors.Count], duration);
                yield return duration;
                WaterColors.Add(currentColor);
            }
            DropsParticle.Stop();
            DropsParticle.transform.localPosition = ParticlePos[WaterColors.Count-1];

            if (_addingRotine != null)
            {
                StopCoroutine( _addingRotine );
            }
        }

        public void RevelColour()
        {
            if (WaterColors.Count > 0)
            {
                HidenMarks[WaterColors.Count - 1].SetActive(false);
                MyLiquid[WaterColors.Count - 1].RevelColour();
            }
        }

        void RevelFullTube()
        {
            for (int h = 0; h < HidenMarks.Length; h++ )
            {
                HidenMarks[h].SetActive(false);
                MyLiquid[h].RevelColour();
            }
        }

        bool GetHidenColor(int i)
        {
            return HidenMarks[i].gameObject.activeInHierarchy;
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
