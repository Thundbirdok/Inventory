namespace Plugins.Inventory.Scripts.Item.ItemTypes
{
    using System;
    using Plugins.Inventory.Scripts.Item.ItemInterfaces;
    using UnityEngine;

    public class WeaponItem : 
        IItem,
        ILimitedStackItem,
        IItemWithIcon,
        IItemWithWeight,
        IWeaponItem
    {
        public int Id { get; }

        public Sprite Icon { get; }

        public int MaxAmount { get; }

        public int Damage { get; }
        public int BulletsId { get; }

        public float Weight { get; }

        public WeaponItem(int id, int maxAmount, int damage, int bulletsId, float weight, Sprite icon)
        {
            Icon = icon;
            Id = id;
            MaxAmount = maxAmount;
            Damage = damage;
            BulletsId = bulletsId;
            Weight = weight;
        }
    }

    [Serializable]
    public class WeaponJsonItem : 
        IJsonItem,
        IJsonLimitedStackItem,
        IJsonItemWithIcon,
        IJsonItemWithWeight,
        IJsonWeaponItem
    {
        public int Id { get; set; }
        public int MaxAmount { get; set; }
        public string IconName { get; set; }
        public float Weight { get; set; }
        public int Damage { get; set; }
        public int BulletsId { get; set; }
    }
}
