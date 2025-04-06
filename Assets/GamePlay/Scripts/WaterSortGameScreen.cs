using TMPro;
using UnityEngine;
using Core.Events;
using Core.Variables;
using Core.DB.Variables;

namespace Core.Screen
{
    public class WaterSortGameScreen : MonoBehaviour
    {
        [SerializeField] SOEvents UpdateMovesEvent, StartColoringEvent, RestartLevelEvent;
        [SerializeField] SOIntegerEvents ActiveStateEvent, SwitchProtectorEvent, ChangeBackgroundEvent;
        [SerializeField] DBInt LvlNum, LvlIndex;
        [SerializeField] SOInterger TotalMoves, CanPlay, LevelFailStateIndex, CompletedTubes, GamePlayStateIndex, PauseStateIndex, TempLevelIndex, MinLvl, CurrrentLvl;
        [SerializeField] Transform ColoringHolder;
        [SerializeField] GameObject SwipeColorsModePanel, PowerButtons, TopBar, PauseBtn;
        [SerializeField] TextMeshProUGUI MovesText;

        string _coloringPath = "ColoringPart/lvl ";

        private void OnEnable()
        {
            ChangeBackgroundEvent.InvokeSOEvent(GamePlayStateIndex.Value);
            SwitchProtectorEvent.EventHandler += SwitchProtector;
            UpdateMovesEvent.EventHandler += UpdateMovesText;
            StartColoringEvent.EventHandler += PrepareForColoring;
            RestartLevelEvent.EventHandler += RegenrateColoring;
        }

        private void OnDisable()
        {
            SwitchProtectorEvent.EventHandler -= SwitchProtector;
            UpdateMovesEvent.EventHandler -= UpdateMovesText;
            StartColoringEvent.EventHandler -= PrepareForColoring;
            RestartLevelEvent.EventHandler -= RegenrateColoring;
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
            Instantiate(Resources.Load(_coloringPath + (TempLevelIndex.Value == -1 ? LvlIndex.Value : TempLevelIndex.Value)), ColoringHolder);
        }

        void RegenrateColoring()
        {
            Destroy(ColoringHolder.GetChild(0).gameObject); 
            Instantiate(Resources.Load(_coloringPath + (TempLevelIndex.Value == -1 ? LvlIndex.Value : TempLevelIndex.Value)), ColoringHolder);
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
                ActiveStateEvent.InvokeSOEvent(LevelFailStateIndex.Value);
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
            ActiveStateEvent.InvokeSOEvent(PauseStateIndex.Value);
        }
    }
}
