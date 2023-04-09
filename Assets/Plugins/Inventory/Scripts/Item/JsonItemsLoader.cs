using UnityEngine;

namespace Plugins.Inventory.Scripts.Item
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json.Linq;
    using Plugins.Inventory.Scripts.Item.ItemInterfaces;

    public static class JsonItemsLoader
    {
        public static IEnumerable<IItem> LoadItems
        (
            string jsonPath,
            Type jsonObject,
            InventoryItemsInstaller.ConvertFromJsonTokenToItem convert
        )
        {
            var jArray = GetJToken(jsonPath);

            return jArray?.Select(jToken => jToken.ToObject(jsonObject))
                .Select
                (
                    jsonItem => convert(jsonItem)
                )
                .ToList();
        }
        
        private static JArray GetJToken(string jsonPath)
        {
            try
            {
                var json = File.ReadAllText(jsonPath);

                if (string.IsNullOrEmpty(json))
                {
                    return null;
                }
                
                return JArray.Parse(json);
            }
            catch (FileNotFoundException exception)
            {
                Debug.LogError(exception.Message);
                
                return null;
            }
        }
    }
}
