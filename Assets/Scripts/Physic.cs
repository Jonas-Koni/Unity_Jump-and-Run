using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Physic : Level
{
    private int _numberObjects;
    public GameObject[] _pendulumList;
    public Rigidbody2D _rigidbody2D;


    private void Awake()
    {
        _numberObjects = 5;
        _rigidbody2D = Rigidbody;
    }

    public override void UpdateSection()
    {
        for (int id = 0; id < _pendulumList.Length; id++)
        {
            _pendulumList[id].GetComponent<Pendulum>().UpdatePendulum2();
        }
    }


    public override void GenerateSection()
    {
        _pendulumList = new GameObject[_numberObjects];

        for (int id = 0; id < _pendulumList.Length; id++)
        {
            Pendulum newPendulumScript;
            GameObject newPendulumObject = new GameObject("Physic");

            newPendulumObject.layer = LayerMask.NameToLayer("ground");
            newPendulumObject.tag = "sticky";
            newPendulumScript = (Pendulum) newPendulumObject.AddComponent<Pendulum>();
            newPendulumScript.PendulumId = id;
            newPendulumScript._physic = this;
            if(id == 0)
            {
                newPendulumScript.PendulumStart = PosStart;
                newPendulumScript.PendulumStart.y += 2f;
            } 
            else
            {
                newPendulumScript.PendulumStart = _pendulumList[id - 1].GetComponent<Pendulum>().PendulumEnd;
            }
            newPendulumScript.RigidBodyScript = Rigidbody;
            newPendulumScript.CharacterScriptScript = CharacterScript;
            newPendulumScript.GenerateSectionPendulum();
            _pendulumList[id] = newPendulumObject;



        }
        PosEnd = _pendulumList[_pendulumList.Length - 1].GetComponent<Pendulum>().PendulumEnd;

    }

    public override void DestroyContent()
    {
        for (int id = 0; id < _pendulumList.Length; id++)
        {
            _pendulumList[id].GetComponent<Pendulum>().DestroyPendulum();
            GameObject[] GoLine = GameObject.FindGameObjectsWithTag("lineToOrigin");
            for (int g = 0; g < GoLine.Length; g++)
            {
                Destroy(GoLine[g]);
            }
            //Destroy(_bookList[id]);
        }

    }

}
