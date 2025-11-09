using UnityEngine;
using UnityEditor;


//This script was made by ChatGpt :D I use this to put in alphabetical order the names of the artWorks and the museums
public class SortChildrenByName : MonoBehaviour
{
    [ContextMenu("Sort Children Alphabetically")]
    public void SortChildren()
    {
        // Get all the Children
        Transform parent = this.transform;
        int childCount = parent.childCount;

        // Put children in an array
        Transform[] children = new Transform[childCount];
        for (int i = 0; i < childCount; i++)
        {
            children[i] = parent.GetChild(i);
        }

        // Order by name 
        System.Array.Sort(children, (a, b) => a.name.CompareTo(b.name));

        // Update the children
        for (int i = 0; i < children.Length; i++)
        {
            children[i].SetSiblingIndex(i);
        }

    }
}
