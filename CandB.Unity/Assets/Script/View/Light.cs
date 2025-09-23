using CandB.Script.Core;
using UnityEngine;
using R3;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace CandB.Script.View
{
    public class Light : CandBComponent
    {
        [SerializeField] private UnityEngine.Light[] _lights;

        private void Start()
        {
            EnvironmentService.OnChangeIsLightOn
                .Subscribe(isLightOn =>
                {
                    if (isLightOn)
                    {
                        foreach (var light in _lights)
                        {
                            light.DOIntensity(4f, 1f);

                        }
                    }
                    else
                    {
                        foreach (var light in _lights)
                        {
                            light.DOIntensity(0.5f, 1f);
                        }
                    }
                })
                .AddTo(this);

            EnvironmentService.SetLight(false);
        }
    }
}
