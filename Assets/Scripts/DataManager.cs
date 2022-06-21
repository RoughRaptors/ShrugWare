namespace ShrugWare
{
    public class DataManager
    {
        // KEEP IN SYNC WITH BUILD SETTINGS
        public enum Scenes
        {
            MainScene = 0, // DO NOT CHANGE THIS
            GetOutOfFire = 1,
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
            MICROGAME_END = 15 // PUT ALL MICROGAMES BEFORE THIS AND KEEP THE VALUE UP TO DATE
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
            ButtonClick
        }

        public static float MICROGAME_DURATION_SECONDS = 3.5f; // we can override this in the child Microgame's Start() - microgameDurationRemaining
        public static float SECONDS_TO_START_MICROGAME = 1.25f; // how long we wait for the microgame to start when on the scene - to orient the player
        public static float SECONDS_BETWEEN_MICROGAMES = 3.5f; // how long we wait before starting another microgame inside of GameManager

        public static int PLAYER_START_HP = 100;
        public static int PLAYER_MAX_HP = 100;
        public static int PLAYER_STARTING_LIVES = 3;
    }
}