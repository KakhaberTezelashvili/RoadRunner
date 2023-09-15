using ProductionService.Core.Services.Batches;
using ProductionService.Core.Services.Machines;
using ProductionService.Core.Services.Units;
using ProductionService.Core.Services.Units.Batches;
using ProductionService.Core.Services.Units.Locations;
using ProductionService.Shared.Dtos.Processes;
using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Processes;

/// <inheritdoc cref="IProcessService" />
public class ProcessService : IProcessService
{
    private readonly IProcessRepository _processRepository;
    private readonly IMachineService _machineService;
    private readonly IProcessValidator _processValidator;
    private readonly IBatchService _batchService;
    private readonly IUnitService _unitService;
    private readonly IUnitBatchService _unitBatchService;
    private readonly IUnitLocationService _unitLocationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessService" /> class.
    /// </summary>
    /// <param name="processRepository">Repository provides methods to retrieve/handle processes.</param>
    /// <param name="machineService">Service provides methods to retrieve/handle machines.</param>
    /// <param name="processValidator">Validator provides methods to validate processes.</param>
    /// <param name="batchService">Service provides methods to retrieve/handle batches.</param>
    /// <param name="unitService">Service provides methods to retrieve/handle units.</param>
    /// <param name="unitBatchService">Service provides methods to retrieve/handle unit batch data.</param>
    /// <param name="unitLocationService">Service provides methods to retrieve/handle unit locations.</param>
    public ProcessService(
        IProcessRepository processRepository,
        IMachineService machineService,
        IProcessValidator processValidator,
        IBatchService batchService,
        IUnitService unitService,
        IUnitBatchService unitBatchService,
        IUnitLocationService unitLocationService)
    {
        _processRepository = processRepository;
        _machineService = machineService;
        _processValidator = processValidator;
        _batchService = batchService;
        _unitService = unitService;
        _unitBatchService = unitBatchService;
        _unitLocationService = unitLocationService;
    }

    /// <inheritdoc />
    /// For the details look at Delphi method "uBatchUtils._CreateNewBatch".
    public async Task<int> CreateBatchAsync(int userKeyId, BatchCreateArgs args)
    {
        const int loadNumber = 1;
        const int loadNumberUsage = 2;
        const int batchCharge = 0;
        const int batchError = 0;
        const int batchNativeError = 0;
        const string batchNativeErrorText = "";
        DateTime initiateTime = DateTime.Now;

        await _processValidator.CreateBatchValidateAsync(args);

        // Get machine data.
        MachineModel machine = await _machineService.GetByKeyIdAsync(args.MachineKeyId);
        var machineType = (MachineType)machine.McTyp.Type;
        var newBatch = new ProcessModel()
        {
            Type = (int)machineType,
            InitiateTime = initiateTime,
            MachKeyId = machine.KeyId,
            Charge = batchCharge,
            ProgKeyId = args.ProgramKeyId,
            Error = batchError,
            NativeError = batchNativeError,
            NativeErrortext = batchNativeErrorText,
            LoadNumber = CalcLoadNumber(loadNumberUsage, machine.RefNum, loadNumber, initiateTime),
            InitiatorUserKeyId = userKeyId,
            Status = ProcessStatus.Initiated
        };

        int batchKeyId = await _processRepository.CreateProcessAsync(newBatch);
        if (args.UnitKeyIds != null)
            await _batchService.LinkUnitsToBatchAsync(batchKeyId, machineType, userKeyId, args);

        return batchKeyId;
    }

    /// <inheritdoc />
    public async Task ApproveBatchAsync(int batchKeyId, int userKeyId, BatchApproveArgs args)
    {
        ProcessModel process = await _processValidator.ApproveBatchValidateAsync(batchKeyId, userKeyId, args);
        process.ApproveUserKeyId = userKeyId;
        process.ApproveTime = DateTime.Now;
        process.Status = ProcessStatus.Done;

        await _processRepository.UpdateProcessAsync(process);
        await _batchService.UpdateBatchesStatusToOkAsync(batchKeyId);

        var machineType = (MachineType)process.Type;
        BatchType batchType = machineType == MachineType.Sterilizer ? BatchType.PrimarySteri : BatchType.PostWash;
        WhatType nextWhatType = GetNextWhatTypeByMachineType(machineType);
        IList<UnitBatchContentsDto> units = await _unitBatchService.GetBatchContentsAsync(GetCurrentWhatTypeByMachineType(machineType), batchKeyId, new List<int>());

        foreach (UnitBatchContentsDto unit in units)
        {
            DateTime locationTime = DateTime.Now;
            await _unitLocationService.AddAsync(
                unit.KeyId, userKeyId, args.LocationKeyId, args.PositionLocationKeyId, locationTime, nextWhatType);

            if (machineType == MachineType.Sterilizer)
            {
                if (unit.Status < UnitStatus.Stock)
                    await _unitService.UpdateUnitStatusOrErrorAsync(unit.KeyId, (int)UnitStatus.Stock, 0);
            }
            else
                await _unitLocationService.UpdateAsync(batchKeyId, batchType, unit.KeyId, args.LocationKeyId, locationTime);
        }
    }

    /// <inheritdoc />
    public async Task DisapproveBatchAsync(int batchKeyId, int userKeyId, BatchDisapproveArgs args)
    {
        ProcessModel process = await _processValidator.DisapproveBatchValidateAsync(batchKeyId, userKeyId, args);
        process.ApproveTime = DateTime.Now;
        process.DisapproveUserKeyId = userKeyId;
        process.Error = args.Error;
        process.Status = ProcessStatus.Done;

        await _processRepository.UpdateProcessAsync(process);
        await _batchService.UpdateBatchesStatusToFailedAsync(batchKeyId);

        var machineType = (MachineType)process.Type;
        WhatType nextWhatType = GetNextWhatTypeByMachineType(machineType);
        IList<UnitBatchContentsDto> units = await _unitBatchService.GetBatchContentsAsync(GetCurrentWhatTypeByMachineType(machineType), batchKeyId, new List<int>());

        foreach (UnitBatchContentsDto unit in units)
        {
            DateTime locationTime = DateTime.Now;
            // Add UnitLocation record to all units with error.
            await _unitLocationService.AddAsync(
                unit.KeyId, userKeyId, args.LocationKeyId, args.PositionLocationKeyId, locationTime, nextWhatType, args.Error);
            // Mark all units in the batch with the error.
            await _unitService.UpdateUnitStatusOrErrorAsync(unit.KeyId, error: args.Error);
        }
    }

    /// <inheritdoc />
    public Task<ProcessModel> GetProcessByBatchKeyIdAsync(int batchKeyId) => _processRepository.GetProcessAsync(batchKeyId);

    /// <inheritdoc />
    public async Task<BatchDetailsDto> GetBatchDetailsAsync(int batchKeyId)
    {
        await _processValidator.FindByKeyIdValidateAsync(batchKeyId);
        return await _processRepository.GetBatchDetailsByBatchKeyIdAsync(batchKeyId);
    }

    private static string CalcLoadNumber(int loadNumberUsage, int machineRefNumber,
        int loadNumber, DateTime startDate)
    {
        switch (loadNumberUsage)
        {
            case 1:
                return string.Format("{1:00}{1:00}{1:00}{1:000}", machineRefNumber, loadNumber,
                    startDate.Year % 100, Math.Truncate(Convert.ToDecimal(startDate - new DateTime(startDate.Year, 1, 1))) + 1);
            case 2:
                return string.Format("{1:00}{1:00}{1:00}{1:00}{1:00}", machineRefNumber, loadNumber,
                    startDate.Year % 100, startDate.Month, startDate.Day);
            default:
                return "";
        }
    }

    private static WhatType GetCurrentWhatTypeByMachineType(MachineType machineType) => machineType == MachineType.Sterilizer ? WhatType.SteriPreBatch : WhatType.WashPreBatch;

    private static WhatType GetNextWhatTypeByMachineType(MachineType machineType) => machineType == MachineType.Sterilizer ? WhatType.SteriPostBatch : WhatType.WashPostBatch;
}