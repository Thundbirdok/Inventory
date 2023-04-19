namespace Plugins.Inventory.Scripts.InventoryGrid
{
    using System;
    using System.Collections.Generic;
    using Plugins.Inventory.Scripts.Slot;
    using Plugins.Inventory.Scripts.Storages;
    using UnityEngine;
    using UnityEngine.Pool;
    using Object = UnityEngine.Object;

    [Serializable]
    public class InventorySlotsSpawner
    {
        public List<SlotProvider> Slots { get; } = new List<SlotProvider>();

        [SerializeField]
        private SlotProvider slotPrefab;

        [SerializeField]
        private Transform container;

        [SerializeField]
        private int addAmount = 15;

        [SerializeField]
        private int poolDefaultCapacity = 30;

        [SerializeField]
        private int poolMaxSize = 300;

        private ObjectPool<SlotProvider> _pool;

        private ItemsSlotsStorage _storage;
        
        public void Init(ItemsSlotsStorage storage)
        {
            _storage = storage;
            
            _pool = new ObjectPool<SlotProvider>
            (
                CreateFunc,
                ActionOnGet,
                ActionOnRelease,
                ActionOnDestroy,
                false,
                poolDefaultCapacity,
                poolMaxSize
            );

            InstantiateSlots(storage.MaxAmount + addAmount);
            ActivateSlots();
        }

        public void AddSlots()
        {
            _storage.ChangeMaxAmount(_storage.MaxAmount + addAmount);
            
            InstantiateSlots(addAmount);
            ActivateSlots();
        }

        public void RemoveSlots()
        {
            if (_storage.MaxAmount == 0)
            {
                return;
            }
            
            _storage.ChangeMaxAmount(_storage.MaxAmount - addAmount);
            
            var newAmount = Slots.Count - addAmount;

            for (var i = Slots.Count - 1; i >= newAmount; i--)
            {
                _pool.Release(Slots[i]);
            }
            
            ActivateSlots();
        }

        private SlotProvider CreateFunc() => Object.Instantiate(slotPrefab, container);

        private void ActionOnGet(SlotProvider obj)
        {
            obj.gameObject.SetActive(true);
            Slots.Add(obj);
        }

        private void ActionOnRelease(SlotProvider obj)
        {
            obj.gameObject.SetActive(false);
            Slots.Remove(obj);
        }

        private void ActionOnDestroy(SlotProvider obj) => Object.Destroy(obj);

        private void InstantiateSlots(int amount)
        {
            var newSlotsAmount = Slots.Count + amount;

            for (var i = Slots.Count; i < newSlotsAmount; i++)
            {
                _pool.Get();
            }
        }
        
        private void ActivateSlots()
        {
            for (var i = 0; i < Slots.Count; i++)
            {
                Slots[i].IsActive = i < _storage.MaxAmount;
            }
        }
    }
}
