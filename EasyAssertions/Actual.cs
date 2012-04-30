namespace EasyAssertions
{
    public class Actual<T>
    {
        private readonly T actual;

        public Actual(T actual)
        {
            this.actual = actual;
        }

        public T And
        {
            get { return actual; }
        }
    }
}