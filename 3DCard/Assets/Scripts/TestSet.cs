using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSet : MonoBehaviour
{
    public GameObject TestObj;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Func();
        }
    }
    public void Func()
    {
        print("Instantiate之前");
        GameObject testObj = Instantiate(TestObj, new Vector3(0, 0, 0), Quaternion.identity);
        Test test = testObj.GetComponent<Test>();
        test.enabled = true;
        print("Instantiate之后");
    }
}
