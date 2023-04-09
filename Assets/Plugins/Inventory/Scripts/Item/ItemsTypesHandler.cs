namespace Plugins.Inventory.Scripts.Item
{
    using System.Collections;
    using System.Collections.Generic;
    using Plugins.Inventory.Scripts.Item.ItemInterfaces;
    using UnityEngine;

    public class ItemsTypesHandler<T> : IEnumerable where T : IItem
    {
        private readonly Dictionary<int, T> _items = new Dictionary<int, T>();
        public IReadOnlyDictionary<int, T> Items => _items;

        public ItemsTypesHandler(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                if (_items.TryAdd(item.Id, item) == false)
                {
                    Debug.LogError("Item id " + item.Id + " already occupied");
                }
            }
        }

        public IEnumerator GetEnumerator() => Items.GetEnumerator();
    }
}
