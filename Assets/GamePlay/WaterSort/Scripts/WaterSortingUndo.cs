using UnityEngine;
using Core.Variables;
using System.Collections.Generic;

namespace Core.GamePlay.WaterSort
{
    [CreateAssetMenu(fileName = "Undo", menuName = "ScriptableObjects/WaterSort/Undo")]
    public class WaterSortingUndo : ScriptableObject
    {
        [SerializeField] SOInterger DoingUndo;
        [SerializeField] SOWaterTube OpenTube;
        Stack<UndoData> _undoMoves = new Stack<UndoData>();
        public void AddUndo(TubeHandler senderTube, TubeHandler getterTube, int liquidLayers)
        {
            UndoData newUndo = new UndoData();
            newUndo.SenderTube = senderTube;
            newUndo.GetterTube = getterTube;
            newUndo.LiquidLayers = liquidLayers;
            _undoMoves.Push(newUndo);
        }

        void OnUndoBtnClick()
        {
            if (_undoMoves.Count > 0 && DoingUndo.Value == 0)
            {
                DoingUndo.Value = 1;
                UndoData lastUndo = new UndoData();
                lastUndo = _undoMoves.Pop();
                if (OpenTube.Tube != null)
                {
                    OpenTube.Tube.MoveBackIn();
                }
                OpenTube.Tube = lastUndo.GetterTube;
                OpenTube.Tube.RemoveFromCompleted();
                lastUndo.SenderTube.UndoWater(lastUndo.GetterTube, lastUndo.LiquidLayers);
            }
        }
    }
}
