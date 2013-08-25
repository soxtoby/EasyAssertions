using SmartFormat;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EasyAssertions
{
    /// <summary>
    /// A helper class for building collection assertion failure messages in a consistent format.
    /// </summary>
    public class CollectionFailureMessage : FailureMessage
    {
        private const int SampleSize = 10;

        private string itemType;
        /// <summary>
        /// The nature of the item being asserted on (e.g. "key" or "value").
        /// </summary>
        public string ItemType
        {
            get { return EscapeForTemplate(itemType); }
            set { itemType = value; }
        }

        /// <summary>
        /// Provides access to the items in the expected collection without modification.
        /// </summary>
        public ICollection<object> RawExpectedItems { get; protected set; }

        /// <summary>
        /// The collection of items that the actual collection is being compared against.
        /// Objects are wrapped in &lt; &gt; and strings are wrapped in " ".
        /// </summary>
        public virtual ICollection<object> ExpectedItems
        {
            get
            {
                return RawExpectedItems
                    .Select(Output)
                    .Cast<object>()
                    .ToList();
            }
            set { RawExpectedItems = value; }
        }

        /// <summary>
        /// Provides access to the items in the actual collection without modification.
        /// </summary>
        public ICollection<object> RawActualItems { get; protected set; }

        /// <summary>
        /// The collection of items being asserted on.
        /// Objects are wrapped in &lt; &gt; and strings are wrapped in " ".
        /// </summary>
        public virtual ICollection<object> ActualItems
        {
            get
            {
                return RawActualItems
                    .Select(Output)
                    .Cast<object>()
                    .ToList();
            }
            set { RawActualItems = value; }
        }

        /// <summary>
        /// Outputs a sample of the items in the <see cref="ExpectedItems"/> collection,
        /// or "empty." if there are no items in the collection.
        /// </summary>
        public string ExpectedSample
        {
            get { return Sample(ExpectedSampleItems); }
        }

        /// <summary>
        /// Outputs a sample of the items in the <see cref="ActualItems"/> collection,
        /// or "empty." if there are no items in the collection.
        /// </summary>
        public string ActualSample
        {
            get { return Sample(ActualSampleItems); }
        }

        /// <summary>
        /// Returns a sample of the first ten items in the given collection,
        /// or "empty." if there are no items in the collection.
        /// </summary>
        protected virtual string Sample(ICollection<object> items)
        {
            switch (items.Count)
            {
                case 0:
                    return "empty.";
                case 1:
                    return "[" + items.Single() + "]";
                default:
                    return "[{SampleItems:{0.BR}    {}|,}{BR}]".FormatSmart(new { SampleItems = SampleItems(items), BR });
            }
        }

        /// <summary>
        /// The first ten items of the <see cref="ExpectedItems"/> collection,
        /// with ellipses appended to the end if the collection contains more items.
        /// </summary>
        public virtual ICollection<object> ExpectedSampleItems
        {
            get { return SampleItems(ExpectedItems); }
        }

        /// <summary>
        /// The first ten items of the <see cref="ActualItems"/> collection,
        /// with ellipses appended to the end if the collection contains more items.
        /// </summary>
        public virtual ICollection<object> ActualSampleItems
        {
            get { return SampleItems(ActualItems); }
        }

        private static ICollection<object> SampleItems(ICollection<object> items)
        {
            return items.Count > SampleSize
                ? items.Take(SampleSize).Concat(new[] { "..." }).ToList()
                : items;
        }

        /// <summary>
        /// The source representation of the <see cref="ExpectedItems"/> collection.
        /// Returns null if the source representation is a collection initializer.
        /// </summary>
        public override string ExpectedExpression
        {
            get
            {
                string expectedExpression = TestExpression.GetExpected();
                return NewCollectionPattern.IsMatch(expectedExpression)
                    ? null
                    : expectedExpression;
            }
        }

        private static readonly Regex NewCollectionPattern = new Regex(@"^new.*\{.*\}");
    }
}