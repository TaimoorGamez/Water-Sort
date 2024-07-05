using UnityEngine;
using System.Collections;

namespace Core.GamePlay.WaterSort
{
    public class WaterSortTutorialThree : MonoBehaviour
    {
        [SerializeField] WaterColor FirstLiquid, SecondLiquid, ThirdLiquid;
        [SerializeField] GameObject HandObj, ExtraTube, InfoText;
        [SerializeField] Color[] CurrentColors;

        int _colorIndex = 0;

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
        }

        private void OnMouseDown()
        {
            ExtraTube.SetActive(true);
            HandObj.SetActive(false);
            InfoText.SetActive(true);
            Destroy(gameObject);
        }
    }
}
