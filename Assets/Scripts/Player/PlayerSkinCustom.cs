using UnityEngine;

public class PlayerSkinCustom : MonoBehaviour
{
    [HideInInspector] public int IDHair,
        IDHat,
        IDAcessories,
        IDAcessories2,
        IDAcessories3,
        IDGloves,
        IDShirts,
        IDBottoms,
        IDShoes,
        IDSkin;

    [Header("Meshs of all Objects in Scene")]
    public GameObject[] Hairs;

    public GameObject[] Hats, Acessories1, Acessories2, Acessories3, Gloves, Shirts, Bottoms, Shoes;

    public GameObject BodyObject;
    public Material[] SkinMaterials;


    public void Start()
    {
        LoadIds();
        ChangeHair();
        ChangeHat();
        ChangeAcessories();
        ChangeAcessories2();
        ChangeAcessories3();
        ChangeGloves();
        ChangeShirts();
        ChangeBottoms();
        ChangeShoes();
        ChangeSkin();
    }

    private void LoadIds()
    {
        IDHair = PlayerPrefs.GetInt("HairID");
        IDHat = PlayerPrefs.GetInt("HatID");
        IDAcessories = PlayerPrefs.GetInt("Acessories1ID");
        IDAcessories2 = PlayerPrefs.GetInt("Acessories2ID");
        IDAcessories3 = PlayerPrefs.GetInt("Acessories3ID");
        IDGloves = PlayerPrefs.GetInt("GlovesID");
        IDShirts = PlayerPrefs.GetInt("ShirtsID");
        IDBottoms = PlayerPrefs.GetInt("BottomsID");
        IDShoes = PlayerPrefs.GetInt("ShoesID");
        IDSkin = PlayerPrefs.GetInt("SkinID");
    }

    void ChangeHair()
    {
        for (int i = 0; i < Hairs.Length; i++)
        {
            if (!Hairs[i].activeInHierarchy)
            {
                Hairs[i].SetActive(true);
            }
        }

        for (int i = 0; i < Hairs.Length; i++)
        {
            if (Hairs[i].activeInHierarchy && i != IDHair)
            {
                Hairs[i].SetActive(false);
            }
        }
    }

    void ChangeHat()
    {
        for (int i = 0; i < Hats.Length; i++)
        {
            if (!Hats[i].activeInHierarchy)
            {
                Hats[i].SetActive(true);
            }
        }

        for (int i = 0; i < Hats.Length; i++)
        {
            if (Hats[i].activeInHierarchy && i != IDHat)
            {
                Hats[i].SetActive(false);
            }
        }
    }

    void ChangeAcessories()
    {
        for (int i = 0; i < Acessories1.Length; i++)
        {
            if (!Acessories1[i].activeInHierarchy)
            {
                Acessories1[i].SetActive(true);
            }
        }

        for (int i = 0; i < Acessories1.Length; i++)
        {
            if (Acessories1[i].activeInHierarchy && i != IDAcessories)
            {
                Acessories1[i].SetActive(false);
            }
        }
    }

    void ChangeAcessories2()
    {
        for (int i = 0; i < Acessories2.Length; i++)
        {
            if (!Acessories2[i].activeInHierarchy)
            {
                Acessories2[i].SetActive(true);
            }
        }

        for (int i = 0; i < Acessories2.Length; i++)
        {
            if (Acessories2[i].activeInHierarchy && i != IDAcessories2)
            {
                Acessories2[i].SetActive(false);
            }
        }
    }

    void ChangeAcessories3()
    {
        for (int i = 0; i < Acessories3.Length; i++)
        {
            if (!Acessories3[i].activeInHierarchy)
            {
                Acessories3[i].SetActive(true);
            }
        }

        for (int i = 0; i < Acessories3.Length; i++)
        {
            if (Acessories3[i].activeInHierarchy && i != IDAcessories3)
            {
                Acessories3[i].SetActive(false);
            }
        }
    }

    void ChangeGloves()
    {
        for (int i = 0; i < Gloves.Length; i++)
        {
            if (!Gloves[i].activeInHierarchy)
            {
                Gloves[i].SetActive(true);
            }
        }

        for (int i = 0; i < Gloves.Length; i++)
        {
            if (Gloves[i].activeInHierarchy && i != IDGloves)
            {
                Gloves[i].SetActive(false);
            }
        }
    }

    void ChangeShirts()
    {
        for (int i = 0; i < Shirts.Length; i++)
        {
            if (!Shirts[i].activeInHierarchy)
            {
                Shirts[i].SetActive(true);
            }
        }

        for (int i = 0; i < Shirts.Length; i++)
        {
            if (Shirts[i].activeInHierarchy && i != IDShirts)
            {
                Shirts[i].SetActive(false);
            }
        }
    }

    void ChangeBottoms()
    {
        for (int i = 0; i < Bottoms.Length; i++)
        {
            if (!Bottoms[i].activeInHierarchy)
            {
                Bottoms[i].SetActive(true);
            }
        }

        for (int i = 0; i < Bottoms.Length; i++)
        {
            if (Bottoms[i].activeInHierarchy && i != IDBottoms)
            {
                Bottoms[i].SetActive(false);
            }
        }
    }

    void ChangeShoes()
    {
        for (int i = 0; i < Shoes.Length; i++)
        {
            if (!Shoes[i].activeInHierarchy)
            {
                Shoes[i].SetActive(true);
            }
        }

        for (int i = 0; i < Shoes.Length; i++)
        {
            if (Shoes[i].activeInHierarchy && i != IDShoes)
            {
                Shoes[i].SetActive(false);
            }
        }
    }

    public void ChangeSkin()
    {
        for (int i = 0; i < IDSkin; i++)
        {
            BodyObject.GetComponent<SkinnedMeshRenderer>().material = SkinMaterials[i];
        }
    }
}