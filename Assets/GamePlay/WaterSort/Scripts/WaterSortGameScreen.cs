using TMPro;
using UnityEngine;
using Core.Events;
using Core.Economy;
using Core.Variables;
using Core.DB.Variables;

namespace Core.Screen
{
    public class WaterSortGameScreen : MonoBehaviour
    {
        [SerializeField] GameObject SwipeColorsModePanel, PowerButtons;
        [SerializeField] SOEvents InitLevelEvent, SwipeColorsModeEvent, UpdateMovesEvent;
        [SerializeField] SOIntegerEvents ChangeStateEvent;
        [SerializeField] TextMeshProUGUI MovesText, CashText;
        [SerializeField] DBInt LvlNum;
        [SerializeField] SOInterger TotalMoves, CanPlay, LevelFailStateIndex;
        [SerializeField] CoreEconomy Coins;


        private void OnEnable()
        {
            SwipeColorsModeEvent.EventHandler += SwitchSwipeColorsMode;
            UpdateMovesEvent.EventHandler += UpdateMovesText;
        }

        private void OnDisable()
        {
            SwipeColorsModeEvent.EventHandler -= SwitchSwipeColorsMode;
            UpdateMovesEvent.EventHandler -= UpdateMovesText;
        }

        private void Start()
        {
            InitLevelEvent.InvokeSOEvent();
            if (LvlNum.Value < 5)
            {
                PowerButtons.SetActive(false);
            }
            else
            {
                PowerButtons.SetActive(true);
            }
            CashText.text = Coins.Amount.ToString();
        }

        void SwitchSwipeColorsMode()
        {
            if (LvlNum.Value > 5)
            {
                MovesText.text = TotalMoves.Value.ToString();
            }
            SwipeColorsModePanel.SetActive(!SwipeColorsModePanel.activeInHierarchy);
        }
        

        void UpdateMovesText()
        {
            if (LvlNum.Value > 5)
            {
                TotalMoves.Value--;
                MovesText.text = TotalMoves.Value.ToString();
            }

            if (TotalMoves.Value < 1)
            {
                CanPlay.Value = 0;
                ChangeStateEvent.InvokeEvent(LevelFailStateIndex.Value);
            }
        }
    }
}
