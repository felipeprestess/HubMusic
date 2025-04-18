using GraphQL.Types;

public class SongType : ObjectGraphType<Song>
{
    public SongType()
    {
        Field(x => x.Id).Description("Id of the song");
        Field(x => x.Title).Description("Title of the song");
        Field(x => x.Artist).Description("Artist of the song");
        Field(x => x.Duration).Description("Duration of the song");
        Field(x => x.Genre).Description("Genre of the song");
        Field(x => x.Album, nullable: true).Description("Album of the song");
    }
}