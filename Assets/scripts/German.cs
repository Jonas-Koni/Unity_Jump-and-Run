using System;
using UnityEngine;

public class German : Level
{
    private const int NUMBER_BOOKS = 20;
    public const int NUMBER_BOOK_TYPES = 7;
    private GameObject[] _bookList;

    public override void GenerateSection()
    {
        _bookList = new GameObject[NUMBER_BOOKS];

        for (int id = 0; id < _bookList.Length; id++)
        {
            Book newBookScript;
            GameObject newBookObject;

            System.Type bookType = GetBookType(id);

            newBookObject = new GameObject(GetBookType(id).ToString())
            {
                layer = LayerMask.NameToLayer("ground"),
                tag = "sticky"
            };
            newBookScript = (Book)newBookObject.AddComponent(bookType);
            newBookScript.BookId = id;
            newBookScript.BookType = bookType;
            if (id == 0)
            {
                newBookScript.BookStart = PosStart;
            }
            else
            {
                newBookScript.BookStart = _bookList[id - 1].GetComponent<Book>().BookEnd;
            }
            newBookScript.InitBook();

            _bookList[id] = newBookObject;
        }
        PosEnd = _bookList[_bookList.Length - 1].GetComponent<Book>().BookEnd;
    }

    public override void DestroyContent()
    {
        for (int id = 0; id < _bookList.Length; id++)
        {
            _bookList[id].GetComponent<Book>().DestroyBooks();
        }
    }

    public override void UpdateSection()
    {
        for (int id = 0; id < _bookList.Length; id++)
        {
            System.Type bookType = GetBookType(id);
            ((Book)_bookList[id].GetComponent(bookType)).UpdateBook();
        }
    }

    private System.Type GetBookType(int id)
    {
        if (id == 0)
        {
            return typeof(BookStart);
        }
        if (id == NUMBER_BOOKS - 1)
        {
            return typeof(BookEnd);
        }
        System.Random random = new(LevelGenerator.Seed * id * (int)PosStart.x * 2); //not knowing if works bc StartX same

        int randomBookType = random.Next(1, NUMBER_BOOK_TYPES - 2);
        return randomBookType switch
        {
            0 => typeof(BookStart),
            1 => typeof(BookHorizontalMovement),
            2 => typeof(BookStackVerticalMovement),
            3 => typeof(BookStackDiagonalMovement),
            4 => typeof(BookDrop), //not implemented -> normal book
            5 => typeof(BookOld), //not implemented -> normal book
            6 => typeof(BookEnd), //not implemented -> normal book
            _ => throw new InvalidOperationException()
        };
    }
}
