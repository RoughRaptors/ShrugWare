namespace ShrugWare
{
    public class DataManager
    {
        // KEEP IN SYNC WITH BUILD SETTINGS
        [System.Serializable]
        public enum Scenes
        {
            MainMenuScene = 0,
            OverworldScene,
            TutorialScene,
            MerchantScene,
            WarbossKard, 
            MICROGAME_START = 5, // KEEP IN SYNC
            GetOutOfFire = 5,    // KEEP IN SYNC
            CastFrostbolt,
            TauntTheAdds,
            InterceptTheFireball,
            PerformTheRotation,
            StackPowerstones,
            SidestepTheFireball,
            DodgeTheFireballPattern,
            StackUpForMeteor,
            SpreadOutFromMeteor,
            EqualizeGroupSize,
            InterruptTheAbility,
            KnockbackedSafelty,
            MatchPolarity,
            StealTheChest,
            MICROGAME_END = 19,// PUT ALL MICROGAMES BEFORE THIS AND KEEP THE VALUE UP TO DATE
            Tuzi,
            DodgeFireballsMinigame,
        }

        public enum StatModifierType
        {
            PlayerCurHealth = 0,
            PlayerMaxHealth,
            BossCurHealth,
            Timescale,
            OutgoingDamage,
            IncomingDamage
        }

        [System.Serializable]
        public struct StatEffect
        {
            public StatModifierType effectType;
            public float amount;
            public bool asPercentage;
        }

        public enum ItemType
        {
            Armor = 0,
            Consumable
        }

        public enum Currencies
        {
            Generic = 0,
            DauntingInferno
        }

        public enum ArmorSlot
        {
            Head = 0,
            Chest,
            Gloves,
            Legs,
            Boots,
            MAX = 4
        }

        public enum ArmorSet
        {
            DauntingInferno = 0
        }

        public enum AudioEffectTypes
        {
            MainMenu = 0,
            BetweenMicrogame,
            UsePotion,
            MerchantPurchase,
            ButtonClick,
            MicrogameTimerTick,
            MicrogameTimerDing
        }

        public enum OverworldLevelType
        {
            Start = 0,
            Tutorial,
            End,
            Merchant,
            Trash,
            Boss
        }

        public static float MICROGAME_DURATION_SECONDS = 3.5f; // we can override this in the child Microgame's Start() - microGameTime
        public static float MINIGAME_DURATION_SECONDS = 45.0f;
        public static float SECONDS_TO_START_MICROGAME = 1.35f; // how long we wait for the microgame to start when on the scene - to orient the player
        public static float SECONDS_BETWEEN_MICROGAMES = 3.75f; // how long we wait before starting another microgame inside of GameManager

        public static int PLAYER_START_HP = 100;
        public static int PLAYER_MAX_HP = 100;
        public static int PLAYER_STARTING_LIVES = 3;
    }
}