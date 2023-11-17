using UnityEngine;


public abstract class Book : German
{
    public SpriteRenderer SpriteRenderer;
    public BoxCollider2D BoxCollider;

    private GameObject _levelGenerator;
    public LevelGenerator LevelGeneratorScript;

    private PhysicsMaterial2D _materialFriction;

    public int BookId;


    public Vector2 BookStart;
    public Vector2 BookEnd;
    public BookType BookType;


    public void InitBook()
    {
        _levelGenerator = GameObject.Find("LevelGenerator");
        LevelGeneratorScript = _levelGenerator.GetComponent<LevelGenerator>();

        _materialFriction = LevelGeneratorScript.MaterialFriction;

        SpriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        BoxCollider = gameObject.AddComponent<BoxCollider2D>();
        BoxCollider.sharedMaterial = _materialFriction;
        BoxCollider.size = new Vector2(1, 1);

        GenerateSectionBook();
    }

    public void BookMarginTop(int book, BookVertical newScript, GameObject[] stack)
    {
        if (book == 0)
        {
            newScript.MarginTopBook = 0;
            return;
        }
        BookVertical oldScript = stack[book - 1].GetComponent<BookVertical>();
        float marginTop = oldScript.MarginTopBook + oldScript.BookTexture.width * 0.01f / 2 + newScript.BookTexture.width * 0.01f / 2;
        newScript.MarginTopBook = marginTop;

    }
    public abstract void UpdateBook();

    public abstract void GenerateSectionBook();

    public virtual void DestroyBooks()
    {
        Destroy(this.gameObject);
    }
}

#region Start
public class BookStart : Book
{
    private Texture _bookTexture;

    private int _randomBookType;
    private int _amplitude = 2;
    private float _frequency;
    public override void UpdateBook()
    {
        gameObject.transform.position = new Vector3(BookStart.x, BookStart.y, 0);
        SpriteRenderer.sprite = LevelGeneratorScript.BookSprites[_randomBookType];
        BoxCollider.size = new Vector2(_bookTexture.width, _bookTexture.height);
        BoxCollider.size *= 0.01f;
        SpriteRenderer.transform.eulerAngles = new Vector3(0, 0, 90);

        Vector3 moveBook = new Vector3(0, _amplitude * Mathf.Sin(LevelGenerator.Time * 0.01f * _frequency), 0);
        //Debug.Log("Start " +  moveBook);
        SpriteRenderer.transform.position += moveBook;


    }

    public override void GenerateSectionBook()
    {
        float localSeed = LevelGenerator.Seed + BookId + (int)BookStart.x * 3;
        Random.InitState((int)localSeed);

        _randomBookType = (int)(LevelGeneratorScript.BookSprites.Count * Random.value);
        _bookTexture = LevelGeneratorScript.BookSprites[_randomBookType].texture;
        _frequency = Mathf.Abs(Mathf.Sin(Random.value * Mathf.PI)) * 2f + 1f;

        float bookEndHeight;
        if (BookStart.y + _amplitude <= 5f)
        {
            bookEndHeight = BookStart.y + _amplitude - Random.value * 2;
        }
        else
        {
            bookEndHeight = 5f - Random.value * 2;
        }

        BookEnd = new Vector2(BookStart.x + 3f, bookEndHeight);
    }
}
#endregion


#region horizontal
public class BookHorizontal : Book
{
    private Texture _bookTexture;

    private int _randomBookType;
    private float _frequency;

    public override void UpdateBook()
    {
        gameObject.transform.position = new Vector3(BookStart.x, BookStart.y, 0);
        SpriteRenderer.sprite = LevelGeneratorScript.BookSprites[_randomBookType];
        BoxCollider.size = new Vector2(_bookTexture.width, _bookTexture.height);
        BoxCollider.size *= 0.01f;
        SpriteRenderer.transform.eulerAngles = new Vector3(0, 0, 90);

        Vector3 moveBook = new Vector3(3 * Mathf.Sin(LevelGenerator.Time * 0.01f * _frequency) + 4f, 0, 0);
        SpriteRenderer.transform.position += moveBook;
    }



    public override void GenerateSectionBook()
    {
        float localSeed = LevelGenerator.Seed + BookId + (int)BookStart.x * 3;
        Random.InitState((int)localSeed);

        _randomBookType = (int)(LevelGeneratorScript.BookSprites.Count * Random.value);
        _bookTexture = LevelGeneratorScript.BookSprites[_randomBookType].texture;
        _frequency = Mathf.Abs(Mathf.Sin(Random.value * Mathf.PI)) * 2f + 1f;

        float bookEndHeight;
        if (BookStart.y <= 5f)
        {
            bookEndHeight = BookStart.y - Random.value * 2;
        }
        else
        {
            bookEndHeight = 5f - Random.value * 2;
        }

        BookEnd = new Vector2(BookStart.x + 9f, bookEndHeight);
    }

}
#endregion


#region vertical
public class BookVerticalStack : Book
{

    public int _amplitude;
    public float _frequency;
    private int _numberBooks;
    public GameObject[] _stack;
    public Vector2 MoveBook { get; private set; }


    public override void UpdateBook()
    {
        MoveBook = new Vector2(0, _amplitude * Mathf.Sin(LevelGenerator.Time * 0.01f * _frequency));
        for (int book = 0; book < _stack.Length; book++)
        {
            _stack[book].GetComponent<BookVertical>().UpdateBookofBookStack();
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

    public override void GenerateSectionBook()
    {
        float localSeed = LevelGenerator.Seed + BookId + (int)BookStart.x * 3;
        Random.InitState((int)localSeed);

        _numberBooks = (int)(Random.value * 3f + 2f);
        _stack = new GameObject[_numberBooks];

        for (int book = 0; book < _stack.Length; book++)
        {
            Book newBookVerticalScript;
            GameObject newBookVerticalObject;


            newBookVerticalObject = new GameObject(book.ToString());
            newBookVerticalObject.layer = LayerMask.NameToLayer("ground");
            newBookVerticalObject.tag = "book";
            newBookVerticalScript = (Book)newBookVerticalObject.AddComponent<BookVertical>();
            newBookVerticalScript.InitBook();
            newBookVerticalScript.BookId = book;
            newBookVerticalScript.BookStart = BookStart;
            ((BookVertical)newBookVerticalScript).ParentVerticalStack = this.gameObject;
            newBookVerticalScript.BookMarginTop(
                book,
                (BookVertical)newBookVerticalScript,
                _stack);
            _stack[book] = newBookVerticalObject;
        }

        _frequency = Mathf.Abs(Mathf.Sin(Random.value * Mathf.PI)) * 2f + 1f;
        _amplitude = (int)(Mathf.Abs(Mathf.Sin(Random.value * Mathf.PI)) * 2f + 1f);

        float bookEndHeight;
        if (BookStart.y <= 5f)
        {
            bookEndHeight = BookStart.y - Random.value * 2;
        }
        else
        {
            bookEndHeight = 5f - Random.value * 2;
        }

        BookEnd = new Vector2(BookStart.x + 2f, bookEndHeight);
    }

    public void DestroyBookContent()
    {
        for (int book = 0; book < _stack.Length; book++)
        {
            Destroy(_stack[book]);
        }

    }
}

public class BookVertical : BookVerticalStack
{
    public Texture BookTexture;
    private int _randomBookType;
    public float MarginTopBook;
    public GameObject ParentVerticalStack;
    public void UpdateBookofBookStack()
    {
        gameObject.transform.position = new Vector3(BookStart.x, BookStart.y - MarginTopBook + ParentVerticalStack.GetComponent<BookVerticalStack>().MoveBook.y, 0);
        SpriteRenderer.sprite = LevelGeneratorScript.BookSprites[_randomBookType];
        float test = LevelGeneratorScript.BookSprites[0].texture.width;
        BoxCollider.size = new Vector2(BookTexture.width, BookTexture.height);
        BoxCollider.size *= 0.01f;
        SpriteRenderer.transform.eulerAngles = new Vector3(0, 0, 90);

        //Vector3 moveBook = new Vector3(0, _amplitude * Mathf.Sin(LevelGenerator.Time * 0.01f * _frequency), 0);
        //_spriteRenderer.transform.position += moveBook;
    }



    public override void GenerateSectionBook()
    {
        _randomBookType = (int)(LevelGeneratorScript.BookSprites.Count * Random.value);
        BookTexture = LevelGeneratorScript.BookSprites[_randomBookType].texture;

    }


}
#endregion


#region diagonal
public class BookDiagonal : Book
{
    private Texture _bookTexture;

    private int _randomBookType;


    public override void UpdateBook()
    {
        gameObject.transform.position = new Vector3(BookStart.x, BookStart.y, 0);
        SpriteRenderer.sprite = LevelGeneratorScript.BookSprites[_randomBookType];
        BoxCollider.size = new Vector2(_bookTexture.width, _bookTexture.height);
        BoxCollider.size *= 0.01f;
        SpriteRenderer.transform.eulerAngles = new Vector3(0, 0, 90);
    }



    public override void GenerateSectionBook()
    {
        float localSeed = LevelGenerator.Seed + BookId + (int)BookStart.x * 3;
        Random.InitState((int)localSeed);

        _randomBookType = (int)(LevelGeneratorScript.BookSprites.Count * Random.value);
        _bookTexture = LevelGeneratorScript.BookSprites[_randomBookType].texture;


        float bookEndHeight;
        if (BookStart.y <= 5f)
        {
            bookEndHeight = BookStart.y - Random.value * 2;
        }
        else
        {
            bookEndHeight = 5f - Random.value * 2;
        }

        BookEnd = new Vector2(BookStart.x + 2f, bookEndHeight);
    }

}
#endregion


#region Drop
public class BookDrop : Book
{
    private Texture _bookTexture;

    private int _randomBookType;


    public override void UpdateBook()
    {
        gameObject.transform.position = new Vector3(BookStart.x, BookStart.y, 0);
        SpriteRenderer.sprite = LevelGeneratorScript.BookSprites[_randomBookType];
        BoxCollider.size = new Vector2(_bookTexture.width, _bookTexture.height);
        BoxCollider.size *= 0.01f;
        SpriteRenderer.transform.eulerAngles = new Vector3(0, 0, 90);
    }



    public override void GenerateSectionBook()
    {
        float localSeed = LevelGenerator.Seed + BookId + (int)BookStart.x * 3;
        Random.InitState((int)localSeed);

        _randomBookType = (int)(LevelGeneratorScript.BookSprites.Count * Random.value);
        _bookTexture = LevelGeneratorScript.BookSprites[_randomBookType].texture;


        float bookEndHeight;
        if (BookStart.y <= 5f)
        {
            bookEndHeight = BookStart.y - Random.value * 2;
        }
        else
        {
            bookEndHeight = 5f - Random.value * 2;
        }

        BookEnd = new Vector2(BookStart.x + 2f, bookEndHeight);
    }

}
#endregion


#region old
public class BookOld : Book
{
    private Texture _bookTexture;

    private int _randomBookType;


    public override void UpdateBook()
    {
        gameObject.transform.position = new Vector3(BookStart.x, BookStart.y, 0);
        SpriteRenderer.sprite = LevelGeneratorScript.BookSprites[_randomBookType];
        BoxCollider.size = new Vector2(_bookTexture.width, _bookTexture.height);
        BoxCollider.size *= 0.01f;
        SpriteRenderer.transform.eulerAngles = new Vector3(0, 0, 90);
    }



    public override void GenerateSectionBook()
    {
        float localSeed = LevelGenerator.Seed + BookId + (int)BookStart.x * 3;
        Random.InitState((int)localSeed);

        _randomBookType = (int)(LevelGeneratorScript.BookSprites.Count * Random.value);
        _bookTexture = LevelGeneratorScript.BookSprites[_randomBookType].texture;

        float bookEndHeight;
        if (BookStart.y <= 5f)
        {
            bookEndHeight = BookStart.y - Random.value * 2;
        }
        else
        {
            bookEndHeight = 5f - Random.value * 2;
        }

        BookEnd = new Vector2(BookStart.x + 2f, bookEndHeight);
    }

}
#endregion


#region End
public class BookEnd : Book
{
    private Texture _bookTexture;

    private int _randomBookType;


    public override void UpdateBook()
    {
        gameObject.transform.position = new Vector3(BookStart.x, BookStart.y, 0);
        SpriteRenderer.sprite = LevelGeneratorScript.BookSprites[_randomBookType];
        BoxCollider.size = new Vector2(_bookTexture.width, _bookTexture.height);
        BoxCollider.size *= 0.01f;
        SpriteRenderer.transform.eulerAngles = new Vector3(0, 0, 90);
    }



    public override void GenerateSectionBook()
    {
        float localSeed = LevelGenerator.Seed + BookId + (int)BookStart.x * 3;
        Random.InitState((int)localSeed);

        _randomBookType = (int)(LevelGeneratorScript.BookSprites.Count * Random.value);
        _bookTexture = LevelGeneratorScript.BookSprites[_randomBookType].texture;

        float bookEndHeight;
        if (BookStart.y <= 5f)
        {
            bookEndHeight = BookStart.y - Random.value * 2;
        }
        else
        {
            bookEndHeight = 5f - Random.value * 2;
        }

        BookEnd = new Vector2(BookStart.x + 2f, bookEndHeight);
    }

}
#endregion
