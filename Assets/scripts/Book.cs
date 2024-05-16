using UnityEngine;


public abstract class Book : German
{
    public SpriteRenderer SpriteRenderer;
    public BoxCollider2D BoxCollider;

    private GameObject _levelGenerator;
    public LevelGenerator LevelGeneratorScript;

    private PhysicsMaterial2D _materialFriction;

    public int BookId;
    public float MarginTopBook;
    public Texture BookTexture;

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
}

#region Start
public class BookStart : Book
{
    private Texture _bookTexture;

    private int _randomBookType;
    private float _amplitude;
    private float _frequency;
    public override void UpdateBook()
    {
        gameObject.transform.position = new Vector3(BookStart.x, BookStart.y, 0);
        SpriteRenderer.sprite = LevelGeneratorScript.BookSprites[_randomBookType];
        BoxCollider.size = new Vector2(_bookTexture.width, _bookTexture.height);
        BoxCollider.size *= 0.01f;
        SpriteRenderer.transform.eulerAngles = new Vector3(0, 0, 90);

        Vector3 moveBook = new Vector3(0, _amplitude * Mathf.Sin(LevelGenerator.Time * 0.01f * _frequency), 0);
        SpriteRenderer.transform.position += moveBook;


    }

    public override void GenerateSectionBook()
    {
        float localSeed = LevelGenerator.Seed + BookId + (int)BookStart.x * 3;
        Random.InitState((int)localSeed);

        _amplitude = 2f;
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
public class BookHorizontalMovement : Book
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
        if (BookStart.y <= 5f && BookStart.y >= 1f)
        {
            bookEndHeight = BookStart.y - Random.value * 2;
        }
        else
        {
            bookEndHeight = Random.value * 3;
        }

        BookEnd = new Vector2(BookStart.x + 9f, bookEndHeight);
    }

}
#endregion


#region vertical
public class BookStackVerticalMovement : Book
{
    public GameObject[] Stack;
    public int Amplitude;
    public float Frequency;
    public Vector2 MoveBook;

    private int _numberBooks;


    public override void UpdateBook()
    {
        MoveBook = new Vector2(0, Amplitude * Mathf.Sin(LevelGenerator.Time * 0.01f * Frequency));
        for (int book = 0; book < Stack.Length; book++)
        {
            Stack[book].GetComponent<BookVerticalMovement>().UpdateBookofBookStack();
        }
    }

    public override void DestroyBooks()
    {
        for (int book = 0; book < Stack.Length; book++)
        {
            Destroy(Stack[book]);
        }
        Destroy(this.gameObject);
    }

    public override void GenerateSectionBook()
    {
        float localSeed = LevelGenerator.Seed + BookId + (int)BookStart.x * 3;
        Random.InitState((int)localSeed);

        _numberBooks = (int)(Random.value * 3f + 2f);
        Stack = new GameObject[_numberBooks];

        for (int book = 0; book < Stack.Length; book++)
        {
            Book newBookVerticalScript;
            GameObject newBookVerticalObject;


            newBookVerticalObject = new GameObject(book.ToString());
            newBookVerticalObject.layer = LayerMask.NameToLayer("ground");
            newBookVerticalObject.tag = "book";
            newBookVerticalScript = (Book)newBookVerticalObject.AddComponent<BookVerticalMovement>();
            newBookVerticalScript.InitBook();
            newBookVerticalScript.BookId = book;
            newBookVerticalScript.BookStart = BookStart;
            ((BookVerticalMovement)newBookVerticalScript).ParentVerticalStack = this.gameObject;
            newBookVerticalScript.BookMarginTop(
                book,
                (BookVerticalMovement)newBookVerticalScript,
                Stack);
            Stack[book] = newBookVerticalObject;
        }

        Frequency = Mathf.Abs(Mathf.Sin(Random.value * Mathf.PI)) * 2f + 1f;
        Amplitude = (int)(Mathf.Abs(Mathf.Sin(Random.value * Mathf.PI)) * 2f + 1f);

        float bookEndHeight;
        if (BookStart.y <= 5f && BookStart.y >= 1f)
        {
            bookEndHeight = BookStart.y - Random.value * 2;
        }
        else
        {
            bookEndHeight = Random.value * 3;
        }

        BookEnd = new Vector2(BookStart.x + 3f, bookEndHeight);
    }

    public void DestroyBookContent()
    {
        for (int book = 0; book < Stack.Length; book++)
        {
            Destroy(Stack[book]);
        }

    }
}

public class BookVerticalMovement : BookStackVerticalMovement
{
    public GameObject ParentVerticalStack;

    private int _randomBookType;
    public void UpdateBookofBookStack()
    {
        gameObject.transform.position = new Vector3(BookStart.x, BookStart.y - MarginTopBook + ParentVerticalStack.GetComponent<BookStackVerticalMovement>().MoveBook.y, 0);
        SpriteRenderer.sprite = LevelGeneratorScript.BookSprites[_randomBookType];
        float test = LevelGeneratorScript.BookSprites[0].texture.width;
        BoxCollider.size = new Vector2(BookTexture.width, BookTexture.height);
        BoxCollider.size *= 0.01f;
        SpriteRenderer.transform.eulerAngles = new Vector3(0, 0, 90);
    }



    public override void GenerateSectionBook()
    {
        _randomBookType = (int)(LevelGeneratorScript.BookSprites.Count * Random.value);
        BookTexture = LevelGeneratorScript.BookSprites[_randomBookType].texture;
    }
}
#endregion


#region diagonal
public class BookStackDiagonalMovement : Book
{
    public GameObject[] Stack;
    public int Amplitude;
    public float Frequency;
    public float Slope;
    public Vector2 MoveBook;

    private int _numberBooks;
    public float DeltaX;
    public float DeltaY;


    public override void UpdateBook()
    {
        float relativePosition = Mathf.Sin(LevelGenerator.Time * 0.01f * Frequency);
        MoveBook = new Vector2(DeltaX * relativePosition, DeltaY * relativePosition);
        for (int book = 0; book < Stack.Length; book++)
        {
            Stack[book].GetComponent<BookDiagonalMovement>().UpdateBookofBookStack();
        }
    }

    public override void DestroyBooks()
    {
        for (int book = 0; book < Stack.Length; book++)
        {
            Destroy(Stack[book]);
        }
        Destroy(this.gameObject);
    }

    public override void GenerateSectionBook()
    {
        float localSeed = LevelGenerator.Seed + BookId + (int)BookStart.x * 3;
        Random.InitState((int)localSeed);

        _numberBooks = (int)(Random.value * 3f + 2f);
        Stack = new GameObject[_numberBooks];

        for (int book = 0; book < Stack.Length; book++)
        {
            Book newBookDiagonalMovementScript;
            GameObject newBookDiagonalMovementObject;


            newBookDiagonalMovementObject = new GameObject(book.ToString())
            {
                layer = LayerMask.NameToLayer("ground"),
                tag = "book"
            };
            newBookDiagonalMovementScript = (Book)newBookDiagonalMovementObject.AddComponent<BookDiagonalMovement>();
            newBookDiagonalMovementScript.InitBook();
            newBookDiagonalMovementScript.BookId = book;
            newBookDiagonalMovementScript.BookStart = BookStart;
            ((BookDiagonalMovement)newBookDiagonalMovementScript).ParentVerticalStack = this.gameObject;
            ((BookDiagonalMovement)newBookDiagonalMovementScript).BookStackScript = this.gameObject.GetComponent<BookStackDiagonalMovement>();
            newBookDiagonalMovementScript.BookMarginTop(
                book,
                newBookDiagonalMovementScript,
                Stack);
            Stack[book] = newBookDiagonalMovementObject;
        }

        Frequency = Mathf.Abs(Mathf.Sin(Random.value * Mathf.PI)) * 2f + 1f;
        Amplitude = (int)(Mathf.Abs(Mathf.Sin(Random.value * Mathf.PI)) * 2.5f + 1.5f);
        Slope = Mathf.Tan(Random.value * 2f * Mathf.PI);

        float bookEndHeight;
        if (BookStart.y <= 5f && BookStart.y >= 1f)
        {
            bookEndHeight = BookStart.y - Random.value * 2;
        }
        else
        {
            bookEndHeight = Random.value * 3;
        }
        DeltaX = Amplitude * Mathf.Pow(Slope * Slope + 1, -0.5f);
        DeltaY = Slope * DeltaX;

        BookEnd = new Vector2(BookStart.x + 3f + 2*DeltaX, bookEndHeight);
    }

    public void DestroyBookContent()
    {
        for (int book = 0; book < Stack.Length; book++)
        {
            Destroy(Stack[book]);
        }

    }
}

public class BookDiagonalMovement : BookStackDiagonalMovement
{
    public GameObject ParentVerticalStack;

    private int _randomBookType;
    public BookStackDiagonalMovement BookStackScript;
    public void UpdateBookofBookStack()
    {
        gameObject.transform.position = new Vector3((BookStart.x + BookStackScript.DeltaX) + (BookStackScript.MoveBook.x), (BookStart.y - MarginTopBook) + (BookStackScript.MoveBook.y), 0);
        SpriteRenderer.sprite = LevelGeneratorScript.BookSprites[_randomBookType];
        BoxCollider.size = new Vector2(BookTexture.width, BookTexture.height);
        BoxCollider.size *= 0.01f;
        SpriteRenderer.transform.eulerAngles = new Vector3(0, 0, 90);
    }



    public override void GenerateSectionBook()
    {
        _randomBookType = (int)(LevelGeneratorScript.BookSprites.Count * Random.value);
        BookTexture = LevelGeneratorScript.BookSprites[_randomBookType].texture;
    }

}
#endregion


#region Drop
public class BookDrop : Book //not implemented -> normal book
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
        if (BookStart.y <= 5f && BookStart.y >= 1f)
        {
            bookEndHeight = BookStart.y - Random.value * 2;
        }
        else
        {
            bookEndHeight = Random.value * 3;
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
        if (BookStart.y <= 5f && BookStart.y >= 1f)
        {
            bookEndHeight = BookStart.y - Random.value * 2;
        }
        else
        {
            bookEndHeight = Random.value * 3;
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
        if (BookStart.y <= 5f && BookStart.y >= 1f)
        {
            bookEndHeight = BookStart.y - Random.value * 2;
        }
        else
        {
            bookEndHeight = Random.value * 3;
        }

        BookEnd = new Vector2(BookStart.x + 2f, bookEndHeight);
    }

}
#endregion
