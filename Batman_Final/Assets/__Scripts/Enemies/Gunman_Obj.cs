using UnityEngine;
using System.Collections;

public class Gunman_Obj : MonoBehaviour
{
	private bool			ducked = false;
	private bool			fired = false;
	private bool			stood = false;
	private bool			waited = false;
	private float			duck = 0.66f;
	private float			freezeTimer;
	private Vector3			startScale;
	private Vector3			spriteStartScale;
	private PE_Obj			thisPeo;
	private Enemy_Obj		thisEnemy;
	private SpriteRenderer	thisRenderer;
	private Animator		thisAnimator;

	public GameObject	bullet;
	public GameObject	spriteObj;
	public Sprite		standingSprite;
	public Sprite		duckingSprite;
	public float		attackTimer = 0;
	public float		attackTimerVal = 2f;

	void Start()
	{
		attackTimer = 0;
		startScale = transform.localScale;
		spriteStartScale = spriteObj.transform.localScale;
		thisPeo = GetComponent<PE_Obj>();
		thisEnemy = GetComponent<Enemy_Obj>();
		thisRenderer = spriteObj.GetComponent<SpriteRenderer>();
		thisAnimator = spriteObj.GetComponent<Animator>();
	}

	void Update()
	{
		if (freezeTimer > 0)
		{
			freezeTimer -= Time.deltaTime;
			return;
		}

		if (attackTimer > attackTimerVal)
		{
			attackTimer = 0;
			ducked = false;
			fired = false;
			stood = false;
			waited = false;
			Unduck();
		}
		else
			attackTimer += Time.deltaTime;

		if (attackTimer > 0.5f && stood == false)
		{
			stood = true;
			thisAnimator.enabled = true;
			thisAnimator.Play("Loading");
		}
		if (attackTimer > 1.25f && fired == false)
		{
			thisAnimator.SetBool("GunmanFired", true);
			Fire();
			fired = true;
		}
		if (attackTimer > 1.85f && waited == false)
		{
			waited = true;
			thisAnimator.SetBool("GunmanFired", false);
			thisAnimator.enabled = false;
			thisRenderer.sprite = standingSprite;
		}
		if (attackTimer > 2f && ducked == false)
		{
			Duck();
			ducked = true;
		}
	}

	void Fire()
	{
		if (thisEnemy.health <= 0)
			return;

		Vector3 bulletPos = transform.position;
		if (thisPeo.facing == PE_Facing.right)
			bulletPos.x += 1f;
		else
			bulletPos.x -= 1f;
		
		bulletPos.y += transform.lossyScale.y / 4f;
		
		GameObject newBullet = Instantiate(bullet, bulletPos, Quaternion.identity) as GameObject;
		if (thisPeo.facing == PE_Facing.left)
		{
			newBullet.GetComponent<Bullet_Obj>().xSpeed *= -1f;
		}
	}

	void Duck()
	{
		thisRenderer.sprite = duckingSprite;
		Vector3 scale = transform.localScale;
		Vector3 spriteScale = spriteObj.transform.localScale;
		scale.y *= duck;
		spriteScale.y /= duck;
		transform.localScale = scale;
		spriteObj.transform.localScale = spriteScale;
		
		// Move location back to ground
		Vector3 pos = transform.position;
		Vector3 spritePos = spriteObj.transform.position;
		pos.y -= duck / 2f;
		transform.position = pos;
		spriteObj.transform.position = spritePos;
	}

	void Unduck()
	{
		thisRenderer.sprite = standingSprite;
		transform.localScale = startScale;
		spriteObj.transform.localScale = spriteStartScale;

		Vector3 pos = transform.position;
		Vector3 spritePos = spriteObj.transform.position;
		pos.y += duck / 2f;
		spritePos.y = 0;
		transform.position = pos;
		spriteObj.transform.position = spritePos;
	}

	public void Freeze(float timerVal)
	{
		freezeTimer = timerVal;
	}
}
