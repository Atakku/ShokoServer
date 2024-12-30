using Shoko.Server.Repositories;
using Shoko.Server.Server;

#nullable enable
namespace Shoko.Server.Models.AniDB;

public class AniDB_Anime_Staff
{
    #region Server DB columns

    public int AniDB_Anime_StaffID { get; set; }

    public int AnimeID { get; set; }

    public int CreatorID { get; set; }

    public string Role { get; set; } = string.Empty;

    public CreatorRoleType RoleType { get; set; }

    public int Ordering { get; set; }

    #endregion

    public SVR_AniDB_Anime? Anime
        => RepoFactory.AniDB_Anime.GetByAnimeID(AnimeID);

    public AniDB_Creator? Creator
        => RepoFactory.AniDB_Creator.GetByCreatorID(CreatorID);
}
