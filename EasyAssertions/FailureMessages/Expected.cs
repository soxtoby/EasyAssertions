namespace EasyAssertions
{
    public interface Expected
    {
        /// <summary>
        /// The source representation of the expected value.
        /// </summary>
        string Expression { get; }

        /// <summary>
        /// The expected value.
        /// </summary>
        object Value { get; }
    }
}