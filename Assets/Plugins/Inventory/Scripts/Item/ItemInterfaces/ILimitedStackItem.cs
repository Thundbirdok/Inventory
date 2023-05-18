namespace Plugins.Inventory.Scripts.Item.ItemInterfaces
{
    public interface ILimitedStackItem
    {
        public int MaxAmount { get; }
    }
    
    public interface IJsonLimitedStackItem
    {
        public int MaxAmount { get; set; }
    }
}
