namespace GarageFlowService.Domain.Enums;

public enum WorkOrderStatus
{
    Received = 1,
    Diagnosis = 2,
    WaitingApproval = 3,
    InProgress = 4,
    Finished = 5,
    Delivered = 6,
    Canceled = 7
}

