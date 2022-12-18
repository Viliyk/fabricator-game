using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanTakeDamage : MonoBehaviour
{
    [SerializeField] private SimpleFlash flashEffect = null;
    [SerializeField] private GameObject explosion = null;

    public float HP = 500;
    public Slider healthBar;

    void Start()
    {
        healthBar.maxValue = HP;
    }

    void Update()
    {
        healthBar.value = HP;

        // Destroy this unit when HP reaches 0
        if (HP <= 0)
            Die();
    }

    public void TakeDamage(float amount)
    {
        HP -= amount;

        if (flashEffect != null && HP > 0)
            flashEffect.Flash();
    }

    private void Die()
    {
        if (explosion != null)
        {
            GameObject deathExplosion = Instantiate(explosion, transform.position, Quaternion.identity);
            //deathExplosion.transform.localScale = new Vector3(0.04f, 0.04f, 0.04f);
        }

        UnitSelection.Instance.selectedObjects.Remove(GetComponent<ISelectable>());
        Destroy(gameObject);
    }
}
