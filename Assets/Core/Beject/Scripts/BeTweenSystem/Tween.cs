using UnityEngine;

namespace MBSCore.BeTweenSystem
{
    public class Tween : MonoBehaviour, ITween
    {
        TweenState ITween.TweenProcessing()
        {
            return TweenState.Waiting;
        }
    }
}