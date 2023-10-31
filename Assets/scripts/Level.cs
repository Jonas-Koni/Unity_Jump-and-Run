using UnityEngine;

public class Level : MonoBehaviour
{

    public int LevelId { get; set; }
    public float SpeedCharacter { get; set; }
    public Rigidbody2D Rigidbody { get; set; }
    public Character CharacterScript { get; set; }
    public Vector2 PosStart { get; set; }
    public Vector2 PosEnd { get; set; }


    public virtual void DisplayLevel(GameObject[] levels, int level, Transform grass) { }
    public virtual void GenerateSection(int seed) { }
    public virtual void UpdateSection() {  }
    public virtual void DestroyContent() { }
}
