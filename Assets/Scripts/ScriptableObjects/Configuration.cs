using System;
using System.Linq;
using UnityEngine;
using Enums;
using System.Collections.Generic;
using System.IO;

[CreateAssetMenu]
public class Configuration : ScriptableObject
{
    //TODO: [OPTIMIZATION] Change all publics into privates and get only through methods

    public bool randomizeLevels;

    public float moneyBase;
    public int moneyMultiplierInterval;

    [Tooltip("Values of elements 0 - 4 denote the hint system for the tutorial, 5 and 6 denote the hint timing for the other levels in general")]
    public List<float> TutorialEnablePathRatio = new List<float>();

    public MovementSpeedInfo characterSpeeds;

    public float baseDecisionMakingTime;

    public List<AI_behavior> ai_behavior_description;

    public List<CSVData> csvData = new List<CSVData>();
    public List<ShopItemInfo> itemInfo = new List<ShopItemInfo>();

    public AppearanceAnimation[] appearanceAnimations;
    public string[] hairStyleColors;

    public int unlockSkinRate = 5;

    public void ImportCSV( CSVTypeToLoad infoType )
    {
        Debug.Log(Application.dataPath);
        loadLevelCsv(File.ReadAllText(Path.Combine(Application.dataPath, infoType.ToString() + ".csv")), infoType);

        // Requires setup access to sheets
        //GetLevels($"https://docs.google.com/spreadsheets/d/1XBfvwr-p618cLXEjzKnmb91T0EHktRqPcuv4KaPZZWA/export?format=csv", 
        //    result => { LoadCSV(result); });
    }

    private void loadLevelCsv(string data, CSVTypeToLoad infoType)
    {
        List<Dictionary<string, object>> items = CSVReader.Read(data);

        if(infoType == CSVTypeToLoad.levels)
        {
            csvData.Clear();

            for(int i = 0; i < items.Count; i++)
            {
                csvData.Add(new CSVData
                {
                    levelNumber = int.Parse(items[i]["Levels"].ToString()),
                    locationToGenerate = items[i]["Locations"].ToString(),
                    levelLength = int.Parse(items[i]["Count"].ToString()),
                    clothTypes = items[i]["Clothes"].ToString(),
                    AICount = int.Parse(items[i]["AICount"].ToString()),
                    levelDifficulty = items[i]["AIType"].ToString(),
                    isTutorial = items[i]["AIType"].ToString() == "TUTORIAL",
                });


            }
		}
        else if(infoType == CSVTypeToLoad.items)
        {
            itemInfo.Clear();

            ShopSection[] ss = Enum.GetValues(typeof(ShopSection)).OfType<ShopSection>().ToArray();

            int faceOrder = -1;
            int costumeOrder = -1;
            int hairstyleOrder = -1;
            int accessoryOrder = -1;

            for(int i = 0; i < items.Count; i++)
            {
                ShopSection currentSection = ss.First((x) => x.ToString() == items[i]["ShopSection"].ToString());

                itemInfo.Add(new ShopItemInfo
                {
                    itemName = items[i]["Name"].ToString(),
                    orderInShop = currentSection == ShopSection.Face ? ++faceOrder :
                                  currentSection == ShopSection.Costume ? ++costumeOrder :
                                  currentSection == ShopSection.Hairstyle ? ++hairstyleOrder :
                                                                            ++accessoryOrder,
                    shopSection = items[i]["ShopSection"].ToString(),
                    lockStatus = items[i]["LockStatus"].ToString(),
                    lockType = items[i]["LockType"].ToString(),
                    selectionState = items[i]["SelectionState"].ToString(),
                    highlight = items[i]["Highlight"].ToString(),
                    levelToUnlock = int.Parse(items[i]["UnlockLevel"].ToString()),
                    priceToUnlock = int.Parse(items[i]["LockPrice"].ToString()),
                });
            }
        }
    }

    public string GetGiftName( int _levelIndex )
    {
        string giftName = string.Empty;

        //Debug.Log($"LevelIndex: {_levelIndex} and itemInfo length: {itemInfo.Count}");

        if(itemInfo.Any(x => x.levelToUnlock == _levelIndex))
            giftName = itemInfo.First(x => x.levelToUnlock == _levelIndex).itemName;

        return giftName;
    }

    [Serializable]
    public class CSVData
    {
        public int levelNumber;
        public string locationToGenerate;
        public int levelLength;
        public string clothTypes;
        public int AICount;
        public string levelDifficulty;
        public bool isTutorial;
    }

    [Serializable]
    public class ShopItemInfo
    {
        public string itemName;
        public int orderInShop; // start from 0. Take from the list
        public string shopSection; // Read ShopSection enums
        public string lockStatus; // Read LockStatus
        public string lockType; // Read LockType
        public string selectionState; // Read SelectionState
        public string highlight; // Read Highlight
        public int levelToUnlock; // level to hit
        public int priceToUnlock; // money
    }

    [Serializable]
    public class AppearanceAnimation //TODO: [ANIMATION] When all the animations ready, check by enum Appearance
    {
        public AnimationClip[] correct;
        public AnimationClip[] wrong;
    }
}

[Serializable]
public struct MovementSpeedInfo
{
    public float default_speed;
    public float min_speed;
    public float max_speed;
}

//TODO: [FUTURE] can be turned into AnimationCurve to change based on level
[Serializable]
public struct AI_behavior 
{
    public AIDifficulty difficulty_type;
    public float wrong_card_pick_percent;
    public float card_pick_delay;
}
