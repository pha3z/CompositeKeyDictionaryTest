## CompositeKeyDictionaryTest
A performance comparison of Composite Key Dictionary vs Nested Dictionary

Depends on Nuget pkg BenchmarkDotNet.

There are two Data Structure Classes:
- CompositeDictionary
- NestedDictionary

Both implement ICompositeDictionary

Tests are run using different composite keys of type:
int, int
string, string
int, string
string, int

Seperate LOOKUP and ADD tests are run.

Hardcoded values that you can change are:
- Total number of First Parts of keys (same data used for storage values)
- Total number of Second Parts of keys

In the case of a nested dictionary, the number of first part keys will determine the number of nested dictionaries.
The second part of keys is saved in the nested dictionaries.

## Conclusion:
On my machine, the true Composite-Key Dictionary outperformed the Nested Dictionary... especially in some cases of ADD.

These tests were built fairly quickly just as a proof of concept. I was really only interested in proving that a ValueTuple CompositeKey dictionary would perform as fast or better than a nested dictionary. The tests prove that this is the case.  You can count on a standard composite key dictionary in most cases.

There is one situation where a Nested Dictionary would work better:
If your program needs to look up a particular subdictionary and then do successive lookups exclusively on that one subdictionary.
In such a case, the first lookup would load a chunk of the second-level dictionary into hot cache. Then successive lookups would be less likely to require memory fetches. Such as case could be enormously faster than a true composite key dictionary depending on the data layout in memory.



