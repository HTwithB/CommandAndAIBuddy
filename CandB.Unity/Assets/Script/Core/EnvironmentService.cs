using System;
using CandB.Script.Gateway;
using R3;
using UnityEngine;

namespace CandB.Script.Core
{
    public class EnvironmentService
    {
        public static readonly Position BoxMovedToShutterPosition = Position.Zero;
        private readonly ReactiveProperty<AchievementState> _achievementState = new();

        private readonly IAudioGateway _audioGateway;
        private readonly StageService _stageService;

        private readonly ReactiveProperty<UIState> _uiState = new();

        public EnvironmentService(
            IAudioGateway audioGateway,
            StageService stageService
        )
        {
            _audioGateway = audioGateway;
            _stageService = stageService;
        }

        public ReadOnlyReactiveProperty<UIState> UI => _uiState;
        public Observable<bool> OnChangeIsTableOpen => _uiState.Only(state => state.IsTabletOpen);
        public Observable<bool> OnChangeIsPromptOpen => _uiState.Only(state => state.IsPromptOpen);
        public Observable<bool> OnChangeIsBlackCurtainOpen => _uiState.Only(state => state.IsBlackCurtainOpen);
        public Observable<bool> OnChangeIsWhiteCurtainOpen => _uiState.Only(state => state.IsWhiteCurtainOpen);
        public Observable<string> OnChangePromptPlaceholder => _uiState.Only(state => state.PromptPlaceholder);

        public ReadOnlyReactiveProperty<StageState> Stage => _stageService.State;
        public Observable<Position> OnChangeBuddyPosition => _stageService.State.Only(state => state.BuddyPosition);
        public Observable<bool> OnChangePlantMoved => _stageService.State.Only(state => state.IsPlantMoved);

        public Observable<StageState.BuddyRotationType> OnChangeBuddyRotation =>
            _stageService.State.Only(state => state.BuddyRotation);

        public Observable<bool> OnChangeIsLightOn => _stageService.State.Only(state => state.IsLightOn);
        public ReadOnlyReactiveProperty<AchievementState> Achievement => _achievementState;

        public void Setup()
        {
            _stageService.State
                .Where(_ => !_uiState.CurrentValue.IsTabletOpen)
                .Where(s => s.BuddyPosition.Y <= 7)
                .Subscribe(_ => OpenTablet());
            _stageService.State
                .Where(_ => _uiState.CurrentValue.IsTabletOpen)
                .Where(s => s.BuddyPosition.Y > 7)
                .Subscribe(_ => CloseTablet());

            _uiState.Value = new UIState().WithPromptPlaceholder("バディへの指示を入力してください...");
        }

        #region UIState

        public void OpenTablet()
        {
            if (_uiState.Value.IsTabletOpen)
            {
                Debug.LogWarning("Tablet is already open");
                return;
            }

            _uiState.Value = _uiState.Value.WithTabletOpen(true);
        }

        public void CloseTablet()
        {
            if (!_uiState.Value.IsTabletOpen)
            {
                Debug.LogWarning("Tablet is already closed");
                return;
            }

            _uiState.Value = _uiState.Value.WithTabletOpen(false);
        }

        public void OpenPrompt()
        {
            if (_uiState.Value.IsPromptOpen)
            {
                Debug.LogWarning("Prompt is already open");
                return;
            }

            _uiState.Value = _uiState.Value.WithPromptOpen(true);
        }

        public void ClosePrompt()
        {
            if (!_uiState.Value.IsPromptOpen)
            {
                Debug.LogWarning("Prompt is already closed");
                return;
            }

            _uiState.Value = _uiState.Value.WithPromptOpen(false);
        }

        public void OpenBlackCurtain()
        {
            if (_uiState.Value.IsBlackCurtainOpen)
            {
                Debug.LogWarning("BlackCurtain is already open");
                return;
            }

            _uiState.Value = _uiState.Value.WithBlackCurtainOpen(true);
        }

        public void CloseBlackCurtain()
        {
            if (!_uiState.Value.IsBlackCurtainOpen)
            {
                Debug.LogWarning("BlackCurtain is already closed");
                return;
            }

            _uiState.Value = _uiState.Value.WithBlackCurtainOpen(false);
        }
        
        public void OpenWhiteCurtain()
        {
            if (_uiState.Value.IsWhiteCurtainOpen)
            {
                Debug.LogWarning("WhiteCurtain is already open");
                return;
            }

            _uiState.Value = _uiState.Value.WithWhiteCurtainOpen(true);
        }

        public void SetUserPromptPlaceholder(string placeholder)
        {
            _uiState.Value = _uiState.Value.WithPromptPlaceholder(placeholder);
        }

        #endregion

        #region StageState

        public void MoveBuddy(Position position)
        {
            _stageService.MoveBuddy(position);
        }

        public void RotateBuddy(StageState.BuddyRotationType rotation)
        {
            _stageService.RotateBuddy(rotation);
        }

        public void SetLight(bool isOn)
        {
            if (!_achievementState.CurrentValue.WasLightTurnedOn && isOn)
                _achievementState.Value = _achievementState.Value.WithWasLightTurnedOn(true);

            _stageService.SetLight(isOn);
        }

        public void MovePlant()
        {
            if (!_achievementState.CurrentValue.WasPlantMoved)
                _achievementState.Value = _achievementState.Value.WithWasPlantMoved(true);

            _stageService.MovePlant();
        }

        public void SetIsEnteringPassword(bool isEntering)
        {
            _stageService.SetIsEnteringPassword(isEntering);
        }

        public void UnlockEnvironment()
        {
            if (!_achievementState.CurrentValue.WasBuddyOpenedShutter)
                _achievementState.Value = _achievementState.Value.WithWasBuddyOpenedShutter(true);
            if (!_achievementState.CurrentValue.WasBuddyGotCarKey)
                _achievementState.Value = _achievementState.Value.WithWasBuddyGotCarKey(true);

            _stageService.SetIsEnteringPassword(false);
        }

        #endregion

        public void GetCageKey()
        {
            if (!_achievementState.CurrentValue.WasBuddyGotCageKey)
                _achievementState.Value = _achievementState.Value.WithWasBuddyGotCageKey(true);
        }

        public void EscapeFromCage()
        {
            if (!_achievementState.CurrentValue.WasBuddyEscapedFromCage)
                _achievementState.Value = _achievementState.Value.WithWasBuddyEscapedFromCage(true);
        }

        public void PlaySound(SoundEffectId id) => _audioGateway.Play(id);
        public void StopSound(SoundEffectId id) => _audioGateway.Stop(id);
    }

    public static class EnvironmentServiceUtil
    {
        public static Observable<T1> Only<T1, T2>(this ReadOnlyReactiveProperty<T2> subject, Func<T2, T1> selector)
        {
            return subject.Select(selector).DistinctUntilChanged();
        }
    }
}