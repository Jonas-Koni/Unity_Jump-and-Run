using TMPro;
using UnityEngine;

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

            melonsText.text = "Melons: " + melons + "; level: " + LevelGenerator.CurrentLevel;
        }
    }
    public void Test()
    {
        melonsText.text = "Melons: " + melons + "; level: " + LevelGenerator.CurrentLevel;

    }
}
