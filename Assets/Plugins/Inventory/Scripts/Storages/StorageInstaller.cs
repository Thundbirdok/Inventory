using Zenject;

namespace Plugins.Inventory.Scripts.Storages
{
    using Plugins.Inventory.Scripts.Item;
    using Plugins.Inventory.Scripts.Item.ItemInterfaces;
    using UnityEngine;

    public class StorageInstaller : MonoInstaller
    {
        [SerializeField]
        private int storageSize;

        public override void InstallBindings()
        {
            var itemsTypesHandler = Container.Resolve<ItemsTypesHandler<IItem>>();
            
            var storage = new ItemsSlotsStorage(storageSize, itemsTypesHandler);
            
            Container.Bind<ItemsSlotsStorage>().FromInstance(storage);
        }
    }
}