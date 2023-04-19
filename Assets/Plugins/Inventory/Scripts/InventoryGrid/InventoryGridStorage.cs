namespace Plugins.Inventory.Scripts.InventoryGrid
{
    using System;
    using Plugins.Inventory.Scripts.Item;
    using Plugins.Inventory.Scripts.Item.ItemInterfaces;
    using Plugins.Inventory.Scripts.Slot;
    using Plugins.Inventory.Scripts.Storages;
    using UnityEngine;

    [Serializable]
    public class InventoryGridStorage
    {
        public ItemsSlotsStorage Storage { get; private set; }

        [SerializeField]
        private InventorySlotsSpawner spawner;

        private ItemsTypesHandler<IItem> _typesHandler;
        
        public void Init(ItemsSlotsStorage storage, ItemsTypesHandler<IItem> typesHandler)
        {
            Storage = storage;
            _typesHandler = typesHandler;
            
            spawner.Init(storage);

            SetSlots();

            Storage.OnSlotAdded += AddNewSlot;
            Storage.OnSlotRemoved += SlotRemoved;
        }

        public void Dispose()
        {
            Storage.OnSlotAdded -= AddNewSlot;
            Storage.OnSlotRemoved -= SlotRemoved;
        }

        public void AddSlots() => spawner.AddSlots();

        public void RemoveSlots() => spawner.RemoveSlots();

        public bool Add(int itemId, int amount)
        {
            return Storage.Add(itemId, amount);
        }

        public bool Spend(int itemId, int amount)
        {
            return Storage.Spend(itemId, amount);
        }

        private void SetSlots()
        {
            for (var i = 0; i < Storage.Count; i++)
            {
                AddNewSlot(i);
            }
        }

        private void AddNewSlot(int slotIndex)
        {
            for (var i = 0; i < spawner.Slots.Count; i++)
            {
                if (spawner.Slots[i].IsEmpty == false)
                {
                    continue;
                }

                AddItemToEmptySlot(Storage[slotIndex], spawner.Slots[i]);
                    
                break;
            }
        }

        private void AddItemToEmptySlot(Slot slot, SlotProvider emptySlot)
        {
            Sprite icon = null;
                
            if (_typesHandler.Items.TryGetValue(slot.itemId, out var item))
            {
                if (item is IItemWithIcon itemWithIcon)
                {
                    icon = itemWithIcon.Icon;
                }
            }
                
            emptySlot.SetSlot(slot, icon);
        }

        private void SlotRemoved(Slot slot)
        {
            for (var i = 0; i < spawner.Slots.Count; i++)
            {
                if (spawner.Slots[i].Slot != slot)
                {
                    continue;
                }

                spawner.Slots[i].SetEmpty();
                
                break;
            }
        }
    }
}
