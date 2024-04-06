public interface IDamageable
{
    int MaxHealth { get; }
    int Health { get; set; }
    void TakeDamage(int _damage);
}