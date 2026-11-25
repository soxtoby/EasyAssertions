namespace EasyAssertions;

/// <summary>
/// The frames of the assertions running in one execution flow.
/// Immutable, so flows that inherit the same stack, such as concurrent tasks started from a test, can't interfere with each other.
/// </summary>
/// <param name="Frames">Frames of running assertions, followed by the frames of the most recently finished assertions, which later assertions can chain onto.</param>
/// <param name="Depth">The number of assertions currently running, which is also the index of the next assertion's frame.</param>
/// <param name="LastIndex">The index of the frame that failure messages take their source expressions from: the innermost running assertion, or the one that failed.</param>
record AssertionStack(IReadOnlyList<AssertionFrame> Frames, int Depth, int LastIndex)
{
    public static readonly AssertionStack Empty = new(Array.Empty<AssertionFrame>(), 0, 0);

    public AssertionStack Enter(AssertionCall assertion, string actualSuffix, string expectedSuffix)
    {
        var frame = PreviousFrameAtDepth?.ChainAssertion(assertion)
            ?? assertion.CreateFrame(OuterFrame, actualSuffix, expectedSuffix);

        return new AssertionStack(Frames.Take(Depth).Append(frame).ToList(), Depth + 1, Depth);
    }

    public AssertionStack Exit(bool success) =>
        this with
            {
                Depth = Depth - 1,
                LastIndex = success ? LastIndex - 1 : LastIndex
            };

    public AssertionFrame? LastFrame => Frames.ElementAtOrDefault(LastIndex);

    AssertionFrame? PreviousFrameAtDepth => Frames.ElementAtOrDefault(Depth);
    AssertionFrame? OuterFrame => Frames.ElementAtOrDefault(Depth - 1);
}
