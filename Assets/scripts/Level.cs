using UnityEngine;

public class Level : MonoBehaviour
{

    public int LevelId { get; set; }
    public float SpeedCharacter { get; set; }
    public Rigidbody2D Rigidbody { get; set; }
    public Character CharacterScript { get; set; }
    public Vector2 PosStart { get; set; }
    public Vector2 PosEnd { get; set; }

    public virtual void GenerateSection() { } //LevelGenerator Start; CheckLevel
    public virtual void DisplayLevel() { } //GameObject Levels //Player Dead; StartLevels; CheckLevel
    public virtual void UpdateSection() {  } //Fixed Update
    public virtual void RefreshData() { } //CheckLevel
    public virtual void DestroyContent() { } //CheckLevel; Dead Player
}
