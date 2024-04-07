using System.Collections.Generic;
using UnityEngine;
public class ObjectPool<T> where T : MonoBehaviour, IPoolable
{
	private List<IPoolable> activePool = new List<IPoolable>();
	private List<IPoolable> inactivePool = new List<IPoolable>();
	private T prefab;

	public ObjectPool(T prefab)
	{
		this.prefab = prefab;
	}

	public IPoolable RequestObject(Vector2 position)
	{
		if (inactivePool.Count <= 0)
		{
			Debug.LogError("No More Inactive Pool Items. Increase Pool Size");
			return null;
		}
		else
		{
			IPoolable currentPool = inactivePool[0];
			currentPool.SetPosition(position);
			ActivateItem(currentPool);
			return currentPool;
		}
	}

	public IPoolable AddNewItemToPool()
	{
		T instance = GameObject.Instantiate(prefab);
		instance.gameObject.SetActive(false);
		inactivePool.Add(instance);
		return instance;
	}

	public IPoolable ActivateItem(IPoolable item)
	{
		item.EnablePoolabe();
		item.Active = true;
		int index = inactivePool.IndexOf(item);
		if (index != -1)
		{
			inactivePool.RemoveAt(index);
		}
		activePool.Add(item);
		return item;
	}

	public void DeactivateItem(IPoolable item)
	{
		int index = activePool.IndexOf(item);
		if (index != -1)
		{
			activePool.RemoveAt(index);
		}
		item.DisablePoolabe();
		item.Active = false;
		inactivePool.Add(item);
	}
}
