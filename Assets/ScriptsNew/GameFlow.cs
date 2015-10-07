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
	public bool dealdone,myturn ;


	private float fingerStartTime ;
	private Vector2 fingerStartPos;
	
	private bool isSwipe = false;
	private float minSwipeDist  = 50.0f;
	private float maxSwipeTime = 0.5f;

	public GameObject timer;
	
	void Awake()
	{
		Application.runInBackground = true;
		screenHeightUnit = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera>().orthographicSize * 2.0f; 
		screenWidthUnit = screenHeightUnit * Screen.width/Screen.height ;
		cardData = new Dictionary<string,object>[6];
		bench = new Pile();
		self = new Pile();
		opponent = new Pile();

		timer = GameObject.Find("Timer");
		timer.SetActive (false);

	}

	// Use this for initialization
	void Start () {
		StartCoroutine (OnDeal());
	}
	

	IEnumerator OnDeal() 
	{
		yield return StartCoroutine (LoadData());
		yield return StartCoroutine (DisplayCards());
		dealdone = true;
		myturn = true;
		yield return StartCoroutine (SelfChoose());
	}

	IEnumerator SelfChoose()
	{

		timer.SetActive (true);
		ProgressBarToolkit.CircularProgressBar2D script = (ProgressBarToolkit.CircularProgressBar2D)timer.GetComponent<ProgressBarToolkit.CircularProgressBar2D>();;
		script.Init(6.0f);
		float time1 = Time.time;

		while (myturn &&  (Time.time - time1) < 6.0f  ) 
		{
			yield return null;
		}

		int i;
		for(i=0; i<bench.Count(); i++)
		{
			if(cards[bench.Get(i)].picture.tag ==  "Focus") 
			{
				break;
			}
		}
		if (i == bench.Count ())	i = 0;
		Debug.Log (bench.Get (i).ToString());

		GameObject obj = cards[bench.Get(i)].picture;
		obj.tag =  "Untagged";
		self.AddNode(bench.Get(i));
		bench.RemoveNode(i);
		
		Vector3 destination = new Vector3 (screenWidthUnit / 4.0f * (self.Count() - 0.5f) - screenWidthUnit / 2.0f, -0.3f * screenHeightUnit);
		StartCoroutine(Fade(obj,destination,0.375f));

		timer.SetActive (false);
		Debug.Log (bench.Get (i).ToString());
	
	}
	
	/*
	IEnumerator OpponentChoose()
	{
		
	}
	*/

	public void SetMyturn(bool b)
	{
		myturn = b;
	}

	IEnumerator LoadData()
	{
		float time1 = Time.time;
		string path;
		path = "http://i1hm3pvto9.execute-api.us-east-1.amazonaws.com/prod/test";
		
		WWW www1 = new WWW (path);
		yield return www1; 
	
		var str = Json.Deserialize(www1.text) as Dictionary<string,object>;
		List<object> dataObject = str["results"] as List<object>;
		
		for (int i=0; i<6; i++) 
		{
			cardData[i] = dataObject[i] as Dictionary<string,object>;
		}
		
		float time2 = Time.time;
		float timeDiff = time2 - time1;
		
		if(timeDiff < 3) {yield return new WaitForSeconds(3-timeDiff);}

	}

	IEnumerator DisplayCards()
	{
		dealdone = false;
		WWW www2;
		Vector3 destination;
		for (int i=0; i<6; i++) 
		{
			Debug.Log (i.ToString());
			www2 = new WWW(cardData[i]["image"].ToString());
			yield return www2;

			GameObject current = new GameObject();
			SpriteRenderer renderer =  current.AddComponent<SpriteRenderer>();
			renderer.sortingOrder = 100;	
			Sprite sprite = new Sprite();
			float pixelsToUnits = www2.texture.width / (screenWidthUnit/1.5f);
			sprite = Sprite.Create(www2.texture, new Rect(0, 0, www2.texture.width, www2.texture.height),new Vector2(0.5f, 0.5f),pixelsToUnits);
			renderer.sprite = sprite;

			Item card = new Item(current, int.Parse(cardData[i]["sale_price"].ToString()),int.Parse(cardData[i]["retail_price"].ToString()),cardData[i]["url"].ToString(),cardData[i]["category"].ToString(),cardData[i]["brand"].ToString(),cardData[i]["image"].ToString());
			cards[i] = card;
			bench.AddNode(i);

			yield return new WaitForSeconds(1f);
			destination = new Vector3 (screenWidthUnit / 4.0f * (i  + bench.offset + 0.5f) - screenWidthUnit / 2.0f, 0);
			yield return StartCoroutine(Fade(current,destination,0.375f));

			if((bench.Count() + bench.offset) >= 4  && bench.Count()< 6) HMove(false,bench);
		}
	
		for (int i=0; i<6; i++) 
		{
			cards[i].picture.AddComponent<BoxCollider2D>() ;
			cards[i].picture.AddComponent<ItemBehavior>() ;
		}
	

	}




	IEnumerator Fade(GameObject current,Vector3 destination,float scale)
	{
		while (current.transform.position != destination ) {
			current.transform.localScale = Vector3.Lerp (current.transform.localScale, new Vector3(scale,scale,scale), 0.4f);
			current.transform.position = Vector3.Lerp (current.transform.position, destination, 0.4f);
			yield return null;
		}
		current.GetComponent<SpriteRenderer> ().sortingOrder = 0;
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


	void Update () {
		
		if (dealdone && Input.touchCount > 0){
			
			foreach (Touch touch in Input.touches)
			{
				switch (touch.phase)
				{
				case TouchPhase.Began :
					/* this is a new touch */
					isSwipe = true;
					fingerStartTime = Time.time;
					fingerStartPos = touch.position;
					break;
					
				case TouchPhase.Canceled :
					/* The touch is being canceled */
					isSwipe = false;
					break;
					
				case TouchPhase.Ended :
					
					float gestureTime = Time.time - fingerStartTime;
					float gestureDist = (touch.position - fingerStartPos).magnitude;
					
					if (isSwipe && gestureTime < maxSwipeTime && gestureDist > minSwipeDist){
						Vector2 direction = touch.position - fingerStartPos;
						if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
						{
							if(fingerStartPos.y <= 0.35f * Screen.height && fingerStartPos.y >= 0.65f *  Screen.height) 
							{
								HMove(Mathf.Sign(direction.x)>0,bench);
								if (gestureDist > 2*minSwipeDist) HMove(Mathf.Sign(direction.x)>0,bench);
							}
							if(fingerStartPos.y >= 0.35f * Screen.height && fingerStartPos.y <= 0.65f *  Screen.height) 
							{
								HMove(Mathf.Sign(direction.x)>0,bench);
								if (gestureDist > 2*minSwipeDist) HMove(Mathf.Sign(direction.x)>0,bench);
							}
						}					
					}
					isSwipe = false;
					break;
				}
			}
		}
	}

}
