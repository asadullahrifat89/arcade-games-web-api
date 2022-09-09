namespace AstroOdysseyCore
{
    public interface IGameProfileRepository
    {
        Task<QueryRecordResponse<GameProfile>> GetGameProfile(GetGameProfileQuery query);
    }
}
