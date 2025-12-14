using System;
using UnityEngine;
using System.Collections.Generic;

namespace Core.DB.Variables
{
    public class DBInteger
    {
        public string PrefName;
        public int DefaultValue;

        public int Value
        {
            get => PlayerPrefs.GetInt(PrefName, DefaultValue);

            set
            {
                PlayerPrefs.SetInt(PrefName, value);
                PlayerPrefs.Save();
            }
        }

        public DBInteger(string name, int defaultValue = 0)
        {
            PrefName = name;
            DefaultValue = defaultValue;

            // Initialize only once
            if (!PlayerPrefs.HasKey(PrefName))
            {
                PlayerPrefs.SetInt(PrefName, DefaultValue);
                PlayerPrefs.Save();
            }
        }
    }

    public static class DBIntsHolder
    {
        //---------------------Ads Related -----------------------
        public static DBInteger NoAds = new DBInteger("NoAds", 0);
        public static DBInteger AdBlocked = new DBInteger("AdBlocked", 0);

        //---------------------Currencies Data -------------------
        public static DBInteger CashWallet = new DBInteger("CashWallet", 0);       

        //---------------------Daily Reward -----------------------
        public static DBInteger ToDay = new DBInteger("ToDay", 0);
        public static DBInteger RewardClaimed = new DBInteger("RewardClaimed", 0);

        //---------------------Daily Tasks ------------------------
        public static DBInteger Task0 = new DBInteger("Task0", 0);
        public static DBInteger Task1 = new DBInteger("Task1", 0);
        public static DBInteger Task2 = new DBInteger("Task2", 0);
        public static DBInteger Task3 = new DBInteger("Task3", 0);

        //---------------------Spin Wheel -------------------------
        public static DBInteger SpinAvailable = new DBInteger("DailySpinAvailable", 1);

        //---------------------Sound Related ----------------------
        public static DBInteger Music = new DBInteger("Music", 1);
        public static DBInteger Sound = new DBInteger("RewardClaimed", 1);

        //---------------------Store Data -------------------------
        public static DBInteger CurrentActiveCap = new DBInteger("CurrentActiveCap", 0);
        public static DBInteger CurrentActiveSpray = new DBInteger("CurrentActiveSpray", 0);
        public static DBInteger CurrentActiveFlameThrower = new DBInteger("CurrentActiveFlameThrower", 0);


        //---------------------Game Flow --------------------------
        public static DBInteger FFT = new DBInteger("FFT", 0);
        public static DBInteger LvlNum = new DBInteger("LvlNum", 0);
        public static DBInteger LvlIndex = new DBInteger("LvlIndex", 0);
        public static DBInteger RemaingUndo = new DBInteger("RemaingUndo", 0);
        public static DBInteger RemainingSwaps = new DBInteger("RemainingSwaps", 0);
        public static DBInteger RemainingTubes = new DBInteger("RemainingTubes", 0);

    }

    public static class DBIntDictionariesHolder
    {
        public static Dictionary<int, DBInteger> PowersData = new Dictionary<int, DBInteger>()
        {
            { 0, DBIntsHolder.RemaingUndo },
            { 1, DBIntsHolder.RemainingSwaps },
            { 2, DBIntsHolder.RemainingTubes }
        };

        public static Dictionary<int, DBInteger> TaskIndexies = new Dictionary<int, DBInteger>()
        {
            { 0, DBIntsHolder.Task0 },
            { 1, DBIntsHolder.Task1 },
            { 2, DBIntsHolder.Task2 },
            { 3, DBIntsHolder.Task3 }
        };

        public static Dictionary<string, DBInteger> PowerStatusData = new Dictionary<string, DBInteger>(StringComparer.Ordinal)
        {
            { "UndoColor", DBIntsHolder.RemaingUndo },
            { "SwapColor", DBIntsHolder.RemainingSwaps },
            { "ExtraTube", DBIntsHolder.RemainingTubes }
        };

        public static Dictionary<string, DBInteger> StoreActiveItems = new Dictionary<string, DBInteger>(StringComparer.Ordinal)
        {
            { "Cap", DBIntsHolder.CurrentActiveCap },
            { "Spray", DBIntsHolder.CurrentActiveSpray },
            { "FlameThrower", DBIntsHolder.CurrentActiveFlameThrower }
        };

        public static Dictionary<string, DBInteger> NonConsumableProductsData = new Dictionary<string, DBInteger>(StringComparer.Ordinal)
        {
            { "NoAds", DBIntsHolder.NoAds }
        };
    }
}
