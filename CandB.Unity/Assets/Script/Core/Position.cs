using System;

namespace CandB.Script.Core
{
    public struct Position
    {
        public int X;
        public int Y;

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Position other)
        {
            return X == other.X && Y == other.Y;
        }

        public static bool operator ==(Position left, Position right)
        {
            return left.Equals(right);
        }

        // 3. != 演算子のオーバーロード (== の実装は必須)
        // == の結果を単純に反転させるだけでOKです。
        public static bool operator !=(Position left, Position right)
        {
            return !(left == right);
        }

        public static Position operator +(Position a, Position b)
        {
            return new Position(a.X + b.X, a.Y + b.Y);
        }

        public static Position operator *(Position a, int scalar)
        {
            return new Position(a.X * scalar, a.Y * scalar);
        }

        public override bool Equals(object? obj)
        {
            return obj is Position other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static Position Zero = default;
        public static Position North = new(0, 1);
        public static Position East = new(1, 0);
        public static Position South = new(0, -1);
        public static Position West = new(-1, 0);
    }

    public static class PositionExtensions
    {
        public static int Distance(this Position a, Position b)
        {
            // 周囲8方向に移動可能としたマンハッタン距離
            return Math.Max(Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y));
        }
    }
}