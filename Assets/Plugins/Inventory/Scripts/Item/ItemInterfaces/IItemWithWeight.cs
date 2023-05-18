using UnityEngine;

namespace Plugins.Inventory.Scripts.Item.ItemInterfaces
{
    public interface IItemWithWeight
    {
        public float Weight { get; }
    }
    
    public interface IJsonItemWithWeight
    {
        public float Weight { get; set; }
    }
}
