using UnityEngine;
using Core.Events;
using Core.Variables;
using System.Collections.Generic;

namespace Core.GamePlay.WaterSort
{
    [CreateAssetMenu(fileName = "UndoManager", menuName = "ScriptableObjects/WaterSort/UndoManager")]
    public class UndoManager : ScriptableObject
    {
        [SerializeField] SOInterger DoingUndo, UsingAnyFeature, CanPlay;
        [SerializeField] SOWaterTube OpenTube;
        [SerializeField] SOEvents UndoEvent;
        [SerializeField] SOIntegerEvents ToastMsgEvent;

        Stack<UndoData> _undoMoves = new Stack<UndoData>();

        public void AddUndo(TubeHandler senderTube, TubeHandler getterTube, int liquidLayers)
        {
            //Debug.Log(liquidLayers);
            UndoData newUndo = new UndoData();
            newUndo.SenderTube = senderTube;
            newUndo.GetterTube = getterTube;
            newUndo.LiquidLayers = liquidLayers;
            _undoMoves.Push(newUndo);
        }

        private void OnEnable()
        {
            UndoEvent.EventHandler += OnUndoBtnClick;
        }

        private void OnDisable()
        {
            UndoEvent.EventHandler -= OnUndoBtnClick;
        }

        void OnUndoBtnClick()
        {
            //Debug.Log("Here38");
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
                }
                else
                {
                    _undoMoves.Push(lastMove);
                }
            }
            else if (_undoMoves.Count < 1)
            {
                ToastMsgEvent.InvokeEvent(2);
            }
        }
    }
}
