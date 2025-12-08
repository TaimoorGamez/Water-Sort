using UnityEngine;
using DG.Tweening;
using Core.Events;
using Core.Variables;
using Core.DB.Variables;
using System.Collections;
using Core.GamePlay.Coloring;
using System.Collections.Generic;

namespace Core.GamePlay.WaterSort
{
    public class WaterSortLevelInit : MonoBehaviour
    {
        [SerializeField] SOIntegerEvents SwipeProtectorEvent, ToastMsgEvent;
        [SerializeField] DBInt LvlIndex, LvlNum;
        [SerializeField] SO2IntergerEvent TaskEvent;
        [SerializeField] SOInterger IsHiddenLevel, CanPlay, TotalMoves, BtnOnceClicked, MainMenuStateIndex, CurrrentLvl, TempLvlIndex;
        [SerializeField] SOEvents InitLevelEvent, ExtraTubeEvent, RestartLevelEvent, DestroyLevelEvent, StartColoringEvent, ChangeExtraTubeStatEvent,
                                  UpdateMovesEvent;
        [SerializeField] TubeHandler TubePrefab;
        [SerializeField] Vector3[] TubePositions, BowlPositions;
        [SerializeField] BowlColorHandler BowlObj;

        string _sortingLvlPath = "WaterSortLevels/lvl ";
        List<TubeHandler> _colorTubes = new List<TubeHandler>();
        List<TubeHandler> _totalTubes = new List<TubeHandler>();
        Coroutine lvlMakingRotine;
        int _totalTubesCount = 0, _maxColorsInTube = 4;
        SOColors _levelColors;
        Dictionary<int, List<Color32>> _currentLevelColors = new Dictionary<int, List<Color32>>();
        Vector3 _bowlScale = new Vector3(1.5f, 0.1f, 1.5f);

        private void OnEnable()
        {
            InitLevelEvent.EventHandler += InitNewLevel;
            ExtraTubeEvent.EventHandler += OnAddTubeClick;
            RestartLevelEvent.EventHandler += RestartLevel;
            DestroyLevelEvent.EventHandler += DestroyLevel;
            StartColoringEvent.EventHandler += ColoringPreparation;
        }

        private void OnDisable()
        {
            InitLevelEvent.EventHandler -= InitNewLevel;
            ExtraTubeEvent.EventHandler -= OnAddTubeClick;
            RestartLevelEvent.EventHandler -= RestartLevel;
            DestroyLevelEvent.EventHandler -= DestroyLevel;
            StartColoringEvent.EventHandler -= ColoringPreparation;
        }

        void InitNewLevel()
        {
            CanPlay.Value = 0;
            _colorTubes.Clear();
            _totalTubes.Clear();
            DestroyLevel();
            if (LvlNum.Value % 5 == 0)
            {
                IsHiddenLevel.Value = 1;
            }
            else if (IsHiddenLevel.Value == 1)
            {
                IsHiddenLevel.Value = 0;
            }

            if (LvlIndex.Value < 5)
            {
                Instantiate(Resources.Load(_sortingLvlPath + LvlIndex.Value), transform);
                CurrrentLvl.Value = LvlIndex.Value;
            }
            else
            {
                if (TempLvlIndex.Value == -1)
                {
                    _levelColors = Resources.Load<SOColors>(_sortingLvlPath + LvlIndex.Value);
                    CurrrentLvl.Value = _levelColors.Colors.Length;
                    _totalTubesCount = CurrrentLvl.Value + 2;
                    lvlMakingRotine = StartCoroutine(GenerateLvl());
                    TotalMoves.Value = CurrrentLvl.Value * _maxColorsInTube;
                }
                else
                {
                    InitCustomLvl();
                }
            }
            UpdateMovesEvent.InvokeSOEvent();
        }

        void InitCustomLvl()
        {
            _levelColors = Resources.Load<SOColors>(_sortingLvlPath + TempLvlIndex.Value);
            CurrrentLvl.Value = _levelColors.Colors.Length;
            _totalTubesCount = CurrrentLvl.Value + 2;
            lvlMakingRotine = StartCoroutine(GenerateLvl());
            TotalMoves.Value = CurrrentLvl.Value * _maxColorsInTube;
            UpdateMovesEvent.InvokeSOEvent();
        }

        IEnumerator GenerateLvl()
        {
            for (int t = 0; t < _totalTubesCount; t++)
            {
                TubeHandler newTube = Instantiate(TubePrefab, transform);
                newTube.transform.position = TubePositions[t];
                _totalTubes.Add(newTube);
                if (t < CurrrentLvl.Value)
                {
                    _colorTubes.Add(newTube);
                }
            }
            List<TubeHandler> tempTubes = new List<TubeHandler>();
            foreach (TubeHandler tube in _colorTubes)
            {
                tempTubes.Add(tube);
            }
            yield return new WaitForSeconds(0.25f);
            for (int l = 0; l < CurrrentLvl.Value; l++)
            {
                for (int b = 0; b < _maxColorsInTube; b++)
                {
                    int tubeNum = Random.Range(0, _colorTubes.Count);

                    if (_colorTubes[tubeNum].WaterColors.Count < _maxColorsInTube)
                    {
                        if (IsHiddenLevel.Value == 1)
                        {
                            _colorTubes[tubeNum].SetHidenColour(_levelColors.Colors[l]);
                        }
                        else
                        {
                            _colorTubes[tubeNum].SetColor(_levelColors.Colors[l]);
                        }

                    }
                    yield return new WaitForSeconds(0.01f);
                    if (_colorTubes[tubeNum].WaterColors.Count == _maxColorsInTube)
                    {
                        _colorTubes[tubeNum].WaterAdded();
                        _colorTubes.RemoveAt(tubeNum);
                    }
                }
            }
            yield return new WaitForSeconds(0.1f);
            for (int c = 0; c < tempTubes.Count; c++)
            {
                _currentLevelColors[c] = new List<Color32>();
                foreach (Color col in tempTubes[c].WaterColors)
                {
                    _currentLevelColors[c].Add(col);
                }
            }
            SwipeProtectorEvent.InvokeSOEvent(0);
            CanPlay.Value = 1;
            if (lvlMakingRotine != null)
            {
                StopCoroutine(lvlMakingRotine);
            }
        }

        void RestartLevel()
        {
            if (BtnOnceClicked.Value == 0)
            {
                BtnOnceClicked.Value = 1;
                CanPlay.Value = 0;
                lvlMakingRotine = StartCoroutine(ReGenerateLevel());
            }
        }

        IEnumerator ReGenerateLevel()
        {
            DestroyLevel();
            yield return new WaitForSeconds(0.5f); 
            _totalTubesCount = CurrrentLvl.Value + 2;
            SwipeProtectorEvent.InvokeSOEvent(1);
            for (int t = 0; t < _totalTubesCount; t++)
            {
                TubeHandler newTube = Instantiate(TubePrefab, transform);
                newTube.transform.position = TubePositions[t];
                if (t < CurrrentLvl.Value)
                {
                    List<Color32> tubeColors = _currentLevelColors[t];
                    yield return new WaitForSeconds(0.015f);
                    for (int c = 0; c < _maxColorsInTube; c++)
                    {
                        if (IsHiddenLevel.Value == 1)
                        {
                            newTube.SetHidenColour(tubeColors[c]);
                        }
                        else
                        {
                            newTube.SetColor(tubeColors[c]);
                        }
                        yield return new WaitForSeconds(0.01f);
                    }
                }
            }
            yield return new WaitForSeconds(0.1f);
            SwipeProtectorEvent.InvokeSOEvent(0);
            CanPlay.Value = 1;
            BtnOnceClicked.Value = 0;
            if (lvlMakingRotine != null)
            {
                StopCoroutine(lvlMakingRotine);
            }
        }

        void OnAddTubeClick()
        {
            if (_totalTubesCount < 10 && CanPlay.Value == 1)
            {
                TubeHandler newTube = Instantiate(TubePrefab, transform);
                newTube.transform.position = TubePositions[_totalTubesCount];
                _totalTubesCount++;
                _totalTubes.Add(newTube);
                ChangeExtraTubeStatEvent.InvokeSOEvent();
                TaskEvent.InvokeSOEvent(5, 1);
            }
            else if(_totalTubes.Count >= 10)
            {
                ToastMsgEvent.InvokeSOEvent(5);
            }
        }

        void ColoringPreparation()
        {
            float tweenTime = 1;
            for (int t = 0; t < _totalTubes.Count; t++)
            {
                TubeHandler currentTube = _totalTubes[t];
                if (currentTube.WaterColors.Count > 0)
                {
                    currentTube.TubeCap.gameObject.SetActive(false);
                    currentTube.transform.DOKill();
                    currentTube.transform.DOScale(_bowlScale, tweenTime);
                    currentTube.transform.DOLocalMove(BowlPositions[t], tweenTime).OnKill(() =>
                    {
                        BowlColorHandler colorBowl = Instantiate(BowlObj, transform);
                        colorBowl.transform.position = currentTube.transform.position;
                        colorBowl.SetColor(currentTube.CurrentColor);
                        Destroy(currentTube.gameObject, 0.15f);
                    });
                }
                else
                {
                        Destroy(currentTube.gameObject);
                }
            }
        }

        void DestroyLevel()
        {
            int childCount = transform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}
