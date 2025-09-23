using System.Threading.Tasks;

namespace CandB.Script.Gateway
{
    public interface ILlmGateway
    {
        public delegate void EmptyCallback();

        public delegate void Callback<T>(T message);

        public bool IsChatProcessing { get; }

        public Task<string> Chat(string query, Callback<string>? callback = null,
            EmptyCallback? completionCallback = null, bool addToHistory = true);

        public void SetGrammar(string grammar);

        public void ClearChat();

        public void AddPlayerMessage(string content);

        public void AddAIMessage(string content);

        public void SetSystemPrompt(string prompt);
    }
}