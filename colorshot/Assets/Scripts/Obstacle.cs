using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
	public Animator aimAnim;

	public Animator waveAnim;

	public SpriteRenderer spriteRenderer;

	public GameObject shieldPrefab;

	public TextMeshPro textMesh;
	

	public float shieldDistanceFromCenter = 1.3f;

	public float delayBetweenChangingDirection = 3f;

	private bool waving;

	private bool playerInside;

	private Vector2 nextObstaclePosition;

	private List<GameObject> shields = new List<GameObject>();

	private Color color;

	private int id;

	public int value;

	public void SetObstacle(Color _color, int obstacleId, int _value)
	{
		aimAnim.gameObject.GetComponent<SpriteRenderer>().color = _color;
		waveAnim.gameObject.GetComponent<SpriteRenderer>().color = _color;
		base.gameObject.GetComponent<SpriteRenderer>().color = _color;
		color = _color;
		id = obstacleId;
		value = _value;
		textMesh.text = value.ToString();
		textMesh.color = color;
		if (value == 0)
		{
			textMesh.gameObject.SetActive(value: false);
		}
		if (obstacleId == 1)
		{
			CreateShields(2);
		}
		else if (obstacleId > 1)
		{
			if (ScoreManager.Instance.currentScore < 10)
			{
				CreateShields(UnityEngine.Random.Range(2, 6));
			}
			else if (ScoreManager.Instance.currentScore < 20)
			{
				CreateShields(UnityEngine.Random.Range(3, 6));
			}
			else if (ScoreManager.Instance.currentScore < 40)
			{
				CreateShields(UnityEngine.Random.Range(4, 6));
			}
			else if (ScoreManager.Instance.currentScore < 60)
			{
				CreateShields(UnityEngine.Random.Range(5, 6));
			}
		}
		StartCoroutine(CheckPosition());
	}

	private IEnumerator CheckPosition()
	{
		while (true)
		{
			if (base.transform.position.y < Camera.main.transform.position.y - 8f && GameManager.Instance.uIManager.gameState == GameState.PLAYING)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			yield return new WaitForSecondsRealtime(0.5f);
		}
	}
    
    public void SetNextObstaclePosition(Vector2 _nextObstaclePosition)
	{
		nextObstaclePosition = _nextObstaclePosition;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			if (!playerInside)
			{
				playerInside = true;
				PlayWaveIn();
				spriteRenderer.enabled = false;
				collision.gameObject.GetComponent<SpriteRenderer>().color = color;
				ActivateAim();
			}
		}
		else
		{
			if (playerInside || !collision.CompareTag("Bullet"))
			{
				return;
			}
			if (value == 0)
			{
				UnityEngine.Object.Destroy(collision.gameObject);
			}
			else
			{
				if (value <= 0)
				{
					return;
				}
				value--;
				textMesh.text = value.ToString();
				AudioManager.Instance.PlayEffects(AudioManager.Instance.bullet);
				ScoreManager.Instance.UpdateScore(1);
				if (value == 0)
				{
					textMesh.gameObject.SetActive(value: false);
					GameManager.Instance.readyToShoot = true;
					for (int i = 0; i < shields.Count; i++)
					{
						shields[i].GetComponent<Shield>().HideShield();
						StopAllCoroutines();
					}
				}
				UnityEngine.Object.Destroy(collision.gameObject);
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			PlayWaveOut();
			DeActivateAim();
			playerInside = false;
		}
	}

	public void ActivateAim()
	{
		aimAnim.transform.parent.gameObject.SetActive(value: true);
		if (id != 0)
		{
			Vector2 vector = nextObstaclePosition - (Vector2)base.transform.position;
			vector.Normalize();
			float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			aimAnim.transform.parent.gameObject.transform.localEulerAngles = new Vector3(0f, 0f, num - 90f);
		}
	}

	public void DeActivateAim()
	{
		aimAnim.transform.parent.gameObject.SetActive(value: false);
	}

	public void PlayWaveIn()
	{
		if (!waving)
		{
			waving = true;
			waveAnim.Play("WaveInside");
			StartCoroutine(ResetWaving(0.3f));
		}
	}

	public void PlayWaveOut()
	{
		if (!waving)
		{
			waving = true;
			waveAnim.Play("WaveOut");
			StartCoroutine(ResetWaving(0.3f));
		}
	}

	private IEnumerator ResetWaving(float delay)
	{
		yield return new WaitForSeconds(delay);
		waving = false;
	}

	private void CreateShields(int numOfShileds)
	{
		int num = 0;
		int num2 = 360 / numOfShileds;
		int shieldSpeed = (UnityEngine.Random.Range(0, 2) == 1) ? ((ScoreManager.Instance.currentScore >= 20) ? UnityEngine.Random.Range(60, 110) : UnityEngine.Random.Range(45, 90)) : ((ScoreManager.Instance.currentScore >= 20) ? UnityEngine.Random.Range(-110, -60) : UnityEngine.Random.Range(-90, -45));
		for (int i = 0; i < numOfShileds; i++)
		{
			shields.Add(UnityEngine.Object.Instantiate(shieldPrefab, base.transform));
			shields[i].transform.localEulerAngles = new Vector3(0f, 0f, num);
			shields[i].GetComponent<Shield>().SetShieldSpeed(shieldSpeed);
			num += num2;
			shields[i].transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;
		}
		StartCoroutine(ChangeSmallBallRotation(delayBetweenChangingDirection));
	}

	private IEnumerator ChangeSmallBallRotation(float delay)
	{
		yield return new WaitForSeconds(delay);
		int shieldSpeed = 0;
		for (int i = 0; i < shields.Count; i++)
		{
			shields[i].GetComponent<Shield>().SetShieldSpeed(shieldSpeed);
		}
		yield return new WaitForSeconds(0.25f);
		shieldSpeed = ((UnityEngine.Random.Range(0, 2) == 1) ? ((ScoreManager.Instance.currentScore >= 20) ? UnityEngine.Random.Range(60, 110) : UnityEngine.Random.Range(45, 90)) : ((ScoreManager.Instance.currentScore >= 20) ? UnityEngine.Random.Range(-110, -60) : UnityEngine.Random.Range(-90, -45)));
		for (int j = 0; j < shields.Count; j++)
		{
			shields[j].GetComponent<Shield>().SetShieldSpeed(shieldSpeed);
		}
		StartCoroutine(ChangeSmallBallRotation(delayBetweenChangingDirection));
	}
}
