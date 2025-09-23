using CandB.Script.Core;
using Febucci.UI.Core;
using R3;
using UnityEngine;

namespace CandB.Script.View
{
    public class OutputText : CandBComponent
    {
        private static readonly Vector3 OutputTextBottom = new(0, 100);
        private static readonly Vector3 OutputTextTop = new(0, 225);

        [SerializeField] private RectTransform _textOutputTransform;
        [SerializeField] private TypewriterCore _typewriter;

        private const float DisplayDuration = 5;

        private float? _currentDisplayDuration = null;
        public bool IsDisplaying => _currentDisplayDuration != null;

        private void Start()
        {
            var userPromptService = Resolve<UserPromptService>();
            userPromptService.OnAIResponse
                .ObserveOnMainThread()
                .Subscribe(response =>
                {
                    Debug.Log($"LLM Response: {response.message}");
                    EnvironmentService.PlaySound(SoundEffectId.Text);
                    _typewriter.ShowText($"<color=#{ToHex(response.color)}>{response.message}</color>");

                    // AIの場合は画面から消さないようにする
                    _currentDisplayDuration = null;
                });
            userPromptService.OnUserPrompt
                .ObserveOnMainThread()
                .Subscribe(prompt =>
                {
                    EnvironmentService.PlaySound(SoundEffectId.Text);
                    _typewriter.ShowText($"<color=#{ToHex(prompt.color)}>{prompt.message}</color>");

                    _currentDisplayDuration = 0;
                });

            var buddyActionService = Resolve<BuddyActionService>();
            buddyActionService.OnActionLog
                .ObserveOnMainThread()
                .Subscribe(log =>
                {
                    Debug.Log($"Action Log: {log}");
                    _typewriter.ShowText(log);
                    _currentDisplayDuration = 0;
                });

            EnvironmentService.OnChangeIsPromptOpen
                .ObserveOnMainThread()
                .Subscribe(isOpen =>
                {
                    if (isOpen)
                        _textOutputTransform.anchoredPosition = OutputTextTop;
                    else
                        _textOutputTransform.anchoredPosition = OutputTextBottom;
                })
                .AddTo(this);

            _typewriter.onTextShowed.AddListener(() => { EnvironmentService.StopSound(SoundEffectId.Text); });

            InitGameObject();
        }

        private void InitGameObject()
        {
            _textOutputTransform.anchoredPosition = OutputTextBottom;
        }

        private void Update()
        {
            if (_currentDisplayDuration != null)
            {
                _currentDisplayDuration += Time.deltaTime;

                if (_currentDisplayDuration > DisplayDuration)
                {
                    _typewriter.ShowText("");
                    _currentDisplayDuration = null;
                }
            }
        }

        private static string ToHex(Color32 color, bool includeAlpha = false)
        {
            var hex = color.r.ToString("x2") + color.g.ToString("x2") + color.b.ToString("x2");
            if (includeAlpha)
                hex += color.a.ToString("x2");
            return hex;
        }
    }
}