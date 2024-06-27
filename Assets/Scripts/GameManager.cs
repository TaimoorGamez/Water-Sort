using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

namespace Core.GamePlay.WaterSort
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        public TubeHandler openTube = null;
        public bool doingUndo = false, hidenLvl = false;
        public int completedTubes = 0;

        [SerializeField] Color[] allColours;
        [SerializeField] Transform lvlParent;
        [SerializeField] Transform[] tubePositions;
        [SerializeField] GameObject undoBtn, extraTubeBtn, completePanel;
        [SerializeField] TubeHandler tubeObj;
        [SerializeField] int startlvl = 1;
        [SerializeField] Text lvlnumText;

        int[] tubesInlvl = { 7, 8, 9 };
        int lvlNum = 0, instantiatedLvl = 0, totalTubes = 0;
        Coroutine lvlMakingRotine;
        bool onceClicked = false;
        List<TubeHandler> _levelTubes = new List<TubeHandler>();
        Stack<UndoData> _undoMoves = new Stack<UndoData>();

        void Start()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }

            lvlNum = PlayerPrefs.GetInt("lvlNum", startlvl);
            OnNextBtnClick();
        }

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
                lvlnumText.text = "Level: " + lvlNum.ToString();

                if (lvlNum % 5 == 0)
                {
                    hidenLvl = true;
                }
                else if (hidenLvl)
                {
                    hidenLvl = false;
                }

                if (lvlNum < 5)
                {
                    Instantiate(Resources.Load("Levels/lvl " + lvlNum.ToString()), lvlParent);
                    instantiatedLvl = lvlNum;
                    completePanel.SetActive(false);
                    onceClicked = false;
                }
                else if (lvlNum < 8)
                {
                    totalTubes = lvlNum + 2;
                    instantiatedLvl = lvlNum;
                    lvlMakingRotine = StartCoroutine(GenerateLvl());
                }
                else
                {
                    int randomNum = Random.Range(0, 3);
                    totalTubes = tubesInlvl[randomNum];
                    instantiatedLvl = randomNum + 5;
                    lvlMakingRotine = StartCoroutine(GenerateLvl());
                }

                if (lvlNum > 1)
                {
                    undoBtn.SetActive(true);
                }
                if (lvlNum > 4)
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
            if (_undoMoves.Count > 0 && !doingUndo)
            {
                doingUndo = true;
                UndoData lastUndo = new UndoData();
                lastUndo = _undoMoves.Pop();
                if (openTube != null)
                {
                    openTube.MoveBackIn();
                }
                openTube = lastUndo.GetterTube;
                openTube.RemoveFromCompleted();
                lastUndo.SenderTube.UndoWater(lastUndo.GetterTube, lastUndo.LiquidLayers);
            }
        }

        public void UndoComplete()
        {
            if (doingUndo)
            {
                doingUndo = false;
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
                lvlNum++;
                PlayerPrefs.SetInt("lvlNum", lvlNum);
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