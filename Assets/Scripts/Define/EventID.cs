using UnityEditor;
using UnityEngine;

public enum EventID
{
    None,
    EnemyDie,
    PlayerDie,

    UpdateGem,
    UpdateCoin,

    SelectCharInList,
    PickChar,

    UpdateController,

    BuyGemFirstTime,
    RemoveAd,
    WeeklyPack,

    EndScoreLine,

    CompleteQuest,
    UpdateQuest,

    EndPartOfMap,
    CompleteQuestInGame
}
