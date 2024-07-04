using UnityEngine;
using UnityEngine.UI;
using Core.DB.Variables;
using System.Collections;
using System.Collections.Generic;

namespace Core.GamePlay.WaterSort
{
    public class GameManager : MonoBehaviour
    {
        public TubeHandler OpenTube = null;
        public bool DoingUndo = false, hidenLvl = false;
        public int completedTubes = 0;

        [SerializeField] Color[] allColours;
        [SerializeField] Transform lvlParent;
        [SerializeField] Transform[] tubePositions;
        [SerializeField] GameObject undoBtn, extraTubeBtn, completePanel;
        [SerializeField] TubeHandler tubeObj;
        [SerializeField] Text lvlnumText;
        [SerializeField] DBInt LvlNum;

        int[] tubesInlvl = { 7, 8, 9 };
        int instantiatedLvl = 0, totalTubes = 0;
        Coroutine lvlMakingRotine;
        bool onceClicked = false;
        List<TubeHandler> _levelTubes = new List<TubeHandler>();
        Stack<UndoData> _undoMoves = new Stack<UndoData>();

        public void OnNextBtnClick()
        {
            if (!onceClicked)
            {
                onceClicked = true;
                _levelTubes.Clear();
                _undoMoves.Clear();
                foreach (Transform child in lvlParent)
                {
                    Destroy(child.gameObject);
                }
                lvlnumText.text = "Level: " + LvlNum.Value.ToString();

                if (LvlNum.Value % 5 == 0)
                {
                    hidenLvl = true;
                }
                else if (hidenLvl)
                {
                    hidenLvl = false;
                }

                if (LvlNum.Value < 5)
                {
                    Instantiate(Resources.Load("Levels/lvl " + LvlNum.Value.ToString()), lvlParent);
                    instantiatedLvl = LvlNum.Value;
                    completePanel.SetActive(false);
                    onceClicked = false;
                }
                else if (LvlNum.Value < 8)
                {
                    totalTubes = LvlNum.Value + 2;
                    instantiatedLvl = LvlNum.Value;
                    lvlMakingRotine = StartCoroutine(GenerateLvl());
                }
                else
                {
                    int randomNum = Random.Range(0, 3);
                    totalTubes = tubesInlvl[randomNum];
                    instantiatedLvl = randomNum + 5;
                    lvlMakingRotine = StartCoroutine(GenerateLvl());
                }

                if (LvlNum.Value > 1)
                {
                    undoBtn.SetActive(true);
                }
                if (LvlNum.Value > 4)
                {
                    extraTubeBtn.SetActive(true);
                }

            }
        }

        public void AddUndo(TubeHandler senderTube, TubeHandler getterTube, int liquidLayers)
        {
            UndoData newUndo = new UndoData();
            newUndo.SenderTube = senderTube;
            newUndo.GetterTube = getterTube;
            newUndo.LiquidLayers = liquidLayers;
            _undoMoves.Push(newUndo);
        }

        public void OnUndoBtnClick()
        {
            if (_undoMoves.Count > 0 && !DoingUndo)
            {
                DoingUndo = true;
                UndoData lastUndo = new UndoData();
                lastUndo = _undoMoves.Pop();
                if (OpenTube != null)
                {
                    OpenTube.MoveBackIn();
                }
                OpenTube = lastUndo.GetterTube;
                OpenTube.RemoveFromCompleted();
                lastUndo.SenderTube.UndoWater(lastUndo.GetterTube, lastUndo.LiquidLayers);
            }
        }

        public void OnAddTubeClick()
        {
            if (totalTubes < 10)
            {
                totalTubes++;
                TubeHandler newTube = Instantiate(tubeObj, lvlParent);
                newTube.transform.position = tubePositions[totalTubes - 1].position;
            }
        }


        public void CheckComplete()
        {
            if (completedTubes == instantiatedLvl)
            {
                completePanel.SetActive(true);
                _undoMoves.Clear();
                LvlNum.Value++;
                completedTubes = 0;
            }
        }

        private IEnumerator GenerateLvl()
        {
            for (int t = 0; t < totalTubes; t++)
            {
                TubeHandler newTube = Instantiate(tubeObj, lvlParent);
                newTube.transform.position = tubePositions[t].position;
                if (t < instantiatedLvl)
                { _levelTubes.Add(newTube); }
            }
            yield return new WaitForSeconds(0.25f);
            for (int l = 0; l < instantiatedLvl; l++)
            {
                for (int b = 0; b < 4; b++)
                {
                    int tubeNum = Random.Range(0, _levelTubes.Count);

                    if (_levelTubes[tubeNum].MyLiquid.WaterColors.Count < 4)
                    {
                        if (hidenLvl)
                        {
                            _levelTubes[tubeNum].MyLiquid.SetHidenColour(allColours[l]);
                        }
                        else
                        {
                            _levelTubes[tubeNum].MyLiquid.SetColor(allColours[l]);
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
            yield return new WaitForSeconds(0.75f);
            completePanel.SetActive(false);
            onceClicked = false;
            if (lvlMakingRotine != null)
            {
                StopCoroutine(lvlMakingRotine);
            }
        }

    }
}