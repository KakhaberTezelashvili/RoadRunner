using LinqKit;
using TDOC.EntityFramework.KeyGeneration;
using TDOC.EntityFramework.Utilities;

namespace ProductionService.Infrastructure.Repositories;

/// <inheritdoc cref="IOperationDataRepository" />
public class OperationDataRepository : RepositoryBase<OperationDataModel>, IOperationDataRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OperationDataRepository" /> class.
    /// </summary>
    /// <param name="context">EF database context.</param>
    public OperationDataRepository(TDocEFDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<int> GetOperationKeyIdByPatientKeyIdAsync(int patientKeyId, int? factoryKeyId, int? doctorKeyId, int? userKeyId,
        int? locationKeyId, int? positionLocationKeyId)
    {
        DateTime startDate = DateTime.Today;
        ExpressionStarter<OperationDataModel> wherePredicate = PredicateBuilder.New<OperationDataModel>(true);
        wherePredicate.And(od => od.PatKeyId == patientKeyId &&
                                 od.StartTime.HasValue &&
                                 od.StartTime.Value.Date == startDate);

        if ((doctorKeyId ?? 0) > 0)
        {
            wherePredicate.And(od => od.DoctKeyId == doctorKeyId);
        }

        int operationKeyId = await EntitySet.AsNoTracking()
            .Where(wherePredicate)
            .Select(od => od.KeyId)
            .FirstOrDefaultAsync();

        if (operationKeyId == 0)
        {
            operationKeyId = await CreateDummyOperationAsync(patientKeyId, factoryKeyId, doctorKeyId, userKeyId,
                locationKeyId, positionLocationKeyId, startDate);
        }

        return operationKeyId;
    }

    /// <summary>
    /// Returns the next unique operation identifier in format "A-[number]".
    /// </summary>
    /// <param name="factoryKeyId">The link to the factory.</param>
    /// <returns>Unique operation identifier.</returns>
    private async Task<string> GetUniqueOperationIdentAsync(int factoryKeyId)
    {
        string ident;

        do
        {
            int counter = await SequenceNumberGenerator.GetSequenceNumberAsync(_context, SequenceType.Operation, TableTypes.Factory, factoryKeyId);
            ident = $"A-{counter}";
        } while (await EntitySet.CountAsync(od => od.Ident == ident) > 0);

        return ident;
    }

    /// <summary>
    /// Creates a dummy operation based on input parameters.
    /// </summary>
    /// <param name="patientKeyId">The link to the patient should be assigned to the operation.</param>
    /// <param name="factoryKeyId">The link to the supplying factory.</param>
    /// <param name="doctorKeyId">The link to the doctor should be assigned to the operation.</param>
    /// <param name="userKeyId">The link to the user responsible for creating the operation.</param>
    /// <param name="locationKeyId">The link to the location where the operation is created.</param>
    /// <param name="positionLocationKeyId">
    /// The link to the position location where the operation is created.
    /// </param>
    /// <param name="date">The date is used as operation start/end time.</param>
    /// <returns>Key id of the created operation.</returns>
    private async Task<int> CreateDummyOperationAsync(int patientKeyId,
        int? factoryKeyId,
        int? doctorKeyId,
        int? userKeyId,
        int? locationKeyId,
        int? positionLocationKeyId,
        DateTime date)
    {
        string operationIdent = await GetUniqueOperationIdentAsync(factoryKeyId ?? 0);
        DateTime dateTime = DateTime.Now;
        OperationDataModel operation = new()
        {
            Ident = operationIdent,
            PatKeyId = SqlUtilities.SQLKeyId(patientKeyId),
            StartTime = date,
            EndTime = date,
            Status = (int)OperationStatus.Started,
            DoctKeyId = SqlUtilities.SQLKeyId(doctorKeyId),
            PrefListOrderMode = PrefListOrderCreateMode.Manual,
            SuppFacKeyId = SqlUtilities.SQLKeyId(factoryKeyId),
            Created = dateTime,
            CreatedKeyId = SqlUtilities.SQLKeyId(userKeyId)
        };
        EntitySet.Add(operation);
        await CommitAsync();

        // Creating operation action for just created dummy operation
        // Should be uncommented after resolving issue with using DB-tables without primary key (TOPDATAACTION is one of them)
        //await _operationActionsRepository.AddOperationActionAsync(
        //    new OperationActionData()
        //    {
        //        OperationKeyId = operation.KeyId,
        //        ActionType = OperationDataActionType.Created,
        //        OperationStatus = (OperationStatus)operation.Status,
        //        ActionTime = dateTime,
        //        UserKeyId = userKeyId,
        //        LocationKeyId = locationKeyId,
        //        PositionLocationKeyId = positionLocationKeyId
        //    });
        return operation.KeyId;
    }
}