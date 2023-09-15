using ProductionService.Core.Services.Texts;
using ProductionService.Shared.Dtos.Processes;

namespace ProductionService.Core.Services.Processes;

/// <inheritdoc cref="IProcessValidator" />
public class ProcessValidator : ValidatorBase<ProcessModel>, IProcessValidator
{
    private readonly IProcessRepository _processRepository;
    private readonly ITextValidator _textValidator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessValidator" /> class.
    /// </summary>
    /// <param name="processRepository">Repository provides methods to retrieve/handle processes.</param>
    /// <param name="textValidator">Validator provides methods to validate error codes, control codes etc.</param>
    public ProcessValidator(IProcessRepository processRepository, ITextValidator textValidator) : base(processRepository)
    {
        _processRepository = processRepository;
        _textValidator = textValidator;
    }

    /// <inheritdoc />
    public async Task CreateBatchValidateAsync(BatchCreateArgs args)
    {
        ObjectNullValidate(args);
        await FindOtherEntityByKeyIdValidateAsync<ProgramModel>(args.ProgramKeyId);
        await FindOtherEntityByKeyIdValidateAsync<LocationModel>(args.LocationKeyId);
        await FindOtherEntityByKeyIdValidateAsync<PosLocationModel>(args.PositionLocationKeyId);

        try
        {
            await FindOtherEntityByKeyIdValidateAsync<MachineModel>(args.MachineKeyId);
        }
        catch (InputArgumentException exception) when (exception.Code is GenericErrorCodes.ArgumentsNotValid) 
        {
            throw DomainException(DomainBatchErrorCodes.MachineNotSelected);
        }
    }

    /// <inheritdoc />
    public async Task<ProcessModel> ApproveBatchValidateAsync(int batchKeyId, int userKeyId, BatchApproveArgs args)
    {
        ObjectNullValidate(args);
        await FindByKeyIdValidateAsync(batchKeyId);
        await FindOtherEntityByKeyIdValidateAsync<UserModel>(userKeyId);

        ProcessModel process = await _processRepository.GetProcessAsync(batchKeyId);
        if (process == null || process.Type == null)
            throw ArgumentNotFoundException();

        await FindOtherEntityByKeyIdValidateAsync<PosLocationModel>(args.PositionLocationKeyId);

        LocationModel location = await FindOtherEntityByKeyIdValidateAsync<LocationModel>(args.LocationKeyId);
        ValidateProcessTypeAndLocationProcessType(location, process);

        // Wrong machine type
        var machineType = (MachineType)process.Type;
        if (machineType != MachineType.Sterilizer && machineType != MachineType.Washer)
            throw ArgumentNotValidException();

        // Batch already approved
        if (process.ApproveTime != null)
            throw ArgumentNotValidException();

        return process;
    }

    /// <inheritdoc />
    public async Task<ProcessModel> DisapproveBatchValidateAsync(int batchKeyId, int userKeyId, BatchDisapproveArgs args)
    {
        ObjectNullValidate(args);
        await FindByKeyIdValidateAsync(batchKeyId);
        await FindOtherEntityByKeyIdValidateAsync<UserModel>(userKeyId);
        await _textValidator.GetErrorValidateAsync(args.Error, 1);

        ProcessModel process = await _processRepository.GetProcessAsync(batchKeyId);
        if (process == null || process.Type == null)
            throw ArgumentNotFoundException();

        LocationModel location = await FindOtherEntityByKeyIdValidateAsync<LocationModel>(args.LocationKeyId);
        ValidateProcessTypeAndLocationProcessType(location, process);

        await FindOtherEntityByKeyIdValidateAsync<PosLocationModel>(args.PositionLocationKeyId);

        var machineType = (MachineType)process.Type;
        if (machineType != MachineType.Sterilizer && machineType != MachineType.Washer)
            throw ArgumentNotValidException();

        if (process.ApproveTime != null)
            throw ArgumentNotValidException();

        return process;
    }

    private void ValidateProcessTypeAndLocationProcessType(LocationModel location, ProcessModel process)
    {
        if (
            ((MachineType)process.Type == MachineType.Sterilizer && location.Process != ProcessType.SteriPostBatchWF) ||
            ((MachineType)process.Type == MachineType.Washer && location.Process != ProcessType.WashPostBatchWF)
        )
        {
            throw ArgumentNotValidException();
        }
            
    }
}