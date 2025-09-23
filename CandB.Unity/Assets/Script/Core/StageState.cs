namespace CandB.Script.Core
{
    public readonly struct StageState
    {
        public const string BoxPassword = "1234";
        public const string BoxPassword2 = "１２３４";
        
        public enum BuddyRotationType
        {
            North = 0,
            East = 90,
            South = 180,
            West = 270,
        }

        // Vector2.zero は未取得を意味する

        // バディの位置
        public readonly Position BuddyPosition;

        // バディの向き
        public readonly BuddyRotationType BuddyRotation;

        // ライトがオンになっているか
        public readonly bool IsLightOn;

        // ゴーグルの位置
        public readonly Position CraftItemPosition;
        
        // 観葉植物を移動させたか
        public readonly bool IsPlantMoved;

        // 車の鍵の位置
        public readonly Position CarKeyPosition;

        // シャッターの鍵の位置
        public readonly Position ShutterKeyPosition;

        // 木箱の位置
        public readonly Position BoxPosition;

        // パスワード入力中かどうか
        public readonly bool IsEnteringPassword;

        public static StageState Default => new();

        public StageState(
            Position buddyPosition,
            BuddyRotationType buddyRotation,
            bool isLightOn,
            Position craftItemPosition,
            bool isPlantMoved,
            Position carKeyPosition,
            Position shutterKeyPosition,
            Position boxPosition,
            bool isEnteringPassword
        )
        {
            BuddyPosition = buddyPosition;
            BuddyRotation = buddyRotation;
            IsLightOn = isLightOn;
            CraftItemPosition = craftItemPosition;
            IsPlantMoved = isPlantMoved;
            CarKeyPosition = carKeyPosition;
            ShutterKeyPosition = shutterKeyPosition;
            BoxPosition = boxPosition;
            IsEnteringPassword = isEnteringPassword;
        }

        public StageState WithBuddyPosition(Position position) =>
            new(
                position, 
                BuddyRotation, 
                IsLightOn, 
                CraftItemPosition, 
                IsPlantMoved,
                CarKeyPosition, 
                ShutterKeyPosition, 
                BoxPosition,
                IsEnteringPassword);

        public StageState WithBuddyRotation(BuddyRotationType rotation) =>
            new(
                BuddyPosition, 
                rotation, 
                IsLightOn, 
                CraftItemPosition,
                IsPlantMoved,
                CarKeyPosition, 
                ShutterKeyPosition,
                BoxPosition,
                IsEnteringPassword);

        public StageState WithLightOn(bool isLightOn) =>
            new(
                BuddyPosition, 
                BuddyRotation, 
                isLightOn, 
                CraftItemPosition, 
                IsPlantMoved,
                CarKeyPosition, 
                ShutterKeyPosition,
                BoxPosition,
                IsEnteringPassword);

        public StageState WithCraftItemPosition(Position position) =>
            new(
                BuddyPosition,
                BuddyRotation,
                IsLightOn,
                position,
                IsPlantMoved,
                CarKeyPosition,
                ShutterKeyPosition,
                BoxPosition,
                IsEnteringPassword);
        
        public StageState WithPlantMoved(bool isMoved) =>
         new(BuddyPosition,
             BuddyRotation,
             IsLightOn,
             CraftItemPosition,
             isMoved,
             CarKeyPosition,
             ShutterKeyPosition,
             BoxPosition,
             IsEnteringPassword);

        public StageState WithCarKeyPosition(Position position) =>
            new(
                BuddyPosition,
                BuddyRotation,
                IsLightOn, 
                CraftItemPosition,
                IsPlantMoved,
                position, 
                ShutterKeyPosition, 
                BoxPosition,
                IsEnteringPassword);

        public StageState WithShutterKeyPosition(Position position) =>
            new(
                BuddyPosition, 
                BuddyRotation, 
                IsLightOn,
                CraftItemPosition,
                IsPlantMoved,
                CarKeyPosition,
                position,
                BoxPosition,
                IsEnteringPassword);

        public StageState WithBoxPosition(Position position) =>
            new(
                BuddyPosition,
                BuddyRotation,
                IsLightOn,
                CraftItemPosition,
                IsPlantMoved,
                CarKeyPosition, 
                ShutterKeyPosition,
                position,
                IsEnteringPassword);
        
        public StageState WithIsEnteringPassword(bool isEntering) =>
            new(
                BuddyPosition,
                BuddyRotation,
                IsLightOn,
                CraftItemPosition,
                IsPlantMoved,
                CarKeyPosition,
                ShutterKeyPosition,
                BoxPosition,
                isEntering);
    }
}