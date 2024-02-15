using Exiled.API.Features;
using System.Collections.Generic;
using System.Linq;

namespace SCPCosmetics.Types
{
    public abstract class CosmeticHandler
    {
        public Dictionary<string, CosmeticComponent> _playerLinkedCosmetics = new Dictionary<string, CosmeticComponent>();

        public Dictionary<string, CosmeticComponent> PlayerLinkedCosmetics 
        {
            get
            {
                if (_playerLinkedCosmetics is null)
                    _playerLinkedCosmetics = new Dictionary<string, CosmeticComponent>();

                _playerLinkedCosmetics = _playerLinkedCosmetics.Where(Entry => Player.TryGet(Entry.Key, out Player _check) && (Entry.Value != null && !Entry.Value.Destroyed)).ToDictionary(x => x.Key, x => x.Value);

                return _playerLinkedCosmetics;
            } 
            set
            {
                _playerLinkedCosmetics = value;
            }
        }

        public virtual void RegisterCosmetic()
        {
            PlayerLinkedCosmetics = new Dictionary<string, CosmeticComponent>();
            Log.Info($"Registered - {this.GetType()}");
        }

        public virtual void UnregisterCosmetic() 
        {
            foreach (CosmeticComponent cosmeticComponent in PlayerLinkedCosmetics.Values)
            {
                UnityEngine.Object.Destroy(cosmeticComponent);
            }

            PlayerLinkedCosmetics = new Dictionary<string, CosmeticComponent>();
            Log.Info($"Unregistered - {this.GetType()}");
        }

        public virtual void DistributeUpdate()
        {
            foreach (CosmeticComponent cosmeticComponent in PlayerLinkedCosmetics.Values)
            {
                cosmeticComponent.UpdateCosmetic();
            }
        }
    }
}
