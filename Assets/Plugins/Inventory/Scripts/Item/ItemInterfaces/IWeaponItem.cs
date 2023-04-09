using UnityEngine;

namespace Plugins.Inventory.Scripts.Item.ItemInterfaces
{
    public interface IWeaponItem
    {
        public int Damage { get; }
        
        public int BulletsId { get; }
    }
}
