using System.Collections;
using TMPro;
using UnityEngine;

namespace Kamaria.UIDamage
{
    public sealed class TextDamage : MonoBehaviour
    {
        [SerializeField] private TextMeshPro textDamage;
        [SerializeField] private Vector3 offset = new Vector3();
        [SerializeField] private float showTime;
        [SerializeField] private Color colorPlayer;
        [SerializeField] private Color colorEnemy;

        private Camera camera;
        
        private void OnEnable()
        {
            camera = Camera.main;
            transform.localPosition += offset;
            StartCoroutine(ShowTime());
        }

        private void OnDisable()
        {
            transform.localPosition -= offset;
        }

        public void Init(int damage, bool isPlayer)
        {
            textDamage.text = damage.ToString();

            textDamage.color = isPlayer ? colorPlayer : colorEnemy;
        }
        
        private void Update()
        {
            transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward,
                camera.transform.rotation * Vector3.up);
        }

        private IEnumerator ShowTime()
        {
            yield return new WaitForSeconds(showTime);
            this.gameObject.SetActive(false);
        }
    }
}