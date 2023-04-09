namespace Plugins.Inventory.Scripts.InventoryGrid
{
    using System.Linq;
    using Plugins.Inventory.Scripts.Item;
    using Plugins.Inventory.Scripts.Item.ItemInterfaces;
    using Plugins.Inventory.Scripts.Item.ItemTypes;
    using Plugins.Inventory.Scripts.Storages;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;
    using Random = UnityEngine.Random;

    public class InventoryGrid : MonoBehaviour
    {
        [SerializeField]
        private InventoryGridStorage storage;

        [SerializeField]
        private Button fireButton;
        
        [SerializeField]
        private Button addRandomBulletsButton;
        
        [SerializeField]
        private Button addRandomItemButton;

        [SerializeField]
        private Button deleteRandomSlotButton;
        
        [SerializeField]
        private Button addSlotsButton;

        [SerializeField]
        private Button removeSlotsButton;

        private ItemsTypesHandler<BulletItem> _bulletsTypesHandler;
        private ItemsTypesHandler<WeaponItem> _weaponsTypesHandler;
        private ItemsTypesHandler<BodyItem> _bodiesTypesHandler;
        private ItemsTypesHandler<HeadItem> _headsTypesHandler;
        
        [Inject]
        private void Construct
        (
            ItemsSlotsStorage itemsStorage,
            ItemsTypesHandler<IItem> itemTypesHandler,
            ItemsTypesHandler<BulletItem> bulletsTypesHandler,
            ItemsTypesHandler<WeaponItem> weaponsTypesHandler,
            ItemsTypesHandler<BodyItem> bodiesTypesHandler,
            ItemsTypesHandler<HeadItem> headsTypesHandler
        )
        {
            storage.Init(itemsStorage, itemTypesHandler);
            
            _bulletsTypesHandler = bulletsTypesHandler;
            _weaponsTypesHandler = weaponsTypesHandler;
            _bodiesTypesHandler = bodiesTypesHandler;
            _headsTypesHandler = headsTypesHandler;
        }

        private void OnEnable() => Subscribe();

        private void OnDisable() => Unsubscribe();

        private void OnDestroy()
        {
            storage.Dispose();
        }

        private void Subscribe()
        {
            fireButton.onClick.AddListener(FireRandomWeapon);
            addRandomBulletsButton.onClick.AddListener(AddRandomBullets);
            addRandomItemButton.onClick.AddListener(AddRandomItem);
            deleteRandomSlotButton.onClick.AddListener(DeleteRandomSlot);
            addSlotsButton.onClick.AddListener(AddSlots);
            removeSlotsButton.onClick.AddListener(RemoveSlots);
        }

        private void Unsubscribe()
        {
            addSlotsButton.onClick.RemoveListener(AddSlots);
            removeSlotsButton.onClick.RemoveListener(RemoveSlots);
            addRandomItemButton.onClick.RemoveListener(AddRandomItem);
        }

        private void AddSlots() => storage.AddSlots();

        private void RemoveSlots() => storage.RemoveSlots();

        private void AddRandomItem()
        {
            switch (Random.Range(0, 3))
            {
                case 0:
                    
                    var weaponIndex = Random.Range(0, _weaponsTypesHandler.Items.Count);
                    var weapon = _weaponsTypesHandler.Items.ElementAt(weaponIndex);

                    storage.Add(weapon.Key, 1);
                    
                    break;
                
                case 1:
                    
                    var bodyIndex = Random.Range(0, _bodiesTypesHandler.Items.Count);
                    var body = _bodiesTypesHandler.Items.ElementAt(bodyIndex);

                    storage.Add(body.Key, 1);
                    
                    break;
                
                case 2:
                    
                    var headIndex = Random.Range(0, _headsTypesHandler.Items.Count);
                    var head = _headsTypesHandler.Items.ElementAt(headIndex);

                    storage.Add(head.Key, 1);
                    
                    break;
            }
        }

        private void AddRandomBullets()
        {
            var randomIndex = Random.Range(0, _bulletsTypesHandler.Items.Count);
            var item = _bulletsTypesHandler.Items.ElementAt(randomIndex);

            storage.Add(item.Key, item.Value.MaxAmount);
        }
        
        private void FireRandomWeapon()
        {
            var weaponIndex = Random.Range(0, _weaponsTypesHandler.Items.Count);
            var weapon = _weaponsTypesHandler.Items.ElementAt(weaponIndex);
            
            storage.Spend(weapon.Value.BulletsId, 1);
        }

        private void DeleteRandomSlot()
        {
            if (storage.Storage.Count == 0)
            {
                Debug.LogError("All slots empty");
                
                return;
            }
            
            var randomIndex = Random.Range(0, storage.Storage.Count);

            storage.Storage.RemoveAt(randomIndex);
        }
    }
}
