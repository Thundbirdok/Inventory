namespace Plugins.Inventory.Scripts.Item.ItemInterfaces
{
    using UnityEngine;

    public interface IItemWithIcon
    {
        public Sprite Icon { get; }
    }
    
    public interface IJsonItemWithIcon
    {
        public string IconName { get; set; }
    }
}
