using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hints : MonoBehaviour
{

    Dictionary<string, string> hints = new Dictionary<string, string>()
    {
        {"Placing","Use your mouse cursor to place the white ball inside the D shape on the table.\nOnce your happy with the placement press LMB to place it."},
        {"Shooting","Move your mouse left and right to adjust aim, and use scrollwheel to adjust thrust.\nTo shoot press LMB.\nNote what color ball is on"},
        {"Passive","Now wait for the balls to stop moving.\nYou can speed up by pressing LMB."},
        {"Waiting","Relax, it's the other player turn.\nThere's nothing you can do now."}
    };

    public void NewHint(string hintHeader)
    {
        string content;
        if (hints.TryGetValue(hintHeader, out content))
        {
            gameObject.transform.GetChild(0).GetComponent<Text>().text = hintHeader;
            gameObject.transform.GetChild(1).GetComponent<Text>().text = content;
        }
    }

}
