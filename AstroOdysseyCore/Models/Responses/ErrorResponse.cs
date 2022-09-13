namespace AstroOdysseyCore
{
    public class ErrorResponse
    {
        public string[] Errors { get; set; } = new string[] { };

        public ErrorResponse BuildExternalError(params string[] error)
        {
            return new ErrorResponse() { Errors = error?.Where(x => x is not null)?.ToArray() };
        }
    }
}
