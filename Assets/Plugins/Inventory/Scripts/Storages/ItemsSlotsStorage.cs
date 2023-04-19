namespace Plugins.Inventory.Scripts.Storages
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Plugins.Inventory.Scripts.Item;
    using Plugins.Inventory.Scripts.Item.ItemInterfaces;
    using Plugins.Inventory.Scripts.Slot;
    using UnityEngine;
    
    public class ItemsSlotsStorage : IEnumerable<Slot>
    {
        public event Action<int> OnSlotAdded;
        public event Action<Slot> OnSlotRemoved;

        public int Count => Slots.Count;
        
        public int MaxAmount { get; private set; }
        
        private readonly ItemsTypesHandler<IItem> _typesHandler;
        
        public List<Slot> Slots { get; }

        public ItemsSlotsStorage
        (
            int maxAmount, 
            List<Slot> slots,
            ItemsTypesHandler<IItem> typesHandler
        )
        {
            _typesHandler = typesHandler;
            MaxAmount = maxAmount;

            Slots = slots ?? new List<Slot>(maxAmount);
        }

        public IEnumerator<Slot> GetEnumerator()
        {
            return (Slots as IEnumerable<Slot>).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Slot this[int index] => Slots[index];

        public void ChangeMaxAmount(int maxCapacity)
        {
            if (maxCapacity < 0)
            {
                return;
            }
            
            MaxAmount = maxCapacity;

            for (var i = Slots.Count - 1; i >= MaxAmount; i--)
            {
                Slots.RemoveAt(i);
            }
        }

        public bool Add(int itemId, int amount)
        {
            if (TryAddToSlotsWithSameItem(itemId, ref amount))
            {
                return true;
            }

            if (TryAddToNewSlot(itemId, ref amount))
            {
                return true;
            }
            
            return false;
        }

        public bool Spend(int itemId, int amount)
        {
            if (IsEnoughToSpend(itemId, amount) == false)
            {
                return false;
            }
            
            for (var i = Slots.Count - 1; i >= 0; i--)
            {
                if (Slots[i].itemId != itemId)
                {
                    continue;
                }

                if (amount < Slots[i].Amount)
                {
                    Slots[i].Amount -= amount;

                    return true;
                }

                var newSlotAmount = Mathf.Max(Slots[i].Amount - amount, 0);
                var newAmount = Mathf.Max(amount - Slots[i].Amount, 0);
                
                if (newSlotAmount == 0)
                {
                    RemoveAt(i);
                }
                else
                {
                    Slots[i].Amount = newSlotAmount;
                }

                if (newAmount == 0)
                {
                    return true;
                }
            }
                
            return true;
        }

        public bool IsEnoughToSpend(int itemId, int amount)
        {
            return CountItem(itemId) >= amount;
        }

        public int CountItem(int itemId)
        {
            var amount = 0;
            
            for (var i = Slots.Count - 1; i >= 0; i--)
            {
                if (Slots[i].itemId != itemId)
                {
                    continue;
                }

                amount += Slots[i].Amount;
            }

            return amount;
        }

        public void RemoveAt(int index)
        {
            var slot = Slots[index];
            
            Slots.RemoveAt(index);

            OnSlotRemoved?.Invoke(slot);
        }

        public ItemSlotsStorageJson ToJsonData()
        {
            return new ItemSlotsStorageJson()
            {
                maxAmount = MaxAmount,
                slots = Slots
            };
        }
        
        private bool TryAddToNewSlot(int itemId, ref int amount)
        {
            if (Slots.Count >= MaxAmount)
            {
                return false;
            }

            var slot = new Slot()
            {
                itemId = itemId,
                Amount = amount
            };
            
            Slots.Add(slot);

            OnSlotAdded?.Invoke(Slots.Count - 1);
            
            return true;
        }

        private bool TryAddToSlotsWithSameItem(int itemId, ref int amount)
        {
            for (var i = 0; i < Slots.Count; i++)
            {
                var itemIndex = Slots[i].itemId;

                if (itemIndex.Equals(itemId) == false)
                {
                    continue;
                }

                if (TryAddAmountToSlot(itemId, ref amount, Slots[i]))
                {
                    return true;
                }
            }

            return amount == 0;
        }

        private bool TryAddAmountToSlot(int itemId, ref int amount, Slot slotWithSameItem)
        {
            if (_typesHandler.Items.TryGetValue(itemId, out var item) == false)
            {
                return false;
            }

            var newAmount = slotWithSameItem.Amount + amount;

            if (item is ILimitedStackItem limitedStackItem)
            {
                return TryAddLimitedItem
                (
                    ref amount,
                    slotWithSameItem,
                    limitedStackItem,
                    newAmount
                );
            }

            slotWithSameItem.Amount = newAmount;

            return true;
        }

        private bool TryAddLimitedItem
        (
            ref int amount,
            Slot slotWithSameItem,
            ILimitedStackItem limitedStackItem,
            int newAmount
        )
        {
            var maxStackAmount = limitedStackItem.MaxAmount;

            if (newAmount <= maxStackAmount)
            {
                slotWithSameItem.Amount = newAmount;

                return true;
            }

            slotWithSameItem.Amount = maxStackAmount;

            amount = newAmount - maxStackAmount;

            return false;
        }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class ItemSlotsStorageJson
    {
        [JsonProperty("MaxAmount")]
        public int maxAmount;

        [JsonProperty("Slots")]
        public List<Slot> slots;
    }
}
