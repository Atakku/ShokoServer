using System;
using Shoko.Server.Providers.TMDB;
using Shoko.Server.Server;

#nullable enable
namespace Shoko.Server.Models.AniDB;

public class AniDB_Character
{
    #region Server DB columns

    public int AniDB_CharacterID { get; set; }

    public int CharacterID { get; set; }

    public string Name { get; set; } = string.Empty;

    public string OriginalName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ImagePath { get; set; } = string.Empty;

    public PersonGender Gender { get; set; }

    public CharacterType Type { get; set; }

    public DateTime LastUpdated { get; set; }

    #endregion
}
