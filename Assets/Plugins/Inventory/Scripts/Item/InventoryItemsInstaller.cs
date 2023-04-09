using Zenject;

namespace Plugins.Inventory.Scripts.Item
{
    using System;
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

        private void GetItems<T, U>(string configName, ConvertFromJsonTokenToItem convert, List<IItem> allItems) where T : IItem
        {
            var bulletsPath = Path.Combine
            (
                _configsFullPath,
                configName + ".json"
            );

            var items =  JsonItemsLoader.LoadItems
            (
                bulletsPath,
                typeof(U),
                convert
            );

            var handler = new ItemsTypesHandler<T>(items.Cast<T>()); 

            Container.Bind<ItemsTypesHandler<T>>().FromInstance(handler).AsCached();
            
            allItems.AddRange(items);
        }

        private IItem BulletConvert(object jsonObject)
        {
            var jsonItem = (BulletJsonItem)jsonObject;
            
            return new BulletItem
            (
                id: jsonItem.id,
                maxAmount: jsonItem.maxAmount,
                weight: jsonItem.weight,
                icon: _spritesHandler.GetSprite(jsonItem.iconName)
            );
        }

        private IItem WeaponsConvert(object jsonObject)
        {
            var jsonItem = (WeaponJsonItem)jsonObject;
            
            return new WeaponItem
            (
                id: jsonItem.id,
                maxAmount: 1,
                damage: jsonItem.damage,
                bulletsId: jsonItem.bulletsId,
                weight: jsonItem.weight,
                icon: _spritesHandler.GetSprite(jsonItem.iconName)
            );
        }

        private IItem BodiesConvert(object jsonObject)
        {
            var jsonItem = (BodyJsonItem)jsonObject;
            
            return new BodyItem
            (
                id: jsonItem.id,
                maxAmount: 1,
                defence: jsonItem.defence,
                weight: jsonItem.weight,
                icon: _spritesHandler.GetSprite(jsonItem.iconName)
            );
        }

        private IItem HeadsConvert(object jsonObject)
        {
            var jsonItem = (HeadJsonItem)jsonObject;

            return new HeadItem
            (
                id: jsonItem.id,
                maxAmount: 1,
                defence: jsonItem.defence,
                weight: jsonItem.weight,
                icon: _spritesHandler.GetSprite(jsonItem.iconName)
            );
        }
    }
}