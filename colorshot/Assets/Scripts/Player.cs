using UnityEngine;

public class Player : MonoBehaviour
{
	public GameObject bulletPrefab;

	public Vector2 targetPos;

	private SpriteRenderer rend;

	private Animator animator;

	private bool inside;

	private bool changedColor;

	private void Start()
	{
		rend = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Obstacle"))
		{
			inside = false;
			changedColor = false;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (GameManager.Instance.uIManager.gameState != GameState.GAMEOVER)
		{
			if (collision.CompareTag("Shield") && !inside && !changedColor)
			{
				changedColor = true;
				rend.color = collision.GetComponent<SpriteRenderer>().color;
				UnityEngine.Object.Destroy(collision.gameObject.transform.parent.gameObject);
			}
			if (collision.CompareTag("Obstacle") && !inside)
			{
				inside = true;
			}
		}
	}

	public void PlayGameOver()
	{
		animator.Play("PlayerDeath");
	}

	public void ResetPlayer()
	{
		animator.Play("PlayerIdle");
	}

	public void ShotBullet()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(bulletPrefab);
		gameObject.transform.position = (Vector2)base.transform.position;
		gameObject.GetComponent<SpriteRenderer>().color = rend.color;
		Vector2 a = targetPos - (Vector2)base.transform.position;
		a.Normalize();
		gameObject.GetComponent<Rigidbody2D>().AddForce(a * 900f);
	}
}
