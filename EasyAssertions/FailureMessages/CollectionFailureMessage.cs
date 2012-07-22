using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SmartFormat;

namespace EasyAssertions
{
    public class CollectionFailureMessage : FailureMessage
    {
        private const int SampleSize = 10;

        protected ICollection<object> RawActualItems;

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

        protected ICollection<object> RawExpectedItems;

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

        public string ExpectedSample
        {
            get { return Sample(ExpectedSampleItems); }
        }

        public string ActualSample
        {
            get { return Sample(ActualSampleItems); }
        }

        private string Sample(ICollection<object> items)
        {
            switch (items.Count)
            {
                case 0:
                    return "empty.";
                case 1:
                    return "[" + items.Single() + "]";
                default:
                    return "[{ActualSampleItems:{0.BR}    {}|,}{BR}]".FormatSmart(this);
            }
        }

        public virtual ICollection<object> ExpectedSampleItems
        {
            get
            {
                return ExpectedItems.Count > SampleSize
                    ? ExpectedItems.Take(SampleSize).Concat(new[] { "..." }).ToList()
                    : ExpectedItems;
            }
        }

        public virtual ICollection<object> ActualSampleItems
        {
            get
            {
                return ActualItems.Count > SampleSize
                    ? ActualItems.Take(SampleSize).Concat(new[] { "..." }).ToList()
                    : ActualItems;
            }
        }

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