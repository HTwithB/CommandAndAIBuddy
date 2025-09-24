namespace CandB.Script.Core
{
    public readonly struct UIState
    {
        // タブレットを表示しているか
        public readonly bool IsTabletOpen;

        // チャットを表示しているか
        public readonly bool IsPromptOpen;

        public readonly bool IsBlackCurtainOpen;
        
        public readonly bool IsWhiteCurtainOpen;

        // チャットのプレースホルダー
        public readonly string PromptPlaceholder;

        public UIState(bool isTabletOpen, bool isPromptOpen, bool isBlackCurtainOpen, bool isWhiteCurtainOpen, string placeholder)
        {
            IsTabletOpen = isTabletOpen;
            IsPromptOpen = isPromptOpen;
            IsBlackCurtainOpen = isBlackCurtainOpen;
            IsWhiteCurtainOpen = isWhiteCurtainOpen;
            PromptPlaceholder = placeholder;
        }

        public UIState WithPromptOpen(bool isPromptOpen) =>
            new UIState(IsTabletOpen, isPromptOpen, IsBlackCurtainOpen, IsWhiteCurtainOpen, PromptPlaceholder);

        public UIState WithTabletOpen(bool isTabletOpen) =>
            new UIState(isTabletOpen, IsPromptOpen, IsBlackCurtainOpen, IsWhiteCurtainOpen, PromptPlaceholder);

        public UIState WithBlackCurtainOpen(bool isBlackCurtainOpen) =>
            new UIState(IsTabletOpen, IsPromptOpen, isBlackCurtainOpen, IsWhiteCurtainOpen, PromptPlaceholder);

        public UIState WithWhiteCurtainOpen(bool isWhiteCurtainOpen) =>
            new UIState(IsTabletOpen, IsPromptOpen, IsBlackCurtainOpen, isWhiteCurtainOpen, PromptPlaceholder);
        
        public UIState WithPromptPlaceholder(string placeholder) =>
            new UIState(IsTabletOpen, IsPromptOpen, IsBlackCurtainOpen, IsWhiteCurtainOpen, placeholder);
    }
}