using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ArtCard : MonoBehaviour
{
    public string idArtCard;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    public void SelectArtCard()
    {
        SelectedCard selectedCard = FindFirstObjectByType<SelectedCard>();
        if (selectedCard.idSelectedCard != "")
        {
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            gameManager.SelectBoxWithId();

            foreach (ArtCard artCard in FindObjectsByType<ArtCard>(FindObjectsSortMode.None))
            {
                if (artCard.idArtCard == selectedCard.idSelectedCard)
                {
                    artCard.DeselectArtCard();
                }
            }
        }
                    selectedCard.idSelectedCard = this.idArtCard;
                    ChangeColor(Color.gray);

                
    }

    public void DeselectArtCard()
    {
        SelectedCard selectedCard = FindFirstObjectByType<SelectedCard>();

                ChangeColor(Color.white);
            
        
    }


    public void ChangeColor(Color color)
    {
        this.gameObject.GetComponent<Image>().color = color;
    }

   
}
