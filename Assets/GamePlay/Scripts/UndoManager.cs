using UnityEngine;
using Core.Events;
using Core.Variables;
using Core.DB.Variables;
using System.Collections.Generic;

namespace Core.GamePlay.WaterSort
{
    [CreateAssetMenu(fileName = "UndoManager", menuName = "ScriptableObjects/WaterSort/UndoManager")]
    public class UndoManager : ScriptableObject
    {
        [SerializeField] DBInt LvlNum;
        [SerializeField] SOInterger DoingUndo, UsingAnyFeature, CanPlay, GamePlayStateIndex, MinLvlIndex;
        [SerializeField] SOWaterTube OpenTube;

        Stack<UndoData> _undoMoves = new Stack<UndoData>();

        public void AddUndo(TubeHandler senderTube, TubeHandler getterTube, int liquidLayers)
        {
            SimpleEventsHolder.RegisterMoveEvent?.Invoke();
            UndoData newUndo = new UndoData();
            newUndo.SenderTube = senderTube;
            newUndo.GetterTube = getterTube;
            newUndo.LiquidLayers = liquidLayers;
            _undoMoves.Push(newUndo);
        }

        private void OnEnable()
        {
            SimpleEventsHolder.UndoEvent += OnUndoBtnClick;
            SimpleEventsHolder.RestartLevelEvent += RestForNewLevel;
            SimpleEventsHolder.InitLvlEvent += RestForNewLevel;
        }

        private void OnDisable()
        {
            SimpleEventsHolder.UndoEvent -= OnUndoBtnClick;
            SimpleEventsHolder.RestartLevelEvent -= RestForNewLevel;
            SimpleEventsHolder.InitLvlEvent -= RestForNewLevel;
        }

        void RestForNewLevel()
        {
            _undoMoves.Clear();
        }

        void OnUndoBtnClick()
        {
            if (_undoMoves.Count > 0 && DoingUndo.Value == 0 && UsingAnyFeature.Value == 0 && CanPlay.Value == 1)
            {
                DoingUndo.Value = 1;
                UsingAnyFeature.Value = 1;
                UndoData lastMove = new UndoData();
                lastMove = _undoMoves.Pop();
                if (!lastMove.GetterTube.IsBussy && !lastMove.SenderTube.IsBussy)
                {
                    if (OpenTube.Tube != null)
                    {
                        OpenTube.Tube.MoveBackIn();
                    }
                    OpenTube.Tube = lastMove.GetterTube;
                    OpenTube.Tube.RemoveFromCompleted();
                    lastMove.SenderTube.UndoWater(lastMove.GetterTube, lastMove.LiquidLayers);
                    if (LvlNum.Value >= MinLvlIndex.Value)
                    {
                        SimpleEventsHolder.UpdateUndoStatusEvent?.Invoke();
                    }
                    DoubleIntegerEventHolder.TaskEvent?.Invoke(3, 1);
                }
                else
                {
                    _undoMoves.Push(lastMove);
                }
            }
            else if (_undoMoves.Count < 1)
            {
                SingleIntegerEventsHolder.ShowToastEvent?.Invoke(2);
            }
            else if (UsingAnyFeature.Value == 1)
            {
                SingleIntegerEventsHolder.ShowToastEvent?.Invoke(4);
            }
        }
    }
}
