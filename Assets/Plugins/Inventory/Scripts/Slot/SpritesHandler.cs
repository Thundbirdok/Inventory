namespace Plugins.Inventory.Scripts.Slot
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Zenject;

    [CreateAssetMenu(fileName = "SpritesHandler", menuName = "Inventory/SpritesHandler")]
    public class SpritesHandler : ScriptableObjectInstaller
    {
        [SerializeField]
        private List<Sprite> sprites = new List<Sprite>();
        public IReadOnlyList<Sprite> Sprites => sprites;

        public override void InstallBindings()
        {
            Container.Bind<SpritesHandler>().FromInstance(this).AsSingle().NonLazy();
        }

        public Sprite GetSprite(string spriteName)
        {
            return sprites.FirstOrDefault(sprite => sprite.name == spriteName);
        }
    }
}
