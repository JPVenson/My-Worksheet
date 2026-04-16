namespace MyWorksheet.Website.Server.Services.ServerInfo.InfoItems;

//public class ServerPullPerformanceCounterListener : ServerPullInfoListener
//{
//	private readonly PerformanceCounter _counter;
//	private readonly string _counterCategory;
//	private readonly string _counterName;

//	public ServerPullPerformanceCounterListener(Action<string, object> publisher, string key,
//		string counterCategory, string counterName)
//		: this(publisher, key, new PerformanceCounter(counterCategory, counterName))
//	{
//	}

//	public ServerPullPerformanceCounterListener(Action<string, object> publisher, string key,
//		PerformanceCounter counter) : base(publisher)
//	{
//		_counterCategory = counter.CategoryName;
//		_counterName = counter.CounterName;
//		Key = key;
//		_counter = counter;
//		Consumers.Add(PublishValue);
//	}

//	public override string Key { get; }

//	private void Timer1_Elapsed(object sender, ElapsedEventArgs e)
//	{
//		PublishValue();
//	}

//	protected override void OnDispose()
//	{
//		Consumers.Remove(PublishValue);
//		_counter.Dispose();
//		base.OnDispose();
//	}

//	public override void PublishValue()
//	{
//		var counterSample = _counter.NextValue();
//		Publish(counterSample);
//	}
//}