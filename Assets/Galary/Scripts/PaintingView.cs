using Core.Events;
using Core.States;
using UnityEngine;
using Core.GamePlay;
using UnityEngine.UI;
using Core.Plugins.Firebase;

namespace Core.Screen
{
    public class PaintingView : MonoBehaviour
    {
        [SerializeField] RawImage PaintingImg;
        [SerializeField] Image[] Stars;

        int _currentLvl;

        public void InitPainting(Texture paintingTex, int lvlNum, int paintingStars)
        {
            PaintingImg.texture = paintingTex;
            for(int s = 0; s < paintingStars; s++)
            {
                Stars[s].enabled = true;
            }
            PaintingImg.enabled = true;
            _currentLvl = lvlNum;
        }

        public void GenerateCustomLvl()
        {
            LevelsManager.I.TempLvlIndex = _currentLvl;
            StateManager.I.ActiveState(StateManager.I.GamePlayStatePath);
            StateManager.I.DestroyState(StateManager.I.MainMenuStatePath);
            SimpleEventsHolder.InitLvlEvent?.Invoke();
            FirebaseHandler.I?.LogEvent($"Glry_Lvl:{_currentLvl}");
        }
    }
}
