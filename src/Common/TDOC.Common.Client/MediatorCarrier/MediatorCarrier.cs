using System.Collections.Concurrent;
using TDOC.Common.MediatorCarrier;

namespace TDOC.Common.Client.MediatorCarrier;

/// <summary>
/// Defines methods for subscribe/unsubscribe to and from MediatR notifications.
/// </summary>
public class MediatorCarrier : IMediatorCarrier, INotificationHandler<INotification>
{
    private readonly ConcurrentDictionary<Type, ConcurrentBag<(Delegate action, bool needsToken)>> _actions = new();
    private readonly IExceptionService _exceptionService;

    /// <summary>
    /// Initializes a new instance of the <see cref="MediatorCarrier" /> class.
    /// </summary>
    /// <param name="exceptionService">Service provides methods to handle exceptions.</param>
    public MediatorCarrier(IExceptionService exceptionService)
    {
        _exceptionService = exceptionService;
    }

    public Task Handle(INotification notification, CancellationToken cancellationToken)
    {
        async Task HandleLocal(INotification n, CancellationToken c)
        {
            Type notificationType = n.GetType();

            if (!_actions.TryGetValue(notificationType, out ConcurrentBag<(Delegate action, bool needsToken)> subscribers))
                return;

            foreach ((Delegate action, bool needsToken) in subscribers)
            {
                object[] parameters = needsToken ? new object[] { n, c } : new object[] { n };

                object result = action.DynamicInvoke(parameters);
                if (result is Task task)
                {
                    try
                    {
                        await task.ConfigureAwait(false);
                    }
                    catch (Exception exception)
                    {
                        await _exceptionService.ShowException(exception);
                    }
                }
            }
        }
        if (notification is null)
            throw new ArgumentNullException(nameof(notification));
        return HandleLocal(notification, cancellationToken);
    }

    public void Subscribe<TNotification>(Action<TNotification> handler)
        where TNotification : INotification => Subscribe<TNotification>((handler, false));

    public void Subscribe<TNotification>(Action<TNotification, CancellationToken> handler)
        where TNotification : INotification => Subscribe<TNotification>((handler, true));

    public void Subscribe<TNotification>(Func<TNotification, Task> handler)
        where TNotification : INotification => Subscribe<TNotification>((handler, false));

    public void Subscribe<TNotification>(Func<TNotification, CancellationToken, Task> handler)
        where TNotification : INotification => Subscribe<TNotification>((handler, true));

    public void Unsubscribe<TNotification>(Action<TNotification> handler)
        where TNotification : INotification => Unsubscribe<TNotification>((Delegate)handler);

    public void Unsubscribe<TNotification>(Action<TNotification, CancellationToken> handler)
        where TNotification : INotification => Unsubscribe<TNotification>((Delegate)handler);

    public void Unsubscribe<TNotification>(Func<TNotification, Task> handler)
        where TNotification : INotification => Unsubscribe<TNotification>((Delegate)handler);

    public void Unsubscribe<TNotification>(Func<TNotification, CancellationToken, Task> handler)
        where TNotification : INotification => Unsubscribe<TNotification>((Delegate)handler);

    private void Subscribe<TNotification>((Delegate handler, bool needsCancellation) subscriber)
        where TNotification : INotification
    {
        Type notificationType = typeof(TNotification);
        if (_actions.TryGetValue(notificationType, out ConcurrentBag<(Delegate action, bool needsToken)> subscribers))
            subscribers.Add(subscriber);
        else
            _actions.TryAdd(notificationType, new ConcurrentBag<(Delegate, bool)>(new[] { subscriber }));
    }

    private void Unsubscribe<TNotification>(Delegate handler)
        where TNotification : INotification
    {
        Type notificationType = typeof(TNotification);
        if (!_actions.TryGetValue(notificationType, out ConcurrentBag<(Delegate action, bool needsToken)> subscribers)) 
            return;
        var remainingSubscribers = new ConcurrentBag<(Delegate, bool)>(subscribers.Where(subscriber => !subscriber.action.Equals(handler)));
        _actions.TryRemove(notificationType, out _);
        _actions.TryAdd(notificationType, remainingSubscribers);
    }
}