namespace Attriba;

public class SpecificationQueryParameterParser<T>(Func<KeyValuePair<string, string>, ISpecification<T>> unitSpecificationFactory, string orSeparator = ",", string andSeparator = "~")
{
    public ISpecification<T> Parse(KeyValuePair<string, string> pair)
    {
        string[] values = pair.Value.Split(orSeparator);
        var orPairs = values.Select(value => new KeyValuePair<string, string>(pair.Key, value));
        var orSpecs = orPairs.Select(ParseAsAnd);
        
        return new OrSpecification<T>(orSpecs);
    }

    public ISpecification<T> ParseAsAnd(KeyValuePair<string, string> kvp)
    {
        var andPairs = kvp.Split(andSeparator);
        var andSpecs = andPairs.Select(x => unitSpecificationFactory(x));
        return new AndSpecification<T>(andSpecs);
    }
}

public static class KeyValuePairExtensions
{
    public static IEnumerable<KeyValuePair<string, string>> Split(this KeyValuePair<string, string> kvp, string separator)
    {
        var keyParts = kvp.Key.Split(separator);
        var valueParts = kvp.Value.Split(separator);

        if (keyParts.Length != valueParts.Length)
        {
            throw new ArgumentException($"Key and value must have the same number of parts, found {keyParts.Length} key part and {valueParts.Length} value parts");
        }

        for (var i = 0; i < keyParts.Length; i++)
        {
            yield return new KeyValuePair<string, string>(keyParts[i], valueParts[i]);
        }
    }
}