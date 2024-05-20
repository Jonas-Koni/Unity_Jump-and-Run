using System;
using UnityEditor.Tilemaps;
using UnityEngine;

public abstract class Book : German
{
    public SpriteRenderer SpriteRenderer;
    public BoxCollider2D BoxCollider;
    public LevelGenerator LevelGeneratorScript;

    public Texture BookTexture;
    public int RandomBookTextureType;

    public int BookId;
    public int Index;
    public System.Type BookType;

    public float MarginTopBook;
    public float PositionMostRightPoint;
    public Vector2 BookStart;
    public Vector2 BookEnd;
    public Vector2 MarginBookStart;

    public const float MARGIN_BOOK = 2f;

    public void InitBook()
    {
        GameObject levelGenerator = GameObject.Find("LevelGenerator");
        LevelGeneratorScript = levelGenerator.GetComponent<LevelGenerator>();

        SpriteRenderer = gameObject.AddComponent<SpriteRenderer>();

        BoxCollider = gameObject.AddComponent<BoxCollider2D>();
        BoxCollider.sharedMaterial = LevelGeneratorScript.MaterialFriction;
        BoxCollider.size = new Vector2(1, 1);

        GenerateSectionBook();
    }
    public void BookMarginTop(int book, Book newScript, GameObject[] stack)
    {
        if (book == 0)
        {
            newScript.MarginTopBook = 0;
            return;
        }
        Book oldScript = stack[book - 1].GetComponent<Book>();
        float marginTop = oldScript.MarginTopBook + oldScript.BookTexture.width * 0.01f / 2 + newScript.BookTexture.width * 0.01f / 2;
        newScript.MarginTopBook = marginTop;

    }
    public abstract void UpdateBook();
    public abstract void GenerateSectionBook();
    public virtual void DestroyBooks()
    {
        Destroy(this.gameObject);
    }
    public virtual void SetBookTexture()
    {
        float localSeed = LevelGenerator.Seed + BookId + Index + (int)BookStart.x * 3;
        UnityEngine.Random.InitState((int)localSeed);

        RandomBookTextureType = (int)RandomConstantSpreadNumber.GetRandomNumber(0, LevelGenerator.BookSprites.Length);
        BookTexture = LevelGenerator.BookSprites[RandomBookTextureType].texture;

        BoxCollider.size = new Vector2(BookTexture.width, BookTexture.height);
        BoxCollider.size *= 0.01f;

        SpriteRenderer.sprite = LevelGenerator.BookSprites[RandomBookTextureType];
    }
    public virtual void SetBoxColliderStackableObject(GameObject[] stack)
    {
        float sumHeightBoxCollider = 0; //90° gedreht 
        float largestBoxColliderY = 0;

        for (int book = 0; book < stack.Length; book++)
        {
            float boxColliderX = stack[book].GetComponent<BoxCollider2D>().size.x;
            sumHeightBoxCollider += boxColliderX;

            float boxColliderY = stack[book].GetComponent<BoxCollider2D>().size.y;
            if (boxColliderY > largestBoxColliderY)
            {
                largestBoxColliderY = boxColliderY;
            }
        }
        float offSetY = stack[0].GetComponent<BoxCollider2D>().size.x * 0.5f - sumHeightBoxCollider * 0.5f;

        BoxCollider.size = new Vector2(largestBoxColliderY, sumHeightBoxCollider);
        BoxCollider.offset = new Vector2(0, offSetY);
    }
    public virtual void SetBookEnd()
    {
        float bookEndWidth = PositionMostRightPoint;// + 2f;
        float bookEndHeight;
        Vector2 positionBook = BookStart + MarginBookStart;
        if (positionBook.y <= 4f)
        {
            bookEndHeight = positionBook.y + RandomConstantSpreadNumber.GetRandomNumber(1f, 2f);
        } 
        else if(positionBook.y >= 8f)
        {
            bookEndHeight = positionBook.y + RandomConstantSpreadNumber.GetRandomNumber(-2f, -1f);
        }
        else
        {
            bookEndHeight = RandomConstantSpreadNumber.GetRandomNumber(-2f, 2f);
        }
        BookEnd = new Vector2(bookEndWidth, bookEndHeight);
    }
    public virtual float GetOscillationVelocity(float amplitude, float frequency)
    {
        return amplitude * Mathf.Sin(LevelGenerator.Time * Time.deltaTime * frequency * 2f * Mathf.PI);
    }
}

#region Start
public class BookStart : Book
{
    private float _amplitude;
    private float _frequencyInHertz;
    public override void UpdateBook()
    {
        gameObject.transform.position = BookStart + MarginBookStart;
        Vector2 moveBook = new(0, GetOscillationVelocity(_amplitude, _frequencyInHertz));
        SpriteRenderer.transform.position += (Vector3)moveBook;
    }
    public override void GenerateSectionBook()
    {
        base.SetBookTexture();

        _amplitude = 2f;
        _frequencyInHertz = 0.2f;

        SpriteRenderer.transform.eulerAngles = new Vector3(0, 0, 90);

        MarginBookStart = new Vector2(MARGIN_BOOK + BoxCollider.size.y * 0.5f, 0);
        PositionMostRightPoint = BookStart.x + MarginBookStart.x + BoxCollider.size.y * 0.5f;

        base.SetBookEnd();
    }
}
#endregion

#region horizontal 
public class BookHorizontalMovement : Book
{
    private float _frequencyInHertz;
    private float _amplitude;
    public override void UpdateBook()
    {
        gameObject.transform.position = BookStart + MarginBookStart;

        Vector2 moveBook = new(GetOscillationVelocity(_amplitude, _frequencyInHertz), 0);
        SpriteRenderer.transform.position += (Vector3)moveBook;
    }
    public override void GenerateSectionBook()
    {
        base.SetBookTexture();

        _frequencyInHertz = RandomPolynomialSpreadNumber.GetRandomNumber(1, 0.04f, 0.3f);
        _amplitude = RandomPolynomialSpreadNumber.GetRandomNumber(1, 2, 4);

        SpriteRenderer.transform.eulerAngles = new Vector3(0, 0, 90);

        MarginBookStart = new Vector2(_amplitude + MARGIN_BOOK + BoxCollider.size.y * 0.5f, 0);
        PositionMostRightPoint = (BookStart.x + MarginBookStart.x + _amplitude + BoxCollider.size.y * 0.5f);

        base.SetBookEnd();
    }
}
#endregion

#region Drop
public class BookDrop : Book //not implemented -> normal book
{
    public override void UpdateBook()
    {
        gameObject.transform.position = BookStart + MarginBookStart;
    }

    public override void GenerateSectionBook()
    {
        base.SetBookTexture();
        SpriteRenderer.transform.eulerAngles = new Vector3(0, 0, 90);

        MarginBookStart = new Vector2(MARGIN_BOOK + BoxCollider.size.y * 0.5f, 0);
        PositionMostRightPoint = (BookStart.x + MarginBookStart.x + BoxCollider.size.y * 0.5f);

        base.SetBookEnd();
    }

}
#endregion

#region old
public class BookOld : Book
{
    public override void UpdateBook()
    {
        gameObject.transform.position = BookStart + MarginBookStart;
    }

    public override void GenerateSectionBook()
    {
        base.SetBookTexture();

        SpriteRenderer.transform.eulerAngles = new Vector3(0, 0, 90);

        MarginBookStart = new Vector2(MARGIN_BOOK + BoxCollider.size.y * 0.5f, 0);
        PositionMostRightPoint = (BookStart.x + MarginBookStart.x + BoxCollider.size.y * 0.5f);

        base.SetBookEnd();
    }
}
#endregion

#region End
public class BookEnd : Book
{
    public override void UpdateBook()
    {
        gameObject.transform.position = BookStart + MarginBookStart;
    }

    public override void GenerateSectionBook()
    {
        base.SetBookTexture();

        SpriteRenderer.transform.eulerAngles = new Vector3(0, 0, 90);

        MarginBookStart = new Vector2(MARGIN_BOOK + BoxCollider.size.y * 0.5f, 0);
        PositionMostRightPoint = (BookStart.x + MarginBookStart.x + BoxCollider.size.y * 0.5f);

        base.SetBookEnd();
    }
}
#endregion

#region StackableObject
public class StackableObject : Book
{
    protected GameObject[] _stack;
    protected int _numberBooks;
    protected System.Type _bookType;

    public override void GenerateSectionBook()
    {
        float localSeed = LevelGenerator.Seed + BookId + (int)BookStart.x * 3;
        UnityEngine.Random.InitState((int)localSeed);

        _numberBooks = (int)(RandomConstantSpreadNumber.GetRandomNumber(2f, 5f));
        _stack = new GameObject[_numberBooks];

        this.GetComponent<BoxCollider2D>().enabled = false;

        for (int book = 0; book < _stack.Length; book++)
        {
            GameObject newBookObject;

            newBookObject = new GameObject(book.ToString())
            {
                layer = LayerMask.NameToLayer("ground"),
                tag = "sticky"
            };
            newBookObject.transform.parent = this.transform;

            Book newBookScript;
            newBookScript = (Book)newBookObject.AddComponent(_bookType);
            newBookScript.Index = book;
            newBookScript.BookId = BookId;
            newBookScript.InitBook();
            newBookScript.BookStart = BookStart;
            newBookScript.BookEnd = BookEnd;
            newBookScript.BookMarginTop(book, newBookScript, _stack);
            _stack[book] = newBookObject;
        }
        SetBoxColliderStackableObject(_stack);
    }

    public override void UpdateBook() 
    {
        for (int book = 0; book < _stack.Length; book++)
        {
            ((Book) _stack[book].GetComponent(_bookType)).UpdateBook();
        }
    }

    public override void DestroyBooks()
    {
        for (int book = 0; book < _stack.Length; book++)
        {
            Destroy(_stack[book]);
        }
        Destroy(this.gameObject);
    }
}
#endregion

#region diagonal
public class BookStackDiagonalMovement : StackableObject
{
    private int _amplitude;
    private float _frequencyInHertz;
    private float _slope;

    private float _amplitudeX;
    private float _amplitudeY;

    public override void UpdateBook()
    {
        base.UpdateBook();
        gameObject.transform.position = BookStart + MarginBookStart;

        Vector2 moveBook = new(GetOscillationVelocity(_amplitudeX, _frequencyInHertz), GetOscillationVelocity(_amplitudeY, _frequencyInHertz));
        SpriteRenderer.transform.position += (Vector3) moveBook;
    }

    public override void GenerateSectionBook()
    {
        _bookType = typeof(BookDiagonalMovement);
        base.GenerateSectionBook();

        _frequencyInHertz = RandomPolynomialSpreadNumber.GetRandomNumber(1, 0.1f, 0.2f);
        _amplitude = (int) RandomPolynomialSpreadNumber.GetRandomNumber(1, 1.5f, 4f);
        _slope = Mathf.Tan(RandomConstantSpreadNumber.GetRandomNumber(0, 2f * Mathf.PI));
        

        _amplitudeX = _amplitude * Mathf.Pow(_slope * _slope + 1, -0.5f);
        _amplitudeY = _slope * _amplitudeX;

        MarginBookStart = new Vector2(_amplitudeX + MARGIN_BOOK + BoxCollider.size.x * 0.5f, 0);
        PositionMostRightPoint = (BookStart.x + MarginBookStart.x + _amplitudeX + BoxCollider.size.x);

        base.SetBookEnd();
    }
}

public class BookDiagonalMovement : BookStackDiagonalMovement
{
    public override void UpdateBook()
    {
        gameObject.transform.localPosition = new Vector3(0, -MarginTopBook, 0);
    }

    public override void GenerateSectionBook()
    {
        base.SetBookTexture();
        SpriteRenderer.transform.eulerAngles = new Vector3(0, 0, 90);

        gameObject.transform.localPosition = new Vector3(0, -MarginTopBook, 0);
    }

}
#endregion

#region vertical
public class BookStackVerticalMovement : StackableObject
{
    private int _amplitude;
    private float _frequencyInHertz;

    public override void UpdateBook()
    {
        base.UpdateBook();
        gameObject.transform.position = BookStart + MarginBookStart;

        Vector2 moveBook = new(0, GetOscillationVelocity(_amplitude, _frequencyInHertz));
        SpriteRenderer.transform.position += (Vector3)moveBook;
    }

    public override void GenerateSectionBook()
    {
        _bookType = typeof(BookVerticalMovement);
        base.GenerateSectionBook();

        _frequencyInHertz = RandomPolynomialSpreadNumber.GetRandomNumber(1, 0.1f, 0.3f);
        _amplitude = (int)(RandomPolynomialSpreadNumber.GetRandomNumber(1, 1f, 3f));

        MarginBookStart = new Vector2(MARGIN_BOOK + BoxCollider.size.x * 0.5f, 0);
        PositionMostRightPoint = (BookStart.x + MarginBookStart.x + BoxCollider.size.x);

        base.SetBookEnd();
    }
}

public class BookVerticalMovement : BookStackVerticalMovement
{
    public override void UpdateBook()
    {
        gameObject.transform.localPosition = new Vector3(0, -MarginTopBook, 0);
    }

    public override void GenerateSectionBook()
    {
        base.SetBookTexture();
        SpriteRenderer.transform.eulerAngles = new Vector3(0, 0, 90);

        gameObject.transform.localPosition = new Vector3(0, -MarginTopBook, 0);
    }
}
#endregion