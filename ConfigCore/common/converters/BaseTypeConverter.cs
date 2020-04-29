namespace ConfigCore.common.converters
{
	public abstract class BaseTypeConverter<T>
	{
		public abstract int Priority
		{
			get;
		}

		public abstract T Convert(string value); 

	}
}
