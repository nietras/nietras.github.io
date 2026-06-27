---
layout: post
title: Steam Machine vs Plasma PC™ 
---

Valve's new 2026 **Steam Machine** has created a lot of discussion online given
it's pricing. In this quick blog post I will look at specs of the Steam Machine
and try create a barebones equivalent or better DIY gaming PC called **Plasma
PC**™ (obviously based on solid, liquid, gas, and plasma).

My thoughts on PC component selection where also covered my old blog post from
2020 [Core Developer PC™ v20.09.dGPU - AMD 3700X vs Intel i7-10700 8c/16t with
NVidia 2060 Super]({{ site.baseurl }}/2020/09/08/CoreDeveloperPC-v20.09.dGPU/),
but there I also took the long term view with high quality base components, here
I'll focus more on getting better value, but I'll note that Core Developer PCs
spec'ed in that blog post hit exactly the price point of around ~10000 DKK that
I look at here too.

## Steam Machine Specifications

| Component | Steam Machine |
|----------|:--------------|
| CPU | Semi-custom AMD Zen 4 6C/12T (roughly Ryzen 5 7540U class) |
| GPU | Semi-custom AMD RDNA 3 (roughly Radeon RX 7600M class) |
| Memory | 16 GB DDR5, 5600 MHz, single-channel, 2x SO-DIMM slots |
| Storage | 512 GB or 2 TB M.2 2230 NVMe |
| Expansion | 1x M.2 2280 slot, microSD |
| Networking | Gigabit Ethernet, Wi‑Fi 6E, Bluetooth 5.3 |
| Ports | USB-C 3.2 Gen 2, 4x USB-A |
| Display targets | 720p, 1080p, 1440p, 4K |
| OS | SteamOS |
| Size | 152 x 162.4 x 156 mm |
| Weight | 2.6 kg |

## Steam Machine pricing

Official EU pricing for launch is in euro, so below I include approximate Danish
pricing converted at about **1 EUR = 7.46 DKK**.

| Model | EUR | ~DKK |
|-------|----:|------------:|
| Steam Machine 512 GB | 1039 | ~7750 |
| Steam Machine 512 GB + Steam Controller | 1108 | ~8270 |
| Steam Machine 2 TB | 1359 | ~10140 |
| Steam Machine 2 TB + Steam Controller | 1428 | ~10650 |

Those prices immediately place the Steam Machine in an awkward but fascinating
spot: materially more expensive than a mass-market console, and as many others
have demonstrated building a DIY PC to match within same budget entirely
feasible. Which is what I will do here too, but for European/Denmark prices, and
based on the 2 TB model, since no one should be buying a gaming console/PC with
just 512 GB given just a single game can take up to 435 GB of space (cough, ARK,
cough). This means the budged is around ~10000 DKK with no controller.

## Plasma PC Components and Specifications

|Component      |Selection           |USD    |DKK    |
|----------|:-------------------|------:|-------:|
|[Case](https://en.wikipedia.org/wiki/Computer_case) |[Fractal Design Define Mini C](https://www.fractal-design.com/products/cases/define/define-mini-c/black/) | [85](https://www.newegg.com/black-fractal-design-define-mini-c-micro-atx-mini-tower/p/N82E16811352064)| [600](https://www.pricerunner.dk/pl/186-3663561/Kabinetter/Fractal-Design-Define-Mini-C-Sammenlign-Priser)|
|[PSU](https://en.wikipedia.org/wiki/ATX#Power_supply) |[Corsair RM650x 650W](https://www.corsair.com/eu/en/Categories/Products/Power-Supply-Units/Power-Supply-Units-Advanced/RMx-Series/p/CP-9020178-EU) | [115](https://www.newegg.com/corsair-rmx-series-rm650x-2018-cp-9020178-na-650w/p/N82E16817139232)| [900](https://www.edbpriser.dk/produkt/corsair-rmx-series-rm650x-3149708/?searched=rm650x)|
|[CPU Cooler](https://en.wikipedia.org/wiki/Computer_cooling) |[Noctua NH-12S](https://noctua.at/en/nh-u12s)|[65](https://www.newegg.com/noctua-nh-u12s/p/N82E16835608040)|[440](https://www.pricerunner.dk/pl/184-3500421/Computer-koeling/Noctua-NH-U12S-Sammenlign-Priser)|
|**Total** |-|**265**|**1940**|

|Component      |Selection           |USD    |DKK    |
|----------|:-------------------|------:|-------:|
|[CPU]() | [AMD Ryzen™ 7 3700X](https://www.amd.com/en/products/cpu/amd-ryzen-7-3700x) | [(a) 290](https://www.newegg.com/amd-ryzen-7-3700x/p/N82E16819113567)| [2300](https://www.edbpriser.dk/produkt/amd-ryzen-7-3700x-3-6-ghz-processor/?searched=3700X)|
|[Motherboard]() | [Gigabyte B550M DS3H](https://www.gigabyte.com/Motherboard/B550M-DS3H-rev-10#kf) | [95](https://www.newegg.com/gigabyte-b550m-ds3h/p/N82E16813145210)| [730](https://www.edbpriser.dk/produkt/gigabyte-b550m-ds3h)|
|**Total** |-|**385**|**3030**|

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
