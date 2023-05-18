namespace Plugins.Inventory.Scripts.Item.ItemInterfaces
{
    public interface IArmorItem
    {
        public int Defence { get; }
    }
    
    public interface IJsonArmorItem
    {
        public int Defence { get; set; }
    }
}
