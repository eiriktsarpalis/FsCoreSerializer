## FsPickler

[![Join the chat at https://gitter.im/nessos/FsPickler](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/nessos/FsPickler?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![NuGet Status](http://img.shields.io/nuget/vpre/FsPickler.svg?style=flat)](https://www.nuget.org/packages/FsPickler/)

FsPickler is a serialization library that facilitates the distribution of .NET objects.
The implementation focuses on performance and completeness in supported types, including F# types.
It supports multiple, pluggable serialization formats including Binary, Xml, JSON and BSON.
The library is based on the functional programming concept of 
[pickler combinators](http://lambda-the-ultimate.org/node/2243) 
which has been adapted to accommodate the object oriented nature of the .NET framework.

Packages of the library are available on Nuget [[1](http://www.nuget.org/packages/FsPickler),[2](http://www.nuget.org/packages/FsPickler.Json),[3](http://www.nuget.org/packages/FsPickler.CSharp)].

### Documentation

* [Tutorial](http://nessos.github.io/FsPickler/tutorial.html) A short introduction to FsPickler.
* [Technical Overview](http://nessos.github.io/FsPickler/overview.html) A walkthrough of the library's implementation details.
* [Performance](http://nessos.github.io/FsPickler/benchmarks.html) Benchmarks comparing FsPickler to other established serialization libraries.
* [API Reference](http://nessos.github.io/FsPickler/reference/index.html) Auto-generated library documentation.

All documentation and related material can be found [here](http://nessos.github.io/FsPickler/).

### Build Status

Head (branch `master`), Build & Unit tests

* Windows/.NET [![Build status](https://ci.appveyor.com/api/projects/status/vwthnxgal50ua8ej/branch/master)](https://ci.appveyor.com/project/nessos/fspickler)
* Linux/Mono 4.2 [![Build Status](https://travis-ci.org/nessos/FsPickler.png?branch=master)](https://travis-ci.org/nessos/FsPickler/branches)
