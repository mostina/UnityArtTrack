using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;
using NUnit.Framework;
using System.Collections.Generic;

[Serializable]
public class ArtWorkJson 
{
    
    public string id;
    public string nome;
    public string autore;
    public string anno;
    public bool in_prestito;
    public bool in_magazzino;
}

[Serializable]
public class ArtWork
{
    public string id;
    public string artWorkName;
    public string artWorkAuthor;
    public string dateOfArt;
    public bool onLoan;
    public bool inStoreHouse;

    public void MapFromJson(ArtWorkJson json)
    {
        id = json.id;
        artWorkName = json.nome;
        artWorkAuthor = json.autore;
        dateOfArt = json.anno;
        onLoan = json.in_prestito;
        inStoreHouse = json.in_magazzino;
    }
}
[Serializable]
public class Iot
{
    public string id;
    public float temperature;
    public float humidity;
    public string latitude;
    public string longitude;
    public string timestamp;
}

[Serializable]
public class MuseumContentJson
{
    public int count;
    public List<ArtWorkJson> artworks;
}

[Serializable]
public class IotContent
{
    public int count; 
    public List<Iot> iot_data;
}

public class Box : MonoBehaviour
{  public ArtWork artWork;
    public Iot iot;  
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

   
   

    // Update is called once per frame
    void Update()
    {
       
    }

    public void InitializateArtWork(ArtWorkJson artWorkToCopy)
    {
        if (artWork == null)
            artWork = new ArtWork();
        artWork.MapFromJson(artWorkToCopy);

    }

    public void HighlightBox()
    {
        MeshRenderer rend = GetComponent<MeshRenderer>();
        Material mat = rend.material;

        mat.EnableKeyword("_EMISSION");

        // prendo la texture base del materiale
        Texture mainTex = mat.GetTexture("_MainTex");

        // imposto un colore base per la luce
        Color glowColor = Color.white * 1.5f;

        // attivo l’emissione in modo che usi la texture + il colore
        mat.SetTexture("_EmissionMap", mainTex);
        mat.SetColor("_EmissionColor", glowColor);
    }

    public void UnhighlightBox()
    {
        MeshRenderer rend = GetComponent<MeshRenderer>();
        Material mat = rend.material;

        // disabilita l’emissione
        mat.DisableKeyword("_EMISSION");

        // opzionale: azzera il colore di emissione
        mat.SetColor("_EmissionColor", Color.black);

        // opzionale: rimuovi la texture di emissione
        mat.SetTexture("_EmissionMap", null);
    }

    public void Initialitiate(Iot iotToCopy)
    {
        if (iot == null)
        iot = new Iot();
        iot.id=iotToCopy.id;
        iot.humidity=iotToCopy.humidity;    
        iot.temperature=iotToCopy.temperature;
        iot.timestamp=iotToCopy.timestamp;
        iot.latitude=iotToCopy.latitude;    
        iot.longitude=iotToCopy.longitude;
    }
}
