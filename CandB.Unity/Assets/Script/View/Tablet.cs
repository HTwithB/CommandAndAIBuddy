using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace CandB.Script.View
{
    public class Tablet : CandBComponent
    {
        [SerializeField] private Animation _tabletAnimation;

        private void Start()
        {
            EnvironmentService.OnChangeIsTableOpen
                .Subscribe(async isOpen =>
                {
                    Debug.Log($"Tablet is now {(isOpen ? "Open" : "Closed")}");
                    if (isOpen)
                    {
                        Debug.Log("Play TabletIn animation");
                        _tabletAnimation.gameObject.SetActive(true);
                        _tabletAnimation.Play("TabletIn");
                    }
                    else
                    {
                        Debug.Log("Play TabletOut animation");
                        _tabletAnimation.Play("TabletOut");
                        await UniTask.WaitForSeconds(0.5f);
                        _tabletAnimation.gameObject.SetActive(false);
                    }
                })
                .AddTo(this);
        }
    }
}