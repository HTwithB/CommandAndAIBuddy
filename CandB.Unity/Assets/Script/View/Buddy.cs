using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;
using R3;
using CandB.Script.Core;
using DG.Tweening;
using System;

namespace CandB.Script.View
{
    public class Buddy : CandBComponent
    {
        [SerializeField]
        private NavMeshAgent _agent;
        [SerializeField] private Transform _robotTargetTransform;

        [SerializeField]
        private Animator _animator;

        // 移動中かどうか(暫定的に)
        private bool _isMoving = false;

        // 新規の依頼が来た場合は、過去のTweenをキャンセルするための参照を保持
        private DG.Tweening.Core.TweenerCore<Quaternion, Vector3, DG.Tweening.Plugins.Options.QuaternionOptions> _rotationTweener;

        private void Start()
        {
            EnvironmentService.OnChangeBuddyPosition
                .Subscribe(position =>
                {
                    Debug.Log($"Move Buddy to {position}");
                    // NavMeshAgent で移動する
                    _robotTargetTransform.transform.position = PositionUtil.ToViewPosition(position);
                    _agent.SetDestination(_robotTargetTransform.transform.position);
                })
                .AddTo(this);

            EnvironmentService.OnChangeBuddyRotation
                .Subscribe(rotation =>
                {
                    _rotationTweener?.Kill();
                    _rotationTweener = _agent.transform.DORotate(new Vector3(
                        _agent.transform.rotation.eulerAngles.x,
                        (float)rotation,
                        _agent.transform.rotation.eulerAngles.z),
                        0.3f);

                })
                .AddTo(this);

            _rotationTweener = _agent.transform.DORotate(new Vector3(
                   _agent.transform.rotation.eulerAngles.x,
                   (float)EnvironmentService.Stage.CurrentValue.BuddyRotation,
                   _agent.transform.rotation.eulerAngles.z),
                   0.3f);

            _robotTargetTransform.transform.position = PositionUtil.ToViewPosition(EnvironmentService.Stage.CurrentValue.BuddyPosition);
            _agent.SetDestination(_robotTargetTransform.transform.position);
        }

        private void Update()
        {
        
            // for debug
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Debug.Log("Move Forward");
                var service = Resolve<StageService>();
                service.MoveBuddy(StageService.MoveDirection.Forward, 1).Forget();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Debug.Log("Move Backward");
                var service = Resolve<StageService>();
                service.RotateBuddy(StageService.MoveDirection.Backward);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Debug.Log("Move Left");
                var service = Resolve<StageService>();
                service.RotateBuddy(StageService.MoveDirection.Left);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Debug.Log("Move Right");
                var service = Resolve<StageService>();
                service.RotateBuddy(StageService.MoveDirection.Right);
            }
            if (Input.GetKeyDown(KeyCode.RightCommand))
            {
                Debug.Log("Action");
                var service = Resolve<StageService>();
                // 何かしらactionを実行する
            }
#endif
            
            // Controlでアクションを起こす
            // 一旦雑にここに書く
            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
            {
                Debug.Log("Action");
                var service = Resolve<BuddyActionService>();
                service.DoAction().Forget();
            }

            // 速度をみてアニメーション変更
            var speed = _agent.velocity.magnitude;
            _animator.SetFloat("Speed", speed);

            if (_agent.velocity.sqrMagnitude > 0.00001f)
            {
                _isMoving = true;
            }
            else
            {
                if (_isMoving && !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
                {

                    // 停止したときの処理をここに記述
                    Debug.Log("NavMeshAgent が停止しました！");
                    _isMoving = false;
                    Resolve<StageService>().NotifyMoveCompleted();

                    _agent.transform.DORotate(new Vector3(
                    _agent.transform.rotation.eulerAngles.x,
                    (float)EnvironmentService.Stage.CurrentValue.BuddyRotation,
                    _agent.transform.rotation.eulerAngles.z),
                    0.5f,
                    RotateMode.FastBeyond360);
                }
            }
        }
    }
}