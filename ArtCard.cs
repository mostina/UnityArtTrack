using System.Linq;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ArtCard : MonoBehaviour
{
    public string selected;
    public bool isBlack=true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selected = FindFirstObjectByType<GameManager>().selectedLabel;
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    //When I select the museum or the art work from the InventoryPanel
    public void SelectArtCard()
    {
        selected = FindFirstObjectByType<GameManager>().selectedLabel;

        //I skip the following "if" only the first time I select something from the InventoryPanel
        if (selected!="")
        {
            foreach (ArtCard card in FindObjectsByType<ArtCard>(FindObjectsSortMode.None))
            {
                if (card.gameObject.name == selected)
                {
                    FindFirstObjectByType<GameManager>().DeselectBoxWithString();
                    card.ChangeColor(Color.black);
                }
            }
        }
        
        
            FindFirstObjectByType<GameManager>().selectedLabel = this.name;
            
            FindFirstObjectByType<GameManager>().SelectBoxWithString();

            if (this.GetComponent<ArtCard>().isBlack)
            {
                this.ChangeColor(Color.blue);
            }
           else{ 
            this.ChangeColor(Color.black);
            FindFirstObjectByType<GameManager>().DeselectBoxWithString();
        }
        GetComponent<ArtCard>().isBlack = !GetComponent<ArtCard>().isBlack;
    }

    //if the artWork or Museum are selected their text is blue, otherwise it's black
    public void ChangeColor(Color color)
    {
        GetComponent<TextMeshProUGUI>().color = color;
        
    }

   
}
