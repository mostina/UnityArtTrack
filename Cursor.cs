using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;


//this script was made by ChatGpt -> it change the cursor when I'
public class UICursorManager : MonoBehaviour
{
    public Texture2D cursorTexture;
    public Vector2 hotSpot = Vector2.zero;

    void Start()
    {
        // Prendi tutti i bottoni nella scena
        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include,FindObjectsSortMode.None);

        foreach (Button btn in buttons)
        {
            EventTrigger trigger = btn.gameObject.AddComponent<EventTrigger>();

            // Pointer Enter
            var entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            entryEnter.callback.AddListener((eventData) => {
                Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
            });
            trigger.triggers.Add(entryEnter);

            // Pointer Exit
            var entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            entryExit.callback.AddListener((eventData) => {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            });
            trigger.triggers.Add(entryExit);
        }
    }
}
