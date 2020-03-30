﻿namespace MBrace.FsPickler.Json

open System
open System.IO
open System.Collections.Generic

open Newtonsoft.Json

open MBrace.FsPickler

#nowarn "44" // BsonWriter

/// <summary>
///     Json format serializer.
/// </summary>
type internal JsonPickleWriter (jsonWriter : JsonWriter, omitHeader, indented, isTopLevelSequence, separator, leaveOpen) =

    do 
        jsonWriter.Formatting <- if indented then Formatting.Indented else Formatting.None
        jsonWriter.CloseOutput <- not leaveOpen

    let isBsonWriter = match jsonWriter with :? Bson.BsonWriter -> true | _ -> false

    let mutable depth = 0
    let mutable isTopLevelSequenceHead = false
    let mutable currentValueIsNull = false

    let arrayStack = new Stack<int> ()
    do arrayStack.Push Int32.MinValue

    // do not write tag if omitting header or array element
    let omitTag () = (omitHeader && depth = 0) || arrayStack.Peek() = depth - 1

    interface IPickleFormatWriter with
        member __.Flush() = jsonWriter.Flush()
            
        member __.BeginWriteRoot (tag : string) =
            if omitHeader then () else

            jsonWriter.WriteStartObject()
            writePrimitive jsonWriter false "FsPickler" formatv4000
            writePrimitive jsonWriter false "type" tag

        member __.EndWriteRoot () = 
            if not omitHeader then jsonWriter.WriteEnd()

        member __.BeginWriteObject (tag : string) (flags : ObjectFlags) =

            if not <| omitTag () then
                jsonWriter.WritePropertyName tag

            if Enum.hasFlag flags ObjectFlags.IsNull then
                currentValueIsNull <- true
                jsonWriter.WriteNull()

            elif Enum.hasFlag flags ObjectFlags.IsSequenceHeader then
                if isTopLevelSequence && depth = 0 then
                    isTopLevelSequenceHead <- true
                else
                    jsonWriter.WriteStartArray()

                arrayStack.Push depth
                depth <- depth + 1
            else
                jsonWriter.WriteStartObject()
                depth <- depth + 1

                if flags = ObjectFlags.None then ()
                else
                    let flagCsv = mkFlagCsv flags
                    writePrimitive jsonWriter false "_flags" flagCsv

        member __.EndWriteObject () = 
            if currentValueIsNull then 
                currentValueIsNull <- false
            else
                depth <- depth - 1
                if arrayStack.Peek () = depth then
                    if isTopLevelSequence && depth = 0 then ()
                    else
                        jsonWriter.WriteEndArray()

                    arrayStack.Pop () |> ignore
                else
                    jsonWriter.WriteEndObject()

        member __.SerializeUnionCaseNames = true
        member __.UseNamedEnumSerialization = true

        member __.PreferLengthPrefixInSequences = false
        member __.WriteNextSequenceElement _ =
            if isTopLevelSequence && depth = 1 then
                if isTopLevelSequenceHead then
                    isTopLevelSequenceHead <- false
                else
                    jsonWriter.WriteWhitespace separator

        member __.WriteCachedObjectId id = writePrimitive jsonWriter false "id" id

        member __.WriteBoolean (tag : string) value = writePrimitive jsonWriter (omitTag ()) tag value
        member __.WriteByte (tag : string) value = writePrimitive jsonWriter (omitTag ()) tag value
        member __.WriteSByte (tag : string) value = writePrimitive jsonWriter (omitTag ()) tag value

        member __.WriteInt16 (tag : string) value = writePrimitive jsonWriter (omitTag ()) tag value
        member __.WriteInt32 (tag : string) value = writePrimitive jsonWriter (omitTag ()) tag value
        member __.WriteInt64 (tag : string) value = writePrimitive jsonWriter (omitTag ()) tag value

        member __.WriteUInt16 (tag : string) value = writePrimitive jsonWriter (omitTag ()) tag value
        member __.WriteUInt32 (tag : string) value = writePrimitive jsonWriter (omitTag ()) tag value
        member __.WriteUInt64 (tag : string) value = writePrimitive jsonWriter (omitTag ()) tag value

        member __.WriteSingle (tag : string) value = writePrimitive jsonWriter (omitTag ()) tag value
        member __.WriteDouble (tag : string) value = writePrimitive jsonWriter (omitTag ()) tag value
        member __.WriteDecimal (tag : string) value = writePrimitive jsonWriter (omitTag ()) tag (string value)

        member __.WriteChar (tag : string) value = writePrimitive jsonWriter (omitTag ()) tag value
        member __.WriteString (tag : string) value = writePrimitive jsonWriter (omitTag ()) tag value

        member __.WriteBigInteger (tag : string) value = writePrimitive jsonWriter (omitTag ()) tag (string value)

        member __.WriteGuid (tag : string) value = writePrimitive jsonWriter (omitTag ()) tag value
        member __.WriteTimeSpan (tag : string) value = writePrimitive jsonWriter (omitTag ()) tag (string value)

        // BSON spec mandates the use of Unix time; 
        // this has millisecond precision which results in loss of accuracy w.r.t. ticks
        // since the goal of FsPickler is to offer faithful representations of .NET objects
        // we choose to override the spec and serialize ticks outright.
        // see also https://json.codeplex.com/discussions/212067 
        member __.WriteDateTime (tag : string) value = 
            if isBsonWriter then
                if not <| omitTag() then
                    jsonWriter.WritePropertyName tag

                jsonWriter.WriteStartObject()
                writePrimitive jsonWriter false "kind" (int value.Kind)
                writePrimitive jsonWriter false "ticks" value.Ticks
                if value.Kind = DateTimeKind.Local then
                    let offset = TimeZoneInfo.Local.GetUtcOffset value
                    writePrimitive jsonWriter false "offset" offset.Ticks

                jsonWriter.WriteEndObject()
            else
                writePrimitive jsonWriter (omitTag ()) tag value

        member __.WriteDateTimeOffset (tag : string) value =
            if isBsonWriter then
                if not <| omitTag() then
                    jsonWriter.WritePropertyName tag

                jsonWriter.WriteStartObject()
                writePrimitive jsonWriter false "ticks" value.Ticks
                writePrimitive jsonWriter false "offset" value.Offset.Ticks
                jsonWriter.WriteEndObject()
            else
                writePrimitive jsonWriter (omitTag()) tag value

        member __.WriteBytes (tag : string) (value : byte []) =
            if not <| omitTag () then 
                jsonWriter.WritePropertyName tag

            match value with
            | null -> jsonWriter.WriteNull()
            | _ -> jsonWriter.WriteValue value

        member __.IsPrimitiveArraySerializationSupported = false
        member __.WritePrimitiveArray _ _ _ = raise <| NotSupportedException()

        member __.Dispose () = 
            if leaveOpen then jsonWriter.Flush()
            else jsonWriter.Close()