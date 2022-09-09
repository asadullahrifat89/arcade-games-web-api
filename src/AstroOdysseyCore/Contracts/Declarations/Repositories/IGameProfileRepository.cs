namespace AstroOdysseyCore
{
    public interface IGameProfileRepository
    {
        Task<QueryRecordResponse<GameProfile>> GetGameProfile(GetGameProfileQuery query);

        Task<bool> AddGameProfile(GameProfile gameProfile);
    }
}
