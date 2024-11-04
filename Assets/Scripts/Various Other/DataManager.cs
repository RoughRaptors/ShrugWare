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
            InfiniteModeScene,
            WarbossKard, 
            Tuzi,
            DodgeFireballsMinigame,
            HeiganDanceMinigame,
            GearScreen,
            MICROGAME_START = 10, // KEEP IN SYNC
            GetOutOfFire = 10,    // KEEP IN SYNC
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
            LineOfSight,
            RunIn,
            LaserLineOfSightVertical,
            LaserLineOfSightHorizonal,
            LaserLineOfSightDiagonal,
            LaserLineOfSightVerticalAlternative,
            AttackTheBossTiming,
            NavigateTheMaze,
            ColoredPlatforms,
            AvoidTheLasers,
            CraftTheItem,
            BuyExpensiveItem,
            BuyCheapItem,
            ShootTheBoss,
            MeleeTheBoss,
            Fishing,
            MICROGAME_END = 40 // PUT ALL MICROGAMES BEFORE THIS AND KEEP THE VALUE UP TO DATE
        }

        public enum StatModifierType
        {
            PlayerCurHealth = 0,
            PlayerMaxHealth,
            BossCurHealth,
            Timescale,
            OutgoingDamage,
            IncomingDamage,
            PlayerMoveSpeed,
            MicrogameTime,
            Lives
        }

        [System.Serializable]
        public struct StatEffect
        {
            public StatModifierType effectType;
            public float amount;
            public bool asPercentage;
            public string effectDescriptionString;
        }

        public enum ItemType
        {
            Armor = 0,
            Consumable
        }

        public enum ItemTemplateIds
        {
            HealthPotion = 0,
            MaxHealthPotion,
            Helm,
            Chest,
            Gloves,
            Legs,
            Boots,
            RunSpeedPotion,
            Accessory,
            MAX = 8
        }

        public enum Currencies
        {
            Gold = 0,
            DauntingInferno,
            DKP
        }

        public enum ArmorSlot
        {
            Head = 0,
            Chest,
            Gloves,
            Legs,
            Boots,
            Accessory,
            MAX = 5,
            NONE,
        }

        public enum ArmorSet
        {
            DauntingInferno = 0
        }

        public enum AudioType
        {
            MainMenu = 0,
            BetweenMicrogame,
            UsePotion,
            MerchantPurchase,
            ButtonClick,
            MicrogameTimerTick,
            MicrogameTimerDing,
            Overworld,
            MicrogameMusic,
            MinigameMusic,
            MicrogameIntro,
            MicrogameOutro,
            MicrogameWin,
            MicrogameLose,
            SFX
        }

        public enum OverworldLevelType
        {
            Start = 0,
            Tutorial,
            Merchant,
            Trash,
            Boss,
            Infinite,
            GearScreen
        }

        public static float MICROGAME_DURATION_SECONDS = 3.75f; // we can override this in the child Microgame's Start() - microGameTime
        public static float SECONDS_TO_START_MICROGAME = 1.5f; // how long we wait for the microgame to start when on the scene - to orient the player
        public static float SECONDS_BETWEEN_MICROGAMES = 3.75f; // how long we wait before starting another microgame inside of GameManager

        public static int PLAYER_START_HP_BOSSENCOUNTER = 100;
        public static int PLAYER_MAX_HP_BOSSENCOUNTER = 100;
        public static int PLAYER_STARTING_LIVES_BOSSENCOUNTER = 0; // used to be 3, make it 0 to make it harder and emphasize progression

        public static int PLAYER_START_HEALTH_MINIGAME = 5;
    }
}