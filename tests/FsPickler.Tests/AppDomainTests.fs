﻿namespace MBrace.FsPickler.Tests

open System
open System.IO

open NUnit.Framework

open MBrace.FsPickler

#if REMOTE_TESTS

type ``AppDomain Tests`` (pickleFormat : string) as self =
    inherit ``FsPickler Serializer Tests`` (pickleFormat)

    let remotePickler = self.PicklerManager.GetRemoteSerializer()

    override __.IsRemotedTest = true
    override __.Pickle (value : 'T) = 
        try remotePickler.Pickle<'T> value
        with :? FailoverSerializerException -> self.PicklerManager.Serializer.Pickle value

    override __.PickleF (pickleF : FsPicklerSerializer -> byte []) = 
        try remotePickler.PickleF pickleF
        with :? FailoverSerializerException -> pickleF self.PicklerManager.Serializer
#endif