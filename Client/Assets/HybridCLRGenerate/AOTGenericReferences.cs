using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"Google.Protobuf.dll",
		"Luban.Runtime.dll",
		"mscorlib.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// Google.Protobuf.Collections.RepeatedField.<GetEnumerator>d__29<int>
	// Google.Protobuf.Collections.RepeatedField.<GetEnumerator>d__29<object>
	// Google.Protobuf.Collections.RepeatedField<int>
	// Google.Protobuf.Collections.RepeatedField<object>
	// Google.Protobuf.FieldCodec.<>c<int>
	// Google.Protobuf.FieldCodec.<>c<object>
	// Google.Protobuf.FieldCodec.<>c__DisplayClass38_0<int>
	// Google.Protobuf.FieldCodec.<>c__DisplayClass38_0<object>
	// Google.Protobuf.FieldCodec.<>c__DisplayClass39_0<int>
	// Google.Protobuf.FieldCodec.<>c__DisplayClass39_0<object>
	// Google.Protobuf.FieldCodec.InputMerger<int>
	// Google.Protobuf.FieldCodec.InputMerger<object>
	// Google.Protobuf.FieldCodec.ValuesMerger<int>
	// Google.Protobuf.FieldCodec.ValuesMerger<object>
	// Google.Protobuf.FieldCodec<int>
	// Google.Protobuf.FieldCodec<object>
	// Google.Protobuf.IDeepCloneable<int>
	// Google.Protobuf.IDeepCloneable<object>
	// Google.Protobuf.IMessage<object>
	// Google.Protobuf.MessageParser.<>c__DisplayClass2_0<object>
	// Google.Protobuf.MessageParser<object>
	// Google.Protobuf.ValueReader<int>
	// Google.Protobuf.ValueReader<object>
	// Google.Protobuf.ValueWriter<int>
	// Google.Protobuf.ValueWriter<object>
	// System.Action<int>
	// System.Action<object>
	// System.ByReference<byte>
	// System.ByReference<int>
	// System.ByReference<object>
	// System.Collections.Generic.ArraySortHelper<int>
	// System.Collections.Generic.ArraySortHelper<object>
	// System.Collections.Generic.Comparer<int>
	// System.Collections.Generic.Comparer<object>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<int,object>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.EqualityComparer<int>
	// System.Collections.Generic.EqualityComparer<object>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ICollection<int>
	// System.Collections.Generic.ICollection<object>
	// System.Collections.Generic.IComparer<int>
	// System.Collections.Generic.IComparer<object>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerable<int>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerator<int>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.IEqualityComparer<int>
	// System.Collections.Generic.IList<int>
	// System.Collections.Generic.IList<object>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.List.Enumerator<int>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List<int>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.ObjectComparer<int>
	// System.Collections.Generic.ObjectComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<int>
	// System.Collections.Generic.ObjectEqualityComparer<object>
	// System.Collections.ObjectModel.ReadOnlyCollection<int>
	// System.Collections.ObjectModel.ReadOnlyCollection<object>
	// System.Comparison<int>
	// System.Comparison<object>
	// System.Func<int,int>
	// System.Func<object,int>
	// System.Func<object,object>
	// System.Func<object>
	// System.IEquatable<object>
	// System.Nullable<long>
	// System.Predicate<int>
	// System.Predicate<object>
	// System.ReadOnlySpan<byte>
	// System.ReadOnlySpan<int>
	// System.ReadOnlySpan<object>
	// System.Span<byte>
	// System.Span<int>
	// System.Span<object>
	// }}

	public void RefMethods()
	{
		// string Luban.StringUtil.CollectionToString<int>(System.Collections.Generic.IEnumerable<int>)
		// string Luban.StringUtil.CollectionToString<object>(System.Collections.Generic.IEnumerable<object>)
		// string string.Join<int>(string,System.Collections.Generic.IEnumerable<int>)
		// string string.Join<object>(string,System.Collections.Generic.IEnumerable<object>)
		// string string.JoinCore<int>(System.Char*,int,System.Collections.Generic.IEnumerable<int>)
		// string string.JoinCore<object>(System.Char*,int,System.Collections.Generic.IEnumerable<object>)
	}
}