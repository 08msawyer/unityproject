/// <summary>
/// Base interface for any damageable entities.
/// This currently includes foxes and frogs.
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// Damages this entity.
    /// </summary>
    /// <param name="sourceClientId">The network ID of the player which caused this damage.</param>
    /// <param name="amount">How much damage was dealt.</param>
    void Damage(ulong sourceClientId, float amount);
}