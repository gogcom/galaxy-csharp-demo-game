using UnityEngine;
using UnityEngine.SceneManagement;

public class DlcAManager : MonoBehaviour
{
    public Material[] poolTableGround = new Material[2];
    private int selectedMaterial = 0;
    public int SelectedMaterial
    {
        set
        {
            selectedMaterial = value;
            ChangeGroundMaterial();
        }
        get
        {
            return selectedMaterial;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    void ChangeGroundMaterial()
    {
        if (SceneManager.GetActiveScene().name != "Load")
        {
            GameObject.Find("Ground").GetComponent<Renderer>().material = poolTableGround[selectedMaterial];
        }
    }

}
