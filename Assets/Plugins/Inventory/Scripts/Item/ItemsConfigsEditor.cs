using UnityEngine;

namespace Plugins.Inventory.Scripts.Item
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Plugins.Inventory.Scripts.Item.ItemInterfaces;
    using Plugins.Inventory.Scripts.Item.ItemTypes;
    using Plugins.Inventory.Scripts.Slot;
    using UnityEditor;

    public class ItemsConfigsEditor : EditorWindow
    {
        private static readonly GUIContent[] TabLabels =
        {
            new GUIContent("Weapons"),
            new GUIContent("Body"),
            new GUIContent("Head"),
            new GUIContent("Bullets"),
        };

        private List<WeaponJsonItem> _weapons;
        private List<BodyJsonItem> _bodies;
        private List<HeadJsonItem> _heads;
        private List<BulletJsonItem> _bullets;

        private Vector2 _scrollPosition;
        
        private static int _currentTab;

        private static SpritesHandler _spriteHandler;
        private static SpritesHandler SpriteHandler
        {
            get
            {
                if (_spriteHandler != null)
                {
                    return _spriteHandler;
                }

                var guids = AssetDatabase.FindAssets("t:" + typeof(SpritesHandler));

                if (guids.Length <= 0)
                {
                    return null;
                }

                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                _spriteHandler = AssetDatabase.LoadAssetAtPath<SpritesHandler>(path);

                return null;
            }
        }

        [MenuItem("Items/ConfigEditor")]
        private static void OpenWindow()
        {
            GetWindow<ItemsConfigsEditor>("ItemsConfigEditor");
        }

        private void OnEnable()
        {
            LoadItems();
        }

        private void LoadItems()
        {
            _weapons = LoadJsonFiles<WeaponJsonItem>("Weapons.json");

            _bodies = LoadJsonFiles<BodyJsonItem>("Bodies.json");

            _heads = LoadJsonFiles<HeadJsonItem>("Heads.json");

            _bullets = LoadJsonFiles<BulletJsonItem>("Bullets.json");
        }

        private void OnGUI()
        {
            GUILayout.Label("Item Editor", EditorStyles.boldLabel);
            
            var newTab = GUILayout.Toolbar(_currentTab, TabLabels);

            if (newTab != _currentTab)
            {
                _scrollPosition = Vector2.zero;

                _currentTab = newTab;
            }

            switch (_currentTab)
            {
                case 0:

                    DrawItems(_weapons);

                    break;

                case 1:

                    DrawItems(_bodies);

                    break;
                
                case 2:

                    DrawItems(_heads);

                    break;

                case 3:

                    DrawItems(_bullets);

                    break;
            }
        }

        private void OnInspectorUpdate() => Repaint();

        private void DrawItems<T>(IReadOnlyCollection<T> items) where T : class, IJsonItem
        {
            if (items == null || items.Count == 0)
            {
                EditorGUILayout.HelpBox("No items found", MessageType.Info);

                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        
            foreach (var item in items)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);

                item.Id = EditorGUILayout.IntField("ID", item.Id);
                
                if (SpriteHandler)
                {
                    if (item is IJsonItemWithIcon itemWithIcon)
                    {
                        var foundedSprite = SpriteHandler.GetSprite(itemWithIcon.IconName);
                        
                        var sprite = EditorGUILayout.ObjectField
                        (
                            "Sprite",
                            foundedSprite,
                            typeof(Sprite),
                            false
                        ) as Sprite;
                    
                        var iconName = sprite != null ? sprite.name : "";
                        itemWithIcon.IconName = EditorGUILayout.TextField("Icon Name", iconName);
                    }
                }

                if (item is IJsonItemWithWeight itemWithWeight)
                {
                    itemWithWeight.Weight = EditorGUILayout.FloatField("Weight", itemWithWeight.Weight);
                }

                if (item is IJsonWeaponItem weaponItem)
                {
                    weaponItem.BulletsId = EditorGUILayout.IntField("Bullets ID", weaponItem.BulletsId);
                    weaponItem.Damage = EditorGUILayout.IntField("Damage", weaponItem.Damage);
                }

                if (item is IJsonLimitedStackItem limitedStackItem)
                {
                    limitedStackItem.MaxAmount = EditorGUILayout.IntField("Max Amount", limitedStackItem.MaxAmount);
                }

                if (item is IJsonArmorItem armorItem)
                {
                    armorItem.Defence = EditorGUILayout.IntField("Defence", armorItem.Defence);
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Save"))
            {
                SaveCurrentTab();
            }
        }

        private void SaveCurrentTab()
        {
            switch (_currentTab)
            {
                case 0:

                    SaveItems(_weapons, "Weapons.json");

                    break;

                case 1:

                    SaveItems(_bodies, "Bodies.json");

                    break;

                case 2:

                    SaveItems(_heads, "Heads.json");

                    break;

                case 3:

                    SaveItems(_bullets, "Bullets.json");

                    break;
            }
        }

        private static List<T> LoadJsonFiles<T>(string fileName) where T : IJsonItem
        {
            var items = new List<T>();

            var path = GetPath(fileName);

            if (!File.Exists(path))
            {
                Debug.LogErrorFormat("File not found: {0}", path);

                return items;
            }

            var json = File.ReadAllText(path);
            var itemsInFile = JArray.Parse(json).ToObject<T[]>();
            items.AddRange(itemsInFile);

            return items;
        }

        private static void SaveItems(IEnumerable items, string fileName)
        {
            var json = JsonConvert.SerializeObject(items, Formatting.Indented);

            var path = GetPath(fileName);

            File.WriteAllText(path, json);
        }

        private static string GetPath(string fileName)
        {
            return Path.Combine
            (
                Application.streamingAssetsPath,
                "Items",
                "Configs",
                fileName
            );
        }
    }
}
