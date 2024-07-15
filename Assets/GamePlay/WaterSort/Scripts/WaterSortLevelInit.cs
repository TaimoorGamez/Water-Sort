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
        [SerializeField] DBInt LvlNum;
        [SerializeField] SOInterger IsHiddenLevel, CurrentLvl, CanPlay;
        [SerializeField] SOEvents InitLevelEvent, ExtraTubeEvent, SwipeColorsModeEvent;
        [SerializeField] TubeHandler TubePrefab;
        [SerializeField] Vector3[] TubePositions;
        [SerializeField] Color[] AllColours;

        List<TubeHandler> _levelTubes = new List<TubeHandler>();
        int[] tubesInlvl = { 7, 8, 9 };
        Coroutine lvlMakingRotine;
        int _totalTubes = 4;

        private void OnEnable()
        {
            InitLevelEvent.EventHandler += InitNewLevel;
            ExtraTubeEvent.EventHandler += OnAddTubeClick;
        }

        private void OnDisable()
        {
            InitLevelEvent.EventHandler -= InitNewLevel;
            ExtraTubeEvent.EventHandler -= OnAddTubeClick;
        }

        void InitNewLevel()
        {
            CanPlay.Value = 0;
            _levelTubes.Clear();
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
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
                Instantiate(Resources.Load("WaterSortLevels/lvl " + LvlNum.Value.ToString()), transform);
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
                _totalTubes = tubesInlvl[randomNum];
                CurrentLvl.Value = randomNum + 5;
                lvlMakingRotine = StartCoroutine(GenerateLvl());
            }
        }

        IEnumerator GenerateLvl()
        {
            for (int t = 0; t < _totalTubes; t++)
            {
                TubeHandler newTube = Instantiate(TubePrefab, transform);
                newTube.transform.position = TubePositions[t];
                if (t < CurrentLvl.Value)
                { _levelTubes.Add(newTube); }
            }
            yield return new WaitForSeconds(0.25f);
            for (int l = 0; l < CurrentLvl.Value; l++)
            {
                for (int b = 0; b < 4; b++)
                {
                    int tubeNum = Random.Range(0, _levelTubes.Count);

                    if (_levelTubes[tubeNum].MyLiquid.WaterColors.Count < 4)
                    {
                        if (IsHiddenLevel.Value == 1)
                        {
                            _levelTubes[tubeNum].MyLiquid.SetHidenColour(AllColours[l]);
                        }
                        else
                        {
                            _levelTubes[tubeNum].MyLiquid.SetColor(AllColours[l]);
                        }
                    }
                    yield return new WaitForSeconds(0.01f);
                    if (_levelTubes[tubeNum].MyLiquid.WaterColors.Count == 4)
                    {
                        _levelTubes[tubeNum].WaterAdded();
                        _levelTubes.RemoveAt(tubeNum);
                    }
                }
            }
            yield return new WaitForSeconds(0.1f);
            SwipeColorsModeEvent.InvokeEvent();
            CanPlay.Value = 1;
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
    }
}
