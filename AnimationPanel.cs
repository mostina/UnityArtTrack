using UnityEngine;
using System.Collections;

public class PanelAnimationScale : MonoBehaviour
{
    public RectTransform panelRect;
    public float duration = 0.3f;

    public void ShowPanel()
    {
        panelRect.gameObject.SetActive(true);
        panelRect.localScale = Vector3.zero;
        StartCoroutine(ScaleIn());
    }

    IEnumerator ScaleIn()
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            panelRect.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, elapsed / duration);
            yield return null;
        }
        panelRect.localScale = Vector3.one;
    }

    public void HidePanel()
    {
        StartCoroutine(ScaleOut());
    }

    IEnumerator ScaleOut()
    {
        float elapsed = 0f;
        Vector3 startScale = panelRect.localScale; // potrebbe essere intermedio se l'animazione è stata interrotta
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            panelRect.localScale = Vector3.Lerp(startScale, Vector3.zero, elapsed / duration);
            yield return null;
        }
        panelRect.localScale = Vector3.zero;
        panelRect.gameObject.SetActive(false);
    }
}
