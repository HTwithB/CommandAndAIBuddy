using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CandB.Script.Gateway;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using JsonUtility = UnityEngine.JsonUtility;

namespace CandB.Script.Core
{
    [Serializable]
    public class MoveInstruction
    {
        public string instruction;
        public int steps;
    }

    [Serializable]
    public class MovePlan
    {
        public List<MoveInstruction> instructions;
        public string comment;

        public string ToInstructionsString()
        {
            return JsonUtility.ToJson(new MovePlan
            {
                instructions = instructions,
                comment = "",
            });
        }
    };

    [Serializable]
    public class CheckMovePlanResult
    {
        public bool result;
    }

    // ユーザーのテキスト入力を処理し、LLMやその他のサービスを使う
    public class UserPromptService
    {
        public enum Status
        {
            WaitingForInstruction = 0,
            WaitingForConfirmation,
            WaitingForPassword,
        }

        private readonly Subject<(string message, Color color)> _aiResponseSubject = new();
        private readonly Subject<(bool approved, string? feedback)> _approveMovePlanSubject = new();

        private readonly EnvironmentService _environmentService;
        private readonly BuddyActionService _buddyActionService;

        private readonly ILlmGateway _llmGateway;
        private readonly StageService _stageService;

        private readonly ReactiveProperty<Status> _status = new(Status.WaitingForInstruction);

        private readonly Subject<(string message, Color color)> _userPromptSubject = new();

        public UserPromptService(
            ILlmGateway llmGateway,
            EnvironmentService environmentService,
            BuddyActionService buddyActionService,
            StageService stageService
        )
        {
            _llmGateway = llmGateway;
            _environmentService = environmentService;
            _buddyActionService = buddyActionService;
            _stageService = stageService;
        }

        public ReadOnlyReactiveProperty<Status> CurrentStatus => _status;
        public Observable<(bool approved, string? feedback)> OnApproveMovePlan => _approveMovePlanSubject;

        // ユーザの入力をフィードバックする
        public Observable<(string message, Color color)> OnUserPrompt => _userPromptSubject;

        // AIの返答を変える
        public Observable<(string message, Color color)> OnAIResponse => _aiResponseSubject;

        private static string CreateResponseMovePlan(MovePlan? movePlan)
        {
            if (movePlan?.instructions == null || movePlan.instructions.Count == 0)
            {
                if (movePlan?.comment != null && movePlan.comment.Length > 0)
                {
                    return movePlan.comment;
                }

                return "指示がわかりませんでした。";
            }

            var response = new StringBuilder();
            response.AppendLine("次のように移動します。問題ないでしょうか？");
            foreach (var instruction in movePlan.instructions)
            {
                var jaDirection = instruction.instruction switch
                {
                    nameof(StageService.MoveDirection.Forward) => "前",
                    nameof(StageService.MoveDirection.Right) => "右",
                    nameof(StageService.MoveDirection.Left) => "左",
                    nameof(StageService.MoveDirection.Backward) => "後ろ",
                    _ => "不明な方向"
                };
                response.AppendLine($"- {jaDirection}に{instruction.steps}歩進みます。");
            }

            return response.ToString();
        }

        private static string CreateMovePlanningPrompt(string prompt, string? feedback = null)
        {
            var promptForMoving = $@"
## あなたの役割
あなたはAI（名前はバディ）で人間と会話しています。
自然言語を理解し、バディを移動させるための指示を出す役割を担っています。

## タスク
'ユーザーからの入力'を読み取り、箇条書きでバディの移動計画を整理し、その後 JSON 形式で出力してください。

### 移動計画の整理
整理する際の注意事項は以下の通りです。
- 移動指示は必ず具体的な方向（左、右、後ろ、前、正面など）とステップ数を含めること
- 歩数に関しては、'歩数の具体的な表現の解釈ルール' に従って解釈すること
- 複数の移動指示がある場合は、指示の順番を必ず守ること
- 移動に関する指示がない場合、'instructions' の値は空配列にすること
- 'ユーザーからの入力'に対してのあなたの気持ちを一言で表現して、'comment' キーの値として記載すること
- 移動方向は {nameof(StageService.MoveDirection.Forward)}: 正面, {nameof(StageService.MoveDirection.Right)}: 右, {nameof(StageService.MoveDirection.Left)}: 左, {nameof(StageService.MoveDirection.Backward)}: 後ろ を表す

歩数の具体的な表現の解釈ルール
- 2歩だけ進む
  - 少し
  - ちょっと
  - ほんの少し
  - 軽く
- 3歩進む
  - そのまま進んで
  - まっすぐ進んで
  - 進んで
- 5歩進む
  - かなり
  - そこそこ
  - しっかり
  - たくさん
- 0 歩進む
  - を向いて
  - 振り返って
  - 方向転換して
  - 向きを変えて
  - 立ち止まって
- 10歩進む
  - 突き当たりまで
  - 行けるところまで
  - 壁にぶつかるまで
  - 端まで

移動計画の整理は以下の順序で実行しなさい。
1. 'ユーザーからの入力'を元に、移動指示を分解する。
  a. 例）: 'まっすぐ突き当たりまで進んで、右に曲がってください。そのまま 3 歩進んだところで止まってください。'
    → 1. まっすぐ突き当たりまで進んで
    → 2. 右に曲がってください
    → 3. そのまま 3 歩進んだところで止まってください
  b. 例）: '助けて！'
    移動に関する指示がないため、分解できない。
  c. 例）: '左を向いて'
    → 1. 左を向いて
2. 分解した移動指示を箇条書きで整理する
  a. 例）: 'まっすぐ突き当たりまで進んで、右に曲がってください。そのまま 3 歩進んだところで止まってください。'
    → - 前に10歩進みます
    → - 右に0歩進みます
    → - 前に3歩進みます
  b. 例）: '助けて！'
    移動に関する指示がないため、整理できない。
  c. 例）: '左を向いて'
    → - 左に0歩進みます

### 出力形式
移動計画は必ず以下の要件を満たす JSON 形式のみを回答しなさい。
1. 'instructions' キーに対してオブジェクトの配列を値として持つオブジェクトです
2. 配列内のオブジェクトは、値に '{nameof(StageService.MoveDirection.Forward)}', '{nameof(StageService.MoveDirection.Right)}', '{nameof(StageService.MoveDirection.Left)}', '{nameof(StageService.MoveDirection.Backward)}' のどれかが入る 'instruction' フィールドを持つこと、また値に歩数を数値で持つ steps フィールドを持つこと
3. 'instructions' 配列は空であってもよいが、必ず存在すること

### 返答例

例1)
ユーザーからの入力: 'まっすぐ突き当たりまで進んで、右に曲がってください。そのまま 3 歩進んだところで止まってください。'
あなたの返答:
'{{""instructions"":[{{""instruction"": ""{nameof(StageService.MoveDirection.Forward)}"", ""steps"": 10}}, {{""instruction"": ""{nameof(StageService.MoveDirection.Right)}"", ""steps"": 0}}, {{""instruction"": ""{nameof(StageService.MoveDirection.Forward)}"", ""steps"": 3}}], ""comment"": ""頑張るぞー！""}}'

例2)
ユーザーからの入力: '左に行けるところまで進んでください。'
あなたの返答:
'{{""instructions"":[{{""instruction"": ""{nameof(StageService.MoveDirection.Left)}"", ""steps"": 10}}], ""comment"": ""頑張るぞー！""}}'

例3)
ユーザーからの入力: '後ろに2歩下がって、右を向いてください。そのまま突き当たりまで進んでください。さらに左に2歩進んでください。'
あなたの返答:
'{{""instructions"":[{{""instruction"": ""{nameof(StageService.MoveDirection.Backward)}"", ""steps"": 2}}, {{""instruction"": ""{nameof(StageService.MoveDirection.Right)}"", ""steps"": 0}}, {{""instruction"": ""{nameof(StageService.MoveDirection.Forward)}"", ""steps"": 10}}, {{""instruction"": ""{nameof(StageService.MoveDirection.Left)}"", ""steps"": 2}}], ""comment"": ""頑張るぞー！""}}'

例4)
ユーザーからの入力: '右を向いてしっかり進んでください。壁に突き当ったら左に行けるところまで進んでください。'
あなたの返答:
'{{""instructions"":[{{""instruction"": ""{nameof(StageService.MoveDirection.Right)}"", ""steps"": 5}}, {{""instruction"": ""{nameof(StageService.MoveDirection.Left)}"", ""steps"": 0}}], ""comment"": ""頑張るぞー！""}}'

例5)
ユーザーからの入力: '左を向いて'
あなたの返答:
'{{""instructions"":[{{""instruction"": ""{nameof(StageService.MoveDirection.Left)}"", ""steps"": 0}}], ""comment"": ""わかりました！""}}'

例6)
ユーザーからの入力: 'わーん、助けてー！'
あなたの返答:
'{{""instructions"":[], ""comment"": ""何かお困りですか？""}}'
解説:
STEP1) 移動指示を分解しようとしましたが、移動に関する指示が見つかりませんでした。
STEP2) 移動に関する指示がないため、'instructions' 配列は空のままとします。

## ユーザーからの入力
{prompt}
あなたの返答:
";
            var promptForFeedback = $@"
## あなたの役割
あなたはAI（名前はバディ）で人間と会話しています。
自然言語を理解し、バディを移動させるための指示を出す役割を担っています。

## タスク
'あなたが作成した間違った移動計画'と'ユーザーからのフィードバック'を読み取り、バディの移動計画を修正し、その後 JSON 形式で出力してください。

### 出力形式
移動計画は必ず以下の要件を満たす JSON 形式のみを回答しなさい。
1. 'instructions' キーに対してオブジェクトの配列を値として持つオブジェクトです
2. 配列内のオブジェクトは、値に '{nameof(StageService.MoveDirection.Forward)}', '{nameof(StageService.MoveDirection.Right)}', '{nameof(StageService.MoveDirection.Left)}', '{nameof(StageService.MoveDirection.Backward)}' のどれかが入る 'instruction' フィールドを持つこと、また値に歩数を数値で持つ steps フィールドを持つこと
3. 'instructions' 配列は空であってもよいが、必ず存在すること

例1)
移動計画:
前に1歩進みます
'{{""instructions"":[{{""instruction"": ""{nameof(StageService.MoveDirection.Forward)}"", ""steps"": 1}}], ""comment"": """"}}'

例2)
移動計画:
右に10歩進みます
'{{""instructions"":[{{""instruction"": ""{nameof(StageService.MoveDirection.Right)}"", ""steps"": 10}}], ""comment"": """"}}'

例3)
移動計画:
左に0歩進みます or 左を向きます
'{{""instructions"":[{{""instruction"": ""{nameof(StageService.MoveDirection.Left)}"", ""steps"": 0}}], ""comment"": """"}}'

例4)
移動計画:
右に0歩進みます or 右を向きます
'{{""instructions"":[{{""instruction"": ""{nameof(StageService.MoveDirection.Right)}"", ""steps"": 0}}], ""comment"": """"}}'

例5)
移動計画:
後ろに0歩進みます or 後ろを向きます
'{{""instructions"":[{{""instruction"": ""{nameof(StageService.MoveDirection.Backward)}"", ""steps"": 0}}], ""comment"": """"}}'

{feedback ?? ""}

## あなたが修正した移動計画の出力:
";

            // feedback があればそれも含める
            var ret = feedback != null ? promptForFeedback : promptForMoving;
            Debug.LogFormat("UserPromptService:CreateMovePlanningPrompt: {0}", ret);
            return ret;
        }

        public void Setup()
        {
            _llmGateway.SetSystemPrompt("");
        }

        public void ProcessUserPrompt(string prompt)
        {
            DisplayUserPrompt(prompt);
            ProcessAsync().Forget();
            return;

            async UniTaskVoid ProcessAsync()
            {
                if (_status.CurrentValue != Status.WaitingForInstruction || _llmGateway.IsChatProcessing)
                {
                    Debug.LogFormat("UserPromptService:ProcessUserPrompt: invalid state {0}, IsChatProcessing: {1}",
                        _status.CurrentValue, _llmGateway.IsChatProcessing);
                    return;
                }

                // move plan を生成
                var movePlan = await CreateMovePlan(prompt);
                var responseMessage = CreateResponseMovePlan(movePlan);
                Debug.LogFormat("UserPromptService:ProcessUserPrompt: move planning response {0}",
                    responseMessage);
                // ユーザーに move plan を提示
                DisplayAIResponse(responseMessage);
                if (movePlan == null)
                {
                    DisplayAIResponse("指示がわかりませんでした。もう一度お願いします。", Color.red);
                    return;
                }

                // ユーザーに確認を求めるためのループ
                while (true)
                {
                    // ユーザーの確認を待つ
                    ChangeUserPromptPlaceholder(Placeholder.EnterFeedback);
                    _status.Value = Status.WaitingForConfirmation;
                    var result = await OnApproveMovePlan.Do(x => Debug.LogFormat(
                        "UserPromptService:ProcessUserPrompt: move plan approved: {0}, feedback: {1}",
                        x.approved, x.feedback ?? "値なし")).FirstAsync();
                    if (result.approved)
                    {
                        ChangeUserPromptPlaceholder(Placeholder.EnterInstruction);
                        // 承認されたのでループを抜けて実行へ
                        Debug.Log("UserPromptService:ProcessUserPrompt: move plan approved");
                        break;
                    }
                    else
                    {
                        // feedback を元に move plan を再生成し、再度ユーザー確認へ
                        string? feedbackText = (result.feedback != null)
                            ? $@"
## あなたが作成した間違った移動計画
{responseMessage.Split('\n', StringSplitOptions.RemoveEmptyEntries)
    .Skip(1)
    .Aggregate((a, b) => a + "\n" + b)}
{(movePlan != null ? movePlan.ToInstructionsString() : string.Empty)}

## ユーザーからのフィードバック
{result.feedback}

必ず 'あなたが作成した間違った移動計画' を 'ユーザーからのフィードバック' を元に必ず修正して出力してください。"
                            : null;
                        Debug.LogFormat("UserPromptService:ProcessUserPrompt: recreate move plan with feedback {0}",
                            feedbackText);
                        movePlan = await CreateMovePlan(prompt, feedbackText);
                        responseMessage = CreateResponseMovePlan(movePlan);
                        Debug.LogFormat("UserPromptService:ProcessUserPrompt: move planning response {0}",
                            responseMessage);
                        DisplayAIResponse(responseMessage);
                        continue;
                    }
                }

                _status.Value = Status.WaitingForInstruction;

                if (movePlan == null)
                {
                    DisplayAIResponse("指示がわかりませんでした。もう一度お願いします。", Color.red);
                    return;
                }

                // move plan を実行
                await MoveByInstructions(movePlan);
            }
        }

        public void ProcessPasswordInput(string password)
        {
            // TODO: Validation追加(状態を持つ場所が微妙)

            DisplayUserPrompt(password, Color.yellow);

            if (password.Contains(StageState.BoxPassword) || password.Contains(StageState.BoxPassword2))
            {
                DisplayAIResponse("ロックを解除できました。ドアを開けるボタンと車の鍵が入っているようです。");

                // TODO: シャッターの開く音を鳴らす

                _environmentService.UnlockEnvironment();
            }
            else
            {
                DisplayAIResponse("パスワードが間違っています。");
                _environmentService.SetIsEnteringPassword(false);
            }
        }

        public void ProcessFeedbackInput(string prompt)
        {
            if (_status.CurrentValue != Status.WaitingForConfirmation || _llmGateway.IsChatProcessing)
            {
                Debug.LogFormat("UserPromptService:ProcessFeedbackInput: invalid state {0}, IsChatProcessing: {1}",
                    _status.CurrentValue, _llmGateway.IsChatProcessing);
                return;
            }

            DisplayUserPrompt(prompt);
            ProcessAsync().Forget();
            return;

            async UniTaskVoid ProcessAsync()
            {
                (bool approved, string? feedback) result;
                var approveWords = new List<string> { "OK", "ok", "Ok", "oK", "問題ない", "大丈夫", "いいよ", "いいです", "はい" };
                if (approveWords.Any(prompt.Contains))
                {
                    // 承認して、実行へ
                    result.approved = true;
                    result.feedback = null;
                    Debug.Log("UserPromptService:ProcessUserPrompt: move plan approved");
                    _approveMovePlanSubject.OnNext(result);
                    // 実行待機状態にする
                    _status.Value = Status.WaitingForInstruction;
                    return;
                }

                Debug.LogFormat("UserPromptService:ProcessUserPrompt: recreate move plan with feedback: {0}", prompt);
                DisplayUserPrompt(prompt);
                // feedback を ProcessUserPrompt 内に投げる
                result.approved = false;
                result.feedback = prompt;
                _approveMovePlanSubject.OnNext(result);
                return;
            }
        }

        private async UniTask MoveByInstructions(MovePlan movePlan)
        {
            foreach (var item in movePlan.instructions)
            {
                var moveDirection = StageService.MoveDirection.None;
                // move buddy
                switch (item.instruction)
                {
                    case nameof(StageService.MoveDirection.Forward):
                        moveDirection = StageService.MoveDirection.Forward;
                        break;
                    case nameof(StageService.MoveDirection.Right):
                        moveDirection = StageService.MoveDirection.Right;
                        break;
                    case nameof(StageService.MoveDirection.Left):
                        moveDirection = StageService.MoveDirection.Left;
                        break;
                    case nameof(StageService.MoveDirection.Backward):
                        moveDirection = StageService.MoveDirection.Backward;
                        break;
                    default:
                        Debug.LogWarningFormat("UserPromptService:ProcessUserPrompt: unknown instruction {0}",
                            item.instruction);
                        continue;
                }

                await _stageService.MoveBuddy(moveDirection, item.steps);
            }
        }

        private async UniTask<MovePlan?> CreateMovePlan(string prompt, string? feedback = null)
        {
            // generate movement
            _llmGateway.SetSystemPrompt("");
            _llmGateway.SetGrammar(
                $@"
root ::= ""{{\""instructions\"": ["" ( instruction ("","" instruction){{0,4}} )? ""] , \""comment\"":\"""" comment-string ""\"" }}""
instruction ::= ""{{\""instruction\"":"" instruction-val "", \""steps\"":"" number ""}}""
instruction-val ::= (""\""{nameof(StageService.MoveDirection.Forward)}\"""" | ""\""{nameof(StageService.MoveDirection.Right)}\"""" | ""\""{nameof(StageService.MoveDirection.Left)}\"""" | ""\""{nameof(StageService.MoveDirection.Backward)}\"""")
number ::= [0-9]+
comment-string ::= [^\n}}""\\<>]{{0,0}}
");
            var movePlanningText = "";
            await _llmGateway.Chat(CreateMovePlanningPrompt(prompt, feedback),
                ret => { movePlanningText = ret; }, null, false);
            // recieve move planning
            Debug.LogFormat("UserPromptService:ProcessUserPrompt: move planning response {0}",
                movePlanningText);
            MovePlan? movePlan;
            try
            {
                movePlan = JsonUtility.FromJson<MovePlan>(movePlanningText);
            }
            catch (Exception _)
            {
                Debug.LogWarning(
                    "UserPromptService:ProcessUserPrompt: move planning response parsing failed");
                return null;
            }

            return movePlan;
        }

        private void DisplayAIResponse(string response, Color? color = null)
        {
            color ??= Color.white;
            _aiResponseSubject.OnNext((response, color.Value));
        }

        private void DisplayUserPrompt(string prompt, Color? color = null)
        {
            color ??= Color.cyan;
            _userPromptSubject.OnNext((prompt, color.Value));
        }

        private enum Placeholder
        {
            [StringValue("バディへの指示を入力してください。")] EnterInstruction,

            [StringValue("「OK」または必要な場合、フィードバックを入力してください。")]
            EnterFeedback,
            [StringValue("パスワードを入力してください。")] EnterPassword
        }

        private void ChangeUserPromptPlaceholder(Placeholder placeholder)
        {
            _environmentService.SetUserPromptPlaceholder(placeholder.GetStringValue());
        }

        private enum Functions
        {
            MoveTo,
            ResponseTo
        }
    }

    public class StringValueAttribute : Attribute
    {
        public string StringValue { get; protected set; }

        public StringValueAttribute(string value)
        {
            this.StringValue = value;
        }
    }

    public static class CommonAttribute
    {
        public static string GetStringValue(this Enum value)
        {
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            if (fieldInfo == null) return string.Empty;
            return fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) is StringValueAttribute[]
            {
                Length: > 0
            } attribs
                ? attribs[0].StringValue
                : string.Empty;
        }
    }
}