using System.Collections.Generic;
using UnityEngine;
public class ObjectPool<T> where T : MonoBehaviour, IPoolable
{
	private List<IPoolable> activePool = new List<IPoolable>();
	private List<IPoolable> inactivePool = new List<IPoolable>();
	private T prefab;

	public ObjectPool(T _prefab)
	{
		this.prefab = _prefab;
	}

	public IPoolable RequestObject(Vector2 _position)
	{
		if (inactivePool.Count <= 0)
		{
			Debug.LogError("No More Inactive Pool Items. Increase Pool Size");
			return null;
		}
		else
		{
			IPoolable currentPool = inactivePool[0];
			currentPool.SetPosition(_position);
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

	private IPoolable ActivateItem(IPoolable _item)
	{
		_item.EnablePoolabe();
		_item.Active = true;
		int index = inactivePool.IndexOf(_item);
		if (index != -1)
		{
			inactivePool.RemoveAt(index);
		}
		activePool.Add(_item);
		return _item;
	}

	public void DeactivateItem(IPoolable _item)
	{
		int index = activePool.IndexOf(_item);
		if (index != -1)
		{
			activePool.RemoveAt(index);
		}
		_item.DisablePoolabe();
		_item.Active = false;
		inactivePool.Add(_item);
	}
}
