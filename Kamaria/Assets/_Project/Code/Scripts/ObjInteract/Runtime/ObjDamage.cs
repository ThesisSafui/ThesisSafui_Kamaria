using Kamaria.Item;
using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.ObjInteract
{
    public sealed class ObjDamage : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private int damagePercentForHp;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IDamageable target)) return;

            int damage = (damagePercentForHp * playerData.Info.Status.MaxHealth) / 100;
            
            target.TakeDamage(damage, EffectAttack.None, transform.position, 0, 0, false, 0, WeaponTypes.None,
                KeyStones.None);
        }
    }
}