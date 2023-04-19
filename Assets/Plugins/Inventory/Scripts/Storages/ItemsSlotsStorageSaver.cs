namespace Plugins.Inventory.Scripts.Storages
{
    using System;
    using System.IO;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using UnityEngine;

    [Serializable]
    public class ItemsSlotsStorageSaver
    {
        [SerializeField]
        private string jsonFileName;

        [NonSerialized]
        private string _path;
        
        public void Save(ItemsSlotsStorage storage)
        {
            using var file = File.CreateText(_path);
            using var writer = new JsonTextWriter(file);
            
            var storageJToken = JToken.FromObject(storage.ToJsonData());
            
            storageJToken.WriteTo(writer);
        }

        public ItemSlotsStorageJson Load()
        {
            _path = Path.Combine(Application.persistentDataPath, jsonFileName + ".json");
            
            var jObject = GetJObject();

            return jObject?.ToObject<ItemSlotsStorageJson>();
        }
        
        private JObject GetJObject()
        {
            try
            {
                if (File.Exists(_path) == false)
                {
                    File.Create(_path);

                    return null;
                }
                
                var json = File.ReadAllText(_path);

                if (string.IsNullOrEmpty(json))
                {
                    return null;
                }

                return JObject.Parse(json);
            }
            catch (FileNotFoundException exception)
            {
                Debug.LogError(exception.Message);
                
                return null;
            }
        }
    }
}
