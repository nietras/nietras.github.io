---
layout: post
title: Refreshing CNTK with CUDA 11.8 for Ada Lovelace support
---

This is a quick blog post to announce that I have refreshed the open source
[Microsoft Cognitive Toolkit (CNTK)](https://cntk.ai) library with CUDA 11.8
(and cuDNN 8.9.1.23) in order to get support for NVidia Ada Lovelace/40 series
GPUs. Work was completed in the PR [Building with Visual Studio 2022 (v143) and
CUDA 11.8 and protobuf 3.14.0 via
vcpkg](https://github.com/nietras/CNTK/pull/11) and the following things were
changed:

* Update to CUDA 11.8 and cuDNN 8.9.1.23
* Update from PlatformToolset `v141` to `v143` (Visual Studio 2022 MSVC)
* Support
  `compute_61,sm_61;compute_75,sm_75;compute_80,sm_80;compute_86,sm_86;compute_90,sm_90`.
  That is add Ada Lovelace (RTX 40xx).
* Use protobuf 3.14.0 via vcpkg.
* Add protobuf generated code to git.
* Add `Save` overload that takes `ModelFormat` to `FunctionShim.cs`, to enable
  saving as onnx from C#.
* Add `zlibwapi.dll` to CUDA nuget package since new dependency.
* Split `cublasLt.dll` out into separate nuget package since package size
  otherwise > 512 MB.
* Disable various warnings to avoid build breaking as those warnings treated as
  errors. Mostly data conversion warnings.

And fix various code issues. This builds upon the work detailed in [Reviving
CNTK with CUDA 11.4 for Ampere support]({{ site.baseurl
}}/2021/08/05/reviving-cntk-with-cuda-11-4/) with same comments as that. See the
release at:

[Release 2.9.0](https://github.com/nietras/CNTK/releases/tag/v2.9.0)

Like last time packages cannot be found on [nuget.org](https://nuget.org) since they are
too large, so they have been published to my GitHub packages feed at
[https://github.com/nietras?tab=packages](https://github.com/nietras?tab=packages).
They can also be downloaded as part of the above release.