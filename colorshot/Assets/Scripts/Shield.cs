using UnityEngine;

public class Shield : MonoBehaviour
{
	public Animator anim;

	private int shieldSpeed;

	private void Update()
	{
		if (GameManager.Instance.uIManager.gameState == GameState.PLAYING || GameManager.Instance.uIManager.gameState == GameState.MENU)
		{
			base.transform.Rotate(0f, 0f, (float)shieldSpeed * Time.deltaTime);
		}
	}

	public void SetShieldSpeed(int _shieldSpeed)
	{
		shieldSpeed = _shieldSpeed;
	}

	public void HideShield()
	{
		shieldSpeed = 0;
		anim.Play("ShieldHide");
	}
}
