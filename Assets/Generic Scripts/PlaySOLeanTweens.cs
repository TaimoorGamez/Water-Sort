using UnityEngine;
using System.Collections;

namespace Core.Animations.DT
{
    public class PlaySOLeanTweens : MonoBehaviour
    {
        [SerializeField] GameObject[] TargetObjs;
        [SerializeField] SOLeanTween[] TweenAnimations;
        [SerializeField] float[] Delays;
        [SerializeField] bool PlayOnEnable = false, PlayLoop = false;
        [SerializeField] float StartDelay = 0;

        Coroutine _animationsRotine;

        private void OnEnable()
        {
            if (PlayOnEnable)
            {
                PlayAnimation();
            }
        }

        public void PlayAnimation()
        {
            _animationsRotine = StartCoroutine(PlayAllAnimations());
        }

        IEnumerator PlayAllAnimations()
        {
            yield return new WaitForSeconds(StartDelay);
            for (int a = 0; a < TweenAnimations.Length; a++)
            {
                TweenAnimations[a].TargetObj = TargetObjs[a];
                TweenAnimations[a].PlayAnimation();
                yield return new WaitForSeconds(Delays[a]);
            }
            AnimationsComplete();
        }

        void AnimationsComplete()
        {
            if (_animationsRotine != null)
            {
                StopCoroutine(_animationsRotine);
                _animationsRotine = null;
            }

            if (PlayLoop)
            {
                PlayAnimation();
            }
        }
    }
}
