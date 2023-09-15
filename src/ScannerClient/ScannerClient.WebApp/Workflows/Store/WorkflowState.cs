using ProductionService.Shared.Dtos.Patients;
using ProductionService.Shared.Dtos.Positions;

namespace ScannerClient.WebApp.Workflows.Store
{
    public class WorkflowState
    {
        public int PositionKeyId { get; }
        public List<PositionLocationsDetailsDto> Workflows { get; }
        public PositionLocationsDetailsDto CurrentWorkflow { get; }
        public PatientDetailsBaseDto CurrentPatient { get; }

        public WorkflowState(int positionKeyId, List<PositionLocationsDetailsDto> workflows, PositionLocationsDetailsDto currentWorkflow, PatientDetailsBaseDto currentPatient)
        {
            PositionKeyId = positionKeyId;
            Workflows = workflows ?? new List<PositionLocationsDetailsDto>();
            CurrentWorkflow = currentWorkflow;
            CurrentPatient = currentPatient;
        }
    }
}