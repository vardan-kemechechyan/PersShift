public interface ISubscribeUnsubscribe
{
	public int SubscribedTimes { get; set; }

	public void Subscribe();
	public void UnSubscribe();
}
