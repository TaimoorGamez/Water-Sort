using TMPro;
using UnityEngine;
using Core.Events;
using Core.Variables;
using Core.DB.Variables;

namespace Core.Screen
{
    public class WaterSortGameScreen : UiScreens
    {
        [SerializeField] DBInt LvlNum;
        [SerializeField] SOInterger TotalMoves, CanPlay, LevelFailStateIndex, CompletedTubes, GamePlayStateIndex, PauseStateIndex, MinLvl, CurrrentLvl;
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
            if (LvlNum.Value < MinLvl.Value)
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
            if (LvlNum.Value >= MinLvl.Value)
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
            if (CompletedTubes.Value != CurrrentLvl.Value)
            {
                SingleIntegerEventsHolder.ActiveStateEvent?.Invoke(LevelFailStateIndex.Value);
            }
        }

        void PrepareForColoring()
        {
            PowerButtons.SetActive(false);
            TopBar.SetActive(false);
        }

        public void OnClickPause()
        {
            CanPlay.Value = 0;
            SingleIntegerEventsHolder.ActiveStateEvent?.Invoke(PauseStateIndex.Value);
        }
    }
}
