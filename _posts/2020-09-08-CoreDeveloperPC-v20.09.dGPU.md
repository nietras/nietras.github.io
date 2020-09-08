---
layout: post
title: Core Developer PC™ v20.09.dGPU - AMD 3700X vs Intel i7-10700 8c/16t with NVidia 2060 Super
---
**TLDR**: Wait... until the NVidia 30 series is available. If you can't 
I detail an 8 core/16t, 32 GB DDR4 RAM, 1 TB NVMe PCIe 3.0 SSD, PC with
the NVidia 2060 Super at about ~$1300/~10000 DKK* (* incl. VAT).

Assembling your own developer PC is seen as a rite of passage for many 
self respecting developers. Personally I have assembled perhaps more than 
fifty PCs both for personal use and for work. 
That is quite a lot and is because, at one point,
I assembled industrial PCs for work due to quality issues with previous PCs.
Those issues were mainly resolved by selecting quality components for industrial use,
not so much due to my PC assembly skills, I think 😅

Last year I assembled 3 new deep learning model servers used for all our
training needs at work in our fully versioned and reproducible machine learning
pipeline written in C# (incl. neural network definitions and training). 
We use Azure DevOps with agents running on these servers for scheduling and
keeping track of all work. Git and nuget packages for code, libraries and
versioned ground truth data. If you clone a training git repository and
run it on the same GPU you will **always** get the exact same results.
However, this is not the post for that 😉

I have always enjoyed selecting components for and assembling a PC. 
There is something very satisfactory in the PC  posting the first 
boot screen without issue after assembly. This doesn't happen
every time though 😅 Good news is it has newer been easier to assemble a PC.
And there are lots of videos online showing you how to.

So Inspired by Scott Hanselman's [Ultimate Developer PC](https://www.hanselman.com/blog/BuildingTheUltimateDeveloperPC30ThePartsListForMyNewComputerIronHeart.aspx)  and
Jeff Atwood's [Building a PC](https://blog.codinghorror.com/building-a-pc-part-ix-downsizing/) series this post explores selecting components for a core
developer PC at this point in time. Tomorrow or soon after everything will be 
different and choices will have to be revisited.

It isn't time for me to update my work-from-home PC just yet, though, 
I am definitely waiting for AMDs [Zen 3](https://en.wikipedia.org/wiki/Zen_3)
and NVidias consumer [Ampere](https://en.wikipedia.org/wiki/Ampere_(microarchitecture))
cards that have just been [announced](https://www.anandtech.com/show/16057/nvidia-announces-the-geforce-rtx-30-series-ampere-for-gaming-starting-with-rtx-3080-rtx-3090).
Especially, the latter looks promising with its new [TF32](https://blogs.nvidia.com/blog/2020/05/14/tensorfloat-32-precision-format/) floating point
format and fixed sparsity support for machine learning.

This means this blog post won't be going into detail on actually assembling the PC.
However, I had a request for a possible configuration of a decent
work-from-home desktop PC that can be used both for software 
development but also deep learning software engineering. 
So I thought I would do a write-up of a PC that is:

* Efficient
* Quiet (almost silent)
* Affordable
* Build to last
* Supports deep learning software engineering
* Unobtrusive with no frills, no RGB LEDs, no annoyances
* Wired - no wifi (always prefer a cable when you can)

This PC won't break any world records when it comes to performance.
It will knock the socks off most laptops without the sound of 
a jet turbine in your ears, though. 🛦
It also won't make you pay attention to it with colorful LEDs.
It's a [monolithic black silent box](https://en.wikipedia.org/wiki/Monolith_(Space_Odyssey)) 
like aliens intended personal compute to be. 😁
It will probably run [Age of Ascent](https://www.ageofascent.com/) and definitely [Age of Empires II](https://en.wikipedia.org/wiki/Age_of_Empires_II) without issue, 
but might take a little while on the final turns in long games of [Master of Orion II](https://en.wikipedia.org/wiki/Master_of_Orion_II:_Battle_at_Antares) 😉


![https://en.wikipedia.org/wiki/File:HAL_2001_monolith_(color_correction).jpg]({{ site.baseurl }}/images/2020-09-CorePC/HAL_2001_monolith_(color_correction).jpg)

The premise is pitting AMD vs Intel CPUs against each other at about the 
same price point. Peripherals are not considered. For GPU there is, in my view, 
not a lot of choice if you are doing deep learning. This is mostly a software 
question. 

## Prices
Prices below are either from [www.newegg.com](www.newegg.com) (US) or 
[www.edbpriser.dk](www.edbpriser.dk)/[www.pricerunner.dk](www.pricerunner.dk) (DK)
for both my American and Danish friends.
Prices are approximate and from the time of writing.
Danish prices incl. 25% VAT.
1 USD = 6.25 DKK.

## Composition
For this blog post I am dividing the components into three parts.
 * **Base** - components that form the basis of the PC and usually have a long longevity and can be the base of many revisions of the PC. This is the 
   cornerstone of assembling your own PC since it allows you to amortize cost
   of some componenents over a long time e.g. ~10 years in this case.
 * **Common** - these are the common system components that are the same no matter if an AMD or Intel CPU is used. Included in this is the GPU here.
 * **System** - the CPU and the CPU specific motherboard. 
   Both an AMD and Intel version will be compared.

## Base Components
Base components are often overlooked. If you buy a pre-built desktop PC these often
have lesser quality base components, even though these are the most likely components
to be reused. I encourage picking high quality components here since they will form
the base or foundation for the developer PC for years to come.

|Component      |Selection           |USD    |DKK    |
|----------|:-------------------|------:|-------:|
|[Case](https://en.wikipedia.org/wiki/Computer_case) |[Fractal Design Define Mini C](https://www.fractal-design.com/products/cases/define/define-mini-c/black/) | [85](https://www.newegg.com/black-fractal-design-define-mini-c-micro-atx-mini-tower/p/N82E16811352064)| [600](https://www.pricerunner.dk/pl/186-3663561/Kabinetter/Fractal-Design-Define-Mini-C-Sammenlign-Priser)|
|[PSU](https://en.wikipedia.org/wiki/ATX#Power_supply) |[Corsair RM650x 650W](https://www.corsair.com/eu/en/Categories/Products/Power-Supply-Units/Power-Supply-Units-Advanced/RMx-Series/p/CP-9020178-EU) | [115](https://www.newegg.com/corsair-rmx-series-rm650x-2018-cp-9020178-na-650w/p/N82E16817139232)| [900](https://www.edbpriser.dk/produkt/corsair-rmx-series-rm650x-3149708/?searched=rm650x)|
|[CPU Cooler](https://en.wikipedia.org/wiki/Computer_cooling) |[Noctua NH-12S](https://noctua.at/en/nh-u12s)|[65](https://www.newegg.com/noctua-nh-u12s/p/N82E16835608040)|[440](https://www.pricerunner.dk/pl/184-3500421/Computer-koeling/Noctua-NH-U12S-Sammenlign-Priser)|
|**Total** |-|**265**|**1940**|

### Case
For the case I went for a micro-ATX [Fractal Design Define Mini C](https://www.fractal-design.com/products/cases/define/define-mini-c/black/) case that 
is an updated version of the case next to me right now. It's designed for 
silent computing with built-in sound dampening material. 
How efficient this is I cannot quantify, but
overall I think this is a great case with a good internal design.

[![Fractal Design Define Mini C]({{ site.baseurl }}/images/2020-09-CorePC/Case-Fractal-Design-Define-Mini-C_16.jpg)](https://www.fractal-design.com/products/cases/define/define-mini-c/black/)

The case comes with two decent 120mm fans [Fractal Design Dynamic X2 GP12](https://www.fractal-design.com/products/fans/dynamic/dynamic-x2-gp-12/black/). 
However, if you want better fans and fans that will last a long time 
I would recommend Noctua fans instead. I had to replace the fans that came 
with my case after about 4 years, I think, since they started being noisy.

The one thing that I really don't like on this case is the front panel connectors
are facing up. Since I have the PC sitting in a mount underneath my desk, this
is a bit annoying, since I can't plug in an USB pen here due to lack of height. 🤦‍

[![Fractal Design Define Mini C]({{ site.baseurl }}/images/2020-09-CorePC/Case-Fractal-Design-Define-Mini-C_11.jpg)](https://www.fractal-design.com/products/cases/define/define-mini-c/black/)


### PSU
A high quality PSU is essential to a stable and silent PC. Luckily there is 
a lot of choice here now a days. A big question here is always how many watts
the PSU should support. I think there is a tendency to go too high here for many,
unless you are seriously considering getting an expensive high-end GPU 
(like the NVidia 3090) with a similar very high-end 
server CPU 650W should be more than enough for most. It may last you 10 years though,
so keep that in mind and perhaps opt for getting a PSU with some headroom 
for the years to come.

Here I picked the [Corsair RM650x 650W](https://www.corsair.com/eu/en/Categories/Products/Power-Supply-Units/Power-Supply-Units-Advanced/RMx-Series/p/CP-9020178-EU)
because it is completely silent with the fan off when power consumption is low, and
still nearly silent at full load. Additionally [reviews](https://www.tomshardware.com/reviews/corsair-rm550x-power-supply,4484.html) of this are great and it
has very low voltage ripple.

As can be seen below this is a modular PSU. Hence you can keep the case neat using
just the cables needed for powering the components. On the downside this isn't 
exactly a cheap PSU, but given it has a 10 year warranty and will likely last that
long it's worth it.

[![Corsair RM650x 650W]({{ site.baseurl }}/images/2020-09-CorePC/PSU-Corsair-RM650x-with-cables.png)](https://www.corsair.com/eu/en/Categories/Products/Power-Supply-Units/Power-Supply-Units-Advanced/RMx-Series/p/CP-9020178-EU)


### CPU Cooler
For CPU coolers I have one rule: **it must be screw mounted**. 
Or at least mounted with a robust mechanism, not push-pin. 
I have seen my share of push-pin coolers getting loose during 
transportation or similar. Crappy stock coolers should be 
avoided as much as possible. Additionally, prefer coolers with 
PWM fans so the fan speed can be controlled. However, some 
motherboards support controlling the speed by regulating voltage.
Stock coolers are often also very loud or poor at keeping modern
CPUs cool.

I picked the [Noctua NH-12S](https://noctua.at/en/nh-u12s) here since it's 
a very quiet high quality cooler with PWM support and a robust 
mounting system. This cooler has 6 year warranty and Noctua is known for
supporting future sockets via new mounting kits and hence it very likely
will be able to be used for a possible future update and last +6 years.
Replacing the fan is easy too.

[![Noctua NH-12S]({{ site.baseurl }}/images/2020-09-CorePC/CPU-Cooler-Noctua-nh-u12s-with-boxes.jpg)](https://noctua.at/en/nh-u12s)


## Common Components incl. GPU

|Component      |Selection           |USD    |DKK    |
|----------|:-------------------|------:|-------:|
|[RAM](https://en.wikipedia.org/wiki/DDR4_SDRAM) |[G.SKILL Ripjaws V 2 x 16GB DDR4 3200 (PC4-25600) F4-3200C16D-32GVK](https://www.gskill.com/product/165/184/1536110922/F4-3200C16D-32GVKRipjaws-VDDR4-3200MHz-CL16-18-18-38-1.35V32GB-(2x16GB)) | [110](https://www.newegg.com/g-skill-32gb-288-pin-ddr4-sdram/p/N82E16820232091) | [850](https://www.edbpriser.dk/produkt/g-skill-ripjaws-v-ddr4-32-gb-2-x-16-gb-dimm-288-pin-3513768/) |
|[SSD](https://en.wikipedia.org/wiki/Solid-state_drive) [NVMe](https://en.wikipedia.org/wiki/NVM_Express) | [ADATA XPG SX8200 Pro 1 TB](https://www.xpg.com/us/feature/583/) | [140](https://www.newegg.com/xpg-sx8200-pro-1tb/p/0D9-0017-000W4)| [1030](https://www.edbpriser.dk/produkt/adata-xpg-sx8200-pro/)|
|[GPU]() | [Gigabyte GeForce RTX 2060 SUPER WINDFORCE OC 8G](https://www.gigabyte.com/Graphics-Card/GV-N206SWF2OC-8GD-rev-20#kf)| [(a) 400](https://www.newegg.com/gigabyte-geforce-rtx-2060-super-gv-n206swf2oc-8gd/p/N82E16814932174) | [3200](https://www.edbpriser.dk/produkt/gigabyte-geforce-rtx-2060-super-windforce-oc-8g/)|
|**Total** |-|**650**|**5080**|

**(a)** Get Tom Clancy’s Rainbow Six Siege Gold Edition w/ purchase, limited offer

### RAM
Cheap, fast enough and plenty RAM. Not much to say here. As an absolute minimum
you should have 16GB, and always be sure you have as many modules as there are
channels on the CPU. Dual channel in this case. If you only have one module when
the CPU supports dual channel, you only get half the bandwidth. I have seen
this mistake happen too many times. And there rarely is a price 
difference these days. In general consumer CPUs/motherboards are dual channel, 
and high end server CPUs have more. 8 for example.

Be aware of RAM height and CPU cooler clearance here. If the RAM module is 
very high and the CPU cooler does not have a lot of clearance they may clash.

[![G.SKILL Ripjaws V 2 x 16GB DDR4 3200 (PC4-25600) F4-3200C16D-32GVK]({{ site.baseurl }}/images/2020-09-CorePC/RAM-G.SKILL-Ripjaws-V-2-x-16GB-DDR4-3200-(PC4-25600)-F4-3200C16D-32GVK.png)](https://www.gskill.com/product/165/184/1536110922/F4-3200C16D-32GVKRipjaws-VDDR4-3200MHz-CL16-18-18-38-1.35V32GB-(2x16GB))


### NVMe SSD
A key component for total system performance, 
especially for developers, is the NVMe SSD. For OS and git repos this 
is a no brainer. Need more space for other things, add a SATA HDD 
or cheap SSD.

I picked the [ADATA XPG SX8200 Pro 1 TB](https://www.xpg.com/us/feature/583/)
NVMe SSD based on multiple reviews of NVMe SSDs. [Anandtech](https://www.anandtech.com)
has a recent review that includes this in the [The Best NVMe SSD for Laptops and Notebooks: SK hynix Gold P31 1TB SSD Reviewed](https://www.anandtech.com/show/16012/the-sk-hynix-gold-p31-ssd-review) article.

[![ADATA XPG SX8200 Pro 1 TB]({{ site.baseurl }}/images/2020-09-CorePC/SSD-NVMe-Adata-XPG-SX8200-PRO-Gen3x4-1-TB.png)](https://www.xpg.com/us/feature/583/)

Below I include a few key benchmarks that I base my choice on for a developer PC.
Mixed 4kB and 128 kB Random Read/Write. Note the highlighted SSD is the 
SK hynix Gold P31 1TB SSD which isn't available yet. Overall the 
SX8200 Pro is great and most importantly it is way cheaper than the otherwise
great Samsung 970 EVO Plus by 30-40% while being just as fast or close to.

Again this SSD has a 5 year warranty and should be usable for at least that long.

512 GB should suffice for many here but performance often scales with size.

[![Mixed 4kB Random Read/Write]({{ site.baseurl }}/images/2020-09-CorePC/SSD-NVMe-sustained-rm.png)](https://www.anandtech.com/show/16012/the-sk-hynix-gold-p31-ssd-review/6)

[![Mixed 128kB Random Read/Write]({{ site.baseurl }}/images/2020-09-CorePC/SSD-NVMe-sustained-sm.png)](https://www.anandtech.com/show/16012/the-sk-hynix-gold-p31-ssd-review/6)

I have not picked a PCIe 4.0 SSD since there aren't that many yet,
and price difference is still a bit high. More SSDs will come soon. Not the least
the Samsung 980 series which looks [very promising](https://wccftech.com/samsung-980-pro-pcie-4-0-ssds-official-with-up-to-1-tb-capacities/). 
It will probably be expensive, though, but 5,000 MB/s sequential write 
would be nice in some of the industrial systems I work on. That's fast 🚀
Only the AMD system supports PCIe 4.0, another reason to chose AMD.


### GPU
For deep learning software engineering there isn't a lot of choice for GPU
when it comes to manufacturer, in my view. NVidia is the defacto choice and this is
due to it's broad set of software libraries like [cuDNN](https://developer.nvidia.com/cudnn) and [TensorRT](https://developer.nvidia.com/tensorrt).

Now a lot of deep learning articles claim you can only be serious about 
deep learning if you buy at least a 2080 Ti (now [RTX 3090](https://www.nvidia.com/en-us/geforce/graphics-cards/30-series/rtx-3090/)) or even the ridiculously priced
[Titan RTX](https://www.pricerunner.dk/pl/37-4745518/Grafikkort/Nvidia-TITAN-RTX-Sammenlign-Priser). As everything else, [it depends](https://wiki.c2.com/?ItDepends) on what you are doing.
Sure if you are building state-of-the-art models for [ImageNet](https://en.wikipedia.org/wiki/ImageNet) 
then you need a lot of compute and not the least a lot of GPU RAM, but
for those that are doing real-world industrial vision you can go a long way 
with less if you ignore the "bigger is better" mantra of GPU manufacturers,
cloud providers and people working only on big tech data. There is a lot of
hyperbole here. Real world data for industrial vision is very different from
these extreme scenarios.

Anyway, the main point here was that we need a GPU for **software engineering**.
To be able to run, test, debug code related to GPU compute. For this I would pick
the cheapest, best value latest GPU supporting all features of the most recent NVidia
architecture. That was [Turing](https://en.wikipedia.org/wiki/Turing_(microarchitecture)), but is now [Ampere](https://en.wikipedia.org/wiki/Ampere_(microarchitecture)). 

At the time of writing this, the Ampere based Geforce RTX 30 Series is
not yet available, though. And it wasn't when I got a request for my pick of 
components. So I picked the [Gigabyte GeForce RTX 2060 SUPER WINDFORCE OC 8G](https://www.gigabyte.com/Graphics-Card/GV-N206SWF2OC-8GD-rev-20#kf), as a good not too 
expensive GPU with all the features of Turing. Compared to the a bit cheaper
2060 this has 2GB more RAM which is worth the price difference. 
The model in question is a no frills version with no RGB LEDs and two decent fans,
that are fully off if the GPU is not loaded. Hence, the GPU should be completely
silent when not loaded, and not too load at load (hoping for no coil whine).

To be clear, I would **not recommend** buying this GPU now that the RTX 30 Series has
been announced. However, if you, as my friend, cannot wait until say October for the 
3070 which is more than twice as fast as the 2060 Super at ~$499, then you have to
live with paying more for less. ;)

Personally, I am waiting for and hoping a 3060/3050 will be coming soon too,
but it might not be before December or next year :( If that's the case I'd get
the 3070 if one can't wait.

[![Gigabyte GeForce RTX 2060 SUPER WINDFORCE OC 8G]({{ site.baseurl }}/images/2020-09-CorePC/GPU-Gigabyte-Geforce-RTX-2060-super-windforce-oc-8g.png)](https://www.gigabyte.com/Graphics-Card/GV-N206SWF2OC-8GD-rev-20#kf)

From Anandtechs [The NVIDIA GeForce RTX 2070 Super & RTX 2060 Super Review: Smaller Numbers, Bigger Performance
](https://www.anandtech.com/show/14586/geforce-rtx-2070-super-rtx-2060-super-review)
the specifications for the 20 series are:

![NVidia 20 Series]({{ site.baseurl }}/images/2020-09-CorePC/GPU-nvidia-20-series.png)

From [NVIDIA Announces the GeForce RTX 30 Series: Ampere For Gaming, Starting With RTX 3080 & RTX 3090
](https://www.anandtech.com/show/16057/nvidia-announces-the-geforce-rtx-30-series-ampere-for-gaming-starting-with-rtx-3080-rtx-3090) 
the specifications for the 30 series currently are:

![NVidia 30 Series]({{ site.baseurl }}/images/2020-09-CorePC/GPU-nvidia-30-series.png)

Hence, comparing the 3070 to 2060 Super for F32 performance you get 
20.4 TFLOPs	vs 7.2 TFLOPS or 20.4 / 7.2 = 2.83x more at $100 higher price. 
I am guessing that a possible 3060 might have around 14-15 TFLOPS at $399. 
However, more importantly I hope the 30 series will support [TF32](https://blogs.nvidia.com/blog/2020/05/14/tensorfloat-32-precision-format/) since 
they share the 3rd generation Tensor Core with the A100, which should give 
a big performance improvement for reduced precision 32-bit floating point 
training/inference. I haven't seen confirmation of this yet, though.

## System Components: AMD

|Component      |Selection           |USD    |DKK    |
|----------|:-------------------|------:|-------:|
|[CPU]() | [AMD Ryzen™ 7 3700X](https://www.amd.com/en/products/cpu/amd-ryzen-7-3700x) | [(a) 290](https://www.newegg.com/amd-ryzen-7-3700x/p/N82E16819113567)| [2300](https://www.edbpriser.dk/produkt/amd-ryzen-7-3700x-3-6-ghz-processor/?searched=3700X)|
|[Motherboard]() | [Gigabyte B550M DS3H](https://www.gigabyte.com/Motherboard/B550M-DS3H-rev-10#kf) | [95](https://www.newegg.com/gigabyte-b550m-ds3h/p/N82E16813145210)| [730](https://www.edbpriser.dk/produkt/gigabyte-b550m-ds3h)|
|**Total** |-|**385**|**3030**|

**(a)** Includes [Assassin's Creed Valhalla for PC with purchase as a limited offer from AMD](https://www.amd.com/en/gaming/equipped-to-win) in Denmark too.

### CPU (AMD)
The AMD Zen 2 consumer CPUs are covered in [The AMD 3rd Gen Ryzen Deep Dive Review: 3700X and 3900X Raising The Bar](https://www.anandtech.com/show/14605/the-and-ryzen-3700x-3900x-review-raising-the-bar)
in great detail. The newer `XT` variants are described in [AMD To Launch New Ryzen 3000 XT CPUs: Zen 2 with More MHz](https://www.anandtech.com/show/15854/amd-ryzen-3000-xt-cpus-zen-2-more-mhz).
In my view the sweet spot for a CPU for core developer PC 
right now is an 8 core/16 thread CPU. I picked the 3700X, because it has a great price
and is only slightly slower than the 3800X. I will compare this
to the Intel CPU later.

[![AMD Ryzen™ 7 3700X]({{ site.baseurl }}/images/2020-09-CorePC/CPU-AMD-Ryzen-7-3700X.jpg)](https://www.amd.com/en/products/cpu/amd-ryzen-7-3700x)

![AMD Matisse 3000 Series]({{ site.baseurl }}/images/2020-09-CorePC/CPU-AMD-Matisse-3000-Series.png)


### Motherboard (AMD)
There are lots of motherboards to pick from. Since, we are selecting components 
for an affordable PC we are looking at motherboards with the B550 chipset. 
Since the introduction of this in June there were finally inexpensive motherboards 
available supporting PCIe 4.0, which is the main differentiation for AMDs CPUs 
compared to Intels current CPUs.

I have great experience with the Gigabyte DS3H series. With these motherboards 
being stable and robust. If you need more features that this provides, like wifi,
you should find another motherboard. The main features needed here are
a x16 PCIe slot and at least one NVMe M.2 slot. Most boards have that now.
Apparently, you are hard pressed to find a motherboard without RGB FUSION support
now a days. One feature that I like in the Gigabyte boards in the "Smart Fan 5"
feature, that allows you to define fan speed curves in the BIOS, so you can 
tune the system to the cooling and noise profile you want at different temperatures.
This includes fan stop if temperatures are low. Fans will last longer if they don't
spin when not needed.

Note you cannot buy AMD Zen 2 APUs (that is CPU with integrated GPU), 
like the [4700G](https://www.amd.com/en/products/apu/amd-ryzen-7-4700g) 
for do-it-yourself PCs yet. These are only OEM, so if you don't really 
need a high-end GPU you  still have to buy some form of GPU to connect displays. 
As you can see below the motherboard does have DVI and HDMI connectors, 
but those only work if the CPU has an integrated GPU.

> AMD B550 Ultra Durable Motherboard with Pure Digital VRM Solution, PCIe 4.0 x16 Slot, Dual PCIe 4.0/3.0 M.2 Connectors, GIGABYTE 8118 Gaming LAN, Smart Fan 5 with FAN STOP,  RGB FUSION 2.0, Q-Flash Plus
> * Supports 3rd Gen AMD Ryzen™ Processors
> * Dual Channel ECC/ Non-ECC Unbuffered DDR4, 4 DIMMs
> * 5+3 Phases Pure Digital VRM Solution with Low RDS(on) MOSFETs
> * Ultra Durable™ PCIe 4.0 Ready x16 Slot
> * Dual Ultra-Fast NVMe PCIe 4.0/3.0 M.2 Connectors
> * High Quality Audio Capacitors and Audio Noise Guard for Ultimate Audio Quality
> * GIGABYTE Exclusive 8118 Gaming GbE LAN with Bandwidth Management
> * Rear HDMI & DVI Support
> * RGB FUSION 2.0 Supports Addressable LED & RGB LED Strips
> * Smart Fan 5 Features Multiple Temperature Sensors , Hybrid Fan Headers with FAN STOP
> * Q-Flash Plus Update BIOS without Installing the CPU, Memory and Graphics Card
> * Anti-Sulfur Resistors Design

[![Gigabyte B550M DS3H]({{ site.baseurl }}/images/2020-09-CorePC/Motherboard-Gigabyte-B550M-DS3H.png)](https://www.gigabyte.com/Motherboard/B550M-DS3H-rev-10#kf)

[![Gigabyte B550M DS3H Back panel]({{ site.baseurl }}/images/2020-09-CorePC/Motherboard-Gigabyte-B550M-DS3H-back-panel.png)](https://www.gigabyte.com/Motherboard/B550M-DS3H-rev-10#kf)


## System Components: Intel

|Component      |Selection           |USD    |DKK    |
|----------|:-------------------|------:|-------:|
|[CPU]() |[Intel i7-10700](https://ark.intel.com/content/www/us/en/ark/products/199316/intel-core-i7-10700-processor-16m-cache-up-to-4-80-ghz.html) | [305](https://www.newegg.com/intel-core-i7-10700-core-i7-10th-gen/p/N82E16819118126)| [2500](https://www.pricerunner.dk/pl/40-5202251/CPU/Intel-Core-i7-10700-2-9GHz-Socket-1200-Box-Sammenlign-Priser)|
|[Motherboard]() | [Gigabyte B460M DS3H](https://www.gigabyte.com/Motherboard/B460M-DS3H-rev-10)| [80](https://www.newegg.com/gigabyte-ultra-durable-b460m-ds3h/p/N82E16813145206) | [640](https://www.pricerunner.dk/pl/35-5214751/Bundkort/Gigabyte-B460M-DS3H-Sammenlign-Priser) |
|**Total** |-|**385**|**3140**|

To be fair we should be using the [i7-10700F](https://ark.intel.com/content/www/us/en/ark/products/199318/intel-core-i7-10700f-processor-16m-cache-up-to-4-80-ghz.html)
without [iGPU](https://en.wikipedia.org/wiki/Graphics_processing_unit#Integrated_graphics) when comparing to
the AMD 3700X but this I could not find on [newegg](newegg.com) and 
the price difference isn't that big, it would reduce the 
price difference between AMD and Intel by about $15/DKK 100,-

### CPU (Intel)
The Intel Comet Lake also called 10th generation Core are covered in
[The Intel Comet Lake Core i9-10900K, i7-10700K, i5-10600K CPU Review: Skylake We Go Again](https://www.anandtech.com/show/15785/the-intel-comet-lake-review-skylake-we-go-again). Since we are trying to compare systems at about the same price point, 
I picked the i7-10700 (not the K version). Note below the big difference in base
frequencies between the non-K and K versions. As mentioned elsewhere the i7-10700
has the integrated 630 GPU, which is good enough for simple code editing, 
watching videos etc. on a few full HD monitors, so you can cut the expensive GPU
if you don't need it.

[![Intel i7-10700]({{ site.baseurl }}/images/2020-09-CorePC/CPU-Intel-10th-gen-core-i7-10700-tray.jpg)](https://ark.intel.com/content/www/us/en/ark/products/199316/intel-core-i7-10700-processor-16m-cache-up-to-4-80-ghz.html)

![Intel Comet Lake CPUs and Turbos]({{ site.baseurl }}/images/2020-09-CorePC/CPU-Intel-Comet-Lake-Turbos.png)


### Motherboard (Intel)
There isn't a lot of differences between the AMD and Intel DS3H motherboards from
Gigabyte, so nothing new to add here, except that since the i7-10700 has integrated
GPU the DVI, VGA, HDMI ports will all work and you can run with a GPU 
if you don't need it. Due to the CPU differences the Intel motherboard only
supports PCIe 3.0, which probably also explains the price difference between the two.
Additionally, the Intel version only has a single NVMe M.2 slot compared to two in
the AMD version.

> Intel® B460 Ultra Durable Motherboard with GIGABYTE 8118 Gaming LAN, PCIe Gen3 x4 M.2, Anti-Sulfur Resistor, Smart Fan 5, DualBIOS™
> * Supports 10th Gen Intel® Core™ Processors
> * Dual Channel Non-ECC Unbuffered DDR4 , 4 DIMMs
> * High Quality Audio Capacitors and Audio Noise Guard with LED Trace Path Lighting
> * Ultra-Fast M.2 with PCIe Gen3 X4 & SATA Interface
> * GIGABYTE Exclusive 8118 Gaming LAN with Bandwidth Management
> * Smart Fan 5 features Multiple Temperature Sensors and Hybrid Fan Headers with FAN STOP
> * Anti-Sulfur Resistors Design
> * Ultra Durable™ 15KV Surge LAN Protection
> * GIGABYTE UEFI DualBIOS™
> * Intel® Optane™ Memory Ready

[![Gigabyte B460M DS3H]({{ site.baseurl }}/images/2020-09-CorePC/Motherboard-Gigabyte-B460M-DS3H.png)](https://www.gigabyte.com/Motherboard/B460M-DS3H-rev-10#kf)

[![Gigabyte B460M DS3H Back panel]({{ site.baseurl }}/images/2020-09-CorePC/Motherboard-Gigabyte-B460M-DS3H-back-panel.png)](https://www.gigabyte.com/Motherboard/B460M-DS3H-rev-10#kf)

## AMD vs Intel
For comparing the AMD Ryzen 7 3700X with the Intel i7-10700 (which isn't covered
in a lot of reviews) I am using the [Intel Core i7-10700 Review - Way to Overclock without the K](https://www.techpowerup.com/review/intel-core-i7-10700/) 
from Techpowerup. Anandtech, my preferred review site, hasn't covered the i7-10700 yet.

![AMD vs Intel CPU Specifications]({{ site.baseurl }}/images/2020-09-CorePC/CPU-AMD-vs-Intel-CPUs.png)

### Benchmarks
Below is a recap of benchmark results from the above mentioned review.
Where the default/normal circumstances (green bar) is used for the Intel CPU.
Overall, the two CPUs are close to equal except for some specific cases.
I mostly pay attention to the software compilation and web browsing
benchmarks with browsing having a slight edge for the i7-10700, 
but nothing significant.

|Benchmark                                  |3700X  |i7-10700 |Relative ↑|
|-------------------------------------------|------:|--------:|---------:|
|Software Compilation - VS2019 C++ [s]↓     |   63.7|     65.0|      1.02|
|Web Browsing — Google Octane [points]↑     |  54774|    58335|      1.07|
|Web Browsing — Mozilla Kraken [ms]↓        |  723.7|    773.2|      1.07|
|Web Browsing — WebXPRT [points]↑           |    280|      291|      1.04|
|Compression — 7-Zip - Compress [MIPS]↑     |  62860|    59709|      0.95|
|Compression — 7-Zip - Decompress [MIPS]↑   |  60039|    44412|      0.74|
|Encryption — VeraCrypt - AES - 1GB [MB/s]↑ |  11100|    14600|      1.32|
|Relative Performance - CPU Tests [%]↑      |  103.0|    100.0|      1.03|
|Relative Performance - Games 1920x1080 [%]↑|   91.2|    100.0|      0.91|

### Power Consumption
Importantly is also power consumption, also summarized below from the
review. The i7-10700 has a slight edge here, but again nothing significant.
Be aware these are whole system figures and the system includes a
2080 Ti GPU which consumes most power by far.

|Power Consumption, Whole System [w]↓      |3700X  |i7-10700 |Relative ↓|
|-------------------------------------------|------:|--------:|---------:|
|Idle                                       |     53|       49|      1.08|
|Single-Threaded, SuperPI                   |     76|       75|      1.01|
|Multi-Threaded, CineBench                  |    146|      137|      1.07|
|Stress Test, Prime95                       |    140|      142|      0.99|

Given that there are no significant differences in either benchmarks nor
power consumption the choice comes down to AMD supports PCIe 4.0 and 
two M.2 NVMe SSD drives with PCIe 4.0 x4 directly to the CPU.
Additionally, the motherboard is supposed to support the upcoming
Zen 3 CPUs, with AMDs long term AM4 socket support.

One thing to keep in mind, though, especially as software developers,
is that a lot of software depends upon libraries developed by Intel.
A long standing issue is that Intel has been and still appears
to be doing unfair CPU dispatching as detailed in Agner Fog's 
[Intel's "cripple AMD" function](https://www.agner.org/forum/viewtopic.php?t=6).
What this means is that software that uses Intel libraries or compilers **may** 
run less optimal on AMD CPUs, since different code paths are chosen regardless
of instruction set support e.g. AVX2 etc. This also can mean that
there may be result differences for floating point computations. 
Now this is expected and normal,
but I have seen some "issues" for extensive numerical end-to-end tests that
then will fail on an AMD CPU due to this. This is in some ways mainly
due to the tests being too "brittle" to floating point differences, but
the least thing you want is to have to spent time on fixing
this issue if that is not a priority. So just keep that in mind.

### Conclusion
The two systems with selected components are reiterated below in full. 
I very much welcome AMD coming back to the game with the Zen 2
CPUs and giving some real competition to Intel and the fact that
we can now buy 8 core CPUs at a much lower cost than 
just a few years back.

I recommend the AMD Ryzen 7 3700X based system due to it being more future proof 👍 

|Component      |Selection           |USD    |DKK    |
|----------|:-------------------|------:|-------:|
|[Case](https://en.wikipedia.org/wiki/Computer_case) |[Fractal Design Define Mini C](https://www.fractal-design.com/products/cases/define/define-mini-c/black/) | [85](https://www.newegg.com/black-fractal-design-define-mini-c-micro-atx-mini-tower/p/N82E16811352064)| [600](https://www.pricerunner.dk/pl/186-3663561/Kabinetter/Fractal-Design-Define-Mini-C-Sammenlign-Priser)|
|[PSU](https://en.wikipedia.org/wiki/ATX#Power_supply) |[Corsair RM650x 650W](https://www.corsair.com/eu/en/Categories/Products/Power-Supply-Units/Power-Supply-Units-Advanced/RMx-Series/p/CP-9020178-EU) | [115](https://www.newegg.com/corsair-rmx-series-rm650x-2018-cp-9020178-na-650w/p/N82E16817139232)| [900](https://www.edbpriser.dk/produkt/corsair-rmx-series-rm650x-3149708/?searched=rm650x)|
|[CPU Cooler](https://en.wikipedia.org/wiki/Computer_cooling) |[Noctua NH-12S](https://noctua.at/en/nh-u12s)|[65](https://www.newegg.com/noctua-nh-u12s/p/N82E16835608040)|[440](https://www.pricerunner.dk/pl/184-3500421/Computer-koeling/Noctua-NH-U12S-Sammenlign-Priser)|
|**Base** |-|**265**|**1940**|
|[RAM](https://en.wikipedia.org/wiki/DDR4_SDRAM) |[G.SKILL Ripjaws V 2 x 16GB DDR4 3200 (PC4-25600) F4-3200C16D-32GVK](https://www.gskill.com/product/165/184/1536110922/F4-3200C16D-32GVKRipjaws-VDDR4-3200MHz-CL16-18-18-38-1.35V32GB-(2x16GB)) | [110](https://www.newegg.com/g-skill-32gb-288-pin-ddr4-sdram/p/N82E16820232091) | [850](https://www.edbpriser.dk/produkt/g-skill-ripjaws-v-ddr4-32-gb-2-x-16-gb-dimm-288-pin-3513768/) |
|[SSD](https://en.wikipedia.org/wiki/Solid-state_drive) [NVMe](https://en.wikipedia.org/wiki/NVM_Express) | [ADATA XPG SX8200 Pro 1 TB](https://www.xpg.com/us/feature/583/) | [140](https://www.newegg.com/xpg-sx8200-pro-1tb/p/0D9-0017-000W4)| [1030](https://www.edbpriser.dk/produkt/adata-xpg-sx8200-pro/)|
|[GPU]() | [Gigabyte GeForce RTX 2060 SUPER WINDFORCE OC 8G](https://www.gigabyte.com/Graphics-Card/GV-N206SWF2OC-8GD-rev-20#kf)| [400](https://www.newegg.com/gigabyte-geforce-rtx-2060-super-gv-n206swf2oc-8gd/p/N82E16814932174) | [3200](https://www.edbpriser.dk/produkt/gigabyte-geforce-rtx-2060-super-windforce-oc-8g/)|
|**Common** |-|**650**|**5080**|
|[CPU]() | [AMD Ryzen™ 7 3700X](https://www.amd.com/en/products/cpu/amd-ryzen-7-3700x) | [290](https://www.newegg.com/amd-ryzen-7-3700x/p/N82E16819113567)| [2300](https://www.edbpriser.dk/produkt/amd-ryzen-7-3700x-3-6-ghz-processor/?searched=3700X)|
|[Motherboard]() | [Gigabyte B550M DS3H](https://www.gigabyte.com/Motherboard/B550M-DS3H-rev-10#kf) | [95](https://www.newegg.com/gigabyte-b550m-ds3h/p/N82E16813145210)| [730](https://www.edbpriser.dk/produkt/gigabyte-b550m-ds3h)|
|**AMD** |-|**385**|**3030**|
|**AMD Full** |-|**1300**|**10050**|
|[CPU]() |[Intel i7-10700](https://ark.intel.com/content/www/us/en/ark/products/199316/intel-core-i7-10700-processor-16m-cache-up-to-4-80-ghz.html) | [305](https://www.newegg.com/intel-core-i7-10700-core-i7-10th-gen/p/N82E16819118126)| [2500](https://www.pricerunner.dk/pl/40-5202251/CPU/Intel-Core-i7-10700-2-9GHz-Socket-1200-Box-Sammenlign-Priser)|
|[Motherboard]() | [Gigabyte B460M DS3H](https://www.gigabyte.com/Motherboard/B460M-DS3H-rev-10)| [80](https://www.newegg.com/gigabyte-ultra-durable-b460m-ds3h/p/N82E16813145206) | [640](https://www.pricerunner.dk/pl/35-5214751/Bundkort/Gigabyte-B460M-DS3H-Sammenlign-Priser) |
|**Intel** |-|**385**|**3140**|
|**Intel Full** |-|**1300**|**10160**|


If you are penny pinching the most expensive component is the GPU 
so if you don't need it, take that out and perhaps get a cheap GPU for the AMD system.
Then you can reduce RAM from 32 GB to 16 GB, which is enough for most 
(I have yet to upgrade to that), and reduce the SSD to 512 GB. 
Even more cash strapped... go with the Ryzen 5 3600 which is
a great value buy at [$210](https://www.newegg.com/amd-ryzen-5-3600/p/N82E16819113569)/[1400 DKK](https://www.pricerunner.dk/pl/40-4981182/CPU/AMD-Ryzen-5-3600-3.6GHz-Socket-AM4-Box-Sammenlign-Priser). Don't forget to include the
cost of a Microsoft Windows license if you need it, though 😉

This got longer than I expected and I still haven't scratched of the many
considerations that go into selecting components for a PC. 😅



