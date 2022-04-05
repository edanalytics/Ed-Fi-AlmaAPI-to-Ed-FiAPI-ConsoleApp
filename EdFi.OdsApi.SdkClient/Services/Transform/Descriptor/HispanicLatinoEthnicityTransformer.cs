namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor
{
    public interface IHispanicLatinoEthnicityTransformer
    {
        bool TransformSrcToEdFi(string srcHispanicLatino);
    }
    public class HispanicLatinoEthnicityTransformer : IHispanicLatinoEthnicityTransformer
    {
        public bool TransformSrcToEdFi(string srcHispanicLatino)
        {
            return srcHispanicLatino == "Hispanic Or Latino" ? true : false;
        }
    }
}
