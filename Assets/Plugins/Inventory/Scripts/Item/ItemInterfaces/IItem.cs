namespace Plugins.Inventory.Scripts.Item.ItemInterfaces
{
    public interface IItem
    {
        public int Id { get; }
    }
    
    public interface IJsonItem
    {
        public int Id { get; set; }
    }
}
