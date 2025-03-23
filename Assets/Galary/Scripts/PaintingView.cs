using Core.Events;
using UnityEngine;
using UnityEngine.UI;
using Core.Variables;

namespace Core.Screen
{
    public class PaintingView : MonoBehaviour
    {
        [SerializeField] SOEvents InitLevelEvent;
        [SerializeField] SOIntegerEvents ActiveStateEvent, DestroyStateEvent;
        [SerializeField] SOInterger GamePlayStateIndex, MainMenuStateIndex, TempLvlIndex;
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
            TempLvlIndex.Value = _currentLvl;
            ActiveStateEvent.InvokeSOEvent(GamePlayStateIndex.Value);
            DestroyStateEvent.InvokeSOEvent(MainMenuStateIndex.Value);
            InitLevelEvent.InvokeSOEvent();
        }
    }
}
