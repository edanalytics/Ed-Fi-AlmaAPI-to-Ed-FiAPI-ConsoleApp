namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor
{
    public interface IEntryGradeLevelTransformer
    {
        string TransformSrcToEdFi(string srcGradeLevelDescriptor);
    }
    public class EntryGradeLevelTransformer : IEntryGradeLevelTransformer
    {
        public string TransformSrcToEdFi(string srcGradeLevelDescriptor)
        {
            return $"uri://ed-fi.org/GradeLevelDescriptor#Other";
        }
    }
}
