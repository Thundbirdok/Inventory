namespace Plugins.Inventory.Scripts.Item.ItemTypes
{
    using System;
    using Plugins.Inventory.Scripts.Item.ItemInterfaces;
    using UnityEngine;

    public class WeaponItem : IItem, ILimitedStackItem, IItemWithIcon, IItemWithWeight, IWeaponItem
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
    public class WeaponJsonItem
    {
        public int id;
        public string iconName;
        public int bulletsId;
        public int damage;
        public float weight;
    }
}
