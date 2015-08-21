using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


public class CardBehavior : MonoBehaviour, IPointerDownHandler {
	
	
	public void OnPointerDown(PointerEventData data)
	{
		Component[] renderes;
		if(gameObject.tag == "Focus")
		{
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
			GameControl gc = (GameControl)GameObject.FindWithTag("GameController").GetComponent<GameControl>();

			for(int i=0; i<gc.player.cards.Count; i++)
			{
				if(gc.player.cards[i].picture == gameObject) 
				{
					StartCoroutine(gc.Transition(transform.Find("PriceTag").gameObject ,gc.player.cards[i].retail_price.ToString(), gc.player.cards[i].price.ToString() )); 
				}
			}
			for(int i=0; i<gc.computer.cards.Count; i++)
			{
				if(gc.computer.cards[i].picture == gameObject) 
				{
					StartCoroutine(gc.Transition(transform.Find("PriceTag").gameObject ,gc.computer.cards[i].retail_price.ToString(), gc.computer.cards[i].price.ToString() )); 
				}
			}


		}
	}
     

}

