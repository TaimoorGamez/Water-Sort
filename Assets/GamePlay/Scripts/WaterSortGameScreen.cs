using TMPro;
using UnityEngine;
using Core.Events;
using Core.Variables;
using Core.DB.Variables;

namespace Core.Screen
{
    public class WaterSortGameScreen : MonoBehaviour
    {
        [SerializeField] SOEvents InitLevelEvent, UpdateMovesEvent, StartColoringEvent;
        [SerializeField] SOIntegerEvents ActiveStateEvent, SwitchProtectorEvent, ChangeBackgroundEvent;
        [SerializeField] TextMeshProUGUI MovesText;
        [SerializeField] DBInt LvlNum, LvlIndex;
        [SerializeField] SOInterger TotalMoves, CanPlay, LevelFailStateIndex, CompletedTubes, GamePlayStateIndex;
        [SerializeField] Transform ColoringHolder;
        [SerializeField] GameObject SwipeColorsModePanel, PowerButtons, TopBar;

        string _coloringPath = "ColoringPart/lvl ";

        private void OnEnable()
        {
            ChangeBackgroundEvent.InvokeSOEvent(GamePlayStateIndex.Value);
            SwitchProtectorEvent.EventHandler += SwitchProtector;
            UpdateMovesEvent.EventHandler += UpdateMovesText;
            StartColoringEvent.EventHandler += PrepareForColoring;
        }

        private void OnDisable()
        {
            SwitchProtectorEvent.EventHandler -= SwitchProtector;
            UpdateMovesEvent.EventHandler -= UpdateMovesText;
            StartColoringEvent.EventHandler -= PrepareForColoring;
        }

        private void Start()
        {
            Instantiate(Resources.Load(_coloringPath + LvlIndex.Value), ColoringHolder);
            InitLevelEvent.InvokeSOEvent();
            if (LvlNum.Value < 5)
            {
                PowerButtons.SetActive(false);
            }
            else
            {
                PowerButtons.SetActive(true);
            }
        }

        void SwitchProtector(int state)
        {
            SwipeColorsModePanel.SetActive(state == 1);
        }
        

        void UpdateMovesText()
        {
            if (LvlNum.Value > 5)
            {
                MovesText.text = TotalMoves.Value.ToString();

                if (TotalMoves.Value < 1)
                {
                    CanPlay.Value = 0;
                    Invoke(nameof(OnMovesEnd), 3);
                }
            }
        }

        void OnMovesEnd()
        {
            if (CompletedTubes.Value != 0)
            { 
                ActiveStateEvent.InvokeSOEvent(LevelFailStateIndex.Value); 
            }
        }

        void PrepareForColoring()
        {
            PowerButtons.SetActive(false);
            TopBar.SetActive(false);
        }
    }
}
