using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

[Serializable]
public class Positions
{
    public Vector3 position;
}

public class GameManager : MonoBehaviour
{
    public MuseumContentJson museumArtContent;
    public IotContent iotContent;
    public GameObject boxPrefab;
    public List<Transform> listPositions;
    public List<Box> boxes;
    public SelectedCard selectedCard;
    public GameObject panelInfo;
    public GameObject panelInventory;
    public string databaseArtWorksURL;
    public string databaseIotDataURL;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FetchArtWorks();
    }

    public void FetchArtWorks()
    {
        StartCoroutine(GetInfoFromDatabase<MuseumContentJson>(databaseArtWorksURL, (result) =>
        {
            museumArtContent = result;
            SpawnaArtWork();
            InvokeRepeating(nameof(FetchIotData), 0f, 60f);
        }));
    }
   

   
    public void FetchIotData()
    {
        StartCoroutine(GetInfoFromDatabase<IotContent>(databaseIotDataURL, (result) =>
        {
            iotContent = result;
            AddIotData();
        }));
    }

    private void AddIotData()
    {
       foreach (Box box in boxes)
        {
            foreach (Iot iot in iotContent.iot_data)
            {
                
                if (box.artWork.id == iot.id)
                {
                    
                    box.Initialitiate(iot);
                }
            }
        }
    }

    IEnumerator GetInfoFromDatabase<T>(string url, UnityAction <T> callback)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();
        
        T content = JsonUtility.FromJson<T>(webRequest.downloadHandler.text);
        
        callback?.Invoke(content);
        
    }

    public void SpawnaArtWork()
    {
        
        foreach (Transform transform in FindObjectsByType<Position>(FindObjectsSortMode.None).Select(p => p.gameObject.transform))
        {
            listPositions.Add(transform);
        }


        foreach (ArtWorkJson artWork in museumArtContent.artworks)
        {
            if (listPositions.Count > 0)
            {
                GameObject spawnedbox = Instantiate(boxPrefab, listPositions[0].position, listPositions[0].rotation);
                spawnedbox.GetComponent<Box>().InitializateArtWork(artWork);
                listPositions.Remove(listPositions[0]);
                boxes.Add(spawnedbox.GetComponent<Box>());
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        

            if (Input.GetMouseButtonDown(1))
        {
            if (selectedCard.idSelectedCard != "")
            {
                SelectBoxWithId();
                selectedCard.idSelectedCard = "";
            }
            panelInfo.GetComponent<PanelAnimationScale>().HidePanel();
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = FindFirstObjectByType<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit click;
            bool clickedsomething = Physics.Raycast(ray, out click);
            
            if (clickedsomething && click.collider.gameObject.GetComponent<Box>()&&!panelInventory.activeSelf&& !panelInfo.activeSelf)
            {
                Box boxClicked = click.collider.gameObject.GetComponent<Box>();
                panelInfo.GetComponent<PanelAnimationScale>().ShowPanel();
                panelInfo.GetComponentInChildren<TextMeshProUGUI>().text = "Nome Opera: " + boxClicked.artWork.artWorkName + "\n" + "\n" + "Nome Autore: " + boxClicked.artWork.artWorkAuthor + "\n" + "\n" + "Data Creazione: " + boxClicked.artWork.dateOfArt + "\n" + "\n" + "Temperatura Ambiente: "  + boxClicked.iot.temperature+" °C"+"\n" + "\n" + "UmiditA': "+boxClicked.iot.humidity +"%" + "\n"+ "\n" + (boxClicked.artWork.inStoreHouse ? "Posizione GPS: in magazzino": "Posizione GPS: " +"\n" + boxClicked.iot.latitude+ "° N, "+"\n"+boxClicked.iot.longitude+"° E")+"\n" +"\n" + "Stato Prestito: " + (boxClicked.artWork.onLoan? "ON" : "OFF") + "\n";
            }
        }
    }

    public void SelectBoxWithId()
    {
        foreach (Box box in boxes)
        {
            if (box.artWork.id == selectedCard.idSelectedCard)
            {
                box.UnhighlightBox();
            }
        }
    }

    public void FindTheBox()
    {
        foreach (Box box in boxes)
        {
            if (box.artWork.id == selectedCard.idSelectedCard)
            {
                box.HighlightBox();
                

                foreach (ArtCard artCard in Resources.FindObjectsOfTypeAll<ArtCard>())
                {
                    if (artCard.idArtCard == box.artWork.id)
                    {
                        artCard.DeselectArtCard();
                    }
                }

            }

        }

    }
}
