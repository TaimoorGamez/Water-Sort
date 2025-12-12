using System;
using System.Collections.Generic;

namespace Core.Events
{
    public delegate void GameEvent();

    public delegate void GameEventInteger(int val);

    public delegate void GameEventWith2Ints(int index, int val);

    public static class SimpleEventsHolder
    {
        public static GameEvent

        //-------------------Game Flow Events-------------------
        SelfDestructionEvent, HideColorBowlEvent, CheckCompleteEvent,
        StartColoringEvent, ColorSelectedEvent, DestroyLevelEvent, InitLvlEvent,
        MoreMovesEvent, UpdateMovesEvent, RestartLevelEvent, ExtraTubeEvent,
        UpdateExtraTubeStatEvent, RegisterMoveEvent, UndoEvent, UpdateUndoStatusEvent,
        SwapColorsEvent, UpdateSwapStateEvent,

        //-------------------Sound Events-------------------
        OnOffBGMusic, OnOffSounds, BtnPressSfxEvent, UpdateMusicStateEvent, UpdateSoundStateEvent,

        //-------------------Spin Wheel Events-------------------
        ResetSpinWheelEvent,

        //-------------------Daily Reward Events-------------------
        UpDateDailyRewardState,

        //-------------------Daily Tasks Events-------------------
        GenerateDailyTasksEvent,

        //------------------Ads Events------------------
        NoAdsBuyEvent, StartLoadingAdsEvent, GrantRewardEvent, StartAdLoaing,
        MultiplayRewardEvent, AddMovesEvent, DoubleDailyRewardEvent,
        RewardSpinWheelEvent, BuyCaps, BuySprays, BuyFlames, AdsBlockerEvent,
        RewardUndoEvent, RewardExtraTubeEvent, RewardSwapColor;
    }

    public static class EventDictionariesHolder
    {
        public static Dictionary<string, GameEvent> NonConsumableProductsEvents = new Dictionary<string, GameEvent>(StringComparer.Ordinal)
        {
        };

        public static Dictionary<string, GameEvent> StoreBuyEvents = new Dictionary<string, GameEvent>(StringComparer.Ordinal)
        {
            { "CapBuy", SimpleEventsHolder.BuyCaps },
            { "SprayBuy", SimpleEventsHolder.BuySprays },
            { "FlameThrowerBuy", SimpleEventsHolder.BuyFlames }
        };

        public static Dictionary<string, GameEvent> PowerEvents = new Dictionary<string, GameEvent>(StringComparer.Ordinal)
        {
            { "UndoColor", SimpleEventsHolder.UndoEvent },
            { "SwapColor", SimpleEventsHolder.SwapColorsEvent },
            { "ExtraTube", SimpleEventsHolder.ExtraTubeEvent }
        };

        public static Dictionary<string, GameEvent> ChangePowerStatusEvent = new Dictionary<string, GameEvent>(StringComparer.Ordinal)
        {
            { "UndoColor", SimpleEventsHolder.UpdateUndoStatusEvent },
            { "SwapColor", SimpleEventsHolder.UpdateSwapStateEvent },
            { "ExtraTube", SimpleEventsHolder.UpdateExtraTubeStatEvent }
        };

        public static Dictionary<string, GameEvent> RewardPowerEvent = new Dictionary<string, GameEvent>(StringComparer.Ordinal)
        {
            { "UndoColor", SimpleEventsHolder.RewardUndoEvent },
            { "SwapColor", SimpleEventsHolder.RewardSwapColor },
            { "ExtraTube", SimpleEventsHolder.RewardExtraTubeEvent }
        };
    }
}
