using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;
using NUnit.Framework;
using System.Collections.Generic;
public enum StatusEnum
{
    storehouse,
    inShipment,
    museum,

}


[Serializable]
public class ArtWork
{
    public string _id;
    public string artName;
    public string artistName;
    public int year;
    public string status;
    public string loanedTo;

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
public class MuseumArtWorks
{
    public int count;
    public List<ArtWork> artworks;
}

[Serializable]
public class MuseumIoT
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

    public void InitializateArtWork(ArtWork artWorkToCopy)
    {
        
            artWork = new ArtWork();
            artWork._id = artWorkToCopy._id;
            artWork.artName = artWorkToCopy.artName;
            artWork.artistName = artWorkToCopy.artistName;
            artWork.year = artWorkToCopy.year;  
            artWork.status = artWorkToCopy.status;
            artWork.loanedTo = artWorkToCopy.loanedTo;  
        
    }
    public void InitializateIoT(Iot iotToCopy)
    {
        
            iot = new Iot();
            iot.id = iotToCopy.id;
            iot.humidity = iotToCopy.humidity;
            iot.temperature = iotToCopy.temperature;
            iot.timestamp = iotToCopy.timestamp;
            iot.latitude = iotToCopy.latitude;
            iot.longitude = iotToCopy.longitude;
        
    }

    public void HighlightBox()
    {
        MeshRenderer rend = GetComponent<MeshRenderer>();
        Material mat = rend.material;

        mat.EnableKeyword("_EMISSION");

        // add the texture
        Texture mainTex = mat.GetTexture("_MainTex");

        //set color for the light
        Color glowColor = Color.white * 1.5f;

        // Active emission
        mat.SetTexture("_EmissionMap", mainTex);
        mat.SetColor("_EmissionColor", glowColor);
    }

    public void UnhighlightBox()
    {
        MeshRenderer rend = GetComponent<MeshRenderer>();
        Material mat = rend.material;

        // disable emission
        mat.DisableKeyword("_EMISSION");

        // set color
        mat.SetColor("_EmissionColor", Color.black);

        // remove the texture
        mat.SetTexture("_EmissionMap", null);
    }

   
}
