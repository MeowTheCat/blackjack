using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


public class ItemBehavior : MonoBehaviour, IPointerClickHandler {
	
	
	public void  OnPointerClick(PointerEventData data)
	{
		Component[] renderes;

		if(gameObject.tag == "Focus")
		{
			GameFlow gf = (GameFlow)GameObject.FindWithTag("GameController").GetComponent<GameFlow>();

			gameObject.tag = "Untagged";


			gameObject.transform.localScale = new Vector3 (0.375f, 0.375f, 0.375f);
			renderes = GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in renderes) {
				renderer.sortingOrder = renderer.sortingOrder - 200;
			}

		}
		else
		{	
			GameObject focus  = GameObject.FindWithTag("Focus");
			if(focus!=null) 
			{   
				focus.tag = "Untagged";
				focus.transform.localScale = new Vector3 (0.375f, 0.375f, 0.375f);
				renderes = focus.GetComponentsInChildren<Renderer>();
				foreach (Renderer renderer in renderes) {
					renderer.sortingOrder = renderer.sortingOrder - 200;
				}
			}
			
			gameObject.transform.localScale = new Vector3 (1f, 1f, 1f);
			renderes = GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in renderes) {
				renderer.sortingOrder = renderer.sortingOrder + 200;
			}
			gameObject.tag = "Focus";
			
		}
	}
	
	
}

