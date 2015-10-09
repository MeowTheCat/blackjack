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

			for(int i=0; i<gf.bench.Count(); i++)
			{
				if(gf.cards[gf.bench.Get(i)].picture ==  gameObject && gf.myturn == true) 
				{
					gameObject.tag = "Focus";
					gf.myturn = false;
				}
			}
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
			
			gameObject.transform.localScale = new Vector3 (0.8f, 0.8f, 0.8f);
			renderes = GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in renderes) {
				renderer.sortingOrder = renderer.sortingOrder + 200;
			}
			gameObject.tag = "Focus";
			
		}
	}
	
	
}

