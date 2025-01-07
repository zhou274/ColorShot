using UnityEngine;

public class Bullet : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Shield") && GameManager.Instance.uIManager.gameState == GameState.PLAYING)
		{
			GameManager.Instance.GameOver();
			GameManager.Instance.camObject.GetComponent<CameraFollowTarget>().ShakeCamera();
			collision.transform.parent.GetComponent<Shield>().anim.Play("ShieldBlink");
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
