namespace DependencyInjection.StaticAccessor
{
    /// <summary>
    /// If the default scope value of <see cref="PinnedScope.Scope"/> is null, then try to get the scope from <see cref="IScopeGoalie"/> instances.
    /// </summary>
    public interface IScopeGoalie : IScopeProvider
    {
    }
}
