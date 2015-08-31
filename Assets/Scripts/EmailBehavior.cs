using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;


public class EmailBehavior : MonoBehaviour, IPointerDownHandler {
	
	GameControl gc;
	string email;
	Card card;

	public void OnPointerDown(PointerEventData data)
	{
		gc = (GameControl)GameObject.FindWithTag("GameController").GetComponent<GameControl>();
		GameObject parent = gameObject.transform.parent.gameObject;
		
		for(int i=0; i<gc.player.cards.Count; i++)
		{
			if(gc.player.cards[i].picture == parent) 
			{
				card = gc.player.cards[i];
			}
		}
		for(int i=0; i<gc.computer.cards.Count; i++)
		{
			if(gc.computer.cards[i].picture == parent) 
			{
				card = gc.computer.cards[i];
			}
		}
	 	email = PlayerPrefs.GetString ("email", "");
		if (!email.Contains ("@")) 
		{
			gc.emailInput.SetActive (true);
			gc.emailSubmit.SetActive (true);
			gc.emailCancel.SetActive (true);
			StartCoroutine(WaitForEmail()); 
		}
		else StartCoroutine(SendMail()); 
	}

	IEnumerator SendMail()
	{
		gameObject.GetComponent<SpriteRenderer> ().sprite =  Resources.Load<Sprite>("check");
		var jsonString = "{ \"url\":\"" + card.url + "\",\"brand\":\"" + card.brand + "\",\"category\":\"" + card.category + "\",\"image\":\"" + card.image_url  + "\",\"email\":\"" + email + "\"}";
		
		var encoding = new System.Text.UTF8Encoding();
		Dictionary<string, string> header = new Dictionary<string, string>();
		header.Add("Content-Type",  "text/json");
		WWW www3 = new WWW("https://i1hm3pvto9.execute-api.us-east-1.amazonaws.com/prod/SendMail",encoding.GetBytes(jsonString),header );
		yield return www3;
		Debug.Log (www3.text);
	}

    IEnumerator WaitForEmail()
	{
		while (!PlayerPrefs.GetString ("email", "").Contains ("@"))
		{
			yield return null;
		}
		email = PlayerPrefs.GetString ("email", "");
		StartCoroutine(SendMail()); 
	}


}