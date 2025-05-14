using System.Linq.Expressions;
using System.Text.Json;
using Nist;

namespace Attriba;

public interface IRecordWithJsonLabels
{
    JsonDocument Labels { get; }
}

public class LabelSpecification<TRecord>(string key, string value) : ISpecification<TRecord> where TRecord : IRecordWithJsonLabels
{
    public Expression<Func<TRecord, bool>> ToExpression() => 
        x => x.Labels.RootElement.GetProperty(key).GetString() == value;

    public static LabelSpecification<TRecord> FromPair(KeyValuePair<string, string> kvp) =>
        new(kvp.Key, kvp.Value);
}

public static class LabelsQueryableExtensions
{
    public static IQueryable<TRecord> FilteredByLabels<TRecord>(
        this IQueryable<TRecord> source,
        DictionaryQueryParameters labelQueryParameters
    ) where TRecord : IRecordWithJsonLabels
    {
        var parser = new SpecificationQueryParameterParser<TRecord>(
            LabelSpecification<TRecord>.FromPair
        );

        var specifications = labelQueryParameters.Select(parser.Parse);

        var specification = new AndSpecification<TRecord>(specifications);

        return source.Where(specification);
    }
}