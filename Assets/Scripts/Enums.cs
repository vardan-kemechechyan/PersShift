namespace Enums
{
    public enum Appearance
    {
        None,
        EVENING,
        SWIMMING,
        OFFICE,
        SPORT,
        WINTER,
        CASUAL,
    }

    public enum Locations
    {
        None,
        START,
        FINISH,
        OFFICE,
        POOL,
        PODIUM,
        SUPERMARKET,
        BEACH,
        CITY,
        PARK,
        WINTER
    }

    public enum GameState
    {
        None,
        Start,
        Play,
        Pause,
        FinishLine,
        LevelComplete,
        LevelFailed,
        LoadLevel
    }

    public enum Control
    {
        Player,
        AI
    }

    public enum AIDifficulty
    {
        NONE,
        TUTORIAL,
        EASY,
        MEDIUM,
        HARD,
        BOSS
    }

    public enum ShopSection
    {
        Face,
        Costume,
        Hairstyle,
        Accessory
	}

    public enum CustomizableItems
    {
        Hairstyle,
        Head,
        Eyes,
        Ears,
        Neck,
        WristLeft,
        WristRight,
        HandLeft,
        HandRight,
        Face,
        Costume,
        NONE
    }
    
    public enum InShopViewTransitions
    {
        HALFBODY,
        FULLBODY,
        NONE
    }

    public enum SelectionState
    {
        SELECTED,
        NOT_SELECTED
    }

    public enum HighLight
    {
        HIGHLIGHTED,
        DEFAULT,
        NONE
    }

    public enum LockStatus
    {
        LOCKED, //0
        UNLOCKED, //1
        NONE
	}
    public enum LockType
    {
        CHECKMARK,
        WATCH_AD,
        LEVEL_UNLOCK,
        BUY_FOR_MONEY,
        NONE
	}

    public enum CSVTypeToLoad
    {
        levels,
        items
    }
}
