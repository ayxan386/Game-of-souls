using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Localization.Settings;

public class LocatorSelector : MonoBehaviour
{
    int currentLocale;

    private void Start()
    {
        int ID = PlayerPrefs.GetInt("LocaleKey",0);
        ChangeLocale(ID);
        currentLocale = ID;
    }

    public void ChangeLocale(int localeID)
    {
        StartCoroutine(SetLocale(localeID));
        currentLocale = localeID;
    }

    public int GetLocale()
    {
        return currentLocale;
    }

    IEnumerator SetLocale(int _localeID)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
        PlayerPrefs.SetInt("LocaleKey", _localeID);
        currentLocale = _localeID;
    }
}
