---
layout: post
title: Reviving CNTK with CUDA 11.4 for Ampere support
---

In this blog post I will quickly cover how I've revived the Microsoft
open source Cognitive Toolkit [CNTK](https://cntk.ai) with CUDA 11.4 
in order to get support for NVidia Ampere/30 series GPUs.

This work is based on the community effort discussed in 
[Is anyone trying to build CNTK with CUDA 11.1?](https://github.com/microsoft/CNTK/issues/3835)
and not the least the branch [`2.7-cuda-11.1`](https://github.com/haryngod/CNTK/tree/2.7-cuda-11.1)
by [@haryngod](https://github.com/haryngod) who did the hard
work of fixing code due to breaking API changes in CUDA libraries.

I, however, did the work in incremental steps and with a focus on
reverse engineering nuget packages so new ones could be made. These
steps were done as a series of pull requests (PRs) in my 
[fork](https://github.com/nietras/CNTK)
of the repository.

 * [Building with Visual Studio 2019 and CUDA 10.2 (incl. patches)](https://github.com/nietras/CNTK/pull/1) -
   this PR and release simply updates the library to use CUDA 10.2 (from 10.0)
   to be able to build CNTK with as few changes as possible with Visual Studio 2019.
   However, still with the C++ v141 tools. The main idea here was to verify that
   I could build CNTK and that it worked as expected in pipelines at my work.
 * [Add SWIG generated wrapper C++ and C# code](https://github.com/nietras/CNTK/pull/3) -
   this PR simply adds the SWIG generated code for C# bindings to git. This is perhaps
   controversial but having this in git makes it much easier to inspect, debug etc.
 * [Add NuGet package definitions (reverse engineered)](https://github.com/nietras/CNTK/pull/4) -
   this PR adds `nuspec` files and similar reverse engineered from existing nuget packages
   for CNTK 2.7.0. This marks the first release of my fork with the first nuget packages 
   as [`Release 2.7.1`](https://github.com/nietras/CNTK/releases/tag/v2.7.1).
 * [CUDA 11.1.1](https://github.com/nietras/CNTK/pull/6) - building from
   the [`2.7-cuda-11.1`](https://github.com/haryngod/CNTK/tree/2.7-cuda-11.1) branch
   this PR switches to using CUDA 11.1.1. There were several other changes needed
   to get this to compile. This was released as 
   [`Release 2.8.0`](https://github.com/nietras/CNTK/releases/tag/v2.8.0).
 * [CUDA 11.4](https://github.com/nietras/CNTK/pull/7) - finally I update to the
   latest available CUDA version (11.4) at this time. This required a lot of changes,
   due to CUDA apparently doing more template instantiations with increasing version,
   requiring adding overloads of several low level methods before the code would compile.
   This was released as 
   [`Release 2.8.1`](https://github.com/nietras/CNTK/releases/tag/v2.8.1).

All the work was done on and for Windows x86-64 and was done as quickly as possible.
I only verified this works with the C# ML pipelines we have at my work, so there are
no guarantees this will work for your workloads.

Note that due to code generation changes that output will change depending on which
GPU you have. Our pipelines are fully reproducible (to the bit) on the same GPU, but
we observed minor changes in output on 20 series GPUs and of course on Ampere GPUs.
This is expected. Just as if the C++ compiler changes code generated for floating points
on CPUs.

