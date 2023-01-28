using AdventGamesCore.Extensions;
using MongoDB.Driver;

namespace AdventGamesCore
{
    public class SessionRepository : ISessionRepository
    {
        #region Fields

        private readonly IMongoDbService _mongoDBService;

        #endregion

        #region Ctor

        public SessionRepository(IMongoDbService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        #endregion

        #region Methods

        public async Task<bool> BeAnExistingSession(string sessionId, string gameId)
        {            
            var SessionId = sessionId;

            var filter = Builders<Session>.Filter.And(
                Builders<Session>.Filter.Eq(x => x.SessionId, SessionId),
                Builders<Session>.Filter.Eq(x => x.GameId, gameId));

            return await _mongoDBService.Exists(filter);
        }

        public async Task<bool> BeAnIncompleteSession(string sessionId, string gameId)
        {            
            var SessionId = sessionId;

            var filter = Builders<Session>.Filter.And(
                Builders<Session>.Filter.Eq(x => x.SessionId, SessionId),
                Builders<Session>.Filter.Eq(x => x.GameId, gameId),
                Builders<Session>.Filter.Eq(x => x.IsComplete, false));

            return await _mongoDBService.Exists(filter);
        }

        public async Task<ServiceResponse> GenerateSession(GenerateSessionCommand command)
        {
            // save raw data
            var session = Session.Initialize(command);
            await _mongoDBService.InsertDocument(session);

            // send back encrypted data
            session.SessionId = session.SessionId.BitShift();            

            return Response.Build().BuildSuccessResponse(session);
        }

        public async Task<bool> CompleteSession(string sessionId, string gameId)
        {
            var SessionId = sessionId;

            var filter = Builders<Session>.Filter.And(
                    Builders<Session>.Filter.Eq(x => x.SessionId, SessionId),
                    Builders<Session>.Filter.Eq(x => x.GameId, gameId));

            var update = Builders<Session>.Update
                  .Set(x => x.IsComplete, true)
                  .Set(x => x.ModifiedOn, DateTime.UtcNow);

            var updated = await _mongoDBService.UpdateDocument(
                update: update,
                filter: filter);

            return updated != null;
        }

        #endregion
    }
}
