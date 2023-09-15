using ProductionService.Shared.Dtos.Patients;
using ProductionService.Shared.Dtos.Positions;

namespace ScannerClient.WebApp.Workflows.Store
{
    public class SetPositionKeyIdAction
    {

        public int PositionKeyId { get; set; }
        public SetPositionKeyIdAction(int positionKeyId)
        {
            PositionKeyId = positionKeyId;
        }
    }

    public class GetWorkflowsAndSetCurrentAction
    {
        public int CurrentWorkflowKeyId { get; set; }
        public GetWorkflowsAndSetCurrentAction(int currentWorkflowKeyId)
        {
            CurrentWorkflowKeyId = currentWorkflowKeyId;
        }
    }

    public class SetWorkflowsAction
    {
        public List<PositionLocationsDetailsDto> Workflows { get; }

        public SetWorkflowsAction(List<PositionLocationsDetailsDto> workflows)
        {
            Workflows = workflows;
        }
    }

    public class SetCurrentWorkflowAction
    {
        public PositionLocationsDetailsDto Workflow { get; }

        public SetCurrentWorkflowAction(PositionLocationsDetailsDto workflow)
        {
            Workflow = workflow;
        }
    }

    public class SetCurrentPatientAction
    {
        public PatientDetailsBaseDto CurrentPatient { get; }

        public SetCurrentPatientAction(PatientDetailsBaseDto currentPatient)
        {
            CurrentPatient = currentPatient;
        }
    }
}
