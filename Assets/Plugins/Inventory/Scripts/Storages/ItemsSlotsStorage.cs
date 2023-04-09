namespace Plugins.Inventory.Scripts.Storages
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Plugins.Inventory.Scripts.Item;
    using Plugins.Inventory.Scripts.Item.ItemInterfaces;
    using Plugins.Inventory.Scripts.Slot;
    using UnityEngine;

    public class ItemsSlotsStorage : IEnumerable<Slot>
    {
        public event Action<int> OnSlotAdded;
        public event Action<Slot> OnSlotRemoved;

        public int Count => _slots.Count;
        public int MaxAmount { get; private set; }

        
        private readonly ItemsTypesHandler<IItem> _typesHandler;

        private readonly List<Slot> _slots;

        public ItemsSlotsStorage(int maxAmount, ItemsTypesHandler<IItem> typesHandler)
        {
            _typesHandler = typesHandler;
            MaxAmount = maxAmount;

            _slots = new List<Slot>(maxAmount);
        }

        public IEnumerator<Slot> GetEnumerator()
        {
            return (_slots as IEnumerable<Slot>).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Slot this[int index] => _slots[index];

        public void ChangeMaxAmount(int maxCapacity)
        {
            if (maxCapacity < 0)
            {
                return;
            }
            
            MaxAmount = maxCapacity;

            for (var i = _slots.Count - 1; i >= MaxAmount; i--)
            {
                _slots.RemoveAt(i);
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
            
            for (var i = _slots.Count - 1; i >= 0; i--)
            {
                if (_slots[i].ItemId != itemId)
                {
                    continue;
                }

                if (amount < _slots[i].Amount)
                {
                    _slots[i].Amount -= amount;

                    return true;
                }

                var newSlotAmount = Mathf.Max(_slots[i].Amount - amount, 0);
                var newAmount = Mathf.Max(amount - _slots[i].Amount, 0);
                
                if (newSlotAmount == 0)
                {
                    RemoveAt(i);
                }
                else
                {
                    _slots[i].Amount = newSlotAmount;
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
            
            for (var i = _slots.Count - 1; i >= 0; i--)
            {
                if (_slots[i].ItemId != itemId)
                {
                    continue;
                }

                amount += _slots[i].Amount;
            }

            return amount;
        }

        public void RemoveAt(int index)
        {
            var slot = _slots[index];
            
            _slots.RemoveAt(index);

            OnSlotRemoved?.Invoke(slot);
        }
        
        private bool TryAddToNewSlot(int itemId, ref int amount)
        {
            if (_slots.Count >= MaxAmount)
            {
                return false;
            }

            var slot = new Slot()
            {
                ItemId = itemId,
                Amount = amount
            };
            
            _slots.Add(slot);

            OnSlotAdded?.Invoke(_slots.Count - 1);
            
            return true;
        }

        private bool TryAddToSlotsWithSameItem(int itemId, ref int amount)
        {
            for (var i = 0; i < _slots.Count; i++)
            {
                var itemIndex = _slots[i].ItemId;

                if (itemIndex.Equals(itemId) == false)
                {
                    continue;
                }

                if (TryAddAmountToSlot(itemId, ref amount, _slots[i]))
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
}
