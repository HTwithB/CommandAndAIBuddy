using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using BuddyRotationType = CandB.Script.Core.StageState.BuddyRotationType;

namespace CandB.Script.Core
{
    public class StageService
    {
        public enum GridStatus
        {
            Empty,
            Obstacle
        }

        public enum MoveDirection
        {
            None = 0,
            Forward,
            Backward,
            Left,
            Right
        }

        private const char ObstacleChar = '#';

        private const char EmptyChar = '.';

        // ステージのグリッドマップ
        private const string GridMap = @"
######
###..#
####.#
##...#
#...##
###..#
###.##
###..#
#....#
######
";

        private readonly IList<IList<GridStatus>> _gridMap = new List<IList<GridStatus>>();

        private readonly ReactiveProperty<StageState> _state = new();
        private bool _isMoving;

        public ReadOnlyReactiveProperty<StageState> State => _state;

        public void Setup()
        {
            _state.Value = StageState.Default
                .WithBuddyPosition(new Position(3, 8))
                .WithBuddyRotation(BuddyRotationType.East)
                .WithCraftItemPosition(new Position(10, 20))
                .WithCarKeyPosition(new Position(10, 30))
                .WithShutterKeyPosition(new Position(10, 40))
                .WithBoxPosition(new Position(10, 50));

            LoadMap(GridMap);
            _isMoving = false;
        }

        public void LoadMap(string gridMap)
        {
            _gridMap.Clear();
            var lines = gridMap.Trim().Split('\n')?
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToArray();
            if (lines == null) return;

            for (var i = lines.Length - 1; i >= 0; i--)
            {
                var row = lines[i].Select(t => t switch
                    {
                        ObstacleChar => GridStatus.Obstacle,
                        /* EmptyChar or */ _ => GridStatus.Empty
                    })
                    .ToList();

                _gridMap.Add(row);
            }
        }

        private static BuddyRotationType TurnRotation(BuddyRotationType rotation,
            MoveDirection moveDirection)
        {
            return (rotation, moveDirection) switch
            {
                (BuddyRotationType.North, MoveDirection.Forward)
                    or (BuddyRotationType.East, MoveDirection.Left)
                    or (BuddyRotationType.South, MoveDirection.Backward)
                    or (BuddyRotationType.West, MoveDirection.Right) => BuddyRotationType.North,
                (BuddyRotationType.East, MoveDirection.Forward)
                    or (BuddyRotationType.South, MoveDirection.Left)
                    or (BuddyRotationType.West, MoveDirection.Backward)
                    or (BuddyRotationType.North, MoveDirection.Right) => BuddyRotationType.East,
                (BuddyRotationType.South, MoveDirection.Forward)
                    or (BuddyRotationType.West, MoveDirection.Left)
                    or (BuddyRotationType.North, MoveDirection.Backward)
                    or (BuddyRotationType.East, MoveDirection.Right) => BuddyRotationType.South,
                (BuddyRotationType.West, MoveDirection.Forward)
                    or (BuddyRotationType.North, MoveDirection.Left)
                    or (BuddyRotationType.East, MoveDirection.Backward)
                    or (BuddyRotationType.South, MoveDirection.Right) => BuddyRotationType.West,
                _ => default
            };
        }

        // Stageの状態を更新するメソッド群

        #region StateUpdate

        public void MoveBuddy(Position position)
        {
            _state.Value = _state.Value.WithBuddyPosition(position);
        }

        // バディを指定した方向に移動させる。ぶつかって移動できない場合はfalseを返す。
        // NOTE: 「goRight 0 => goStraight 1」のときは「右を向いて１歩進む」
        public async UniTask<bool> MoveBuddy(MoveDirection direction, int steps = 1)
        {
            if (_isMoving) await UniTask.WaitUntil(() => !_isMoving);

            var isSuccess = true;
            var nextRotation = TurnRotation(_state.Value.BuddyRotation, direction);
            var nextPosition = _state.Value.BuddyPosition;

            for (var i = 0; i < steps; i++)
            {
                var tmp = nextPosition + UnitVector(nextRotation);
                if (GetGridStatus(tmp) == GridStatus.Obstacle)
                {
                    isSuccess = false;
                    break;
                }

                nextPosition = tmp;
            }

            if (nextPosition == _state.Value.BuddyPosition && nextRotation == _state.Value.BuddyRotation)
                return isSuccess;

            _state.Value = _state.Value.WithBuddyPosition(nextPosition).WithBuddyRotation(nextRotation);

            // 向きを変えない
            if (nextRotation != _state.Value.BuddyRotation) _isMoving = true;

            return isSuccess;

            Position UnitVector(BuddyRotationType rotation)
            {
                return rotation switch
                {
                    BuddyRotationType.North => new Position(0, 1),
                    BuddyRotationType.East => new Position(1, 0),
                    BuddyRotationType.South => new Position(0, -1),
                    BuddyRotationType.West => new Position(-1, 0),
                    _ => Position.Zero
                };
            }
        }

        public void NotifyMoveCompleted()
        {
            _isMoving = false;
            Debug.LogFormat("StageService:NotifyMoveCompleted");
        }

        public void RotateBuddy(BuddyRotationType rotation)
        {
            _state.Value = _state.Value.WithBuddyRotation(rotation);
        }

        public void RotateBuddy(MoveDirection direction)
        {
            var next = TurnRotation(_state.Value.BuddyRotation, direction);
            _state.Value = _state.Value.WithBuddyRotation(next);
        }

        public void SetLight(bool isOn)
        {
            _state.Value = _state.Value.WithLightOn(isOn);
        }
        
        public void MovePlant()
        {
            _state.Value = _state.Value.WithPlantMoved(true);
        }

        public void MoveCraftItem(Position position)
        {
            _state.Value = _state.Value.WithCraftItemPosition(position);
        }

        public void MoveKeyPosition(Position position)
        {
            _state.Value = _state.Value.WithCarKeyPosition(position);
        }

        public void MoveShutterKeyPosition(Position position)
        {
            _state.Value = _state.Value.WithShutterKeyPosition(position);
        }

        public void MoveBoxPosition(Position position)
        {
            _state.Value = _state.Value.WithBoxPosition(position);
        }
        
        public void SetIsEnteringPassword(bool isEntering)
        {
            _state.Value = _state.Value.WithIsEnteringPassword(isEntering);
        }

        #endregion

        // グリッドマップの操作

        #region GridMapControl

        public struct GridStatusAround
        {
            public Position Position;
            public GridStatus Forward;
            public GridStatus Backward;
            public GridStatus Left;
            public GridStatus Right;
            public GridStatus ForwardLeft;
            public GridStatus ForwardRight;
            public GridStatus BackwardLeft;
            public GridStatus BackwardRight;
        }

        public GridStatus GetGridStatus(Position position)
        {
            if (position.Y < 0 || position.Y >= _gridMap.Count ||
                position.X < 0 || position.X >= _gridMap[position.Y].Count) return GridStatus.Obstacle;

            return _gridMap[position.Y][position.X];
        }

        public GridStatusAround GetGridStatusAround(Position position, BuddyRotationType rotation)
        {
            return new GridStatusAround
            {
                Position = position,
                Forward = GetGridStatus(PositionForward()),
                Backward = GetGridStatus(PositionBackward()),
                Left = GetGridStatus(PositionLeft()),
                Right = GetGridStatus(PositionRight()),
                ForwardLeft = GetGridStatus(PositionForwardLeft()),
                ForwardRight = GetGridStatus(PositionForwardRight()),
                BackwardLeft = GetGridStatus(PositionBackwardLeft()),
                BackwardRight = GetGridStatus(PositionBackwardRight())
            };

            Position PositionForward()
            {
                return position + ToPositionVector(TurnRotation(rotation, MoveDirection.Forward));
            }

            Position PositionBackward()
            {
                return position + ToPositionVector(TurnRotation(rotation, MoveDirection.Backward));
            }

            Position PositionLeft()
            {
                return position + ToPositionVector(TurnRotation(rotation, MoveDirection.Left));
            }

            Position PositionRight()
            {
                return position + ToPositionVector(TurnRotation(rotation, MoveDirection.Right));
            }

            Position PositionForwardLeft()
            {
                return PositionForward() + ToPositionVector(TurnRotation(rotation, MoveDirection.Left));
            }

            Position PositionForwardRight()
            {
                return PositionForward() + ToPositionVector(TurnRotation(rotation, MoveDirection.Right));
            }

            Position PositionBackwardLeft()
            {
                return PositionBackward() + ToPositionVector(TurnRotation(rotation, MoveDirection.Left));
            }

            Position PositionBackwardRight()
            {
                return PositionBackward() + ToPositionVector(TurnRotation(rotation, MoveDirection.Right));
            }

            Position ToPositionVector(BuddyRotationType rot)
            {
                return rot switch
                {
                    BuddyRotationType.North => Position.North,
                    BuddyRotationType.East => Position.East,
                    BuddyRotationType.South => Position.South,
                    BuddyRotationType.West => Position.West,
                    _ => Position.Zero
                };
            }
        }

        #endregion
    }
}