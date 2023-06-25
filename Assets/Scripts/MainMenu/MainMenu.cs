using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : LocatorSelector
{
    // int i = 1;
    // [SerializeField] private int NPlayers;
    // [SerializeField] private GameObject Character1;
    // [SerializeField] private GameObject Character2;
    // [SerializeField] private GameObject Character3;
    // [SerializeField] private GameObject Character4;
    // [SerializeField] private GameObject ButtonPlay;
    //
    // public List<GameObject> Characters = new List<GameObject>();
    // public List<string> Names = new List<string>();
    //
    // int locale;
    //
    //
    // private void Update()
    // {
    //     locale = GetLocale();
    //     if (ButtonPlay.activeSelf)
    //     {
    //         if (i == NPlayers)
    //         {
    //             if (locale==0){
    //                 ButtonPlay.GetComponent<TMP_Text>().text = "Play";
    //             }
    //             else
    //             {
    //                 ButtonPlay.GetComponent<TMP_Text>().text = "Jugar";
    //             }
    //         }
    //         else
    //         {
    //             if (locale==0)
    //             {
    //                 ButtonPlay.GetComponent<TMP_Text>().text = "Next Player";
    //             }
    //             else
    //             {
    //                 ButtonPlay.GetComponent<TMP_Text>().text = "Siguiente";
    //             }
    //         }
    //     }
    // }
    //
    //
    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
        // if (i == NPlayers)
        // {
        // }
        // else
        // {
        //     i++;
        //     switch (i)
        //     {
        //         case 2:
        //             SetCharacter(Character1);
        //             Character2.SetActive(true);
        //             break;
        //         case 3:
        //             SetCharacter(Character2);
        //             Character3.SetActive(true);
        //             break;
        //         case 4:
        //             SetCharacter(Character3);
        //             Character4.SetActive(true);
        //             break;
        //     }
        // }
    }
    //
    // private void SetCharacter(GameObject Character)
    // {
    //     for(int y=0; y < Character.transform.childCount; y++)
    //     {
    //         if (!Character.transform.GetChild(y).gameObject.activeSelf)
    //         {
    //             Destroy(Character.transform.GetChild(y).gameObject);
    //         }
    //
    //         if (Character.transform.GetChild(y).gameObject.activeSelf && Character.transform.GetChild(y).name != "Canvas")
    //         {
    //             Characters.Add(Character.transform.GetChild(y).gameObject);
    //             Character.transform.GetChild(y).GetChild(0).GetComponent<CharacterController>().enabled = true;
    //             Character.transform.GetChild(y).GetChild(1).gameObject.SetActive(false);
    //             Character.transform.GetChild(y).gameObject.SetActive(false);
    //         }
    //
    //         if (Character.transform.GetChild(y).gameObject.activeSelf && Character.transform.GetChild(y).name == "Canvas")
    //         {
    //             Names.Add(Character.transform.GetChild(y).gameObject.transform.GetChild(0).GetComponent<TMP_InputField>().text);
    //             Character.transform.GetChild(y).gameObject.SetActive(false);
    //             Destroy(Character.transform.GetChild(y).gameObject);
    //         }
    //     }
    //     Character.SetActive(false);
    // }

    public void CloseGame()
    {
        Application.Quit();
    }
}