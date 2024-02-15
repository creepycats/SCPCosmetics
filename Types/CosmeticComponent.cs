using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SCPCosmetics.Types
{
    public abstract class CosmeticComponent : MonoBehaviour
    {
        public bool Destroyed = false;

        public virtual void UpdateCosmetic()
        {
            if (!Player.TryGet(gameObject, out Player plr))
            {
                Destroy(this);
            }
        }

        public virtual void OnDestroy()
        {
            Destroyed = true;
        }
    }
}
