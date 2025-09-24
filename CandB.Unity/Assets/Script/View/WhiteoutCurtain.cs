using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using UnityEngine;

namespace CandB.Script.View
{
    public class WhiteoutCurtain : CandBComponent
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        private void Start()
        {
            EnvironmentService.OnChangeIsWhiteCurtainOpen
                .Subscribe(async  isWhiteCurtainOpen =>
                {
                    if (isWhiteCurtainOpen)
                    {
                        _canvasGroup.gameObject.SetActive(true);
                        _canvasGroup.DOFade(1f, 1f);
                    }
                    else
                    {
                        _canvasGroup.DOFade(0f, 1f);
                        await UniTask.WaitForSeconds(1f);
                        _canvasGroup.gameObject.SetActive(false);
                    }
                })
                .AddTo(this);
        }
    }
}