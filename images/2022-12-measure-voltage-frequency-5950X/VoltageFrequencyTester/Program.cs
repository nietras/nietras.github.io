using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using RyzenMasterBindings;
using Windows.Win32;
using Windows.Win32.System.Power;

// https://developer.amd.com/amd-ryzentm-master-monitoring-sdk/

unsafe
{
    Action<string> log = t => { Console.WriteLine(t); Trace.WriteLine(t); };
    //PInvoke.GetSystemInfo(out var systemInfo);

    //var processorCount = (int)systemInfo.dwNumberOfProcessors;

    //// PROCESSOR_POWER_INFORMATION.CurrentMhz no longer shows CurrentMhz
    //// https://github.com/microsoft/Windows-Dev-Performance/issues/100
    //PROCESSOR_POWER_INFORMATION* infos = stackalloc PROCESSOR_POWER_INFORMATION[processorCount];
    //var infosSpan = new Span<PROCESSOR_POWER_INFORMATION>(infos, processorCount);
    //var status = PInvoke.CallNtPowerInformation(Windows.Win32.System.Power.POWER_INFORMATION_LEVEL.ProcessorInformation, null, 0,
    //    infos, (uint)(Unsafe.SizeOf<PROCESSOR_POWER_INFORMATION>() * processorCount));
    //if (status.SeverityCode != Windows.Win32.Foundation.NTSTATUS.Severity.Success)
    //{

    //}


    //log($"{processorCount} {status.SeverityCode}");
    var libraryInit = RyzenMasterLibrary.Init();
    log($"{nameof(RyzenMasterLibrary)}.{nameof(RyzenMasterLibrary.Init)} {libraryInit}");

    using var platform = Platform.GetPlatform();
    var platformInit = platform.Init();
    using var deviceManager = platform.GetDeviceManager();
    var deviceCount = deviceManager.GetTotalDeviceCount();

    for (var i = 0u; i < deviceManager.GetTotalDeviceCount(); i++)
    {
        using var device = deviceManager.GetDevice(i);
        log("-----");
        log($"Index: {i}");
        log($"Name: {device.GetName()}");
        log($"Description: {device.GetDescription()}");
        log($"Vendor: {device.GetVendor()}");
        log($"Role: {device.GetRole()}");
        log($"ClassName: {device.GetClassName()}");
        log($"DeviceType: {device.GetDeviceType()}");
        log($"Index (API): {device.GetIndex()}");

        if (device.GetDeviceType() == DeviceType.DT_BIOS)
        {
            using var bios = device.AsBios();
            log($"Mem VDDIO: {bios.GetMemVDDIO()}");
            log($"Mem Clock: {bios.GetCurrentMemClock()}");
            log($"Mem CTRL TCL: {bios.GetMemCtrlTcl()}");
            log($"Mem CTRL Trcdrd: {bios.GetMemCtrlTrcdrd()}");
            log($"Mem CTRL Tras: {bios.GetMemCtrlTras()}");
            log($"Mem CTRL Trp: {bios.GetMemCtrlTrp()}");
            log($"Version: {bios.GetVersion()}");
            log($"Vendor: {bios.GetVendor()}");
            log($"Date: {bios.GetDate()}");
        }
        else if (device.GetDeviceType() == DeviceType.DT_CPU)
        {
            using var cpu = device.AsCpu();
            log($"L1D: {cpu.GetL1DataCacheInfo()?.ToString()}");
            log($"L1I: {cpu.GetL1InstructionCacheInfo()?.ToString()}");
            log($"L2: {cpu.GetL2CacheInfo()?.ToString()}");
            log($"L3: {cpu.GetL3CacheInfo()?.ToString()}");
            log($"CoreCount: {cpu.GetCoreCount()}");
            log($"CorePark: {cpu.GetCorePark()}");
            log($"Package: {cpu.GetPackage()}");
            log($"Chipset: {cpu.GetChipsetName()}");
            var parameters = cpu.GetCpuParameters();
            log($"PeakCoreSpeed: {cpu.GetCpuParameters()?.PeakSpeed}");
            log($"OCMode: {cpu.GetCpuParameters()?.Mode.Flags}");
        }
    }
}
