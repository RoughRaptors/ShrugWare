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
            MICROGAME_END = 13 // PUT ALL MICROGAMES BEFORE THIS AND KEEP THE VALUE UP TO DATE
        }

        public enum StatModifierType
        {
            PlayerHealth = 0,
            BossHealth,
            Timescale,
            OutgoingDamage,
            IncomingDamage
        }

        public struct StatEffect
        {
            public StatModifierType effectType;
            public float amount;
        }

        public enum ItemType
        {
            Armor = 0,
            Consumable
        }

        public static float MICROGAME_DURATION_SECONDS = 3.0f; // we can override this in the child Microgame's Start() - microgameDurationRemaining
        public static float SECONDS_TO_START_MICROGAME = 1.0f; // how long we wait for the microgame to start when on the scene - to orient the player
        public static float SECONDS_BETWEEN_MICROGAMES = 3.0f; // how long we wait before starting another microgame inside of GameManager

        public static float PLAYER_RAID_START_HP = 100.0f;
        public static float PLAYER_RAID_MAX_HP = 100.0f;
        public static int PLAYER_RAID_STARTING_LIVES = 3;
    }
}