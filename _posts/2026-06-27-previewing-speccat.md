---
layout: post
title: Previewing SpecCat.com - Compare CPU, APU, GPU Specifications Instantly
---

[SpecCat.com](https://speccat.com) is a web site I've made, mainly for myself
trying out LLMs, for easily comparing detailed specifications of different
silicon chips; CPUs, APUS, GPUs etc. as shown in screenshots of header below and
full at bottom. It should work reasonably well on mobile, but main use case was
for larger screens. I tried hard to make it colorful and fun, and dark only. I
made this a while ago but never got around to announcing it, so here we are.

Example top part of website for CPUs.
[![Speccat CPU Intro]({{ site.baseurl }}/images/2026-06-previewing-speccat/speccat-cpu-intro.png)](https://speccat.com)

Example top part of website for GPUs.
[![Speccat GPU Intro]({{ site.baseurl }}/images/2026-06-previewing-speccat/speccat-gpu-intro.png)](https://speccat.com)

I made this because I often check chip specifications, since I find it endlessly
fascinating but also for professional use. That is, I often go to
[ark.intel.com](https://ark.intel.com/), which unfortunately is not as great a
site as it once was. It's not dense enough. Slow too. And naturally you can't
compare to other chips/products from AMD, [AMD Product
Specifications](https://www.amd.com/en/products/specifications.html), or NVidia
GPUs, [Compare GeForce Graphics
Cards](https://www.nvidia.com/en-us/geforce/graphics-cards/compare/) or similar. 

I also often have to spec new PCs for work or friends and family based on a
budget. Similar to what I discussed in my old blog post from 2020 [Core
Developer PC™ v20.09.dGPU - AMD 3700X vs Intel i7-10700 8c/16t with NVidia 2060
Super]({{ site.baseurl }}/2020/09/08/CoreDeveloperPC-v20.09.dGPU/), you know
back when DDR RAM was dirt cheap ($110/850 DKK for 2 x 16 GB DDR4 3200)! So
having specs at hand makes that a lot easier.

There are, of course, several media sites that have comparisons like
[TechPowerUp](https://www.techpowerup.com) specs databases; [CPU Specs
Database](https://www.techpowerup.com/cpu-specs/), [GPU Specs
Database](https://www.techpowerup.com/gpu-specs/), [SSD Specs
Database](https://www.techpowerup.com/ssd-specs/). Or [CPU
Monkey](https://www.cpu-monkey.com/en/).

All of which are fine, but none really go into the detail I am looking for or
present such details in a dense enough way and allowing for easy comparison
across all details for many products. My main source has, thus, often been
[Wikipedia ♥](https://www.wikipedia.org/), which has great detailed tables, as
for example in:

* AMD [Zen 5](https://en.wikipedia.org/wiki/Zen_5)
* Intel [Panther Lake](https://en.wikipedia.org/wiki/Panther_Lake_(microprocessor))
* NVidia [Blackwell](https://en.wikipedia.org/wiki/Blackwell_(microarchitecture))
* NVidia [GeForce RTX 50 Series](https://en.wikipedia.org/wiki/GeForce_RTX_50_series)

Given my unhealthy obsession with performance/minimalism I created this as a
vanilla static html, css, javascript web site. Zero libraries or frameworks are
used. I spent quite a bit of time prompting and adjusting output to keep site
small and instant by bundling everything into just two assets, as shown in
Chrome Developer Tool below. Code is a mess for sure, though.

The **entire site clocks in at just ~30 KB**. A lot less than this blog post.
Basic design is very simple, I keep specs in json as js-files for each product
family. Then include these in `index.html`, which means it works fine locally
out-of-the-box, as part of deployment all this is then bundled into one single
`index.html` file together with `app.js` that contains site logic and the
favicon. The AI generated logo of a cat holding a silicon wafer then being the
only other asset, served as highly compressed `avif`-file for browsers that
support this (almost all). In comparison
[ark.intel.com](https://www.intel.com/content/www/us/en/ark.html) is 1.6 MB
(that's +50x more) for just going to welcome page.

![SpecCat Chrome Developer Network Traffic]({{ site.baseurl }}/images/2026-06-previewing-speccat/speccat-dev-network.png)

Given I have used LLMs for this and no matter what I provide no guarantees to
the correctness of the specifications. I checked as much as I could manually,
but if you find any mistakes please to let me know. It is very much a preview
and I do not know if I will invest more time in it or not. Probably depends on
others finding this useful or not, so feedback and suggestions are welcomed.

Ideally, I would like to expand this with other silicon products (e.g. gaming
consoles, mobile SOCs), but also include rumored specifications for upcoming
chips like [Zen 6](https://en.wikipedia.org/wiki/Zen_6) or [Nova
Lake](https://en.wikipedia.org/wiki/Nova_Lake_(microprocessor)) based on leaks
from [Moore's Law Is Dead](https://www.mooreslawisdead.com/) or similar, as
*when* to buy/update a PC for example is a cornerstone of any silicon
enthusiasts reasoning.

That's all!

[![SpecCat Cpu Comparison]({{ site.baseurl }}/images/2026-06-previewing-speccat/speccat-cpu.png)](https://speccat.com)
