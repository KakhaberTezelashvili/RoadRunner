using AdminClient.WebApp.Shared;
using AdminClient.WebApp.Shared.Models;
using AutoMapper;
using Newtonsoft.Json.Linq;
using TDOC.Common.Client.Navigation;
using TDOC.Common.Client.Navigation.Models;
using TDOC.Common.Client.Translations;
using TDOC.Common.Data.Enumerations.Errors.Domain;
using TDOC.Common.Timers;
using TDOC.WebComponents.Popup.Enumerations;
using TDOC.WebComponents.Shared.Enumerations;
using TDOC.WebComponents.Shared.Models;
using TDOC.WebComponents.TextBox;
using static TDOC.Data.Constants.TDocConstants;

namespace AdminClient.WebApp.Details.Shared;

public class DetailsBaseComponent : FluxorComponent
{
    [Inject]
    protected CustomTimer Timer { get; set; }

    [Inject]
    protected NavigationManagerWrapper Navigation { get; set; }

    [Inject]
    protected IMediator Mediator { get; set; }

    [Inject]
    protected IMapper DtoMapper { get; set; }

    [Inject]
    protected IMediatorCarrier MediatorCarrier { get; set; }

    [Inject]
    protected IStringLocalizer<SharedResource> SharedLocalizer { get; set; }

    [Inject]
    protected IStringLocalizer<TdSharedResource> TdSharedLocalizer { get; set; }

    [Inject]
    protected IStringLocalizer<TdTablesResource> TdTablesLocalizer { get; set; }

    [Inject]
    protected IStringLocalizer<TdExceptionalColumnsResource> TdExceptionalColumnsLocalizer { get; set; }

    [Inject]
    protected IStringLocalizer<TdEnumerationsResource> TdEnumsLocalizer { get; set; }

    [Inject]
    private IStringLocalizer<ErrorMessagesResource> _errorMessagesLocalizer { get; set; }

    [Inject]
    private BrowserService _browserService { get; set; }

    [CascadingParameter(Name = nameof(MainLayout))]
    private MainLayout mainLayout { get; set; }

    protected virtual string MainEntityName { get; }
    protected virtual string MainEntityKeyFieldName { get; }
    protected virtual string MainEntityIdFieldName { get; }
    protected virtual string MainEntityStatusFieldName { get; }
    protected virtual string MainEntitySearchTitle { get; }
    protected virtual string MainEntityTitle { get; }
    protected virtual string SideSearchPanelIdentifier { get; }
    protected virtual string SplitterIdentifier { get; }
    protected virtual int BaseDataKeyId { get; }
    protected virtual string BaseDataId { get; set; }
    protected virtual string BaseDataTitle { get; }
    protected virtual string DefaultMediaIconUrl { get; }
    protected TdTextBox RefMainEntityFieldId { get; set; }
    protected TdTextBox RefCreateNewFieldId { get; set; }

    protected virtual IList<ToolbarButtonDetails> ToolbarButtons
    {
        get
        {
            if (toolbarButtons == null)
                toolbarButtons = InitializeToolbarButtons();
            return toolbarButtons;
        }
    }

    protected virtual IEnumerable<KeyValuePair<ObjectStatus, string>> MainEntityStatuses
    {
        get
        {
            if (mainEntityStatuses == null)
                mainEntityStatuses = InitializeMainEntityStatuses();
            return mainEntityStatuses;
        }
    }

    protected MainDetailsLayout RefMainDetailsLayout;
    protected DataChangedNotificationMode EntityChangedNotificationMode => entityChangedNotificationMode;
    protected DataChangedNotificationMode CreateNewDataChangedNotificationMode => createNewDataChangedNotificationMode;
    protected virtual int GetMediaMainEntityKeyId(JObject item) => item[MainEntityKeyFieldName].Value<int>();    
    
    /// <summary>
    /// Entity details confirmation notification types.
    /// </summary>
    private enum EntityDetailsConfirmationNotificationTypes
    {
        /// <summary>
        /// Unsaved changes.
        /// </summary>
        UnsavedChanges,

        /// <summary>
        /// Entity Id changed.
        /// </summary>
        EntityIdChanged
    }

    private enum EntityDetailsConfirmationContext
    {
        /// <summary>
        /// Create new.
        /// </summary>
        CreateNew,

        /// <summary>
        /// Navigation.
        /// </summary>
        Navigation,

        /// <summary>
        /// MainLayout action.
        /// </summary>
        MainLayoutAction
    }

    private bool mainEntityIdChanged;
    private bool entityIsModified;
    private IList<ToolbarButtonDetails> toolbarButtons;
    private IEnumerable<KeyValuePair<ObjectStatus, string>> mainEntityStatuses;
    private DataChangedNotificationMode entityChangedNotificationMode = DataChangedNotificationMode.Once;
    private DataChangedNotificationMode createNewDataChangedNotificationMode = DataChangedNotificationMode.Never;
    private EntityDetailsConfirmationNotificationTypes activeConfirmation;
    private EntityDetailsConfirmationContext? activeConfirmationContext;
    private string newLocationUrl;
    private Action actionToBeExecuted;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await ObtainRecentDataAsync();
        // We should give a moment to initialize MainDetailsLayout.
        Timer.ExecActionAfterSomeDelay(async () => await RefMainDetailsLayout.InitializeContentAsync(), 100);
        MediatorCarrier.Subscribe<CompleteConfirmationNotification>(HandleCompleteConfirmationAsync);
        MediatorCarrier.Subscribe<ComponentDataChangedNotification>(HandleComponentDataChangedAsync);
        MediatorCarrier.Subscribe<HideToastNotification>(HandleHideToastNotificationAsync);
        Navigation.BeforeLocationChanged += BeforeLocationChanged;
        mainLayout.BeforeMainActionCompleted += HandleMainLayoutActionAsync;
        await _browserService.SubscribeToBeforeUnload();
    }

    protected override void Dispose(bool disposing)
    {
        MediatorCarrier.Unsubscribe<CompleteConfirmationNotification>(HandleCompleteConfirmationAsync);
        MediatorCarrier.Unsubscribe<ComponentDataChangedNotification>(HandleComponentDataChangedAsync);
        MediatorCarrier.Unsubscribe<HideToastNotification>(HandleHideToastNotificationAsync);
        Navigation.BeforeLocationChanged -= BeforeLocationChanged;
        mainLayout.BeforeMainActionCompleted -= HandleMainLayoutActionAsync;
        _browserService.UnsubscribeFromBeforeUnload();
        base.Dispose(disposing);
    }

    protected virtual IList<ToolbarButtonDetails> InitializeToolbarButtons() => null;

    protected virtual IEnumerable<KeyValuePair<ObjectStatus, string>> InitializeMainEntityStatuses()
    {
        return Enum.GetValues(typeof(ObjectStatus))
                   .Cast<ObjectStatus>()
                   .Select(status => new KeyValuePair<ObjectStatus, string>(
                       key: status,
                       value: TranslationHelper.GetEnumValueName<ObjectStatus>(status, TdSharedLocalizer, TdEnumsLocalizer)
                   ));
    }

    protected virtual Task ObtainRecentDataAsync() => Task.CompletedTask;

    protected virtual Task ObtainDataByKeyIdAsync(int keyId) => Task.CompletedTask;

    protected virtual bool CheckCreateNewFieldsAreValid() => true;

    protected virtual bool CheckMainEntityFieldsAreValid() => true;

    protected virtual bool CheckCreateNewEntityIsModified() => false;

    protected bool CheckCreateNewEntityIsValid() => CheckCreateNewEntityIsModified() && CheckCreateNewFieldsAreValid();

    protected virtual Task<int> AddDataAsync() => Task.FromResult(0);

    protected virtual void ResetMainEntityFieldsValidation()
    {
    }

    protected virtual void CleanCreateNewEntityData()
    {
    }

    protected virtual Task<bool> UpdateDataAsync() => Task.FromResult(true);

    protected async Task CreateNewAsync()
    {
        if (entityIsModified)
            await ShowUnsavedChangesConfirmation(EntityDetailsConfirmationContext.CreateNew);
        else
            ShowCreateNewPopup();
    }

    protected async Task SaveAndCreateNewAsync()
    {
        if (CheckCreateNewFieldsAreValid() && await AddAndObtainDataAsync())
            ShowCreateNewPopup();
    }

    protected async Task ObtainDataAsync(int keyId)
    {
        await ObtainDataByKeyIdAsync(keyId);
        await ClearMainEntityFlagsAsync();
    }

    protected string ObtainRequiredFieldValidationText(string fieldCaption) =>
       string.Format(_errorMessagesLocalizer[$"{nameof(GenericDomainErrorCodes)}.{nameof(GenericDomainErrorCodes.FieldValueIsRequired)}Title"], fieldCaption);

    protected void ValidateMainEntityFields() => RefMainDetailsLayout.UpdateEntityIsValid(CheckMainEntityFieldsAreValid());

    protected async Task SaveNewAsync()
    {
        if (CheckCreateNewFieldsAreValid() && await AddAndObtainDataAsync())
            RefMainDetailsLayout.HideCreateNewEntityPopup();
    }

    protected async Task<bool> SaveChangesAsync()
    {
        if (!CheckMainEntityFieldsAreValid())
            return false;
        if (mainEntityIdChanged)
        {
            await ShowMainEntityIdChangedConfirmation();
            return false;
        }
        else
            return await UpdateDataAndClearFlagsAsync();
    }

    protected bool CheckMainEntityModified() => entityIsModified;

    protected async Task CancelChangesAsync() => await ObtainDataAsync(BaseDataKeyId);

    protected void ShowCreateNewPopup()
    {
        CleanCreateNewEntityData();
        createNewDataChangedNotificationMode = DataChangedNotificationMode.Always;
        RefMainDetailsLayout.UpdateCreateButtonsState(false);
        RefMainDetailsLayout.ShowCreateNewEntityPopup();
        StateHasChanged();
    }

    protected void MainEntityIdChanged(string value)
    {
        if (BaseDataId != value)
        {
            BaseDataId = value;
            mainEntityIdChanged = true;
            ValidateMainEntityFields();
        }
    }

    private async void BeforeLocationChanged(object sender, LocationChangingEventArgs e)
    {
        if (entityIsModified && !Navigation.Uri.Equals(e?.NewLocation))
        {
            e.NavigationCanceled = true;
            newLocationUrl = e.NewLocation;
            await ShowUnsavedChangesConfirmation(EntityDetailsConfirmationContext.Navigation);
        }
    }

    private async Task HandleComponentDataChangedAsync(ComponentDataChangedNotification notification)
    {
        if (RefMainDetailsLayout.CheckCreateNewEntityPopupVisible())
            RefMainDetailsLayout.UpdateCreateButtonsState(CheckCreateNewEntityIsValid());
        else
            await UpdateEntityModifiedAsync(true);
    }

    private async void HandleMainLayoutActionAsync(object sender, BeforeMainActionCompletedEventArgs e)
    {
        if (entityIsModified)
        {
            actionToBeExecuted = e.Action;
            // Cancel action.
            e.Canceled = true;

            await ShowUnsavedChangesConfirmation(EntityDetailsConfirmationContext.MainLayoutAction);
        }
    }

    private async Task UpdateEntityModifiedAsync(bool value)
    {
        if (entityIsModified != value)
        {
            entityIsModified = value;
            entityChangedNotificationMode = value ? DataChangedNotificationMode.Never : DataChangedNotificationMode.Once;
            await _browserService.PreventNavigation(value);
        }
    }

    private async Task<bool> UpdateDataAndClearFlagsAsync()
    {
        var dataUpdated = await UpdateDataAsync();
        if (dataUpdated)
            await ObtainDataAsync(BaseDataKeyId);
        return dataUpdated;
    }

    private async Task ClearMainEntityFlagsAsync()
    {
        mainEntityIdChanged = false;
        RefMainDetailsLayout.RefreshBaseData();
        RefMainDetailsLayout.UpdateEntityIsValid(null);
        await UpdateEntityModifiedAsync(false);
        StateHasChanged();
        ResetMainEntityFieldsValidation();
    }

    private async Task HandleCompleteConfirmationAsync(CompleteConfirmationNotification notification)
    {
        switch (activeConfirmation)
        {
            case EntityDetailsConfirmationNotificationTypes.EntityIdChanged:
                if (notification.Result == ConfirmationResult.Yes)
                {
                    if (await UpdateDataAndClearFlagsAsync())
                        await CompleteUnsavedChangesConfirmationAsync();
                }
                else if (notification.Result == ConfirmationResult.Cancel)
                    await CompleteUnsavedChangesConfirmationAsync(true);
                break;

            case EntityDetailsConfirmationNotificationTypes.UnsavedChanges:
                if (notification.Result == ConfirmationResult.Yes)
                {
                    if (await SaveChangesAsync())
                        await CompleteUnsavedChangesConfirmationAsync();
                }
                else if (notification.Result == ConfirmationResult.No)
                {
                    await UpdateEntityModifiedAsync(false);
                    await CompleteUnsavedChangesConfirmationAsync(true);
                }
                break;
        }
        activeConfirmationContext = null;
    }

    private async Task HandleHideToastNotificationAsync(HideToastNotification notification)
    {
        TdTextBox refFieldId = RefMainDetailsLayout.CheckCreateNewEntityPopupVisible() ? RefCreateNewFieldId : RefMainEntityFieldId;
        if (refFieldId != null)
            await _browserService.FocusElement(refFieldId.TextBoxId);
    }

    private async Task<bool> AddAndObtainDataAsync()
    {
        int keyId = await AddDataAsync();
        // In case domain validation message "ID not unique" is shown, item key identifier is set to 0.
        if (keyId > 0)
        {
            createNewDataChangedNotificationMode = DataChangedNotificationMode.Never;
            await ObtainDataAsync(keyId);
        }
        return keyId > 0;
    }

    private async Task CompleteUnsavedChangesConfirmationAsync(bool cancelChanges = false)
    {
        switch (activeConfirmationContext)
        {
            case EntityDetailsConfirmationContext.CreateNew:
                if (cancelChanges)
                    await CancelChangesAsync();
                ShowCreateNewPopup();
                break;
            case EntityDetailsConfirmationContext.Navigation:
                Navigation.NavigateTo(newLocationUrl, false);
                break;
            case EntityDetailsConfirmationContext.MainLayoutAction:
                if (cancelChanges)
                    await CancelChangesAsync();
                actionToBeExecuted?.Invoke();
                break;
        }
    }

    private async Task ShowMainEntityIdChangedConfirmation()
    {
        if (RefMainEntityFieldId != null)
            await _browserService.FocusElement(RefMainEntityFieldId.TextBoxId);

        activeConfirmation = EntityDetailsConfirmationNotificationTypes.EntityIdChanged;

        var notification = new ShowConfirmationNotification(
            SharedLocalizer["keyFieldChanges"],
            $"{ContentUrls.ResourceImg}notifications/warningYellow.svg",
            string.Format(SharedLocalizer["idValueHasBeenChanged"], TdTablesLocalizer[MainEntityIdFieldName], MainEntityTitle.ToLower()),
            TdSharedLocalizer["yes"],
            TdSharedLocalizer["no"]);

        await Mediator.Publish(notification);
    }

    private async Task ShowUnsavedChangesConfirmation(EntityDetailsConfirmationContext context)
    {
        activeConfirmation = EntityDetailsConfirmationNotificationTypes.UnsavedChanges;
        activeConfirmationContext = context;
        var notification = new ShowConfirmationNotification(
            SharedLocalizer["saveChanges"],
            $"{ContentUrls.ResourceImg}notifications/warningYellow.svg",
            string.Format(SharedLocalizer["thereAreUnsavedChanges"], MainEntityTitle.ToLower()),
            TdSharedLocalizer["save"],
            TdSharedLocalizer["cancel"],
            TdSharedLocalizer["dontSave"]);
        await Mediator.Publish(notification);
    }
}    