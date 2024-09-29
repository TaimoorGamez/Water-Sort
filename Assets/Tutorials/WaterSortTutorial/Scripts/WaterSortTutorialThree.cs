using UnityEngine;
using Core.Events;
using UnityEngine.UI;
using Core.Variables;
using System.Collections;

namespace Core.GamePlay.WaterSort
{
    public class WaterSortTutorialThree : MonoBehaviour
    {
        [SerializeField] SOEvents SwipeColorsModeEvent;
        [SerializeField] SOInterger CanPlay, LevelCompleteStateIndex;
        [SerializeField] SOIntegerEvents ChangeStateEvent;
        [SerializeField] Button ExtraTubeBtn;
        [SerializeField] WaterColor FirstLiquid, SecondLiquid, ThirdLiquid;
        [SerializeField] GameObject HandObj, ExtraTube, InfoText, InfoTextObj;
        [SerializeField] Color[] CurrentColors;

        int _colorIndex = 0;

        private void OnEnable()
        {
            ChangeStateEvent.EventHandler += HideInfoText;
            ExtraTubeBtn.onClick.AddListener(AddTubbe);
        }

        private void OnDisable()
        {
            ChangeStateEvent.EventHandler -= HideInfoText;
            ExtraTubeBtn.onClick.RemoveListener(AddTubbe);
        }

        private void Start()
        {
            StartCoroutine(SetColors());
        }

        IEnumerator SetColors()
        {
            yield return new WaitForSeconds(0.5f);
            for (int c = 0; c < 4; c++)
            {
                FirstLiquid.SetColor(CurrentColors[_colorIndex]);
                yield return new WaitForSeconds(0.1f);
                if (_colorIndex == 2)
                {
                    _colorIndex = 0;
                }
                else
                {
                    _colorIndex++;
                }
                SecondLiquid.SetColor(CurrentColors[_colorIndex]);
                yield return new WaitForSeconds(0.1f); 
                if (_colorIndex == 2)
                {
                    _colorIndex = 0;
                }
                else
                {
                    _colorIndex++;
                }
                ThirdLiquid.SetColor(CurrentColors[_colorIndex]);
                yield return new WaitForSeconds(0.1f);
            }
            SwipeColorsModeEvent.InvokeSOEvent();
            CanPlay.Value = 1;
            HandObj.SetActive(true);
        }

        void AddTubbe()
        {
            if (CanPlay.Value == 1)
            {
                ExtraTube.SetActive(true);
                HandObj.SetActive(false);
                InfoText.SetActive(true);
                Destroy(ExtraTubeBtn.gameObject);
            }
        }

        void HideInfoText(int stateNum)
        {
            if (LevelCompleteStateIndex.Value == stateNum)
                InfoTextObj.SetActive(false);
        }
    }
}
