using System.Threading.Tasks;
using LLMUnity;

namespace CandB.Script.Gateway.Impl
{
    public class LlmGateway : ILlmGateway
    {
        private readonly LLM _llm;
        private readonly LLMCharacter _llmCharacter;
        private bool _isChatProcessing = false;

        public LlmGateway(
            LLM llm,
            LLMCharacter llmCharacter
        )
        {
            _llm = llm;
            _llmCharacter = llmCharacter;
        }

        public bool IsChatProcessing => _isChatProcessing;

        public Task<string> Chat(string query, ILlmGateway.Callback<string> callback = null,
            ILlmGateway.EmptyCallback completionCallback = null, bool addToHistory = true)
        {
            _isChatProcessing = true;
            return _llmCharacter.Chat($"<|im_start|>user\n{query}<|im_end|>",
                callback == null ? null : callback.Invoke,
                () =>
                {
                    _isChatProcessing = false;
                    completionCallback?.Invoke();
                },
                addToHistory
            );
        }

        public void SetGrammar(string grammar)
        {
            _llmCharacter.grammarString = grammar;
        }

        public void ClearChat() => _llmCharacter.ClearChat();

        public void AddPlayerMessage(string content) => _llmCharacter.AddPlayerMessage(content);

        public void AddAIMessage(string content) => _llmCharacter.AddAIMessage(content);

        public void SetSystemPrompt(string prompt)
        {
            _llmCharacter.prompt = $"<|im_start|>system\n{prompt}<|im_end|>";
        }
    }
}