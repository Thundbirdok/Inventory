namespace Plugins.Inventory.Scripts.Item.ItemTypes
{
    using System;
    using Plugins.Inventory.Scripts.Item.ItemInterfaces;
    using UnityEngine;

    public class BulletItem : 
        IItem,
        ILimitedStackItem,
        IItemWithIcon,
        IItemWithWeight
    {
        public int Id { get; }

        public Sprite Icon { get; }

        public int MaxAmount { get; }

        public float Weight { get; }

        public BulletItem(int id, int maxAmount, float weight, Sprite icon)
        {
            Icon = icon;
            Id = id;
            MaxAmount = maxAmount;
            Weight = weight;
        }
    }

    [Serializable]
    public class BulletJsonItem : 
        IJsonItem,
        IJsonLimitedStackItem,
        IJsonItemWithIcon,
        IJsonItemWithWeight
    {
        public int Id { get; set; }
        public int MaxAmount { get; set; }
        public string IconName { get; set; }
        public float Weight { get; set; }
    }
}
