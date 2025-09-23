using UnityEngine;
using CandB.Script.Core;
using R3;
using Febucci.UI.Core;
using DG.Tweening;

namespace CandB.Script.View
{
    public static class PositionUtil
    {
        public static Vector3 ToViewPosition(Vector3 position)
        {
            return new Vector3(position.x * 2.5f - 5.5f, position.y, position.z * 2.5f - 7.3f);
        }

        public static Vector2 ToViewPosition(Vector2 position)
        {
            return new Vector2(position.x * 2.5f - 5.5f, position.y * 2.5f - 7.3f);
        }

        public static Vector3 ToViewPosition(Position position)
        {
            return new Vector3(position.X * 2.5f - 5.5f, 1, position.Y * 2.5f - 7.3f);
        }
    }
}