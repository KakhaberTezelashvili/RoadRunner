namespace AdminClient.WebApp.Shared.Models;

/// <summary>
/// <see cref="EventArgs" /> for <see cref="MainLayout.BeforeMainActionCompleted" />.
/// </summary>
public class BeforeMainActionCompletedEventArgs : EventArgs
{
    /// <summary>
    /// Flag indicating a canceled action.
    /// </summary>
    public bool Canceled { get; set; }

    /// <summary>
    /// Action to be completed.
    /// </summary>
    public Action Action { get; set; }
}