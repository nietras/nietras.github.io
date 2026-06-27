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

| Component   | Selection                                      |                                                                                                                                        DKK w. VAT |     |
| ----------- | :--------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------: | --- |
| CPU         | AMD Ryzen 5 9600X CPU 3.9 GHz 6c/12t AM5       |       [1300](https://www.pricerunner.dk/pl/40-3327615380/CPUs/AMD-Ryzen-5-9600X-CPU-3.9-GHz-6-kerner-med-12-traade-32-MB-cache-Sammenlign-Priser) |     |
| GPU         | ASUS Dual Radeon RX 9060 XT 16GB GDDR6         |                                   [3000](https://www.pricerunner.dk/pl/37-3427553432/Grafikkort/ASUS-16GB-GDDR6-Grafikkort-RAM-Sammenlign-Priser) |     |
| Motherboard | Asrock B850M Pro-A WiFi Micro-ATX              |                                   [900](https://www.pricerunner.dk/pl/35-3391569627/Bundkort/Asrock-B850M-Pro-A-WiFi-Micro-ATX-Sammenlign-Priser) |     |
| RAM         | Kingston Fury Beast Black DDR5 6000MHz 2x8GB   | [1850](https://www.pricerunner.dk/pl/38-3204693407/RAM/Kingston-Fury-Beast-Black-DDR5-6000MHz-2x8GB-ECC-%28KF560C36BBEK2-16%29-Sammenlign-Priser) |     |
| SSD         | SanDisk Black SN7100 SSD 2TB                   |                                 [2000](https://www.pricerunner.dk/pl/36-3381548698/Harddiske/SanDisk-Black-SN7100-2TB-NVMe-SSD-Sammenlign-Priser) |     |
| Case        | Zalman S3 Tempered Glass incl. 3 x 120 mm fans |                                         [280](https://www.pricerunner.dk/pl/186-3200124333/Kabinetter/Zalman-S3-Tempered-Glass-Sammenlign-Priser) |     |
| PSU         | Corsair RM650e (2025) 650W                     |                               [550](https://www.pricerunner.dk/pl/640-3391267474/Stroemforsyning/Corsair-RM650e%28-2025%29650W-Sammenlign-Priser) |     |
| CPU Cooler  | Thermalright Peerless Assassin 120 SE          |                      [260](https://www.pricerunner.dk/pl/184-3207363733/Computer-koeling/Thermalright-Peerless-Assassin-120-SE-Sammenlign-Priser) |     |
| **Total**   |                                                |                                                                                                                                         **10140** |     | 


| Component                                       | Link                                                                                                                                                     | Price<br>w. VAT | Pcs | Total<br>w. VAT |
| ----------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------- | --------------- | --- | --------------- |
| AMD Ryzen 5 9600X CPU 3.9 GHz 6c/12t AM5        | [pricerunner](https://www.pricerunner.dk/pl/40-3327615380/CPUs/AMD-Ryzen-5-9600X-CPU-3.9-GHz-6-kerner-med-12-traade-32-MB-cache-Sammenlign-Priser)       | 1300            | 1   | 1300            |
| ASUS Dual Radeon RX 9060 XT 16GB GDDR6          | [pricerunner](https://www.pricerunner.dk/pl/37-3427553432/Grafikkort/ASUS-16GB-GDDR6-Grafikkort-RAM-Sammenlign-Priser)                                   | 3000            | 1   | 3000            |
| Asrock B850M Pro-A WiFi Micro-ATX               | [pricerunner](https://www.pricerunner.dk/pl/35-3391569627/Bundkort/Asrock-B850M-Pro-A-WiFi-Micro-ATX-Sammenlign-Priser)                                  | 900             | 1   | 900             |
| Kingston Fury Beast Black DDR5 6000MHz 2x8GB    | [pricerunner](https://www.pricerunner.dk/pl/38-3204693407/RAM/Kingston-Fury-Beast-Black-DDR5-6000MHz-2x8GB-ECC-%28KF560C36BBEK2-16%29-Sammenlign-Priser) | 1850            | 1   | 1850            |
| SanDisk Black SN7100 SSD WDS200T4X0E-00CJA0 2TB | [pricerunner](https://www.pricerunner.dk/pl/36-3381548698/Harddiske/SanDisk-Black-SN7100-2TB-NVMe-SSD-Sammenlign-Priser)                                 | 2000            | 1   | 2000            |
| Zalman S3 Tempered Glass feat. 3 x 120 mm fans  | [pricerunner](https://www.pricerunner.dk/pl/186-3200124333/Kabinetter/Zalman-S3-Tempered-Glass-Sammenlign-Priser)                                        | 280             | 1   | 280             |
| Corsair RM650e( 2025) 650W                      | [pricerunner](https://www.pricerunner.dk/pl/640-3391267474/Stroemforsyning/Corsair-RM650e%28-2025%29650W-Sammenlign-Priser)                              | 550             | 1   | 550             |
| Thermalright Peerless Assassin 120 SE           | [pricerunner](https://www.pricerunner.dk/pl/184-3207363733/Computer-koeling/Thermalright-Peerless-Assassin-120-SE-Sammenlign-Priser)                     | 260             | 1   | 260             |
| Total                                           |                                                                                                                                                          |                 |     | 10140           |

ASUS Dual Radeon RX 9060 XT 16GB GDDR6	pricerunner	3000
AMD Ryzen 5 9600X CPU 3.9 GHz 6c/12t AM5	pricerunner	1300
Asrock B850M Pro-A WiFi Micro-ATX	pricerunner	900
Kingston Fury Beast Black DDR5 6000MHz 2x8GB	pricerunner	1850
SanDisk Black SN7100 SSD WDS200T4X0E-00CJA0 2TB	pricerunner	2000
Thermalright Peerless Assassin 120 SE	pricerunner	260
Zalman S3 Tempered Glass incl. 3 x 120 mm fans	pricerunner	280
Corsair RM650e (2025) 650W	pricerunner	550
Total		

pricerunner
