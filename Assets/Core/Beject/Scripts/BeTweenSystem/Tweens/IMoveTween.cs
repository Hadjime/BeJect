using UnityEngine;

namespace MBSCore.BeTweenSystem
{
    public interface IMoveTween : ITween
    {
        void BeMove(Transform target, Vector3 moveTarget);
    }
}