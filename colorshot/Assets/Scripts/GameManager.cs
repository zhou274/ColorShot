using Newtonsoft.Json.Linq;
using System.Collections;
using UnityEngine;
using TTSDK.UNBridgeLib.LitJson;
using TTSDK;
using StarkSDKSpace;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	public UIManager uIManager;

	public ScoreManager scoreManager;

	[Header("Game settings")]
	[Space(5f)]
	public GameObject camObject;

	[Space(5f)]
	public GameObject player;

	[Space(5f)]
	public int playerSpeed;

	[Space(5f)]
	public Color[] colorTable;

	[Space(5f)]
	public GameObject obstaclePrefab;

	[Space(5f)]
	public float yMinDistanceBetweenObstacles = 5f;

	[Space(5f)]
	public float yMaxDistanceBetweenObstacles = 10f;

	[Space(5f)]
	public float maxXDistanceNextObstacle = 5f;

	[Space(5f)]
	public int deltaObstacle = 3;

	[Space(5f)]
	public bool readyToShoot;

	private GameObject nextObstacle;

	private GameObject tempObstacle;

	private Color tempColor;

	private int obstacleId;

	public bool movingPlayer;

	public bool shooting;

	private float step;

	private Vector2 flyDestination;

    public string clickid;
    private StarkAdManager starkAdManager;
    public static GameManager Instance
	{
		get;
		set;
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		Physics2D.gravity = new Vector2(0f, 0f);
		Application.targetFrameRate = 30;
		step = (float)playerSpeed * Time.deltaTime;
		CreateScene();
	}

	private void Update()
	{
		if ((uIManager.gameState == GameState.PLAYING && Input.GetMouseButtonDown(0) && uIManager.IsButton()) || uIManager.gameState != GameState.PLAYING)
		{
			return;
		}
		if (movingPlayer)
		{
			player.transform.position = Vector2.MoveTowards(player.transform.position, flyDestination, step);
			if (Vector2.Distance(player.transform.position, flyDestination) < 0.001f)
			{
				movingPlayer = false;
				readyToShoot = false;
				flyDestination = tempObstacle.transform.position;
			}
		}
		else if (readyToShoot && !movingPlayer && !shooting)
		{
			MovePlayer();
		}
		else if (Input.GetMouseButton(0) && !shooting && !movingPlayer)
		{
			shooting = true;
			StartCoroutine(ShotBall());
		}
	}

	public void CreateScene()
	{
		ResetPlayerAnimation();
		obstacleId = 0;
		tempColor = colorTable[Random.Range(0, colorTable.Length)];
		player.GetComponent<SpriteRenderer>().color = tempColor;
		nextObstacle = UnityEngine.Object.Instantiate(obstaclePrefab);
		nextObstacle.transform.position = new Vector2(0f, -3f);
		tempObstacle = UnityEngine.Object.Instantiate(obstaclePrefab);
		tempObstacle.transform.position = new Vector2(0f, 3f);
		player.GetComponent<Player>().targetPos = tempObstacle.transform.position;
		flyDestination = tempObstacle.transform.position;
		nextObstacle.GetComponent<Obstacle>().SetObstacle(tempColor, obstacleId, 0);
		nextObstacle.GetComponent<Obstacle>().SetNextObstaclePosition(tempObstacle.transform.position);
		obstacleId++;
		tempColor = colorTable[Random.Range(0, colorTable.Length)];
		tempObstacle.GetComponent<Obstacle>().SetObstacle(tempColor, obstacleId, obstacleId * deltaObstacle);
		camObject.transform.position = new Vector3(0f, 0f, -10f);
		player.transform.position = nextObstacle.transform.position;
		readyToShoot = true;
	}

	private IEnumerator ShotBall()
	{
		player.GetComponent<Player>().ShotBullet();
		yield return new WaitForSeconds(0.4f);
		shooting = false;
	}

	public void MovePlayer()
	{
		readyToShoot = false;
		movingPlayer = true;
		CreateNextObstacle();
		camObject.GetComponent<CameraFollowTarget>().EnableDisableFollow(status: true);
	}

	private void CreateNextObstacle()
	{
		obstacleId++;
		tempColor = colorTable[Random.Range(0, colorTable.Length)];
		float num = UnityEngine.Random.Range(yMinDistanceBetweenObstacles, yMaxDistanceBetweenObstacles);
		nextObstacle = UnityEngine.Object.Instantiate(obstaclePrefab);
		nextObstacle.transform.position = new Vector2(tempObstacle.transform.position.x + UnityEngine.Random.Range(0f - maxXDistanceNextObstacle, maxXDistanceNextObstacle), tempObstacle.transform.position.y + num);
		nextObstacle.GetComponent<Obstacle>().SetObstacle(tempColor, obstacleId, obstacleId * deltaObstacle);
		tempObstacle.GetComponent<Obstacle>().SetNextObstaclePosition(nextObstacle.transform.position);
		player.GetComponent<Player>().targetPos = nextObstacle.transform.position;
		tempObstacle = nextObstacle;
	}

	public void PlayerDeath()
	{
		camObject.GetComponent<CameraFollowTarget>().ShakeCamera();
		player.GetComponent<Player>().PlayGameOver();
	}

	public void ResetPlayerAnimation()
	{
		player.GetComponent<Player>().ResetPlayer();
	}

	public void RestartGame()
	{
		if (uIManager.gameState == GameState.PAUSED)
		{
			Time.timeScale = 1f;
		}
		ClearScene();
		CreateScene();
		readyToShoot = false;
		movingPlayer = false;
		scoreManager.ResetCurrentScore();
		uIManager.ShowGameplay();
		camObject.GetComponent<CameraFollowTarget>().EnableDisableFollow(status: false);
	}

	public void ClearScene()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Obstacle");
		for (int i = 0; i < array.Length; i++)
		{
			UnityEngine.Object.Destroy(array[i]);
		}
	}

	public void GameOver()
	{
		if (uIManager.gameState == GameState.PLAYING)
		{
			movingPlayer = false;
			AudioManager.Instance.PlayEffects(AudioManager.Instance.gameOver);
			uIManager.ShowGameOver();
			scoreManager.UpdateScoreGameover();
		}
	}
	public void ContinueGame()
	{
        ShowVideoAd("192if3b93qo6991ed0",
            (bol) => {
                if (bol)
                {
                    if (uIManager.gameState == GameState.GAMEOVER)
                    {
                        //movingPlayer = true;
                        uIManager.HideGameOver();
                        GameObject[] array = GameObject.FindGameObjectsWithTag("Obstacle");
                        for (int i = 0; i < array.Length; i++)
                        {
                            if (array[i].GetComponent<Obstacle>().value == 0)
                                array[i].GetComponent<Obstacle>().value += 1;
                            //array[i].GetComponent<Obstacle>().textMesh.text = array[i].GetComponent<Obstacle>().value.ToString();
                        }
                        //ClearScene();
                        //CreateScene();
                        readyToShoot = false;
                        movingPlayer = false;
                        //scoreManager.ResetCurrentScore();
                        uIManager.ShowGameplay();
                        camObject.GetComponent<CameraFollowTarget>().EnableDisableFollow(status: false);
                    }



                    clickid = "";
                    getClickid();
                    apiSend("game_addiction", clickid);
                    apiSend("lt_roi", clickid);


                }
                else
                {
                    StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
                }
            },
            (it, str) => {
                Debug.LogError("Error->" + str);
                //AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
            });
        
	}


    public void getClickid()
    {
        var launchOpt = StarkSDK.API.GetLaunchOptionsSync();
        if (launchOpt.Query != null)
        {
            foreach (KeyValuePair<string, string> kv in launchOpt.Query)
                if (kv.Value != null)
                {
                    Debug.Log(kv.Key + "<-参数-> " + kv.Value);
                    if (kv.Key.ToString() == "clickid")
                    {
                        clickid = kv.Value.ToString();
                    }
                }
                else
                {
                    Debug.Log(kv.Key + "<-参数-> " + "null ");
                }
        }
    }

    public void apiSend(string eventname, string clickid)
    {
        TTRequest.InnerOptions options = new TTRequest.InnerOptions();
        options.Header["content-type"] = "application/json";
        options.Method = "POST";

        JsonData data1 = new JsonData();

        data1["event_type"] = eventname;
        data1["context"] = new JsonData();
        data1["context"]["ad"] = new JsonData();
        data1["context"]["ad"]["callback"] = clickid;

        Debug.Log("<-data1-> " + data1.ToJson());

        options.Data = data1.ToJson();

        TT.Request("https://analytics.oceanengine.com/api/v2/conversion", options,
           response => { Debug.Log(response); },
           response => { Debug.Log(response); });
    }


    /// <summary>
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="closeCallBack"></param>
    /// <param name="errorCallBack"></param>
    public void ShowVideoAd(string adId, System.Action<bool> closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            starkAdManager.ShowVideoAdWithId(adId, closeCallBack, errorCallBack);
        }
    }
}
