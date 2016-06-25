﻿namespace Nessos.FsPickler.Json

open System
open System.IO
open System.Text

open Newtonsoft.Json
open Newtonsoft.Json.Bson

open Nessos.FsPickler

/// <summary>
///     BSON format factory methods.
/// </summary>
type BsonPickleFormatProvider(?omitHeader : bool) =
    let omitHeader = defaultArg omitHeader false
    interface IPickleFormatProvider with

        member __.Name = "Bson"
        member __.DefaultEncoding = Encoding.UTF8

        member __.CreateWriter(stream : Stream, encoding : Encoding, _ : bool, leaveOpen : bool) =
#if NET35 || NET40
            let bw = new BinaryWriter(stream, encoding)
#else
            let bw = new BinaryWriter(stream, encoding, leaveOpen)
#endif
            let bsonWriter = new BsonWriter(bw)
            new JsonPickleWriter(bsonWriter, omitHeader, false, false, null, leaveOpen) :> _

        member __.CreateReader(stream : Stream, encoding : Encoding, _ : bool, leaveOpen : bool) =
#if NET35 || NET40
            let br = new BinaryReader(stream, encoding)
#else
            let br = new BinaryReader(stream, encoding, leaveOpen)
#endif
            let bsonReader = new BsonReader(br)
            new JsonPickleReader(bsonReader, omitHeader, false, false, leaveOpen) :> _