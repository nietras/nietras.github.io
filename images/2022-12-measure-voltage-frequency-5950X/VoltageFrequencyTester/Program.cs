using System.Diagnostics;
using System.Security.Policy;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Exceptions;
using FlaUI.Core.Identifiers;
using FlaUI.UIA3;
using FlaUI.UIA3.Patterns;
using RyzenMasterBindings;

Action<string> log = t => { Console.WriteLine(t); Trace.WriteLine(t); };

// start /affinity 1 /b ./y-cruncher.exe bench 500m -PF:none
// C:\Windows\System32\cmd.exe /c start "" /High /Affinity 1 "C:\Windows\System32\mspaint.exe"

// https://github.com/FlaUI/FlaUI

var processes = Process.GetProcessesByName("cpuz");
var process = processes.Length > 0 ? processes[0] : null;
if (process is null)
{
    var processStartInfo = new ProcessStartInfo(@"C:\Program Files\CPUID\CPU-Z\cpuz.exe");
    processStartInfo.WorkingDirectory = @"C:\Program Files\CPUID\CPU-Z\";
    process = Process.Start(processStartInfo);
}
// Not working
//process!.ProcessorAffinity = (nint)0b01;
//var threads = process.Threads;
//foreach (ProcessThread thread in threads)
//{
//    thread.ProcessorAffinity = (nint)0b01;
//}
using (process)
{
    using var app = new FlaUI.Core.Application(process);
    using var automation = new UIA3Automation();

    // If cpu-z has not started need to wait for splash screen to finish
    var window = app.GetMainWindow(automation);
    while (window.IsAvailable && CheckTitleEmpty(window))
    {
        await Task.Delay(100);
    }
    // Then get window again since new after splash screen
    window = app.GetMainWindow(automation);
    log(window.Title);

    var benchTab = window.FindFirstDescendant(cf => cf.ByText("Bench")).AsTabItem();
    benchTab.Select();

    var threadsCheckBox = window.FindFirstDescendant(cf =>
        cf.ByText("Threads").And(cf.ByControlType(ControlType.CheckBox))).AsCheckBox();
    threadsCheckBox.IsChecked = true;

    // Threads combo box has no name (could find by Items but just using AutomationId for now
    var threadsComboBox = window.FindFirstDescendant(cf => cf.ByAutomationId("1054")).AsComboBox();
    threadsComboBox.Value = "  1";

    var benchProgressBar = window.FindFirstDescendant(cf => cf.ByAutomationId("1002")
        .And(cf.ByControlType(ControlType.ProgressBar))).AsProgressBar();

    var progressBars = window.FindAllDescendants(cf => cf.ByControlType(ControlType.ProgressBar)).Select(e => e.AsProgressBar()).ToArray();
    var textBoxes = window.FindAllDescendants(cf => cf.ByControlType(ControlType.Text)).Select(e => e.AsTextBox()).ToArray();

    //var descendants = window.FindAllDescendants();
    //var comboBoxes = descendants.Where(d => d.ControlType == ControlType.ComboBox).Select(d => d.AsComboBox()).ToArray();
    var stressButton = window.FindFirstDescendant(cf => cf.ByName("Stress CPU")
        .And(cf.ByControlType(ControlType.Button))).AsButton();
    stressButton.Invoke();


    for (int i = 0; i < 100; i++)
    {
        await Task.Delay(500);
        //foreach (var progressBar in progressBars)
        //{
        //    log($"{progressBar.Name} {progressBar.AutomationId} {progressBar.Value}");
        //}
        foreach (var textBox in textBoxes)
        {
            if (textBox.IsAvailable && textBox.IsEnabled && textBox.IsPatternSupported(TextPattern.Pattern))
            {
                log($"{textBox.Name} {textBox.Text}");
            }
        }
        // Read bench score, note time for hwinfo log (logs every 2 seconds)
        var properties = benchProgressBar.Properties;

    }

    await Task.Delay(2000);

    stressButton.Invoke();



    //RyzenMasterMonitoring(log);

    log("end");
}


static bool CheckTitleEmpty(Window window)
{
    try
    {
        return string.IsNullOrWhiteSpace(window.Title);
    }
    catch (PropertyNotSupportedException)
    {
        return false;
    }
}


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
