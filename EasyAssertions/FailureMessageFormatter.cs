namespace EasyAssertions
{
    static class FailureMessageFormatter
    {
        private static IFailureMessageFormatter current;

        public static IFailureMessageFormatter Current => current ?? DefaultFailureMessageFormatter.Instance;

        public static void Override(IFailureMessageFormatter newFormatter)
        {
            current = newFormatter;
        }

        public static void Default()
        {
            current = null;
        }
    }
}