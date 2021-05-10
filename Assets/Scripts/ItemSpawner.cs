using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public static Dictionary<int, ItemSpawner> spawners = new Dictionary<int, ItemSpawner>();
    private int nextSpawnerId = 1;

    public int spawnerId;
    public bool hasItem = false;

	private void Start()
	{
		hasItem = false;
		spawnerId = nextSpawnerId;
		nextSpawnerId++;

		spawners.Add(spawnerId, this);

		StartCoroutine(SpawnItem());
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			Player _player = other.GetComponent<Player>();
			if (_player.AttemptPickupItem())
			{
				ItemPickedUp(_player.id);
			}
		}
	}

	private IEnumerator SpawnItem()
	{
		yield return new WaitForSeconds(10f);

		hasItem = true;
	}

	private void ItemPickedUp(int _byPlayer)
	{
		hasItem = false;
		StartCoroutine(SpawnItem());
	}
}
