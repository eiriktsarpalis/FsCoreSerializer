﻿namespace MBrace.FsPickler

/// <summary>
///     XML pickler instance.
/// </summary>
[<Sealed; AutoSerializable(false)>]
type XmlSerializer =
    inherit FsPicklerTextSerializer
        
    val private format : XmlPickleFormatProvider
        
    /// <summary>
    ///     Define a new Xml pickler instance.
    /// </summary>
    /// <param name="indent">Enable indentation of output XML pickles.</param>
    /// <param name="typeConverter">Define a custom type name converter.</param>
    new (registry:IPicklerPluginRegistry, [<O;D(null)>] ?indent : bool, [<O;D(null)>] ?typeConverter : ITypeNameConverter) =
        let xml = new XmlPickleFormatProvider(defaultArg indent false)
        { 
            inherit FsPicklerTextSerializer(xml, ?typeConverter = typeConverter, registry=registry)
            format = xml    
        }

    /// <summary>
    ///     Define a new Xml pickler instance.
    /// </summary>
    /// <param name="indent">Enable indentation of output XML pickles.</param>
    /// <param name="typeConverter">Define a custom type name converter.</param>
    new ([<O;D(null)>] ?indent : bool, [<O;D(null)>] ?typeConverter : ITypeNameConverter) =
        XmlSerializer(PicklerPluginRegistry.Default, ?indent=indent, ?typeConverter=typeConverter)

    /// Gets or sets indentation of serialized pickles.
    member x.Indent
        with get () = x.format.Indent
        and set b = x.format.Indent <- b