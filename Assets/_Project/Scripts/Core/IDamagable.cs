/// <summary>
/// Anything in the game that can be damaged (enemies, asteroids,
/// destructible scenery, …) implements this. Lets the bullet damage
/// targets without knowing their concrete type.
/// </summary>
public interface IDamageable
{
    void TakeDamage(int amount);
}