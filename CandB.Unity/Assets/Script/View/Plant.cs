using R3;
using UnityEngine;

namespace CandB.Script.View
{
    public class Plant : CandBComponent
    {
        [SerializeField] private Animation _animation;

        public void Start()
        {
            EnvironmentService.OnChangePlantMoved
                .Subscribe( moved =>
                {
                    Debug.Log($"Plant moved: {moved}");
                    if (moved)
                    {
                        Debug.Log("Play plant animation");
                        _animation.Play("MovablePlant");
                    }
                })
                .AddTo(this);
        }
    }
}