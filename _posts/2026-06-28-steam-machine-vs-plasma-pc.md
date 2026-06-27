---
layout: post
title: Valve Steam Machine vs DIY Plasma PC™ 
---

Valve's new 2026 **Steam Machine** has created a lot of discussion online given
it's pricing. In this quick blog post I will look at specs of the Steam Machine
and try create a barebones equivalent or better DIY gaming PC called **Plasma
PC**™ (obviously based on solid, liquid, gas, and plasma).

My thoughts on PC component selection where also covered in my old blog post
from 2020 [Core Developer PC™ v20.09.dGPU - AMD 3700X vs Intel i7-10700 8c/16t
with NVidia 2060 Super]({{ site.baseurl
}}/2020/09/08/CoreDeveloperPC-v20.09.dGPU/), but there I also took the long term
view with high quality base components, here I'll focus more on getting better
value, but I'll note that Core Developer PCs spec'ed in that blog post hit
exactly the price point of around ~10000 DKK that I look at here too, almost 6
years ago.

## Steam Machine Links

There are plenty of reviews and teardowns online. Overall, quality of the Steam
Machine looks quite good with a very good big cooler with copper base and large
aluminum fins. It also pitch black 👍 It does have a ton of "appendage"
components, though.

* [Valve Steam Machine Review: GPU & CPU Benchmarks, SteamOS Test, Thermals, Noise, and Price](https://gamersnexus.net/pre-built-pc/valve-steam-machine-review-gpu-cpu-benchmarks-steamos-test-thermals-noise-and-price)
* [
Excellent Repairability: Steam Machine Tear-Down and Accessing RAM & SSD](https://www.youtube.com/watch?v=glXA3ObwSwQ)
* [A look inside the Steam Machine](https://www.theverge.com/games/820545/a-look-inside-the-steam-machine)

![Steam Machine]({{ site.baseurl }}/images/2026-06-steam-machine-vs-plasma-pc/steam-machine.png)

![Steam Machine Inside]({{ site.baseurl }}/images/2026-06-steam-machine-vs-plasma-pc/steam-machine-inside.png)

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

## **Plasma PC™** Components

As shown below you can hit the same price point with current generation
components (Zen 5, RX 9000 series) compared to last gen components in Valve
Steam Machine.

| Component   | Selection                                      |                                                                                                                                        DKK w. VAT | ~EUR  |
| ----------- | :--------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------: | ---: |
| CPU         | AMD Ryzen 5 9600X CPU 3.9 GHz 6c/12t AM5       |       [1300](https://www.pricerunner.dk/pl/40-3327615380/CPUs/AMD-Ryzen-5-9600X-CPU-3.9-GHz-6-kerner-med-12-traade-32-MB-cache-Sammenlign-Priser) |  ~174 |
| GPU         | ASUS Dual Radeon RX 9060 XT 16GB GDDR6         |                                   [3000](https://www.pricerunner.dk/pl/37-3427553432/Grafikkort/ASUS-16GB-GDDR6-Grafikkort-RAM-Sammenlign-Priser) |  ~402 |
| Motherboard | Asrock B850M Pro-A WiFi Micro-ATX              |                                   [900](https://www.pricerunner.dk/pl/35-3391569627/Bundkort/Asrock-B850M-Pro-A-WiFi-Micro-ATX-Sammenlign-Priser) |  ~121 |
| RAM         | Kingston Fury Beast Black DDR5 6000MHz 2x8GB   | [1850](https://www.pricerunner.dk/pl/38-3204693407/RAM/Kingston-Fury-Beast-Black-DDR5-6000MHz-2x8GB-ECC-%28KF560C36BBEK2-16%29-Sammenlign-Priser) |  ~248 |
| SSD         | SanDisk Black SN7100 SSD 2TB                   |                                 [2000](https://www.pricerunner.dk/pl/36-3381548698/Harddiske/SanDisk-Black-SN7100-2TB-NVMe-SSD-Sammenlign-Priser) |  ~268 |
| Case        | Zalman S3 Tempered Glass incl. 3 x 120 mm fans |                                         [280](https://www.pricerunner.dk/pl/186-3200124333/Kabinetter/Zalman-S3-Tempered-Glass-Sammenlign-Priser) |   ~38 |
| PSU         | Corsair RM650e (2025) 650W                     |                               [550](https://www.pricerunner.dk/pl/640-3391267474/Stroemforsyning/Corsair-RM650e%28-2025%29650W-Sammenlign-Priser) |   ~74 |
| CPU Cooler  | Thermalright Peerless Assassin 120 SE          |                      [260](https://www.pricerunner.dk/pl/184-3207363733/Computer-koeling/Thermalright-Peerless-Assassin-120-SE-Sammenlign-Priser) |   ~35 |
| **Total**   |                                                |                                                                                                                                         **10140** | **~1359** | 

## **Plasma PC™** Specifications

Turning this into specifications ala Steam Machine you get the below. Notably,
dual channel memory so 2x higher memory bandwidth. 16 GB VRAM with full Navi 44
GPU die.

| Component | Plasma PC™ |
|----------|:-----------|
| CPU | AMD Ryzen 5 9600X, Zen 5, 6C/12T, 3.9 GHz base, up to 5.4 GHz boost |
| GPU | Radeon RX 9060 XT, 16 GB GDDR6 |
| Memory | 16 GB DDR5, 6000 MHz, dual-channel (2x8 GB) |
| Storage | 2 TB NVMe SSD |
| Expansion | Full desktop AM5 platform, PCIe x16 GPU, additional motherboard expansion |
| Networking | 2.5 GB Ethernet, Wi‑Fi 6E and Bluetooth 5.2 |
| Ports | Multiple USB-A/USB-C plus DisplayPort/HDMI from the discrete GPU |
| Display targets | 1080p, 1440p, 4K |
| OS | SteamOS (free) |

## Conclusion

I honestly don't know why you would buy the steam machine other than due to it's
compact form factor. Which is what everyone else is saying too 😅

Perhaps power consumption, which, of course, can be much higher in **Plasma
 PC™**, hence the name. It doesn't have to be, though, as you can undervolt and
downclock. 

Most of the DIY PC components can be also re-used, even allow for upgrading to
Zen 6 and possibly even Zen 7 as AM5 socket.

That's all!