namespace ClaymoreMiner.RemoteManagement.Mapper
{
    internal interface IMapper<TSource, TResult>
    {
        TResult Map(TSource source);
    }
}