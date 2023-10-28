using Assets.Scripts.Define;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Hellper
{
    public class EntityHelper
    {
        public static bool IsBox(Location location)
        {
            return location != null && location.entityBase != null && (location.entityBase.entityType == EntityType.BallBox
                    || location.entityBase.entityType == EntityType.BombBox
                    || location.entityBase.entityType == EntityType.Box
                    || location.entityBase.entityType == EntityType.GiftBox
                    || location.entityBase.entityType == EntityType.Mario2Bomb);
        }

        public static bool IsBoxKey(Location location)
        {
            return location != null && location.entityBase != null && (location.entityBase.entityType == EntityType.BoxKey
                    || location.entityBase.entityType == EntityType.BoxKey1);
        }

        public static bool IsEnemy(Location location)
        {
            return location != null && location.entityBase != null && location.entityBase.entityType == EntityType.Enemy;
        }

        public static bool IsPlayer(Location location)
        {
            return location != null && location.entityBase != null && location.entityBase.entityType == EntityType.Player;
        }

        public static bool IsEntityType(Location location, EntityType entityType)
        {
            return location != null && location.entityBase != null && location.entityBase.entityType == entityType;
        }
    }
}