using System;
using Kamaria.Utilities.PoolingPattern;
using Project.Scripts;
using UnityEngine;
using UnityEngine.VFX;

public enum TagCheck
{
    DamagePlayer,
    DamageEnemy
}

public sealed class SpawnShieldRipples : MonoBehaviour
{
    [SerializeField] private TagCheck tagCheck;

    private VisualEffect shieldRipplesVFX;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag(tagCheck.ToString())) return;
        
        PoolShieldRipples(other);
    }

    private void PoolShieldRipples(Collider other)
    {
        PoolManager.PoolObjectType poolObj = PoolManager.PoolObjectType.ShieldRipplesVFX;
        
        var ripples = PoolManager.Instance.GetPooledObject(poolObj);

        if (!ripples) return;
        
        ripples.transform.position = transform.position;
        ripples.transform.rotation = Quaternion.identity;
        ripples.transform.SetParent(transform);
        ripples.GetComponent<ShieldRipplesVFX>().Init(Vector3.one);
        ripples.SetActive(true);
        shieldRipplesVFX = ripples.GetComponent<VisualEffect>();
        shieldRipplesVFX.SetVector3("SphereCenter", other.transform.position);
    }
}
