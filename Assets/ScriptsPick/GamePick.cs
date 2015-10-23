using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;
//using System.Linq;
using MiniJSON;

public class GamePick : MonoBehaviour {
	
	public Outfit[] cards = new Outfit[3];
	public 	Dictionary<string,object>[] cardData;

	public float screenWidthUnit, screenHeightUnit;

	public GameObject progress,pick,tagModel,tagModel2,link,linkModel;

	public AudioSource dealSound,cardSound,winSound,loseSound,bonusSound,chipSound,meowSound;

	public bool chosen;

	public Vector3 originalPosition ;
	
	void Awake()
	{

		progress = GameObject.Find("Progress");
		progress.SetActive (false);

		pick = GameObject.Find("Pick");
		pick.SetActive (false);

		screenHeightUnit = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera>().orthographicSize * 2.0f; 
		screenWidthUnit = screenHeightUnit * Screen.width/Screen.height ;

		cardData = new Dictionary<string,object>[4];
			
	}
	
	void Start () {
		StartCoroutine (OnDeal());
	}


	IEnumerator OnDeal() 
	{
		chosen = false;
		originalPosition = new Vector3 (0, 0, 0); 

		yield return StartCoroutine (LoadData());

		foreach (Outfit card in cards) {
			if(card != null) Object.Destroy(card.picture);
		}
		Resources.UnloadUnusedAssets();

		yield return StartCoroutine (DisplayCards());
		//yield return new WaitForSeconds(3);		
		//yield return StartCoroutine (OnDeal());
	}


	IEnumerator LoadData()
	{
		progress.SetActive (true);
		
		ProgressBarToolkit.CircularProgressBar2D script = (ProgressBarToolkit.CircularProgressBar2D)progress.GetComponent<ProgressBarToolkit.CircularProgressBar2D>();;
		script.Init(3);
		
		float time1 = Time.time;
		string path;
		path = "https://i1hm3pvto9.execute-api.us-east-1.amazonaws.com/prod/test";
		
		WWW www1 = new WWW (path);
		yield return www1; 
		
		var str = Json.Deserialize(www1.text) as Dictionary<string,object>;
		List<object> dataObject = str["results"] as List<object>;
		
		for (int i=0; i<3; i++) 
		{
			cardData[i] = dataObject[i] as Dictionary<string,object>;
		}
		
		float time2 = Time.time;
		float timeDiff = time2 - time1;
		
		if(timeDiff < 3) {yield return new WaitForSeconds(3-timeDiff);}
		progress.SetActive (false);
		
	}

	IEnumerator DisplayCards()
	{	
		WWW www2;
		Vector3 destination;
		for (int i=0; i<3; i++) 
		{	
			www2 = new WWW(cardData[i]["image"].ToString());
			yield return www2;
			
			//cardSound.Play ();
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
		while (Mathf.Abs(obj.transform.localScale.x - scale)>0.0001)  {
			obj.transform.localScale = Vector3.Lerp (obj.transform.localScale, new Vector3(scale,scale,scale), 0.4f);
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
	}
	
}
