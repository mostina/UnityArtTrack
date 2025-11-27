using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.UIElements;



public class GameManager : MonoBehaviour
{

    //for UI 
    public GameObject labelArt;
    public List<string> artCards;
    public GameObject panelInfo;
    public GameObject panelInventory;
    public TextMeshProUGUI artName;
    public TextMeshProUGUI artist;
    public TextMeshProUGUI year;
    public TextMeshProUGUI temperature;
    public TextMeshProUGUI humidity;
    public TextMeshProUGUI position;
    public TextMeshProUGUI timestamp;
    public RawImage status;
    public Texture storehouseStatus;
    public Texture shippingStatus;
    public Texture museumStatus;
    public RectTransform artParent;
    public RectTransform museumParent;
    public string selectedLabel = "";
    


    //for instantiate
    public MuseumArtWorks museumArtContent;
    public MuseumIoT iotContent;
    public GameObject boxPrefab;
    public List<Transform> listPositions;
    public List<Box> boxes;
    public string databaseArtWorksURL;
    public string databaseIotDataURL;
    public List<string> loanedPeople;

    //for unlight boxes
    public Box boxClicked;

    //for lerp camera - animations
    public Vector3 startCameraPosition;
    public Vector3 saveBoxPosition;
    public Camera cam;
    public bool inputBlocked ;

    private void Awake()
    {
        inputBlocked = false;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //I call the function where i have the corutine for getting datas from db 
        FetchArtWorks();
        
        //I find the camera 
        cam = FindFirstObjectByType<Camera>();

        //here I save the initial position of camera so I know where to put it back
        startCameraPosition = cam.gameObject.transform.position;

       
    }

    //coroutine for getting artWorks data from db
    public void FetchArtWorks()
    {
        StartCoroutine(GetInfoFromDatabase<MuseumArtWorks>(databaseArtWorksURL, (result) =>
        {
            museumArtContent = result; //it's a list of ArtWorks

            //once the coroutine it's done I spawn the boxes
            SpawnArtWork();

            //and I call the coroutine for the iotdata (it's repeating, cause iot data can change)
            InvokeRepeating(nameof(FetchIotData), 0f, 60f);

        }));
    }

   //coroutine per richiamare i dati iot dal db 
    public void FetchIotData()
    {
        StartCoroutine(GetInfoFromDatabase<MuseumIoT>(databaseIotDataURL, (result) =>
        {
            iotContent = result; //it's a list of IoT
            AddIotData();
        }));
    }

    //Add to boxes the iotdata using the same id so it knows which boxes to edit
    private void AddIotData()
    {
       foreach (Box box in boxes)
        {
            foreach (Iot iot in iotContent.iot_data)
            {
                
                if (box.artWork._id == iot.id)
                {
                    
                    box.InitializateIoT(iot);
                }
            }
        }
    }

    //this is the coroutine I use with unity action so I can use for both iot and artWork
    IEnumerator GetInfoFromDatabase<T>(string url, UnityAction <T> callback)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();
        
        T content = JsonUtility.FromJson<T>(webRequest.downloadHandler.text);
        
        callback?.Invoke(content);
    }


    //here I spawn boxes
    public void SpawnArtWork()
    {
        //here I have the list with all the empty GO I did for the boxes positions
        foreach (Transform transform in FindObjectsByType<Position>(FindObjectsSortMode.None).Select(p => p.gameObject.transform))
        {
            listPositions.Add(transform);
        }

        foreach (ArtWork artWork in museumArtContent.artworks)
        {
            if (listPositions.Count > 0)
            {
                GameObject spawnedbox = Instantiate(boxPrefab, listPositions[0].position, listPositions[0].rotation);
                spawnedbox.transform.localScale = listPositions[0].transform.localScale;
                spawnedbox.GetComponentInParent<Box>().UnhighlightBox();
                spawnedbox.GetComponentInParent<Box>().InitializateArtWork(artWork);
                listPositions.Remove(listPositions[0]);
                //I save the box I just spawned in this list of boxes 
                boxes.Add(spawnedbox.GetComponentInParent<Box>());

                //I save the list of museum I loaned to (only once so I don't have double items
                if (!loanedPeople.Contains(artWork.loanedTo)&& artWork.loanedTo !="")
                { loanedPeople.Add(artWork.loanedTo); }
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        

        //right click -> hide info panel if it is active and I unhighlight the box I just checked
        if (Input.GetMouseButtonDown(1)&&panelInfo.activeInHierarchy)
        {
            RightClick();

        }


        //left click -> I do the raycast for checking wich box I clicked and then I open the infoPanel and edit the info
        if (Input.GetMouseButtonDown(0))
        {
            LeftClick();
        }
    }

    public void RightClick()
    {
        panelInfo.GetComponent<PanelAnimationScale>().HidePanel();

        StartCoroutine(PlayAnimations(boxClicked, "close box", () =>
        {
            StartCoroutine(MoveBoxForAnimation(boxClicked, VectorPosition(), saveBoxPosition, 1f, () =>
            {
                StartCoroutine(MoveCamera(boxClicked, cam.transform.position, startCameraPosition, 1f, () =>
                {
                    Resources.FindObjectsOfTypeAll<ButtonInventory>().First().gameObject.SetActive(true);
                    boxClicked.UnhighlightBox();
                    inputBlocked = false;
                }));
            }));
        }));
    }

    public void LeftClick()
    {
        if (inputBlocked)
        { return; }
        Ray ray = FindFirstObjectByType<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit click;
        bool clickedsomething = Physics.Raycast(ray, out click);

        if (clickedsomething && click.collider.gameObject.GetComponentInParent<Box>() && !panelInventory.activeSelf && !panelInfo.activeSelf)
        {
            boxClicked = click.collider.gameObject.GetComponentInParent<Box>();
            Iot boxClickedIot = click.collider.gameObject.GetComponentInParent<Box>().iot;
            FindFirstObjectByType<ButtonInventory>().gameObject.SetActive(false);
            inputBlocked = true;
            StartCoroutine(MoveCamera(boxClicked, startCameraPosition, boxClicked.gameObject.transform.position - Vector3.back * 4f, 1f, () =>
            {
                MoveObjectToCenterDisplay(boxClicked);
                StartCoroutine(PlayAnimations(boxClicked, "move box", () =>
                {
                    panelInfo.GetComponent<PanelAnimationScale>().ShowPanel();
                }));
            }));

            artName.text = boxClicked.artWork.artName;
            artist.text = boxClicked.artWork.artistName;
            year.text = boxClicked.artWork.year.ToString();
            temperature.text = boxClickedIot.temperature.ToString() + "° C";
            humidity.text = boxClickedIot.humidity.ToString() + "%";
            if (boxClicked.artWork.status == StatusEnum.inShipment.ToString())
            {
                status.texture = shippingStatus;
                position.text = boxClickedIot.latitude + "° N  " + boxClickedIot.longitude + "° E" + "\n (" + boxClicked.artWork.loanedTo + ")";
            }
            else if ((boxClicked.artWork.status == StatusEnum.museum.ToString()))
            {
                status.texture = museumStatus;
                position.text = boxClicked.artWork.loanedTo;
            }
            else
            {
                status.texture = storehouseStatus;
                position.text = "IN MAGAZZINO";
            }
            timestamp.text = boxClickedIot.timestamp.ToString();

        }
    }



    //when I select a label (artWork or Museum) so I know which boxes I have to HighlightBox()
    public void SelectBoxWithString()
    {
            foreach (Box box in boxes)
            {

                if (selectedLabel!="" &&(box.artWork.artName==selectedLabel||box.artWork.loanedTo==selectedLabel||box.artWork.status ==selectedLabel))
                {
                        box.HighlightBox();
                Resources.FindObjectsOfTypeAll<ArtCard>().First(x => x.gameObject.name == selectedLabel).ChangeColor(Color.black);  
                }
                
        }
    }

    //I call this function when I change selection while my InventoryPanel is opened
    public void DeselectBoxWithString()
    {
        foreach (Box box in boxes)
        {
            if (box.artWork.artName == selectedLabel || box.artWork.loanedTo == selectedLabel|| box.artWork.status == selectedLabel)
            {
                box.UnhighlightBox();
            }
        }
    }

    //Here i create the Labels for museum and artWorks when I click on "Inventario" Button
    public void CreateLabels()
    {
        if (artCards.Count == 0)
        {
            //create labels for artWorks in panelInfo
            InstantiateLabels(boxes.Select(x=> x.artWork.artName).ToList(), artParent);
            //create labels for museums in panelInfo
            InstantiateLabels(loanedPeople.ToList(), museumParent);
        }
    }
    //the function for instantiate labels 
    public void InstantiateLabels<T>(List<T> items, RectTransform parent)
    {
        GameObject artWorkSpawned;
        foreach (T t in items)
        {
            artWorkSpawned = Instantiate(labelArt, parent, false);
            artWorkSpawned.GetComponent<TextMeshProUGUI>().text = t.ToString();
            artWorkSpawned.name = t.ToString();
            artCards.Add(artWorkSpawned.name);
        }
    }



        
    public IEnumerator MoveCamera(Box targetBox, Vector3 startPos, Vector3 endPos, float duration, System.Action callback)
    {
        float elapsed = 0f;
    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float t = Mathf.SmoothStep(0f, 1f, elapsed / duration); // this is for having a better movement
            cam.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
    }
    cam.transform.position = endPos;
        callback?.Invoke();
    }

   public void MoveObjectToCenterDisplay(Box targetBox)
    {
        saveBoxPosition = targetBox.gameObject.transform.position;
        StartCoroutine(MoveBoxForAnimation(targetBox, targetBox.gameObject.transform.position, VectorPosition(), 1f, () => { }));
    }

    public Vector3 VectorPosition()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        Vector3 worldCenter = Camera.main.ScreenToWorldPoint(new Vector3(screenCenter.x, screenCenter.y - 3, 3));
        return worldCenter;
    }

    public IEnumerator MoveBoxForAnimation(Box targetBox, Vector3 firstPos, Vector3 secondPos, float duration, System.Action callback)
    {
        
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration); // movimento morbido
            targetBox.gameObject.transform.position = Vector3.Lerp(firstPos, secondPos, t);

            yield return null;
        }
       callback?.Invoke();  
    }

   public IEnumerator PlayAnimations(Box targetBox, string animation, System.Action callback)
    {
        Animation anim = targetBox.GetComponent<Animation>();
        anim.Play(animation);
        yield return new WaitForSeconds(anim[animation].length * 1f);
          callback?.Invoke();
    }

    public void putAllBlack()
    {
        foreach (ArtCard card in Resources.FindObjectsOfTypeAll<ArtCard>())
        {
            card.ChangeColor(Color.black);
            card.isBlack = true;
        }
    }

}

        

    



