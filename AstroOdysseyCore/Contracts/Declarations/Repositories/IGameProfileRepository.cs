namespace AstroOdysseyCore
{
    public interface IGameProfileRepository
    {
        Task<QueryRecordResponse<GameProfile>> GetGameProfile(GetGameProfileQuery query);

        Task<bool> AddGameProfile(GameProfile gameProfile);

        Task<bool> UpdateGameProfile(double score, double bestScore, string userId, string gameId);
    }
}
