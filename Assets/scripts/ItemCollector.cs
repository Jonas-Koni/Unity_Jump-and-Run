using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemCollector : MonoBehaviour
{
    private int melons = 0;

    [SerializeField] private TMP_Text melonsText;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("melon"))
        {
            melons++;
            Destroy(collision.gameObject);

            melonsText.text = "Melons: " + melons;
        }
    }
}
