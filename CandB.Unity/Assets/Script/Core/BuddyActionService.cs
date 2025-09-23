using System.Collections.Generic;
using CandB.Script.Gateway;
using Cysharp.Threading.Tasks;
using R3;

namespace CandB.Script.Core
{
    public class BuddyActionService
    {
        public enum DoneAction
        {
            None = 0,
            LightOn,
            TriedDoorOpen,
            TriedMovePlant,
            TriedLookTable,
            TriedLookShelf,
            TriedLookKeyBox,
            TriedOpenCarDoor,
            TriedOpenCageDoor,
        }

        private readonly Subject<string> _actionLogSubject = new();

        private readonly EnvironmentService _environment;

        private readonly ILlmGateway _llmGateway;
        private readonly StageService _stage;

        public BuddyActionService(
            EnvironmentService environment,
            StageService stage,
            ILlmGateway llmGateway
        )
        {
            _environment = environment;
            _stage = stage;
            _llmGateway = llmGateway;
        }

        public Observable<string> OnActionLog => _actionLogSubject;

        // バディが現在位置で可能な行動をする
        public async UniTask<IList<DoneAction>> DoAction()
        {
            var result = new List<DoneAction>();
            if (TryLightOn()) result.Add(DoneAction.LightOn);
            if (TryDoorOpen()) result.Add(DoneAction.TriedDoorOpen);
            if (await TryMovePlant()) result.Add(DoneAction.TriedMovePlant);
            if (TryLookTable()) result.Add(DoneAction.TriedLookTable);
            if (TryLookShelf()) result.Add(DoneAction.TriedLookShelf);
            if (TryLookKeyBox()) result.Add(DoneAction.TriedLookKeyBox);
            if (await TryOpenCarDoor()) result.Add(DoneAction.TriedOpenCarDoor);
            if (TryOpenCageDoor()) result.Add(DoneAction.TriedOpenCageDoor);

            return result;

            bool TryLightOn()
            {
                if (_environment.Stage.CurrentValue.BuddyPosition != new Position(3, 8)) return false;
                if (_environment.Stage.CurrentValue.BuddyRotation != StageState.BuddyRotationType.North) return false;
                if (_environment.Achievement.CurrentValue.WasLightTurnedOn) return false;
                
                _environment.SetLight(true);
                _actionLogSubject.OnNext("部屋の電気をつけました。これでよく見えますね。");
                //GenerateActionResponse("あなたはスイッチを入れて部屋の電気をつけました。15文字以内で返事を考えて出力してください。").Forget();
                return true;
            }

            bool TryDoorOpen()
            {
                if (_environment.Stage.CurrentValue.BuddyPosition != new Position(4, 8)) return false;
                if (_environment.Stage.CurrentValue.BuddyRotation != StageState.BuddyRotationType.East) return false;
                _environment.PlaySound(SoundEffectId.NotOpenDoor);
                _actionLogSubject.OnNext("ドアは鍵がかかっていて開きません。");
                //GenerateActionResponse("あなたはドアを開けようとしましたが、鍵がかかっていて開きません。15文字以内で返事を考えて出力してください。").Forget();
                return true;
            }

            async UniTask<bool> TryMovePlant()
            {
                if ((_environment.Stage.CurrentValue.BuddyPosition == new Position(2, 6) &&
                     _environment.Stage.CurrentValue.BuddyRotation == StageState.BuddyRotationType.West)
                    ||
                    (_environment.Stage.CurrentValue.BuddyPosition == new Position(1, 5) &&
                     _environment.Stage.CurrentValue.BuddyRotation == StageState.BuddyRotationType.North)
                   )
                {
                    if (!_environment.Achievement.CurrentValue.WasPlantMoved)
                    {
                        _environment.OpenBlackCurtain();
                        await UniTask.WaitForSeconds(1.0f);
                        _environment.MovePlant();
                        _environment.PlaySound(SoundEffectId.Paper);
                        await UniTask.WaitForSeconds(1.0f);
                        _environment.CloseBlackCurtain();
                    }
                   
                    _actionLogSubject.OnNext("植物の下にメモが落ちていました。「1234」という文字が書かれているようです。");
                    return true;
                }

                return false;
            }

            bool TryLookTable()
            {
                if (_environment.Stage.CurrentValue.BuddyPosition == new Position(3, 5) &&
                    _environment.Stage.CurrentValue.BuddyRotation == StageState.BuddyRotationType.East
                   )
                {
                    _actionLogSubject.OnNext("テーブルの上に夫婦の写真があるみたいです。");
                    return true;
                }

                return false;
            }

            bool TryLookShelf()
            {
                if (_environment.Stage.CurrentValue.BuddyPosition == new Position(3, 3) &&
                    _environment.Stage.CurrentValue.BuddyRotation == StageState.BuddyRotationType.East
                   )
                {
                    _actionLogSubject.OnNext("特に何もないようですね。");
                    return true;
                }

                return false;
            }

            bool TryLookKeyBox()
            {
                if (_environment.Achievement.CurrentValue.WasBuddyGotCarKey) return false;
                
                if (_environment.Stage.CurrentValue.BuddyPosition == new Position(4, 1) &&
                    _environment.Stage.CurrentValue.BuddyRotation == StageState.BuddyRotationType.South
                   )
                {
                    _environment.SetIsEnteringPassword(true);
                    _actionLogSubject.OnNext("鍵付きの箱があります。パスワードを入力すれば開けられるようです。");

                    return true;
                }

                return false;
            }

            async UniTask<bool> TryOpenCarDoor()
            {
                if (_environment.Achievement.CurrentValue.WasBuddyGotCageKey) return false;
                if (_environment.Stage.CurrentValue.BuddyPosition == new Position(3, 3) &&
                    _environment.Stage.CurrentValue.BuddyRotation == StageState.BuddyRotationType.West
                   )
                {
                    if (_environment.Achievement.CurrentValue.WasBuddyGotCarKey)
                    {
                        _actionLogSubject.OnNext("扉を開けます。");
                        _environment.OpenBlackCurtain();
                        await UniTask.WaitForSeconds(1.5f);

                        _environment.PlaySound(SoundEffectId.OpenCarDoor);

                        await UniTask.WaitForSeconds(1.5f);

                        _environment.PlaySound(SoundEffectId.CloseCarDoor);

                        await UniTask.WaitForSeconds(1.0f);

                        _environment.CloseBlackCurtain();
                        _environment.GetCageKey();
                        _actionLogSubject.OnNext("檻の鍵を手に入れました。檻から脱出して逃げましょう！");

                        return true;
                    }
                    else
                    {
                        _environment.PlaySound(SoundEffectId.NotOpenDoor);
                        _actionLogSubject.OnNext("鍵がかかっていて開きません。");
                        return true;
                    }
                }

                return false;
            }

            bool TryOpenCageDoor()
            {
                if (_environment.Achievement.CurrentValue.WasBuddyEscapedFromCage) return false;
                if (_environment.Stage.CurrentValue.BuddyPosition != new Position(3, 8)) return false;
                if (_environment.Stage.CurrentValue.BuddyRotation != StageState.BuddyRotationType.West) return false;

                if (_environment.Achievement.CurrentValue.WasBuddyGotCageKey)
                {
                    _environment.EscapeFromCage();
                    _environment.PlaySound(SoundEffectId.UnlockKey);
                    _actionLogSubject.OnNext("檻の鍵が開きました。脱出しましょう！");
                }
                else
                {
                    _environment.PlaySound(SoundEffectId.NotOpenDoor);
                    _actionLogSubject.OnNext("鍵がかかっていて開きません。");
                }

                return true;
            }

            async UniTaskVoid GenerateActionResponse(string prompt)
            {
                _llmGateway.SetSystemPrompt(
                    "あなたはAI（名前はバディ）で人間と会話しています。和やかに和気あいあいと会話してください。"
                );
                _llmGateway.SetGrammar("");
                await _llmGateway.Chat(prompt, ret => _actionLogSubject.OnNext(ret));
            }
        }
    }
}