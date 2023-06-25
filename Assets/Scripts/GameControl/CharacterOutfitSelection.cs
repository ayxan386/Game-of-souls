using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControl
{
    public class CharacterOutfitSelection : MonoBehaviour
    {
        [SerializeField] private Transform startPoint;

        private int currentCharacter;
        private List<GameObject> characters;
 
        public void Selected()
        {
            currentCharacter++;
            characters[currentCharacter].SetActive(true);
            characters[currentCharacter].transform.position = startPoint.position;
        }


      
    }
}