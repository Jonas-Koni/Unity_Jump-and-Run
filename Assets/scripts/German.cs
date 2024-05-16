using System;
using UnityEngine;

public class German : Level
{
    private int _numberBooks;
    private GameObject[] _bookList;
    public int _numberBookTypes;


    private void Awake()
    {
        _numberBooks = 20;
        _numberBookTypes = 7;
    }

    public override void GenerateSection()
    {
        _bookList = new GameObject[_numberBooks];

        for (int id = 0; id < _bookList.Length; id++)
        {
            Book newBookScript;
            GameObject newBookObject;

            System.Type bookType = GetBookType(id);

            newBookObject = new GameObject(GetBookType(id).ToString());
            newBookObject.layer = LayerMask.NameToLayer("ground");
            newBookObject.tag = "book";
            newBookObject.tag = "sticky";
            newBookScript = (Book)newBookObject.AddComponent(bookType);
            newBookScript.BookId = id;
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
            //Destroy(_bookList[id]);
        }

    }

    public override void DisplayLevel(int level)
    {

    }

    public override void RefreshData()
    {
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
        if (id == _numberBooks - 1)
        {
            return typeof(BookEnd);
        }
        System.Random random = new System.Random(LevelGenerator.Seed * id * (int)PosStart.x * 2); //not knowing if works bc StartX same

        //    public enum BookType { Start, horizontal, vertical, diagonal, drop, old, End }

        int randomBookType = random.Next(1, _numberBookTypes - 2);
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
