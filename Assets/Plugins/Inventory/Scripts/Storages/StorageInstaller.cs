using Zenject;

namespace Plugins.Inventory.Scripts.Storages
{
    using System.Collections.Generic;
    using Plugins.Inventory.Scripts.Item;
    using Plugins.Inventory.Scripts.Item.ItemInterfaces;
    using Plugins.Inventory.Scripts.Slot;
    using UnityEngine;

    public class StorageInstaller : MonoInstaller
    {
        [SerializeField]
        private int storageSize;
        
        [SerializeField]
        private ItemsSlotsStorageSaver saver;

        public override void InstallBindings()
        {
            var itemsTypesHandler = Container.Resolve<ItemsTypesHandler<IItem>>();
            
            var storage = GetSlotsStorage(itemsTypesHandler);

            Container.Bind<ItemsSlotsStorage>().FromInstance(storage);
        }

        private void OnDestroy()
        {
            Save();
        }

        private void Save()
        {
            var storage = Container.Resolve<ItemsSlotsStorage>();
            
            saver.Save(storage);
        }

        private ItemsSlotsStorage GetSlotsStorage(ItemsTypesHandler<IItem> itemsTypesHandler)
        {
            var storageJsonData = saver.Load();

            if (storageJsonData == null)
            {
                storageJsonData = new ItemSlotsStorageJson()
                {
                    maxAmount = storageSize,
                    slots = new List<Slot>(storageSize)
                };
            }
            
            var storage = new ItemsSlotsStorage
            (
                storageJsonData.maxAmount,
                storageJsonData.slots,
                itemsTypesHandler
            );
            
            return storage;
        }
    }
}