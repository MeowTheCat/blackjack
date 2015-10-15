using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


public class LinkBehavior2 : MonoBehaviour, IPointerDownHandler {
	
	
	public void OnPointerDown(PointerEventData data)
	{
		GameFlow gf = (GameFlow)GameObject.FindWithTag("GameController").GetComponent<GameFlow>();
		GameObject parent = gameObject.transform.parent.gameObject;
		
		for(int i=0; i<4; i++)
		{
			if(gf.cards[i].picture == parent) 
			{
				Application.OpenURL(gf.cards[i].url);
			}
		}

		
		
	}	
}

