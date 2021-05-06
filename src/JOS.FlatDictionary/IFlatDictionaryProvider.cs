using System.Collections.Generic;

namespace JOS.FlatDictionary
{
    public interface IFlatDictionaryProvider
    {
        Dictionary<string, string> Execute(object @object, string prefix = "");
    }
}
