using Zenject;

namespace Plugins.Inventory.Scripts.Item
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Plugins.Inventory.Scripts.Item.ItemInterfaces;
    using Plugins.Inventory.Scripts.Item.ItemTypes;
    using Plugins.Inventory.Scripts.Slot;
    using UnityEngine;

    public class InventoryItemsInstaller : MonoInstaller
    {
        public delegate IItem ConvertFromJsonTokenToItem(object jsonObject);
        
        [SerializeField]
        private string configsPath;
        
        [SerializeField]
        private string bulletsConfigName;

        [SerializeField]
        private string weaponsConfigName;
        
        [SerializeField]
        private string bodiesConfigName;
        
        [SerializeField]
        private string headsConfigName;
        
        private SpritesHandler _spritesHandler;

        private string _configsFullPath;
        
        public override void InstallBindings()
        {
            _spritesHandler = Container.Resolve<SpritesHandler>();

            _configsFullPath = Path.Combine
            (
                Application.streamingAssetsPath,
                configsPath
            );

            var allItems = new List<IItem>();
            
            GetItems<BulletItem, BulletJsonItem>(bulletsConfigName, BulletConvert, allItems);
            GetItems<WeaponItem, WeaponJsonItem>(weaponsConfigName, WeaponsConvert, allItems);
            GetItems<BodyItem, BodyJsonItem>(bodiesConfigName, BodiesConvert, allItems);
            GetItems<HeadItem, HeadJsonItem>(headsConfigName, HeadsConvert, allItems);
            
            var handler = new ItemsTypesHandler<IItem>(allItems); 

            Container.Bind<ItemsTypesHandler<IItem>>().FromInstance(handler).AsCached();
        }

        private void GetItems<T, TU>(string configName, ConvertFromJsonTokenToItem convert, List<IItem> allItems) where T : IItem
        {
            var itemsPath = Path.Combine
            (
                _configsFullPath,
                configName + ".json"
            );

            var items =  JsonItemsLoader.LoadItems<TU>
            (
                itemsPath,
                convert
            );

            var enumerable = items.ToList();
            var handler = new ItemsTypesHandler<T>(enumerable.Cast<T>()); 

            Container.Bind<ItemsTypesHandler<T>>().FromInstance(handler).AsCached();
            
            allItems.AddRange(enumerable);
        }

        private IItem BulletConvert(object jsonObject)
        {
            var jsonItem = (BulletJsonItem)jsonObject;
            
            return new BulletItem
            (
                id: jsonItem.Id,
                maxAmount: jsonItem.MaxAmount,
                weight: jsonItem.Weight,
                icon: _spritesHandler.GetSprite(jsonItem.IconName)
            );
        }

        private IItem WeaponsConvert(object jsonObject)
        {
            var jsonItem = (WeaponJsonItem)jsonObject;
            
            return new WeaponItem
            (
                id: jsonItem.Id,
                maxAmount: 1,
                damage: jsonItem.Damage,
                bulletsId: jsonItem.BulletsId,
                weight: jsonItem.Weight,
                icon: _spritesHandler.GetSprite(jsonItem.IconName)
            );
        }

        private IItem BodiesConvert(object jsonObject)
        {
            var jsonItem = (BodyJsonItem)jsonObject;
            
            return new BodyItem
            (
                id: jsonItem.Id,
                maxAmount: 1,
                defence: jsonItem.Defence,
                weight: jsonItem.Weight,
                icon: _spritesHandler.GetSprite(jsonItem.IconName)
            );
        }

        private IItem HeadsConvert(object jsonObject)
        {
            var jsonItem = (HeadJsonItem)jsonObject;

            return new HeadItem
            (
                id: jsonItem.Id,
                maxAmount: 1,
                defence: jsonItem.Defence,
                weight: jsonItem.Weight,
                icon: _spritesHandler.GetSprite(jsonItem.IconName)
            );
        }
    }
}