using System.Collections;
using UnityEngine;

public class Ingredient : MonoBehaviour, IInteractive
{
    public string ingredientName;
    private Coroutine animationCoroutine;
    private float animationDelay;
    private float animationTime = 1f;

    public void EnableAnimation()
    {
        animationDelay = Random.Range(2.0f, 3.0f);
        animationCoroutine = StartCoroutine(AnimateScale());
    }

    IEnumerator AnimateScale()
    {
        while (true)
        {
            gameObject.LeanScale(Vector3.one * 1.5f, animationTime / 3).setEaseOutCubic();
            gameObject.LeanScale(Vector3.one, animationTime / 3 * 2).setEaseOutBounce().setDelay(animationTime / 3);

            yield return new WaitForSeconds(animationDelay + animationTime);
        }
    }

    public void Interact()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        LeanTween.cancel(gameObject);
        gameObject.SetActive(false);
    }

    public void ToggleCollision(bool value)
    {
        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = value;
        }
    }
}
