using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Physic : Level
{
    private const int NUMBER_OBJECTS = 5;
    public GameObject[] PendulumList;

    public override void UpdateSection()
    {
        for (int id = 0; id < PendulumList.Length; id++)
        {
            PendulumList[id].GetComponent<Pendulum>().UpdatePendulum();
        }
    }


    public override void GenerateSection()
    {
        PendulumList = new GameObject[NUMBER_OBJECTS];

        for (int id = 0; id < PendulumList.Length; id++)
        {
            GameObject newPendulumObject = new("Pendulum")
            {
                layer = LayerMask.NameToLayer("ground"),
                tag = "sticky"
            };
            newPendulumObject.transform.parent = this.transform;

            Pendulum newPendulumScript;
            newPendulumScript = (Pendulum) newPendulumObject.AddComponent<Pendulum>();
            newPendulumScript.PendulumId = id;
            newPendulumScript.Physic = this;
            newPendulumScript.GenerateSectionPendulum();
            PendulumList[id] = newPendulumObject;
        }
    }

    public override void DestroyContent()
    {
        for (int id = 0; id < PendulumList.Length; id++)
        {
            PendulumList[id].GetComponent<Pendulum>().DestroyPendulum();
            GameObject[] ListLines = GameObject.FindGameObjectsWithTag("lineToOrigin");
            for (int g = 0; g < ListLines.Length; g++)
            {
                Destroy(ListLines[g]);
            }
        }

    }

}
