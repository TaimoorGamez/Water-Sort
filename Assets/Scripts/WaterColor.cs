using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Core.GamePlay.WaterSort
{
    public class WaterColor : MonoBehaviour
    {
        [SerializeField] Renderer MySkin;
        [SerializeField] LineRenderer WaterLine;
        [SerializeField] ParticleSystem WaterDorpParticle;
        [SerializeField] Transform WaterStartPosition, WaterLineEndPos, TubePos;
        [SerializeField] Animation TubeAnimation;
        [SerializeField] GameObject[] HidenMarks;
        [SerializeField] float[] QustionMarkPositions;
        [SerializeField] GameObject QustionMark;
        [SerializeField] float QustionMarkMovemenTime = 0.5f;

        public Color CurrentTopColor = Color.black;
        public List<Color> WaterColors = new List<Color>();

        MaterialPropertyBlock _propBlock;
        float _eachLiquidLayerHeight = 0.25f, _transparencyChangeDuration = 1, _waterPosIncrement = 0.02f;
        Coroutine _transparencyRotine, _posRotine;
        int _totalLayyers = 4;

        private void Start()
        {
            _propBlock = new MaterialPropertyBlock();
            WaterLine.SetPosition(0, TubePos.TransformPoint(WaterStartPosition.localPosition));
            WaterLine.SetPosition(1, TubePos.TransformPoint(WaterLineEndPos.localPosition));
        }

        public void SetHidenColour(Color currentColor)
        {
            if (WaterColors.Count < _totalLayyers - 1)
            {
                HidenMarks[WaterColors.Count].gameObject.SetActive(true);
                QustionMark.transform.position += new Vector3(0, QustionMarkPositions[WaterColors.Count], 0);
                QustionMark.SetActive(true);
            }
            SetColor(currentColor);
        }

        public void SetColor(Color currentColor)
        {
            // Set the color properties in the material property block
            WaterColors.Add(currentColor);
            _propBlock.SetColor("_Color" + WaterColors.Count.ToString(), currentColor);
            // Apply the material property block to the renderer
            MySkin.SetPropertyBlock(_propBlock);
            CurrentTopColor = currentColor;
            // Set transparency property in the material property block
            _propBlock.SetFloat("_TransparencyRange", _eachLiquidLayerHeight * WaterColors.Count);

            // Apply the material property block to the renderer
            MySkin.SetPropertyBlock(_propBlock);
            WaterLineEndPos.localPosition += new Vector3(0, _waterPosIncrement, 0);
        }
        public void AddColor(Color currentColor, int layers)
        {
            for (int c = 0; c < layers; c++)
            {
                WaterColors.Add(currentColor);
                _propBlock.SetColor("_Color" + WaterColors.Count.ToString(), currentColor);
                MySkin.SetPropertyBlock(_propBlock);
            }
            CurrentTopColor = currentColor;
            _propBlock.SetColor("_Color", currentColor);
            WaterLine.GetComponent<Renderer>().SetPropertyBlock(_propBlock);
            Renderer waterDropRenderer = WaterDorpParticle.GetComponent<Renderer>();
            if (waterDropRenderer != null)
            {
                waterDropRenderer.SetPropertyBlock(_propBlock);
            }
            WaterLine.gameObject.SetActive(true);
            if (_transparencyRotine != null)
            {
                StopCoroutine(_transparencyRotine);
            }
            _transparencyRotine = StartCoroutine(SmoothlyChangeTransparency(_eachLiquidLayerHeight * WaterColors.Count));
            _posRotine = StartCoroutine(ChangePosition(GetTargetPosition(WaterColors.Count)));
            WaterDorpParticle.Play();
        }
        private Vector3 GetTargetPosition(int waterColorCount)
        {
            if (waterColorCount < 1 || waterColorCount > 4)
            {
                return TubePos.TransformPoint(WaterStartPosition.localPosition); ; // Return start position as fallback
            }
            return TubePos.TransformPoint(WaterLineEndPos.localPosition + new Vector3(0, _waterPosIncrement, 0));
        }

        public void RemoveColor(int layers)
        {
            for (int c = 0; c < layers; c++)
            {
                WaterColors.RemoveAt(WaterColors.Count - 1);
                if (WaterColors.Count > 0)
                {
                    CurrentTopColor = WaterColors[WaterColors.Count - 1];
                }
                else
                {
                    CurrentTopColor = Color.black;
                }
            }
            StartCoroutine(SmoothlyChangeTransparency(_eachLiquidLayerHeight * WaterColors.Count));
            WaterLineEndPos.localPosition -= new Vector3(0, _waterPosIncrement, 0);
            if (layers < 2)
            {
                TubeAnimation.Play("FixDrop " + WaterColors.Count.ToString());
            }
            else
            {
                TubeAnimation.Play("Dropping " + WaterColors.Count.ToString());
            }
        }

        IEnumerator SmoothlyChangeTransparency(float targetTransparency)
        {
            float startTransparency = _propBlock.GetFloat("_TransparencyRange");
            float elapsedTime = 0f;

            while (elapsedTime < _transparencyChangeDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / _transparencyChangeDuration);
                float currentTransparency = Mathf.Lerp(startTransparency, targetTransparency, t);
                _propBlock.SetFloat("_TransparencyRange", currentTransparency);
                MySkin.SetPropertyBlock(_propBlock);
                yield return null;
            }
        }


        IEnumerator ChangePosition(Vector3 targetPosition)
        {
            float elapsedTime = 0f;
            Vector3 startPosition = WaterDorpParticle.transform.position; // Use the current world position of the line end
            targetPosition.z = WaterDorpParticle.transform.position.z;
            while (elapsedTime < _transparencyChangeDuration)
            {
                Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, elapsedTime / _transparencyChangeDuration);

                WaterDorpParticle.transform.position = currentPos;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure final position is set
            WaterDorpParticle.transform.position = targetPosition;
            WaterLine.gameObject.SetActive(false);
            WaterDorpParticle.Stop();
            WaterLineEndPos.localPosition += new Vector3(0, _waterPosIncrement, 0); // Update the local position of WaterLineEndPos
            if (_posRotine != null)
            {
                StopCoroutine(_posRotine);
            }
        }


        public void RevelColour()
        {
            if (WaterColors.Count > 0)
            {
                HidenMarks[WaterColors.Count - 1].SetActive(false);
            }

            if (WaterColors.Count > 1)
            {
                LeanTween.moveLocalZ(QustionMark, QustionMarkPositions[WaterColors.Count - 2], QustionMarkMovemenTime);
            }
            else
            {
                QustionMark.SetActive(false);
            }
        }

        public void RevelFullTube()
        {
            QustionMark.SetActive(false);
            foreach (GameObject obj in HidenMarks)
            {
                obj.SetActive(false);
            }
        }


        public bool GetHidenColor(int i)
        {
            return HidenMarks[i].gameObject.activeInHierarchy;
        }

        public bool IsEmpty()
        {
            return (WaterColors.Count > 0) ? false : true;
        }

        private void OnDisable()
        {
            if (_transparencyRotine != null)
            {
                StopCoroutine(_transparencyRotine);
            }
        }
    }
}