using GraphQL;
using GraphQL.Types;

public class SongSchema : Schema
{
    public SongSchema(IServiceProvider provider) : base(provider)
    {
        Query = provider.GetRequiredService<SongQuery>();
    }
}