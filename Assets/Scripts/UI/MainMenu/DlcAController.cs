using UnityEngine;
using UnityEngine.UI;

public class DlcAController : MonoBehaviour
{
    void OnEnable()
    {
        gameObject.GetComponentInChildren<Dropdown>().value = GameObject.Find("DlcAManager").GetComponent<DlcAManager>().SelectedMaterial;
    }

    public void ChangeGroundMaterial()
    {
        GameObject.Find("DlcAManager").GetComponent<DlcAManager>().SelectedMaterial = gameObject.GetComponentInChildren<Dropdown>().value;
    }

}
