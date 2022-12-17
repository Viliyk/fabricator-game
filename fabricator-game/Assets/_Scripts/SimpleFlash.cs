using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFlash : MonoBehaviour
{
    [Tooltip("Material to switch to during the flash.")]
    [SerializeField] private Material flashMaterial;

    [Tooltip("Duration of the flash.")]
    [SerializeField] private float duration;

    // The SpriteRenderer that should flash.
    public SpriteRenderer[] spriteRenderers;

    // The MeshRenderer that should flash.
    public MeshRenderer[] meshRenderers;

    // The material that was in use, when the script started.
    private Material[] originalMaterials = new Material[6];

    // The currently running coroutine.
    private Coroutine flashRoutine;

    void Start()
    {
        // Get the materials that the MeshRenderers and SpriteRenderers use,
        // so we can switch back to it after the flash ended.
        for (int i = 0; i < spriteRenderers.Length; i++)
            originalMaterials[i] = spriteRenderers[i].material;
        for (int i = 0; i < meshRenderers.Length; i++)
            originalMaterials[i] = meshRenderers[i].material;
    }

    public void Flash()
    {
        // If the flashRoutine is not null, then it is currently running.
        if (flashRoutine != null)
        {
            // In this case, we should stop it first.
            // Multiple FlashRoutines the same time would cause bugs.
            StopCoroutine(flashRoutine);
        }

        // Start the Coroutine, and store the reference for it.
        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // Swap to the flashMaterial.
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
                spriteRenderers[i].material = flashMaterial;
        }
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            if (meshRenderers[i] != null)
                meshRenderers[i].material = flashMaterial;
        }

        // Pause the execution of this function for "duration" seconds.
        yield return new WaitForSeconds(duration);

        // After the pause, swap back to the original material.
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
                spriteRenderers[i].material = originalMaterials[i];
        }
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            if (meshRenderers[i] != null)
                meshRenderers[i].material = originalMaterials[i];
        }

        // Set the routine to null, signaling that it's finished.
        flashRoutine = null;
    }
}
