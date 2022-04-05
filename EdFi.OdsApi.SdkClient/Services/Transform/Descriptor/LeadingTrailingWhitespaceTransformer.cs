namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor
{
    public interface ILeadingTrailingWhitespaceTransformer
    {
        string TransformSrcToEdFi(string srcTermDescriptor);
    }
    public class LeadingTrailingWhitespaceTransformer : ILeadingTrailingWhitespaceTransformer
    {
        public string TransformSrcToEdFi(string srcText)
        {
            return srcText.Trim();
        }
    }
}