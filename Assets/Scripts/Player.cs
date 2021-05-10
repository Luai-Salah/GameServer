using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
	public int id;
	public string username;
	public Transform shootOrigin;
	public CharacterController controller;
	public float gravity = -9.81f;
	public float moveSpeed = 5f;
	public float jumpSpeed = 5f;
	public float maxHealth = 100f;
	public float health;
	public int itemAmount = 0;
	public int maxItemAmount = 3;

	private bool[] inputs;
	private float yVelocity;

	private void Start()
	{
		gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
		moveSpeed *= Time.fixedDeltaTime;
		jumpSpeed *= Time.fixedDeltaTime;
	}

	public void Initialize(int _id, string _username)
	{
		id = _id;
		username = _username;
		health = maxHealth;

		inputs = new bool[5];
	}

	public void FixedUpdate()
	{
		if (health <= 0f)
			return;

		Vector2 _inputDir = Vector2.zero;

		if (inputs[0])
		{
			_inputDir.y += 1;
		}
		if (inputs[1])
		{
			_inputDir.y -= 1;
		}
		if (inputs[2])
		{
			_inputDir.x -= 1;
		}
		if (inputs[3])
		{
			_inputDir.x += 1;
		}

		Move(_inputDir);
	}

	private void Move(Vector2 _inputDir)
	{
		Vector3 _moveDir = transform.right * _inputDir.x + transform.forward * _inputDir.y;
		_moveDir *= moveSpeed;

		if (controller.isGrounded)
		{
			yVelocity = 0f;
			if (inputs[4])
			{
				yVelocity = jumpSpeed;
			}
		}

		yVelocity += gravity;
		_moveDir.y = yVelocity;

		controller.Move(_moveDir);

		ServerSend.PlayerPosition(this);
		ServerSend.PlayerRotation(this);
	}

	public void SetInput(bool[] _inputs, Quaternion _rot)
	{
		inputs = _inputs;
		transform.rotation = _rot;
	}

	public void Shoot(Vector3 _viewDir)
	{
		if (Physics.Raycast(shootOrigin.position, _viewDir, out RaycastHit _hit, 100f))
		{
			if (_hit.collider.CompareTag("Player"))
			{
				_hit.collider.GetComponent<Player>().TakeDamage(10f);
			}
		}
	}

	public void TakeDamage(float _damage)
	{
		if (health <= 0)
			return;

		health -= _damage;

		if (health <= 0)
		{
			health = 0f;
			controller.enabled = false;
			transform.position = new Vector3(0f, 25f, 0f);
			ServerSend.PlayerPosition(this);
			StartCoroutine( Respawn() );
		}

		ServerSend.PlayerHealth(this);
	}

	private IEnumerator Respawn()
	{
		yield return new WaitForSeconds(5f);

		health = maxHealth;
		controller.enabled = true;
		ServerSend.PlayerRespawned(this);
	}

	public bool AttemptPickupItem()
	{
		if (itemAmount >= maxItemAmount)
		{
			return false;
		}

		itemAmount++;
		return true;
	}
}
