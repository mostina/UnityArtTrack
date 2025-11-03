using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SerchArtWork : MonoBehaviour
{
    
   public  GameObject labelArt;
    public List<ArtCard> artCards;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Box box in FindObjectsByType<Box>(FindObjectsSortMode.None))
        {
            GameObject artWorkSpawned = Instantiate(labelArt);
            artWorkSpawned.transform.SetParent(transform, false);

            artWorkSpawned.GetComponentInChildren<TextMeshProUGUI>().text = box.artWork.artWorkName;
            artWorkSpawned.GetComponent<ArtCard>().idArtCard = box.artWork.id;
            artCards.Add(artWorkSpawned.GetComponent<ArtCard>());

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
