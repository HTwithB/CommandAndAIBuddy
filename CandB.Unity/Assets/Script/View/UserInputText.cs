using DG.Tweening;
using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;
using R3.Triggers;
using R3;
using CandB.Script.Core;

namespace CandB.Script.View
{
    public class UserInputText : CandBComponent
    {
        [SerializeField] private TMP_InputField _textInput;
        [SerializeField] private CanvasGroup _canvasGroup;

        private async void Start()
        {
            var service = Resolve<UserPromptService>();

            _canvasGroup.alpha = 0;
            _canvasGroup.gameObject.SetActive(false);
            _textInput.text = "";

            // 普通のプロンプト入力
            _textInput.OnSubmitAsObservable()
                .Where(_ => !EnvironmentService.Stage.CurrentValue.IsEnteringPassword &&
                            service.CurrentStatus.CurrentValue != UserPromptService.Status.WaitingForConfirmation)
                .Subscribe(_ =>
                {
                    var inputText = _textInput.text;
                    Debug.Log($"Input Text: {inputText}");
                    EnvironmentService.PlaySound(SoundEffectId.Ok);
                    service.ProcessUserPrompt(inputText);
                    _textInput.text = "";
                })
                .AddTo(this);

            // フィードバックを入力
            _textInput.OnSubmitAsObservable()
                .Where(_ => service.CurrentStatus.CurrentValue == UserPromptService.Status.WaitingForConfirmation)
                .Subscribe(_ =>
                {
                    var inputText = _textInput.text;
                    Debug.Log($"Input Feedback: {inputText}");
                    EnvironmentService.PlaySound(SoundEffectId.Ok);
                    service.ProcessFeedbackInput(inputText);
                    _textInput.text = "";
                })
                .AddTo(this);

            // パスワード入力
            _textInput.OnSubmitAsObservable()
                .Where(_ => EnvironmentService.Stage.CurrentValue.IsEnteringPassword)
                .Subscribe(_ =>
                {
                    var inputText = _textInput.text;
                    Debug.Log($"Input Password: {inputText}");
                    EnvironmentService.PlaySound(SoundEffectId.Ok);
                    service.ProcessPasswordInput(inputText);
                    _textInput.text = "";
                })
                .AddTo(this);

            EnvironmentService.OnChangeIsPromptOpen
                .ObserveOnMainThread()
                .Subscribe(isOpen =>
                {
                    if (isOpen)
                    {
                        _canvasGroup.gameObject.SetActive(true);
                        _canvasGroup.DOFade(1, 0.1f)
                            .OnComplete(() => _textInput.ActivateInputField());
                    }
                    else
                    {
                        _canvasGroup.DOFade(0, 0.1f)
                            .OnComplete(() => _canvasGroup.gameObject.SetActive(false));
                    }
                })
                .AddTo(this);

            EnvironmentService.OnChangePromptPlaceholder
                .ObserveOnMainThread()
                .Subscribe(placeholder => { _textInput.placeholder.GetComponent<TMP_Text>().text = placeholder; })
                .AddTo(this);
        }

        private void Update()
        {
            // 本番でも Tab キーで開閉する
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                EnvironmentService.PlaySound(SoundEffectId.Ok);

                if (EnvironmentService.UI.CurrentValue.IsPromptOpen)
                {
                    Debug.Log("Close Tablet");
                    EnvironmentService.ClosePrompt();
                    return;
                }

                Debug.Log("Open Tablet");
                EnvironmentService.OpenPrompt();
            }
        }
    }
}