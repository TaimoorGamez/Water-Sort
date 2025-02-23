using UnityEngine;
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
        [SerializeField] DBInt LvlNum, LvlIndex, MovesMultiplier;
        [SerializeField] SOInterger IsHiddenLevel, CanPlay, TotalMoves, BtnOnceClicked, MainMenuStateIndex, CurrrentLvl;
        [SerializeField] SOEvents InitLevelEvent, ExtraTubeEvent, SwipeColorsModeEvent, RestartLevelEvent, DestroyLevelEvent;
        [SerializeField] TubeHandler TubePrefab;
        [SerializeField] Vector3[] TubePositions, BowlPositions;

        string _sortingLvlPath = "WaterSortLevels/lvl ";
        List<TubeHandler> _colorTubes = new List<TubeHandler>();
        Coroutine lvlMakingRotine;
        int _totalTubes = 0, _maxColorsInTube = 4;
        SOColors _levelColors; 
        Dictionary<int, List<Color32>> _currentLevelColors = new Dictionary<int, List<Color32>>();

        private void OnEnable()
        {
            InitLevelEvent.EventHandler += InitNewLevel;
            ExtraTubeEvent.EventHandler += OnAddTubeClick;
            RestartLevelEvent.EventHandler += RestartLevel;
            DestroyLevelEvent.EventHandler += DestroyLevel;
        }

        private void OnDisable()
        {
            InitLevelEvent.EventHandler -= InitNewLevel;
            ExtraTubeEvent.EventHandler -= OnAddTubeClick;
            RestartLevelEvent.EventHandler -= RestartLevel;
            DestroyLevelEvent.EventHandler -= DestroyLevel;
        }

        void InitNewLevel()
        {
            CanPlay.Value = 0;
            _colorTubes.Clear();
            DestroyLevel();
            if (LvlNum.Value % 5 == 0)
            {
                IsHiddenLevel.Value = 1;
            }
            else if (IsHiddenLevel.Value == 1)
            {
                IsHiddenLevel.Value = 0;
            }

            if (LvlNum.Value < 5)
            {
                Instantiate(Resources.Load(_sortingLvlPath + LvlNum.Value), transform);
                CurrrentLvl.Value = LvlNum.Value;
            }
            else
            {
                _levelColors = Resources.Load<SOColors>(_sortingLvlPath + LvlNum.Value);
                CurrrentLvl.Value = _levelColors.Colors.Length;
                _totalTubes = CurrrentLvl.Value + 2;
                lvlMakingRotine = StartCoroutine(GenerateLvl());
                TotalMoves.Value = CurrrentLvl.Value * MovesMultiplier.Value;
            }
        }

        IEnumerator GenerateLvl()
        {
            for (int t = 0; t < _totalTubes; t++)
            {
                TubeHandler newTube = Instantiate(TubePrefab, transform);
                newTube.transform.position = TubePositions[t];
                if (t < CurrrentLvl.Value)
                { 
                    _colorTubes.Add(newTube);
                    //_currentLevelColors[t] = new List<Color>();
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
            for (int c=0; c< tempTubes.Count; c++)
            {
                foreach (Color col in tempTubes[c].WaterColors)
                {
                    _currentLevelColors[c].Add(col);
                }
            }
            SwipeColorsModeEvent.InvokeSOEvent();
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
            SwipeColorsModeEvent.InvokeSOEvent();
            for (int t = 0; t < _totalTubes; t++)
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
            SwipeColorsModeEvent.InvokeSOEvent();
            CanPlay.Value = 1;
            BtnOnceClicked.Value = 0;
            if (lvlMakingRotine != null)
            {
                StopCoroutine(lvlMakingRotine);
            }
        }

        void OnAddTubeClick()
        {
            if (_totalTubes < 10 && CanPlay.Value==1)
            {
                TubeHandler newTube = Instantiate(TubePrefab, transform);
                newTube.transform.position = TubePositions[_totalTubes];
                _totalTubes++;
            }
        }

        void DestroyLevel()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
