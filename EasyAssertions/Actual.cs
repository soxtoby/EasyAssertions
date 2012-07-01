

namespace EasyAssertions
{
    public class Actual<T>
    {
        internal readonly T Value;

        public Actual(T actual)
        {
            Value = actual;
        }

        public T And
        {
            get
            {
                SourceExpressionProvider.Instance.RegisterContinuation(0);
                return Value;
            }
        }
    }
}