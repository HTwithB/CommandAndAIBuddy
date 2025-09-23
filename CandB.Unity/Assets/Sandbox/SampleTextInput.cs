using DG.Tweening;
using TMPro;
using UnityEngine;
using Febucci.UI.Core;
using Cysharp.Threading.Tasks;
using R3.Triggers;
using R3;

public class SampleTextInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField _textInput;
    [SerializeField] private CanvasGroup _canvasGroup;

    [SerializeField] private RectTransform _textOutputTransform;
    [SerializeField] private TypewriterCore _typewriter;

    private static readonly Vector3 OutputTextBottom = new(0, 100);
    private static readonly Vector3 OutputTextTop = new(0, 225);

    private bool _isOpen = false;

    private async void Start()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.gameObject.SetActive(false);
        _textInput.text = "";

        _textOutputTransform.anchoredPosition = OutputTextBottom;
        await UniTask.WaitForSeconds(3);
        _typewriter.ShowText("何か指示を出してください。");

        _textInput.OnSubmitAsObservable()
            .Subscribe(async _ =>
            {
                var inputText = _textInput.text;
                Debug.Log($"Input Text: {inputText}");
                _typewriter.ShowText($"<color=#ADD8E6>{inputText}</color>");
                _textInput.text = "";
                await UniTask.WaitForSeconds(3f);
                _typewriter.ShowText("何を言っているのか分かりません。");
            })
            .AddTo(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _isOpen = !_isOpen;
            Debug.Log($"Text Input is {(_isOpen ? "Open" : "Closed")}");

            if (_isOpen)
            {
                _textInput.text = "";
                _textOutputTransform.anchoredPosition = OutputTextTop;
                _canvasGroup.gameObject.SetActive(true);
                _canvasGroup.DOFade(1, 0.1f);
            }
            else
            {
                _canvasGroup.DOFade(0, 0.1f).OnComplete(() =>
                {
                    _canvasGroup.gameObject.SetActive(false);
                    _textOutputTransform.anchoredPosition = OutputTextBottom;
                });
            }
        }
    }
}
