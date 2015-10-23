using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;
//using System.Linq;
using MiniJSON;

public class GameFlow : MonoBehaviour {
	
	public Item[] cards = new Item[6];
	public 	Dictionary<string,object>[] cardData;
	public Pile bench,self,opponent;
	public float screenWidthUnit, screenHeightUnit;
	public bool dealdone,myturn,mefirst ;

	public GameObject timer,progress,tagModel,tagModel2,selfPrice,opponentPrice,link,linkModel,winParticle;
	public float turnTime = 6;
	public int delay1,delay2;
	public Texture anime1,anime2;
	public Texture[] winPictureArray,losePictureArray,huangshouArray;
	public AudioSource dealSound,cardSound,winSound,loseSound,bonusSound,chipSound,meowSound;

	void Awake()
	{
		Application.runInBackground = true;

		
		AudioSource[] audios = gameObject.GetComponents<AudioSource>();

		winSound = audios[0];
		cardSound = audios[1];
		dealSound = audios [2];

		screenHeightUnit = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera>().orthographicSize * 2.0f; 
		screenWidthUnit = screenHeightUnit * Screen.width/Screen.height ;
		cardData = new Dictionary<string,object>[6];


		timer = GameObject.Find("Timer");
		timer.SetActive (false);
		progress = GameObject.Find("Progress");
		progress.SetActive (false);

		selfPrice =  GameObject.Find("SelfPrice"); 
		opponentPrice = GameObject.Find("OpponentPrice"); 

		selfPrice.GetComponent<RectTransform>().anchoredPosition = new Vector2 (30f,-Screen.height*0.4f);
		opponentPrice.GetComponent<RectTransform>().anchoredPosition = new Vector2 (30f, Screen.height*0.2f);

		selfPrice.SetActive (false);
		opponentPrice.SetActive (false);

		mefirst = false;
	}

	// Use this for initialization
	void Start () {
		StartCoroutine (OnDeal());
	}

	IEnumerator OnDeal() 
	{

    	yield return StartCoroutine (LoadData());

		bench = new Pile();
		self = new Pile();
		opponent = new Pile();
		
		
		foreach (Item card in cards) {
			if(card != null) Object.Destroy(card.picture);
		}
		Resources.UnloadUnusedAssets();

		selfPrice.SetActive (false);
		opponentPrice.SetActive (false);
		dealdone = false;

		yield return StartCoroutine (DisplayCards());

		dealdone = true;
		selfPrice.SetActive (true);
		opponentPrice.SetActive (true);
		selfPrice.GetComponentInChildren<Text>().text = "$" + self.sum.ToString();
		opponentPrice.GetComponentInChildren<Text>().text = "$" + opponent.sum.ToString();

		yield return StartCoroutine (SelfChoose());
		yield return new WaitForSeconds(1);
		yield return StartCoroutine (OpponentChoose(turnTime));
		yield return new WaitForSeconds(1);
		yield return StartCoroutine (SelfChoose());
		yield return new WaitForSeconds(1);
		yield return StartCoroutine (OpponentChoose(1f));

		if (self.sum >= opponent.sum)
		{  
			winSound.Play ();
			winParticle.GetComponent<ParticleSystem> ().Play ();
		}
 		yield return new WaitForSeconds(3);


		yield return StartCoroutine (OnDeal ());
	}

	IEnumerator OpponentChoose(float x)
	{
		timer.GetComponent<RectTransform>().anchoredPosition = new Vector2 (-30f, 300f);
		timer.SetActive (true);
		
		ProgressBarToolkit.CircularProgressBar2D script = (ProgressBarToolkit.CircularProgressBar2D)timer.GetComponent<ProgressBarToolkit.CircularProgressBar2D>();;
		script.Init(turnTime);
		
		yield return new WaitForSeconds (Random.value * (x-1f)+1f);
		timer.SetActive (false);
	
		float[] factor = new float[bench.Count()];
		float sum =0;
		int i;
		for ( i=0; i<bench.Count(); i++) 
		{
			sum = sum + cards[bench.Get(i)].price ;
		}
		factor[0] = cards[bench.Get(0)].price /sum;

		for (i=1; i<bench.Count(); i++) 
		{
			factor[i]= factor[i-1]+ cards[bench.Get(i)].price/sum;
		}

		factor [i - 1] = 1;
		float random = Random.value;

		for (i=0; i<bench.Count(); i++) 
		{
			if(random <= factor[i]) 
			{
				Debug.Log (i.ToString());
				break;
			}
		}

		cardSound.Play ();
		GameObject obj = cards[bench.Get(i)].picture;

		GameObject priceTag = Instantiate(tagModel, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
		priceTag.name = "PriceTag";
		priceTag.transform.parent = obj.transform;
		priceTag.transform.localPosition = new Vector3 (screenWidthUnit/8.0f/0.4f,  screenWidthUnit/8.0f*1.4f/0.4f , 0);
		priceTag.transform.localScale = new Vector3 (0.05f,0.05f,0.05f);
		priceTag.GetComponent<Renderer> ().sortingOrder = 100;
		priceTag.GetComponent<TextMesh> ().text = "<color=#ff0000ff>$" + cards[bench.Get(i)].price.ToString()  +"</color>"; 

		link = Instantiate(linkModel, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
		link.transform.parent = obj.transform;
		link.transform.localPosition = new Vector3 (screenWidthUnit/8.0f/0.4f - 0.32f,  screenWidthUnit/8.0f*1.4f/0.4f - 0.9f , 0);
		link.transform.localScale = new Vector3 (0.5f,0.5f,0.5f);
		link.GetComponent<Renderer> ().sortingOrder = 1;

		link.AddComponent<BoxCollider2D>() ;
		link.AddComponent<LinkBehavior2>() ;
		
		opponent.sum = opponent.sum + cards [bench.Get (i)].price;
		opponentPrice.GetComponentInChildren<Text>().text = "$" + opponent.sum.ToString();

		opponent.AddNode(bench.Get(i));
		bench.RemoveNode(i);
		
		Vector3 destination = new Vector3 (screenWidthUnit / 4.0f * (opponent.Count() + 0.5f) - screenWidthUnit / 2.0f, 0.28f * screenHeightUnit,0);
		
		StartCoroutine(Fade(obj,destination,0.375f));

		Debug.Log (i.ToString());
	}

	IEnumerator SelfChoose()
	{

		myturn = true;
		timer.GetComponent<RectTransform>().anchoredPosition = new Vector2 (-30f, -300f);
		timer.SetActive (true);

	    ProgressBarToolkit.CircularProgressBar2D script = (ProgressBarToolkit.CircularProgressBar2D)timer.GetComponent<ProgressBarToolkit.CircularProgressBar2D>();;
		script.Init(turnTime);

		float time1 = Time.time;

		while (myturn &&  (Time.time - time1) < turnTime  ) 
		{
			yield return null;
		}
		myturn = false;
	
		int i;
		for(i=0; i<bench.Count(); i++)
		{
			if(cards[bench.Get(i)].picture.tag ==  "Focus") 
			{
				break;
			}
		}
		if (i == bench.Count ())	i = 0;

		cardSound.Play ();
		
		GameObject obj = cards[bench.Get(i)].picture;
		obj.tag =  "Untagged";
	

		GameObject priceTag = Instantiate(tagModel, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
		priceTag.name = "PriceTag";
		priceTag.transform.parent = obj.transform;
		priceTag.transform.localPosition = new Vector3 (screenWidthUnit/8.0f/0.4f,  screenWidthUnit/8.0f*1.4f/0.4f , 0);
		priceTag.transform.localScale = new Vector3 (0.05f,0.05f,0.05f);
		priceTag.GetComponent<Renderer> ().sortingOrder = 100;
		priceTag.GetComponent<TextMesh> ().text = "<color=#ff0000ff>$" + cards[bench.Get(i)].price.ToString()  +"</color>"; 

		link = Instantiate(linkModel, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
		link.transform.parent = obj.transform;
		link.transform.localPosition = new Vector3 (screenWidthUnit/8.0f/0.4f - 0.32f,  screenWidthUnit/8.0f*1.4f/0.4f - 0.9f , 0);
		link.transform.localScale = new Vector3 (0.5f,0.5f,0.5f);
		link.GetComponent<Renderer> ().sortingOrder = 1;
		
		link.AddComponent<BoxCollider2D>() ;
		link.AddComponent<LinkBehavior2>() ;

		self.sum = self.sum + cards [bench.Get (i)].price;
		
		selfPrice.GetComponentInChildren<Text>().text = "$" + self.sum.ToString();
	
		self.AddNode(bench.Get(i));
		bench.RemoveNode(i);


		Vector3 destination = new Vector3 (screenWidthUnit / 4.0f * (self.Count() + 0.5f) - screenWidthUnit / 2.0f, -0.28f * screenHeightUnit,0);
		StartCoroutine(Fade(obj,destination,0.375f));

		timer.SetActive (false);
	}



	IEnumerator LoadData()
	{

		dealSound.Play ();
		myturn = true;
	
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
		
		for (int i=0; i<4; i++) 
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
		for (int i=0; i<4; i++) 
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

			Item card = new Item(current, int.Parse(cardData[i]["sale_price"].ToString()),int.Parse(cardData[i]["retail_price"].ToString()),cardData[i]["url"].ToString(),cardData[i]["category"].ToString(),cardData[i]["brand"].ToString(),cardData[i]["image"].ToString());
			cards[i] = card;
			bench.AddNode(i);

			yield return new WaitForSeconds(1.5f);
			destination = new Vector3 (screenWidthUnit / 4.0f * (i  + bench.offset + 0.5f) - screenWidthUnit / 2.0f, 0,0);
		    StartCoroutine(Fade(current,destination,0.375f));

			//if((bench.Count() + bench.offset) >= 4  && bench.Count()< 4) HMove(false,bench);
		}
	
		for (int i=0; i<4; i++) 
		{
			cards[i].picture.AddComponent<BoxCollider2D>() ;
			cards[i].picture.AddComponent<ItemBehavior>() ;
		}
	}



	IEnumerator Fade(GameObject current,Vector3 destination,float scale)
	{
		//Debug.Log (Time.time.ToString());
		while (current.transform.position != destination ) {
			current.transform.localScale = Vector3.Lerp (current.transform.localScale, new Vector3(scale,scale,scale), 0.4f);
			current.transform.position = Vector3.Lerp (current.transform.position, destination, 0.4f);
			yield return null;
		}
		Component[] renderes = current.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in renderes) {
			renderer.sortingOrder = 0;
		}
		//current.GetComponent<SpriteRenderer> ().sortingOrder = 0;
		//Debug.Log (Time.time.ToString());
	}


	public void HMove(bool direction, Pile x)
	{
		if (!direction && (( x.Count() + x.offset) >= 4 || x.offset >= 1)) 
		{
			x.offset--;
			for(int i=0; i<x.Count(); i++)
			{
				cards[x.Get(i)].picture.transform.position = cards[x.Get(i)].picture.transform.position + new Vector3(-screenWidthUnit / 4.0f,0,0);
			}	
		}

		if (direction && x.offset <= 0) 
		{
			x.offset++;
			for(int i=0; i<x.Count(); i++)
			{
				cards[x.Get(i)].picture.transform.position = cards[x.Get(i)].picture.transform.position + new Vector3(screenWidthUnit / 4.0f,0,0);
			}	
		}
	}

	void OnGUI() {
		if (dealdone && self.sum <= opponent.sum ) {
			if (delay1 % 20 == 0) {
			  
				int i =  (delay1 / 20)%2;
				anime1 = huangshouArray [i];
			} 
			delay1++;
			GUI.DrawTexture (new Rect (0, Screen.height /10 , Screen.width /5, Screen.width /5), anime1, ScaleMode.ScaleToFit, true, 0);

			if (delay2 % 10 == 0) {

				int j =  (delay2 / 10)%9;
				anime2 = losePictureArray[j];
			}      
			delay2++;
			GUI.DrawTexture (new Rect ( Screen.width /30, Screen.height *3/4, Screen.width / 4, Screen.width / 4), anime2, ScaleMode.ScaleToFit, true, 0);
		}

		if (dealdone && self.sum > opponent.sum ) {

			if (delay2 % 30 == 0) {
			
				int j =  (delay2 / 30)%2+2;
				anime1 = huangshouArray[j];
			}      
			delay2++;
			GUI.DrawTexture (new Rect ( 0, Screen.height/10, Screen.width /5, Screen.width /5), anime1, ScaleMode.ScaleToFit, true, 0);

			if (delay1 % 10 == 0) {
				int i = (delay1 / 10)%4;
				anime2 = winPictureArray [i];
			} 
			delay1++;
			GUI.DrawTexture (new Rect (0, Screen.height*3/4 , Screen.width /4, Screen.width /4), anime2, ScaleMode.ScaleToFit, true, 0);
			

		}

	}



}
