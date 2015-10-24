using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;
using MiniJSON;

public class GamePick : MonoBehaviour {
	
	public Outfit[] cards = new Outfit[3];
	public Dictionary<string,object>[] cardData;
	public float screenWidthUnit, screenHeightUnit;
	public GameObject intro,pick,start,cont,board,result, priceTag,tagModel,tagModel2,link,linkModel,green,red,canvas;
	public AudioSource dealSound, cardSound, stepSound, goalSound,loseSound, bonusSound;
	public bool chosen;
	public Vector3 originalPosition ;
	public int level, goal, step,score,revisit;


	void Awake()
	{
		AudioSource[] audios = gameObject.GetComponents<AudioSource>();
		
		cardSound = audios[0];
		goalSound = audios[1];
		loseSound = audios [2];
		stepSound = audios [3];

		pick = GameObject.Find("Pick");
		pick.SetActive (false);

		start = GameObject.Find("Start");
		start.SetActive (false);

		cont = GameObject.Find("Continue");
		cont.SetActive (false);

		board = GameObject.Find("Board");

		result = GameObject.Find("Result");
		result.SetActive (false);

		intro = GameObject.Find("Intro");

		screenHeightUnit = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera>().orthographicSize * 2.0f; 
		screenWidthUnit = screenHeightUnit * Screen.width/Screen.height ;

		cardData = new Dictionary<string,object>[4];

		level = PlayerPrefs.GetInt ("level", 1);
		step = 0;
		score = PlayerPrefs.GetInt ("score", 0);
		board.GetComponentInChildren<Text> ().text = "Lvl " + level.ToString() + "   " + "Score " + score.ToString();

		revisit = PlayerPrefs.GetInt ("revisit", 0);
	}

	void Start()
	{
		StartCoroutine (Ready ());	
	}
	
	public void OnStart () {
		intro.SetActive (false);
		start.SetActive (false);
		StartCoroutine (OnDeal());
	}

	IEnumerator Ready()
	{
		yield return StartCoroutine (LoadData());
		start.SetActive (true);
	}

	IEnumerator OnDeal() 
	{
		goal = (int)Mathf.Round (Mathf.Log (level, 10)) + 2;
		
		//Debug.Log (goal.ToString ());
		canvas = GameObject.Find("Canvas");
		
		for (int i = 0; i<step; i++) 
		{
			float distance = i * 55 + 5;
			GameObject obj = Instantiate (green, new Vector3 (distance, 60, 0), Quaternion.identity) as GameObject;
			obj.transform.SetParent (canvas.transform, false);
		}
		
		for (int i = step; i<goal; i++) 
		{
			float distance = i * 55 + 5;
			GameObject obj = Instantiate (red, new Vector3 (distance, 60, 0), Quaternion.identity) as GameObject;
			obj.transform.SetParent (canvas.transform, false);
		}

		chosen = false;
		originalPosition = new Vector3 (0, 0, 0); 

		foreach (Outfit card in cards) {
			if(card != null) Object.Destroy(card.picture);
		}
		Resources.UnloadUnusedAssets();

		yield return StartCoroutine (DisplayCards());

	}


	IEnumerator LoadData()
	{
		/*
		progress.SetActive (true);
		
		ProgressBarToolkit.CircularProgressBar2D script = (ProgressBarToolkit.CircularProgressBar2D)progress.GetComponent<ProgressBarToolkit.CircularProgressBar2D>();;
		script.Init(3);

		float time1 = Time.time;
		*/
		string path;
		if(revisit == 0 ) path = "https://i1hm3pvto9.execute-api.us-east-1.amazonaws.com/prod/test";
		else path = "https://i1hm3pvto9.execute-api.us-east-1.amazonaws.com/prod/test";
		
		WWW www1 = new WWW (path);
		yield return www1; 
		
		var str = Json.Deserialize(www1.text) as Dictionary<string,object>;
		List<object> dataObject = str["results"] as List<object>;
		
		for (int i=0; i<3; i++) 
		{
			cardData[i] = dataObject[i] as Dictionary<string,object>;
		}

		/*
		float time2 = Time.time;
		float timeDiff = time2 - time1;
		
		if(timeDiff < 3) {yield return new WaitForSeconds(3-timeDiff);}
		progress.SetActive (false);
		*/
	}

	IEnumerator DisplayCards()
	{	
		WWW www2;
		Vector3 destination;
		for (int i=0; i<3; i++) 
		{	
			www2 = new WWW(cardData[i]["image"].ToString());
			yield return www2;
			
			cardSound.Play ();
			GameObject current = new GameObject();
			SpriteRenderer renderer =  current.AddComponent<SpriteRenderer>();
			renderer.sortingOrder = 100;	
			Sprite sprite = new Sprite();
			float pixelsToUnits = www2.texture.width / (screenWidthUnit/1.5f);
			sprite = Sprite.Create(www2.texture, new Rect(0, 0, www2.texture.width, www2.texture.height),new Vector2(0.5f, 0.5f),pixelsToUnits);
			renderer.sprite = sprite;
			current.transform.localScale = new Vector3 (1.2f, 1.2f, 1.2f);
			
			
			GameObject brand = new GameObject();
			renderer =  brand.AddComponent<SpriteRenderer>();
			renderer.sprite = Resources.Load<Sprite>("03_hover");
			brand.transform.parent = current.transform;
			brand.transform.localPosition = new Vector3 (0,  screenWidthUnit * 0.52f , 0);
			brand.transform.localScale = new Vector3 (1.5f,0.7f,1f);
			brand.GetComponent<Renderer> ().sortingOrder = 100;
			
			GameObject brandname = Instantiate(tagModel2, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
			brandname.transform.parent = brand.transform;
			brandname.transform.localPosition = new Vector3 (0,  0 , 0);
			brandname.transform.localScale = new Vector3 (0.02f,0.04f,0.02f);
			brandname.GetComponent<Renderer> ().sortingOrder = 100;
			brandname.GetComponent<TextMesh> ().text =  cardData[i]["brand"].ToString() ; 
			
			Outfit card = new Outfit(current, int.Parse(cardData[i]["sale_price"].ToString()),int.Parse(cardData[i]["retail_price"].ToString()),cardData[i]["url"].ToString(),cardData[i]["category"].ToString(),cardData[i]["brand"].ToString(),cardData[i]["image"].ToString());
			cards[i] = card;
		
			yield return new WaitForSeconds(1.5f);
			destination = new Vector3 (screenWidthUnit / 3.0f * (i  + 0.5f) - screenWidthUnit / 2.0f, screenWidthUnit*3.5f/2.5f/5.5f,0);
			StartCoroutine(Fade(cards[i].picture,destination,0.5f));		
		}
		
		for (int i=0; i<3; i++) 
		{
			cards[i].picture.AddComponent<BoxCollider2D>() ;
			cards[i].picture.AddComponent<ClickBehavior>() ;
		}

	}
	

	IEnumerator Fade(GameObject obj,Vector3 destination,float scale)
	{
		Component[] renderes = obj.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in renderes) {
			renderer.sortingOrder = 0;
		}
		//Debug.Log (Time.time.ToString());
		while (Mathf.Abs(obj.transform.localScale.x - scale)>0.0001) {
			obj.transform.localScale = Vector3.Lerp (obj.transform.localScale, new Vector3 (scale, scale, scale), 0.4f);
			obj.transform.position = Vector3.Lerp (obj.transform.position, destination, 0.4f);
			yield return null;
		}
		//current.GetComponent<SpriteRenderer> ().sortingOrder = 0;
		//Debug.Log (Time.time.ToString());
	}

	IEnumerator Move(GameObject obj,Vector3 destination)
	{
		while (obj.transform.position != destination )  {
			obj.transform.position = Vector3.Lerp (obj.transform.position, destination, 0.4f);
			yield return null;
		}
		Debug.Log ("done");
	}

	public void Pick()
	{
		chosen = true;
		pick.SetActive (false);
		GameObject focus  = GameObject.FindWithTag("Focus");
		focus.tag = "Untagged";
		StartCoroutine(Fade(focus,new Vector3 (0, -screenWidthUnit*3.5f/2.5f/5f,0),0.5f));
		int j = 0;
		for(int i=0; i<3; i++)
		{
			priceTag = Instantiate(tagModel, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
			
			priceTag.transform.parent = cards[i].picture.transform;
			priceTag.transform.localPosition = new Vector3 (screenWidthUnit/8.0f/0.4f,  screenWidthUnit/8.0f*1.4f/0.4f , 0);
			priceTag.transform.localScale = new Vector3 (0.05f,0.05f,0.05f);
			priceTag.GetComponent<Renderer> ().sortingOrder = 100;
			priceTag.GetComponent<TextMesh> ().text = "<color=#ff0000ff>$" + cards[i].price.ToString()  +"</color>"; 
			
			link = Instantiate(linkModel, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
			link.transform.parent = cards[i].picture.transform;
			link.transform.localPosition = new Vector3 (screenWidthUnit/8.0f/0.4f - 0.32f,  screenWidthUnit/8.0f*1.4f/0.4f - 0.9f , 0);
			link.transform.localScale = new Vector3 (0.5f,0.5f,0.5f);
			link.GetComponent<Renderer> ().sortingOrder = 1;
			
			link.AddComponent<BoxCollider2D>() ;
			link.AddComponent<LinkBehavior2>() ;

			if(cards[i].price > cards[j].price) j=i;

		}

		if (cards [j].picture == focus) 
		{
			StartCoroutine(OnWin (j));
		}
		else
		{
			StartCoroutine(OnLose (j));
		}  
	}


	IEnumerator OnWin(int j)
	{
		yield return new WaitForSeconds(1f);
		StartCoroutine(ShakeObject (cards [j].picture));
		step ++;
		score = score + goal*10;
		PlayerPrefs.SetInt("score", score);

	    result.SetActive (true);
		result.GetComponent<Text> ().text = "Score + " + (goal*10).ToString ();

		cont.SetActive (true);
		board.GetComponentInChildren<Text> ().text = "Lvl " + level.ToString() + "   " + "Score " + score.ToString();

		GameObject[] lamps;
		lamps = GameObject.FindGameObjectsWithTag("Respawn");
		foreach (GameObject go in lamps)
		{
			Object.Destroy(go);
		}
		for (int i = 0; i<step; i++) 
		{
			float distance = i * 55 + 5;
			GameObject obj = Instantiate (green, new Vector3 (distance, 60, 0), Quaternion.identity) as GameObject;
			obj.transform.SetParent (canvas.transform, false);
			
		}
		for (int i = step; i<goal; i++) 
		{
			float distance = i * 55 + 5;
			GameObject obj = Instantiate (red, new Vector3 (distance, 60, 0), Quaternion.identity) as GameObject;
			obj.transform.SetParent (canvas.transform, false);
			
		}
		if (step == goal) 
		{
			goalSound.Play ();
			level ++;
			result.GetComponent<Text> ().text = "Level Up!";
			PlayerPrefs.SetInt("level", level);
			board.GetComponentInChildren<Text> ().text = "Lvl " + level.ToString() + "   " + "Score " + score.ToString();
			step = 0;
			
		} else
		{
			stepSound.Play ();
		}
		yield return StartCoroutine (LoadData());
		
		cont.SetActive (true);
		yield return new WaitForSeconds(1f);
		result.SetActive (false);

	}

	IEnumerator OnLose(int j)
	{
		yield return new WaitForSeconds(1f);
		StartCoroutine(ShakeObject (cards [j].picture));
		step = 0;
		loseSound.Play ();
		yield return StartCoroutine (LoadData());
		
		cont.SetActive (true);
	}

	public void OnContinue()
	{
		cont.SetActive (false);
		StartCoroutine (OnDeal());
		PlayerPrefs.SetInt("revisit", 1);
	}


	
	IEnumerator ShakeObject(GameObject obj ) 
	{ 
		float time = 0, last_time = 0;
		int i = 1;
		
		while (time < 2)
		{
			time = time + Time.deltaTime;			
			if (time - last_time > 0.1) {
				if(obj != null) obj.transform.position = new Vector3 (obj.transform.position.x, obj.transform.position.y + i * 0.1f,0);
				i = -i;
				last_time = time;
			}
			yield return null;
		}
		if(i == -1 && obj != null) obj.transform.position = new Vector3 (obj.transform.position.x, obj.transform.position.y + i * 0.1f,0);
	}
	
}
