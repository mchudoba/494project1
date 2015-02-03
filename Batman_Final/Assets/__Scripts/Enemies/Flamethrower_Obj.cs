using UnityEngine;
using System.Collections;

public class Flamethrower_Obj : MonoBehaviour
{
	private Enemy_Obj		thisEnemy;
	private GameObject		flameSprite;
	private SpriteRenderer	thisRenderer;
	private Animator		thisAnimation;
	private Animator		flameAnimation;

	public GameObject	flame;
	public Sprite		stillSprite;
	public float		flameTimer = 0;
	public float		flameTimerVal = 1f;

	void Start()
	{
		thisEnemy = GetComponent<Enemy_Obj>();
		flameSprite = flame.transform.FindChild("Flame_Sprite").gameObject;
		thisRenderer = transform.FindChild("Flamethrower_Sprite").gameObject.GetComponent<SpriteRenderer>();
		thisAnimation = transform.FindChild("Flamethrower_Sprite").gameObject.GetComponent<Animator>();
		flameAnimation = flameSprite.GetComponent<Animator>();

		flameSprite.renderer.enabled = false;
		thisAnimation.enabled = false;
		flameAnimation.enabled = false;
		flameAnimation.speed = 1.2f;
	}

	void Update()
	{
		if (thisEnemy.health <= 0)
		{
			flameSprite.renderer.enabled = false;
			flame.collider.enabled = false;
			return;
		}

		flameTimer -= Time.deltaTime;
		if (flameTimer < 0)
		{
			flameTimer = flameTimerVal;
			flameSprite.renderer.enabled = !flameSprite.renderer.enabled;
			flame.collider.enabled = !flame.collider.enabled;
			thisAnimation.enabled = !thisAnimation.enabled;
			flameAnimation.enabled = !flameAnimation.enabled;
		}

		if (!thisAnimation.enabled)
			thisRenderer.sprite = stillSprite;
	}

}
