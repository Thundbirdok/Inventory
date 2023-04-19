namespace Plugins.Inventory.Scripts.Slot
{
    using System;
    using Newtonsoft.Json;

    [Serializable]
    
    public class Slot : IEquatable<Slot>, IEquatable<int>
    {
        public event Action OnAmountChanged;

        [JsonProperty("itemId")]
        public int itemId;

        [JsonProperty("amount")]
        private int _amount;
        public int Amount
        {
            get
            {
                return _amount;
            }

            set
            {
                if (_amount == value)
                {
                    return;
                }

                _amount = value;
                
                OnAmountChanged?.Invoke();
            }
        }

        public bool Equals(Slot other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(itemId, other.itemId);
        }

        public bool Equals(int other)
        {
            return Equals(itemId, other);
        }
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() == GetType())
            {
                return Equals((Slot)obj);
            }

            if (obj.GetType() == itemId.GetType())
            {
                return Equals((int)obj);
            }

            return false;
        }

        public override int GetHashCode() => itemId;
    }
}
