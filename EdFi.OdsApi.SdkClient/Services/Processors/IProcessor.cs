namespace EdFi.AlmaToEdFi.Cmd.Services.Processors
{
    public interface IProcessor
    {
        public void ExecuteETL(string almaSchoolCode, int stateSchoolId,  string schoolYearId = "");
        public int ExecutionOrder { get; }
    }
}
