using UnityEngine;
using Core.Events;
using Core.Variables;
using System.Collections.Generic;

namespace Core.GamePlay.WaterSort
{
    [CreateAssetMenu(fileName = "UndoManager", menuName = "ScriptableObjects/WaterSort/UndoManager")]
    public class UndoManager : ScriptableObject
    {
        [SerializeField] SOInterger DoingUndo, UsingAnyFeature;
        [SerializeField] SOWaterTube OpenTube;
        [SerializeField] SOEvents UndoEvent;

        Stack<UndoData> _undoMoves = new Stack<UndoData>();

        public void AddUndo(TubeHandler senderTube, TubeHandler getterTube, int liquidLayers)
        {
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
            Debug.Log("Here38");
            if (_undoMoves.Count > 0 && DoingUndo.Value == 0 && UsingAnyFeature.Value == 0)
            {
                DoingUndo.Value = 1;
                UsingAnyFeature.Value = 1;
                UndoData lastUndo = new UndoData();
                lastUndo = _undoMoves.Pop();
                if (OpenTube.Tube != null)
                {
                    OpenTube.Tube.MoveBackIn();
                }
                OpenTube.Tube = lastUndo.GetterTube;
                OpenTube.Tube.RemoveFromCompleted();
                lastUndo.SenderTube.UndoWater(lastUndo.GetterTube, lastUndo.LiquidLayers);
                Debug.Log("Here52");
            }
        }
    }
}
