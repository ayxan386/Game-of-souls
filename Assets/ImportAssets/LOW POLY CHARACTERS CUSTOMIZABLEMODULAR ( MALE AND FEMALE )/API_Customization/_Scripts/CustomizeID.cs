using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeID : MonoBehaviour
{
    public int ID;
    [Header ("Type of the object")]
    public bool Hair;
    public bool Hat, Acessories1, Acessories2, Acessories3, Gloves, Shirts, Bottoms, Shoes, SkinColor;
    public CustomizeScript myreference;

    private void Start()
    {
        if (Hair)
        {
            ID = PlayerPrefs.GetInt("HairID");
        }
        if (Hat)
        {
            ID = PlayerPrefs.GetInt("HatID");
        }
        if (Acessories1)
        {
            ID = PlayerPrefs.GetInt("Acessories1ID");
        }
        if (Acessories2)
        {
            ID = PlayerPrefs.GetInt("Acessories2ID");
        }
        if (Acessories3)
        {
            ID = PlayerPrefs.GetInt("Acessories3ID");
        }
        if (Gloves)
        {
            ID = PlayerPrefs.GetInt("GlovesID");
        }
        if (Shirts)
        {
            ID = PlayerPrefs.GetInt("ShirtsID");
        }
        if (Bottoms) 
        { 
            ID = PlayerPrefs.GetInt("BottomsID");
        }
        if (Shoes)
        {
            ID = PlayerPrefs.GetInt("ShoesID");
        }
        if (SkinColor)
        {
            ID = PlayerPrefs.GetInt("SkinID");
        }
    }
    public void Update()
    {
        if (Hair)
        {
            PlayerPrefs.SetInt("HairID", ID);
        }
        if (Hat)
        {
            PlayerPrefs.SetInt("HatID", ID);
        }
        if (Acessories1)
        {
            PlayerPrefs.SetInt("Acessories1ID", ID);
        }
        if (Acessories2)
        {
            PlayerPrefs.SetInt("Acessories2ID", ID);
        }
        if (Acessories3)
        {
            PlayerPrefs.SetInt("Acessories3ID", ID);
        }
        if (Gloves)
        {
            PlayerPrefs.SetInt("GlovesID", ID);
        }
        if (Shirts)
        {
            PlayerPrefs.SetInt("ShirtsID", ID);
        }
        if (Bottoms)
        {
            PlayerPrefs.SetInt("BottomsID", ID);
        }
        if (Shoes)
        {
            PlayerPrefs.SetInt("ShoesID", ID);
        }
        if (SkinColor)
        {
            PlayerPrefs.SetInt("SkinID", ID);
        }
    }
    //THIS PART CONTROLS THE BUTTONS OF CHANGING CLOTHES.
    public void _Next()
    {
        if (Hair && myreference.IDHair < myreference.Hairs.Length)
        {
            ID += 1;
        }
        if (Hat && myreference.IDHat < myreference.Hats.Length)
        {
            ID += 1;
        }
        if (Acessories1 && myreference.IDAcessories < myreference.Acessories1.Length)
        {
            ID += 1;
        }
        if (Acessories2 && myreference.IDAcessories2 < myreference.Acessories2.Length)
        {
            ID += 1;
        }
        if (Acessories3 && myreference.IDAcessories3 < myreference.Acessories3.Length)
        {
            ID += 1;
        }
        if (Gloves && myreference.IDGloves < myreference.Gloves.Length)
        {
            ID += 1;
        }
        if (Shirts && myreference.IDShirts < myreference.Shirts.Length)
        {
            ID += 1;
        }
        if (Bottoms && myreference.IDBottoms < myreference.Bottoms.Length)
        {
            ID += 1;
        }
        if (Shoes && myreference.IDShoes < myreference.Shoes.Length)
        {
            ID += 1;

        }
        if (SkinColor && myreference.IDSkin < myreference.SkinMaterials.Length)
        {
            ID += 1;

        }
    }
    public void _Previous()
    {
        if (Hair && myreference.IDHair >0)
        {
            ID -= 1;
        }
        if (Hat && myreference.IDHat > 0)
        {
            ID -= 1;
        }
        if (Acessories1 && myreference.IDAcessories > 0)
        {
            ID -= 1;
        }
        if (Acessories2 && myreference.IDAcessories2 > 0)
        {
            ID -= 1;
        }
        if (Acessories3 && myreference.IDAcessories3 > 0)
        {
            ID -= 1;
        }
        if (Gloves && myreference.IDGloves > 0)
        {
            ID -= 1;
        }
        if (Shirts && myreference.IDShirts > 0)
        {
            ID -= 1;
        }
        if (Bottoms && myreference.IDBottoms > 0)
        {
            ID -= 1;
        }
        if (Shoes && myreference.IDShoes > 0)
        {
            ID -= 1;
        }
        if (SkinColor && myreference.IDSkin > 0)
        {
            ID -= 1;
        }
    }
}
