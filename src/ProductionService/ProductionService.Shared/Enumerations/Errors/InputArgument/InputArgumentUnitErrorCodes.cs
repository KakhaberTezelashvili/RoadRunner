namespace ProductionService.Shared.Enumerations.Errors.InputArgument;

/// <summary>
/// Input argument error codes shared between server and clients related to units.
/// </summary>
public enum InputArgumentUnitErrorCodes
{
    /// <summary>
    /// Units are missing from the batch.
    /// </summary>
    UnitListMissingFound = 1,

    /// <summary>
    /// What type is not valid.
    /// </summary>
    WhatTypeNotValid = 2
}