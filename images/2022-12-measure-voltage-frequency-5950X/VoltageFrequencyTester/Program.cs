using System.Diagnostics;
using RyzenMasterBindings;

Action<string> log = t => { Console.WriteLine(t); Trace.WriteLine(t); };

//RyzenMasterMonitoring(log);


// https://developer.amd.com/amd-ryzentm-master-monitoring-sdk/
// GetCPUParameters does not work, SDK has not been updated since 2020,
// Ryzen Master has a later version but API has changed so no idea how to
// use those dlls without new header files.
static void RyzenMasterMonitoring(Action<string> log)
{
    //log($"{processorCount} {status.SeverityCode}");
    var libraryInit = RyzenMasterLibrary.Init();
    log($"{nameof(RyzenMasterLibrary)}.{nameof(RyzenMasterLibrary.Init)} {libraryInit}");
    var platform = Platform.GetPlatform();
    var platformInit = platform.Init();
    var deviceManager = platform.GetDeviceManager();
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
            log($"PeakCoreSpeed: {parameters?.PeakSpeed}");
            log($"OCMode: {parameters?.Mode.Flags}");
        }
    }
}
