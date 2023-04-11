using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using System.Collections;
using System.Collections.Concurrent;

namespace OrleansIoT.Core.Observers;

// Taken from the suggested implementation here: https://github.com/dotnet/orleans/blob/e997335d2d689bb39e67f6bcf6fd70862a22c02f/test/Grains/TestGrains/ObserverManager.cs#L12
public class ObserverManager<TObserver> : ObserverManager<IAddressable, TObserver>
{
    public ObserverManager(TimeSpan expiration, ILogger log, string logPrefix) : base(expiration, log, logPrefix)
    {
    }
}

/// <summary>
/// Maintains a collection of observers.
/// </summary>
/// <typeparam name="TAddress">
/// The address type.
/// </typeparam>
/// <typeparam name="TObserver">
/// The observer type.
/// </typeparam>
public class ObserverManager<TAddress, TObserver> : IEnumerable<TObserver>
{
    /// <summary>
    /// The log prefix.
    /// </summary>
    private readonly string logPrefix;

    /// <summary>
    /// The observers.
    /// </summary>
    private readonly ConcurrentDictionary<TAddress, ObserverEntry> observers = new ConcurrentDictionary<TAddress, ObserverEntry>();

    /// <summary>
    /// The log.
    /// </summary>
    private readonly ILogger log;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObserverManager{TAddress,TObserver}"/> class. 
    /// </summary>
    /// <param name="expiration">
    /// The expiration.
    /// </param>
    /// <param name="log">The log.</param>
    /// <param name="logPrefix">The prefix to use when logging.</param>
    public ObserverManager(TimeSpan expiration, ILogger log, string logPrefix)
    {
        ExpirationDuration = expiration;
        this.log = log;
        this.logPrefix = logPrefix;
        GetDateTime = () => DateTime.UtcNow;
    }

    /// <summary>
    /// Gets or sets the delegate used to get the date and time, for expiry.
    /// </summary>
    public Func<DateTime> GetDateTime { get; set; }

    /// <summary>
    /// Gets or sets the expiration time span, after which observers are lazily removed.
    /// </summary>
    public TimeSpan ExpirationDuration { get; set; }

    /// <summary>
    /// Gets the number of observers.
    /// </summary>
    public int Count => observers.Count;

    /// <summary>
    /// Gets a copy of the observers.
    /// </summary>
    public IDictionary<TAddress, TObserver> Observers
    {
        get
        {
            return observers.ToDictionary(_ => _.Key, _ => _.Value.Observer);
        }
    }

    /// <summary>
    /// Removes all observers.
    /// </summary>
    public void Clear()
    {
        observers.Clear();
    }

    /// <summary>
    /// Ensures that the provided <paramref name="observer"/> is subscribed, renewing its subscription.
    /// </summary>
    /// <param name="address">
    /// The subscriber's address
    /// </param>
    /// <param name="observer">
    /// The observer.
    /// </param>
    /// <exception cref="Exception">A delegate callback throws an exception.</exception>
    public void Subscribe(TAddress address, TObserver observer)
    {
        // Add or update the subscription.
        var now = GetDateTime();
        ObserverEntry entry;
        if (observers.TryGetValue(address, out entry))
        {
            entry.LastSeen = now;
            entry.Observer = observer;
            if (log.IsEnabled(LogLevel.Debug))
            {
                log.LogDebug(logPrefix + ": Updating entry for {0}/{1}. {2} total subscribers.", address, observer, observers.Count);
            }
        }
        else
        {
            observers[address] = new ObserverEntry { LastSeen = now, Observer = observer };
            if (log.IsEnabled(LogLevel.Debug))
            {
                log.LogDebug(logPrefix + ": Adding entry for {0}/{1}. {2} total subscribers after add.", address, observer, observers.Count);
            }
        }
    }

    /// <summary>
    /// Ensures that the provided <paramref name="subscriber"/> is unsubscribed.
    /// </summary>
    /// <param name="subscriber">
    /// The observer.
    /// </param>
    public void Unsubscribe(TAddress subscriber)
    {
        log.LogDebug(logPrefix + ": Removed entry for {0}. {1} total subscribers after remove.", subscriber, observers.Count);
        observers.TryRemove(subscriber, out _);
    }

    /// <summary>
    /// Notifies all observers.
    /// </summary>
    /// <param name="notification">
    /// The notification delegate to call on each observer.
    /// </param>
    /// <param name="predicate">
    /// The predicate used to select observers to notify.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the work performed.
    /// </returns>
    public async Task Notify(Func<TObserver, Task> notification, Func<TObserver, bool> predicate = null)
    {
        var now = GetDateTime();
        var defunct = default(List<TAddress>);
        foreach (var observer in observers)
        {
            if (observer.Value.LastSeen + ExpirationDuration < now)
            {
                // Expired observers will be removed.
                defunct = defunct ?? new List<TAddress>();
                defunct.Add(observer.Key);
                continue;
            }

            // Skip observers which don't match the provided predicate.
            if (predicate != null && !predicate(observer.Value.Observer))
            {
                continue;
            }

            try
            {
                await notification(observer.Value.Observer);
            }
            catch (Exception)
            {
                // Failing observers are considered defunct and will be removed..
                defunct = defunct ?? new List<TAddress>();
                defunct.Add(observer.Key);
            }
        }

        // Remove defunct observers.
        if (defunct != default(List<TAddress>))
        {
            foreach (var observer in defunct)
            {
                observers.TryRemove(observer, out _);
                if (log.IsEnabled(LogLevel.Debug))
                {
                    log.LogDebug(logPrefix + ": Removing defunct entry for {0}. {1} total subscribers after remove.", observer, observers.Count);
                }
            }
        }
    }

    /// <summary>
    /// Notifies all observers which match the provided <paramref name="predicate"/>.
    /// </summary>
    /// <param name="notification">
    /// The notification delegate to call on each observer.
    /// </param>
    /// <param name="predicate">
    /// The predicate used to select observers to notify.
    /// </param>
    public void Notify(Action<TObserver> notification, Func<TObserver, bool> predicate = null)
    {
        var now = GetDateTime();
        var defunct = default(List<TAddress>);
        foreach (var observer in observers)
        {
            if (observer.Value.LastSeen + ExpirationDuration < now)
            {
                // Expired observers will be removed.
                defunct = defunct ?? new List<TAddress>();
                defunct.Add(observer.Key);
                continue;
            }

            // Skip observers which don't match the provided predicate.
            if (predicate != null && !predicate(observer.Value.Observer))
            {
                continue;
            }

            try
            {
                notification(observer.Value.Observer);
            }
            catch (Exception)
            {
                // Failing observers are considered defunct and will be removed..
                defunct = defunct ?? new List<TAddress>();
                defunct.Add(observer.Key);
            }
        }

        // Remove defunct observers.
        if (defunct != default(List<TAddress>))
        {
            foreach (var observer in defunct)
            {
                observers.TryRemove(observer, out _);
                if (log.IsEnabled(LogLevel.Debug))
                {
                    log.LogDebug(logPrefix + ": Removing defunct entry for {0}. {1} total subscribers after remove.", observer, observers.Count);
                }
            }
        }
    }

    /// <summary>
    /// Removed all expired observers.
    /// </summary>
    public void ClearExpired()
    {
        var now = GetDateTime();
        var defunct = default(List<TAddress>);
        foreach (var observer in observers)
        {
            if (observer.Value.LastSeen + ExpirationDuration < now)
            {
                // Expired observers will be removed.
                defunct = defunct ?? new List<TAddress>();
                defunct.Add(observer.Key);
            }
        }

        // Remove defunct observers.
        if (defunct != default(List<TAddress>) && defunct.Count > 0)
        {
            log.Info(logPrefix + ": Removing {0} defunct observers entries.", defunct.Count);
            foreach (var observer in defunct)
            {
                observers.TryRemove(observer, out _);
            }
        }
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
    /// </returns>
    public IEnumerator<TObserver> GetEnumerator()
    {
        return observers.Select(observer => observer.Value.Observer).GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// An observer entry.
    /// </summary>
    private class ObserverEntry
    {
        /// <summary>
        /// Gets or sets the observer.
        /// </summary>
        public TObserver Observer { get; set; }

        /// <summary>
        /// Gets or sets the UTC last seen time.
        /// </summary>
        public DateTime LastSeen { get; set; }
    }
}