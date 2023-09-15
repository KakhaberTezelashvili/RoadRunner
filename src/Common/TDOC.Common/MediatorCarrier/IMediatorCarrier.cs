using MediatR;

namespace TDOC.Common.MediatorCarrier;

/// <summary>
/// Defines methods for subscribe/unsubscribe to and from MediatR notifications.
/// </summary>
public interface IMediatorCarrier
{
    void Subscribe<TNotification>(Action<TNotification> handler)
        where TNotification : INotification;

    void Subscribe<TNotification>(Action<TNotification, CancellationToken> handler)
        where TNotification : INotification;

    void Subscribe<TNotification>(Func<TNotification, Task> handler)
        where TNotification : INotification;

    void Subscribe<TNotification>(Func<TNotification, CancellationToken, Task> handler)
        where TNotification : INotification;

    void Unsubscribe<TNotification>(Action<TNotification> handler)
        where TNotification : INotification;

    void Unsubscribe<TNotification>(Action<TNotification, CancellationToken> handler)
        where TNotification : INotification;

    void Unsubscribe<TNotification>(Func<TNotification, Task> handler)
        where TNotification : INotification;

    void Unsubscribe<TNotification>(Func<TNotification, CancellationToken, Task> handler)
        where TNotification : INotification;
}