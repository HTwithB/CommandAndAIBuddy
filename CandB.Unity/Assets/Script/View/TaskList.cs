using TMPro;
using UnityEngine;
using R3;

namespace CandB.Script.View
{
    public class TaskList : CandBComponent
    {
        [SerializeField] private TextMeshProUGUI _text;

        public string CreateText()
        {
            return $@"タスクリスト
[{(EnvironmentService.Achievement.CurrentValue.WasLightTurnedOn ? "x" : " ")}] 部屋を明るくする
[{(EnvironmentService.Achievement.CurrentValue.WasBuddyOpenedShutter ? "x" : " ")}] 部屋からの脱出方法を探す
[{(EnvironmentService.Achievement.CurrentValue.WasBuddyGotCageKey ? "x" : " ")}] 檻の鍵を見つける
[{(EnvironmentService.Achievement.CurrentValue.WasBuddyEscapedFromCage ? "x" : " ")}] 檻から脱出する";
        }
        
        private void Start()
        {
            EnvironmentService.Achievement
                .Subscribe(x =>
                {
                    _text.text = CreateText();
                })
                .AddTo(this);
            
            _text.text = CreateText();
        }
    }
}