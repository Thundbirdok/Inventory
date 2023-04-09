namespace Plugins.Inventory.Scripts.Slot
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class SlotProvider : MonoBehaviour
    {
        private bool _isActive;
        public bool IsActive
        {
            get
            {
                return _isActive;
            }

            set
            {
                _isActive = value;
                
                activeGroup.interactable = _isActive;
                activeGroup.alpha = _isActive ? activeAlpha : inactiveAlpha;

                if (_isActive == false)
                {
                    SetEmpty();
                }
            }
        }

        public Slot Slot { get; private set; }
        
        public bool IsEmpty { get; private set; } = true;


        [SerializeField]
        private Image icon;

        [SerializeField]
        private TextMeshProUGUI amount;

        [SerializeField]
        private CanvasGroup activeGroup;

        [SerializeField]
        private float activeAlpha = 1f;

        [SerializeField]
        private float inactiveAlpha = 0.3f;

        private void OnEnable()
        {
            Subscribe();
                
            UpdateAmount();
        }

        private void OnDisable() => Unsubscribe();

        public void SetSlot(Slot slot, Sprite itemIcon)
        {
            Unsubscribe();
            
            Slot = slot;

            IsEmpty = false;
            
            if (gameObject.activeSelf)
            {
                Subscribe();
                
                UpdateAmount();
            }

            icon.sprite = itemIcon;
        }

        public void SetEmpty()
        {
            IsEmpty = true;

            Unsubscribe();
            
            Slot = null;
            
            icon.sprite = null;
            amount.text = "";
        }

        private void Subscribe()
        {
            if (IsEmpty)
            {
                return;
            }
            
            Slot.OnAmountChanged += UpdateAmount;
        }

        private void Unsubscribe()
        {
            if (IsEmpty)
            {
                return;
            }
            
            Slot.OnAmountChanged -= UpdateAmount;
        }
        
        private void UpdateAmount()
        {
            if (IsEmpty)
            {
                amount.text = "";
                
                return;
            }

            if (Slot.Amount == 1)
            {
                amount.text = "";
                
                return;
            }
            
            amount.text = Slot.Amount.ToString();
        }
    }
}
