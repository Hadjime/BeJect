using UnityEngine;

namespace MBSCore.BeTweenSystem.Examples
{
    public class BeMovedCube : MonoBehaviour
    {
        private readonly IMoveTween moveTween = new MoveTween();

        [SerializeField] private BeTweenManager tweenManager;
        [SerializeField] private bool playScaledTween = false;
        [SerializeField] private bool playUnscaledTween = false;
        [SerializeField] private Transform target;
        [SerializeField] private float duration;
        [SerializeField] private AnimationCurve moveCurve;

        private int scaledTweenId = 0;
        
        private void Start()
        {
            moveTween.SetDuration(duration);
            moveTween.SetCurve(moveCurve);
            moveTween.BeMove(transform, target.position);
            moveTween.OnComplete += tween => Debug.LogError("Complete"); 
            scaledTweenId = tweenManager.AddScaledTween(moveTween);
        }

        private void ScaledUpdateProcessing()
        {
            if (playScaledTween == false)
            {
                return;
            }
            
            tweenManager.PlayTween(scaledTweenId);
            playScaledTween = false;
        }
        
        private void Update()
        {
            ScaledUpdateProcessing();
        }
    }
}