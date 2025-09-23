namespace CandB.Script.Core
{
    public readonly struct UIState
    {
        // タブレットを表示しているか
        public readonly bool IsTabletOpen;

        // チャットを表示しているか
        public readonly bool IsPromptOpen;

        public readonly bool IsBlackCurtainOpen;

        // チャットのプレースホルダー
        public readonly string PromptPlaceholder;

        public UIState(bool isTabletOpen, bool isPromptOpen, bool isBlackCurtainOpen, string placeholder)
        {
            IsTabletOpen = isTabletOpen;
            IsPromptOpen = isPromptOpen;
            IsBlackCurtainOpen = isBlackCurtainOpen;
            PromptPlaceholder = placeholder;
        }

        public UIState WithPromptOpen(bool isPromptOpen) =>
            new UIState(IsTabletOpen, isPromptOpen, IsBlackCurtainOpen, PromptPlaceholder);

        public UIState WithTabletOpen(bool isTabletOpen) =>
            new UIState(isTabletOpen, IsPromptOpen, IsBlackCurtainOpen, PromptPlaceholder);

        public UIState WithBlackCurtainOpen(bool isBlackCurtainOpen) =>
            new UIState(IsTabletOpen, IsPromptOpen, isBlackCurtainOpen, PromptPlaceholder);

        public UIState WithPromptPlaceholder(string placeholder) =>
            new UIState(IsTabletOpen, IsPromptOpen, IsBlackCurtainOpen, placeholder);
    }
}