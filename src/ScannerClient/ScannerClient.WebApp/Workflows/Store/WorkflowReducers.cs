namespace ScannerClient.WebApp.Workflows.Store
{
    public static class WorkflowReducers
    {
        [ReducerMethod]
        public static WorkflowState ReduceSetPositionKeyIdAction(WorkflowState state, SetPositionKeyIdAction action) => 
            new(action.PositionKeyId, null, null, state.CurrentPatient);

        [ReducerMethod]
        public static WorkflowState ReduceGetWorkflowsAndSetCurrentAction(WorkflowState state, GetWorkflowsAndSetCurrentAction action) => 
            new(state.PositionKeyId, null, null, state.CurrentPatient);

        [ReducerMethod]
        public static WorkflowState ReduceSetWorkflowsAction(WorkflowState state, SetWorkflowsAction action) => 
            new(state.PositionKeyId, action.Workflows, null, state.CurrentPatient);

        [ReducerMethod]
        public static WorkflowState ReduceSetCurrentWorkflowAction(WorkflowState state, SetCurrentWorkflowAction action) => 
            new(state.PositionKeyId, state.Workflows, action.Workflow, state.CurrentPatient);

        [ReducerMethod]
        public static WorkflowState ReduceSetCurrentPatientAction(WorkflowState state, SetCurrentPatientAction action) => 
            new(state.PositionKeyId, state.Workflows, state.CurrentWorkflow, action.CurrentPatient);
    }
}