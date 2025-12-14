using TMPro;
using Core.States;
using UnityEngine;
using Core.Events;
using Core.DB.Variables;
using Core.GamePlay.WaterSort;

namespace Core.Screen
{
    public class WaterSortGameScreen : UiScreens
    {
        [SerializeField] DBInt LvlNum;
        [SerializeField] GameObject SwipeColorsModePanel, PowerButtons, TopBar, PauseBtn;
        [SerializeField] TextMeshProUGUI MovesText;

        private void OnEnable()
        {
            SingleIntegerEventsHolder.SwitchProtectorEvent += SwitchProtector;
            SimpleEventsHolder.UpdateMovesEvent += UpdateMovesText;
            SimpleEventsHolder.StartColoringEvent += PrepareForColoring;
        }

        private void OnDisable()
        {
            SingleIntegerEventsHolder.SwitchProtectorEvent -= SwitchProtector;
            SimpleEventsHolder.UpdateMovesEvent -= UpdateMovesText;
            SimpleEventsHolder.StartColoringEvent -= PrepareForColoring;
        }

        private void Start()
        {
            if (LvlNum.Value < LevelsManager.I.MinLvlCount)
            {
                PowerButtons.SetActive(false);
                PauseBtn.SetActive(false);
            }
            else
            {
                PowerButtons.SetActive(true);
                PauseBtn.SetActive(true);
            }
        }

        void SwitchProtector(int state)
        {
            SwipeColorsModePanel.SetActive(state == 1);
        }


        void UpdateMovesText()
        {
            if (LvlNum.Value >= LevelsManager.I.MinLvlCount)
            {
                MovesText.text = LevelsManager.I.TotalMoves.ToString();

                if (LevelsManager.I.TotalMoves < 1)
                {
                    LevelsManager.I.CanPlay = false;
                    Invoke(nameof(OnMovesEnd), 3);
                }
            }
        }

        void OnMovesEnd()
        {
            if (LevelsManager.I.CompletedTubes != LevelsManager.I.CurrrentLvl)
            {
                SingleIntegerEventsHolder.ActiveStateEvent?.Invoke(StateManager.I.LevelFailStateIndex);
            }
        }

        void PrepareForColoring()
        {
            PowerButtons.SetActive(false);
            TopBar.SetActive(false);
        }

        public void OnClickPause()
        {
            LevelsManager.I.CanPlay = false;
            SingleIntegerEventsHolder.ActiveStateEvent?.Invoke(StateManager.I.PauseStateIndex);
        }
    }
}
