namespace ScannerClient.WebApp.Workflows.Store
{
    public class WorkflowFeature : Feature<WorkflowState>
    {
        public override string GetName() => nameof(WorkflowState);

        protected override WorkflowState GetInitialState() =>
            new(0, null, null, null);
    }
}