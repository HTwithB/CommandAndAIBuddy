namespace CandB.Script.Core
{
    public readonly struct AchievementState
    {
        // ライトをつけたか
        public readonly bool WasLightTurnedOn;
        
        // 観葉植物を移動させたか
        public readonly bool WasPlantMoved;

        // 車の鍵を手に入れたか
        public readonly bool WasBuddyGotCarKey;
        
        // シャッターを開けたか
        public readonly bool WasBuddyOpenedShutter;

        // 檻の鍵を手に入れたかどうか
        public readonly bool WasBuddyGotCageKey;
        
        // 檻から脱出したかどうか
        public readonly bool WasBuddyEscapedFromCage;

        public AchievementState(
            bool wasLightTurnedOn,
            bool wasPlantMoved,
            bool wasBuddyGotCarKey,
            bool wasBuddyOpenedShutter,
            bool wasBuddyGotCageKey,
            bool wasBuddyEscapedFromCage
        )
        {
            WasLightTurnedOn = wasLightTurnedOn;
            WasPlantMoved = wasPlantMoved;
            WasBuddyGotCarKey = wasBuddyGotCarKey;
            WasBuddyOpenedShutter = wasBuddyOpenedShutter;
            WasBuddyGotCageKey = wasBuddyGotCageKey;
            WasBuddyEscapedFromCage = wasBuddyEscapedFromCage;
        }

        public AchievementState WithWasLightTurnedOn(bool flag)
        {
            return new AchievementState(
                flag,
                WasPlantMoved,
                WasBuddyGotCarKey,
                WasBuddyOpenedShutter,
                WasBuddyGotCageKey,
                WasBuddyEscapedFromCage);
        }

        public AchievementState WithWasPlantMoved(bool moved)
        {
            return new AchievementState(
                WasLightTurnedOn,
                moved,
                WasBuddyGotCarKey,
                WasBuddyOpenedShutter,
                WasBuddyGotCageKey,
                WasBuddyEscapedFromCage);
        }

        public AchievementState WithWasBuddyGotCarKey(bool got)
        {
            return new AchievementState(
                WasLightTurnedOn,
                WasPlantMoved,
                got,
                WasBuddyOpenedShutter,
                WasBuddyGotCageKey,
                WasBuddyEscapedFromCage);
        }

        public AchievementState WithWasBuddyOpenedShutter(bool opened)
        {
            return new AchievementState(
                WasLightTurnedOn,
                WasPlantMoved,
                WasBuddyGotCarKey,
                opened,
                WasBuddyGotCageKey,
                WasBuddyEscapedFromCage);
        }

        public AchievementState WithWasBuddyGotCageKey(bool got)
        {
            return new AchievementState(
                WasLightTurnedOn,
                WasPlantMoved,
                WasBuddyGotCarKey,
                WasBuddyOpenedShutter,
                got,
                WasBuddyEscapedFromCage);
        }
        
        public AchievementState WithWasBuddyEscapedFromCage(bool escaped)
        {
            return new AchievementState(
                WasLightTurnedOn,
                WasPlantMoved,
                WasBuddyGotCarKey,
                WasBuddyOpenedShutter,
                WasBuddyGotCageKey,
                escaped);
        }
    }
}