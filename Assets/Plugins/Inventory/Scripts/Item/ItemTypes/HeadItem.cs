namespace Plugins.Inventory.Scripts.Item.ItemTypes
{
    using System;
    using Plugins.Inventory.Scripts.Item.ItemInterfaces;
    using UnityEngine;

    public class HeadItem : 
        IItem,
        ILimitedStackItem,
        IItemWithIcon,
        IItemWithWeight,
        IArmorItem
    {
        public int Id { get; }

        public Sprite Icon { get; }

        public int MaxAmount { get; }

        public float Weight { get; }

        public int Defence { get; }

        public HeadItem(int id, int maxAmount, float weight, int defence, Sprite icon)
        {
            Icon = icon;
            Id = id;
            MaxAmount = maxAmount;
            Weight = weight;
            Defence = defence;
        }
    }

    [Serializable]
    public class HeadJsonItem : 
        IJsonItem,
        IJsonLimitedStackItem,
        IJsonItemWithIcon,
        IJsonItemWithWeight,
        IJsonArmorItem
    {
        public int Id { get; set; }
        public int MaxAmount { get; set; }
        public string IconName { get; set; }
        public float Weight { get; set; }
        public int Defence { get; set; }
    }
}
