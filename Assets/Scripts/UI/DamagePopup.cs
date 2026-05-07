using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public TextMeshPro textComponent;
    private float _timer = 0f;
    private float _lifetime = 1.5f;

    void Update()
    {
        _timer += Time.deltaTime;

        transform.position += Vector3.up * Time.deltaTime * 1.5f;

        transform.LookAt(transform.position + Camera.main.transform.forward);

        if (textComponent != null)
        {
            Color c = textComponent.color;
            c.a = 1f - (_timer / _lifetime);
            textComponent.color = c;
        }
    }
}