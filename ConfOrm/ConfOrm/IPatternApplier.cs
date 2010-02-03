namespace ConfOrm
{
	public interface IPatternApplier<TSubject, TResult> : IPattern<TSubject>
	{
		TResult Apply(TSubject subject);
	}
}