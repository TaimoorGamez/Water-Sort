using UnityEngine;
using Core.Events;
using Core.Variables;
using Core.DB.Variables;
using System.Collections;
using System.Collections.Generic;

namespace Core.GamePlay.WaterSort
{
    public class WaterSortLevelInit : MonoBehaviour
    {
        [SerializeField] DBInt LvlNum, LvlIndex, MovesMultiplier;
        [SerializeField] SOInterger IsHiddenLevel, CurrentLvl, CanPlay, TotalMoves, BtnOnceClicked, MainMenuStateIndex;
        [SerializeField] SOEvents InitLevelEvent, ExtraTubeEvent, SwipeColorsModeEvent, RestartLevelEvent, DestroyLevelEvent;
        [SerializeField] SOIntegerEvents ChangeStateEvent;
        [SerializeField] TubeHandler TubePrefab;
        [SerializeField] Vector3[] TubePositions, BowlPositions;
        [SerializeField] Color[] AllColours;

        string _sortingLvlPath = "WaterSortLevels/lvl ";
        List<TubeHandler> _levelTubes = new List<TubeHandler>();
        Coroutine lvlMakingRotine;
        int _totalTubes = 0, _maxColorsInTube = 4;
        Dictionary<int, List<Color>> _currentLevelColors = new Dictionary<int, List<Color>>();

        private void OnEnable()
        {
            InitLevelEvent.EventHandler += InitNewLevel;
            ExtraTubeEvent.EventHandler += OnAddTubeClick;
            RestartLevelEvent.EventHandler += RestartLevel;
            ChangeStateEvent.EventHandler += OnClickHome;
            DestroyLevelEvent.EventHandler += DestroyLevel;
        }

        private void OnDisable()
        {
            InitLevelEvent.EventHandler -= InitNewLevel;
            ExtraTubeEvent.EventHandler -= OnAddTubeClick;
            RestartLevelEvent.EventHandler -= RestartLevel;
            ChangeStateEvent.EventHandler -= OnClickHome;
            DestroyLevelEvent.EventHandler -= DestroyLevel;
        }

        void InitNewLevel()
        {
            CanPlay.Value = 0;
            _levelTubes.Clear();
            _currentLevelColors.Clear(); 
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
                CurrentLvl.Value = LvlNum.Value;
            }
            else if (LvlNum.Value < 8)
            {
                _totalTubes = LvlNum.Value + 2;
                CurrentLvl.Value = LvlNum.Value;
                lvlMakingRotine = StartCoroutine(GenerateLvl());
            }
            else
            {
                int randomNum = Random.Range(0, 3);
                CurrentLvl.Value = randomNum + 5;
                _totalTubes = CurrentLvl.Value+2;
                lvlMakingRotine = StartCoroutine(GenerateLvl());
            }

            TotalMoves.Value = CurrentLvl.Value * MovesMultiplier.Value;
        }

        IEnumerator GenerateLvl()
        {
            for (int t = 0; t < _totalTubes; t++)
            {
                TubeHandler newTube = Instantiate(TubePrefab, transform);
                newTube.transform.position = TubePositions[t];
                if (t < CurrentLvl.Value)
                { 
                    _levelTubes.Add(newTube);
                    _currentLevelColors[t] = new List<Color>();
                }
            }
            List<TubeHandler> tempTubes = new List<TubeHandler>();
            foreach (TubeHandler tube in _levelTubes)
            {
                tempTubes.Add(tube);
            }
            yield return new WaitForSeconds(0.25f);
            for (int l = 0; l < CurrentLvl.Value; l++)
            {
                for (int b = 0; b < _maxColorsInTube; b++)
                {
                    int tubeNum = Random.Range(0, _levelTubes.Count);

                    if (_levelTubes[tubeNum].WaterColors.Count < _maxColorsInTube)
                    {
                        if (IsHiddenLevel.Value == 1)
                        {
                            _levelTubes[tubeNum].SetHidenColour(AllColours[l]);
                        }
                        else
                        {
                            _levelTubes[tubeNum].SetColor(AllColours[l]);
                        }
                       
                    }
                    yield return new WaitForSeconds(0.01f);
                    if (_levelTubes[tubeNum].WaterColors.Count == _maxColorsInTube)
                    {
                        _levelTubes[tubeNum].WaterAdded();
                        _levelTubes.RemoveAt(tubeNum);
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
            //Debug.Log(_currentLeve;lColors.Count);
            yield return new WaitForSeconds(0.5f);
            SwipeColorsModeEvent.InvokeSOEvent();
            for (int t = 0; t < _totalTubes; t++)
            {
                TubeHandler newTube = Instantiate(TubePrefab, transform);
                newTube.transform.position = TubePositions[t];
                if (t < CurrentLvl.Value)
                {
                    List<Color> tubeColors = _currentLevelColors[t];
                    //Debug.Log(tubeColors.Count);
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

        void OnClickHome(int state)
        {
            if (state == MainMenuStateIndex.Value)
            {
                DestroyLevel();
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
