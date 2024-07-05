using UnityEngine;
using System.Collections;

namespace Core.Animations.LT
{
    public class TutorialHandAnimationPlayer : MonoBehaviour
    {
        [SerializeField] GameObject HandObj, ParticleObj;
        [SerializeField] SOLeanTween ScaleDown, ScaleUp;
        [SerializeField] float Delays;

        Coroutine _animationsRotine;
        bool _whileEnable;

        private void OnEnable()
        {
            _whileEnable = true;
            _animationsRotine = StartCoroutine(PlayAnimation());
        }
        private void OnDisable()
        {
            _whileEnable = false;
            if (_animationsRotine != null)
            {
                StopCoroutine(_animationsRotine);
            }
        }

        IEnumerator PlayAnimation()
        {
            while (_whileEnable)
            {
                ScaleDown.TargetObj = HandObj;
                ScaleDown.PlayAnimation();
                yield return new WaitForSeconds(Delays);
                ParticleObj.SetActive(false);
                ParticleObj.SetActive(true);
                ScaleUp.TargetObj = HandObj;
                ScaleUp.PlayAnimation();
                yield return new WaitForSeconds(Delays);
            }
        }

    }
}
