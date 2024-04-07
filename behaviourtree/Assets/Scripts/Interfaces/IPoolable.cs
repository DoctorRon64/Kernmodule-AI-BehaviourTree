using UnityEngine;

public interface IPoolable
{
	bool Active { get; set; }
	public void DisablePoolabe();
	public void EnablePoolabe();
	public void SetPosition(Vector2 _pos);
}
