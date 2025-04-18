using GraphQL;
using GraphQL.Types;

public class SongQuery : ObjectGraphType
{
    public SongQuery(ApplicationDbContext db)
    {
        Field<SongType>(
            "song",
            arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "id" }),
            resolve: context =>
            {
                var id = context.GetArgument<int>("id");
                return db.Songs.Find(id);
            });

        Field<ListGraphType<SongType>>(
            "songs",
            resolve: context => db.Songs.ToList());
    }
}