namespace Never
{
    /// <summary>
    /// 选择接口，约束了泛型可以new入行为
    /// </summary>
    /// <typeparam name="TOptin"></typeparam>
    public interface IConstructibleOption<out TOptin> : IValuableOption<TOptin> where TOptin : new()
    {
    }
}