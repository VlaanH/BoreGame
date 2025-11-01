using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace BoreGame.BoreConectors;

public class BoreClassic : IBore
{
    public string BorePatch { get; set; }
    public string ServerDefault = "bore.pub";
    private Process? _boreProcess;
    private string? _lastOutput;
    private readonly object _lockObject = new object();
    private string? _currentServer;
    
    public BoreClassic(string borePatch)
    {
        BorePatch = borePatch;
    }
    
    public string StartBoreProcess(string arguments, bool waitForOutput = true, int timeoutSeconds = 10)
    {
        _lastOutput = null;
        
        var processStartInfo = new ProcessStartInfo()
        {
            FileName = BorePatch,
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        _boreProcess = new Process
        {
            StartInfo = processStartInfo,
            EnableRaisingEvents = true
        };
        
        ManualResetEventSlim? outputReceived = waitForOutput ? new ManualResetEventSlim(false) : null;

        _boreProcess.OutputDataReceived += (sender, args) =>
        {
            if (!string.IsNullOrEmpty(args.Data))
            {
                Debug.WriteLine($"Bore output: {args.Data}");
                
                lock (_lockObject)
                {
                    if (_lastOutput == null)
                    {
                        _lastOutput = args.Data;
                        outputReceived?.Set();
                    }
                }
            }
        };

        _boreProcess.ErrorDataReceived += (sender, args) =>
        {
            if (!string.IsNullOrEmpty(args.Data))
            {
                Debug.WriteLine($"Bore error: {args.Data}");
            }
        };

        try
        {
            // Запускаем процесс
            _boreProcess.Start();
            _boreProcess.BeginOutputReadLine();
            _boreProcess.BeginErrorReadLine();

            // Если нужно ждать вывода
            if (waitForOutput && outputReceived != null)
            {
                bool received = outputReceived.Wait(TimeSpan.FromSeconds(timeoutSeconds));
                
                if (!received)
                {
                    throw new TimeoutException($"Не удалось получить вывод bore за {timeoutSeconds} секунд");
                }
            }

            return _lastOutput;
        }
        finally
        {
            outputReceived?.Dispose();
        }
    }


    public bool IsRuned()
    {
        try
        {
            if (!_boreProcess.HasExited!=null)
            {
                return !_boreProcess.HasExited;
            }
            return false;
        }
        catch (Exception e)
        {
            return false;
        }
       
    }
    public static string GetOnlyDigits(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;
    
        var sb = new StringBuilder();
        foreach (char c in input)
        {
            if (char.IsDigit(c))
                sb.Append(c);
        }
        return int.Parse(sb.ToString()).ToString();
    }
    
    public string OpenPort(string openedPort, string server)
    {
        var outData = StartBoreProcess($"local {openedPort} --to {server}");
        Console.WriteLine(outData);
      
        string? tunnelUrl = null;

        string tunnelPort = outData.Split("=")[1];
        
        tunnelUrl = server+":"+ GetOnlyDigits(tunnelPort);

        // Возвращаем URL туннеля (формат: server:port)
        return tunnelUrl;
    }

    public void StopOpened()
    {
        if (_boreProcess != null && !_boreProcess.HasExited)
        {
            try
            {
                _boreProcess.Kill();
                _boreProcess.WaitForExit(5000);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при остановке bore: {ex.Message}");
            }
            finally
            {
                _boreProcess?.Dispose();
                _boreProcess = null;
            }
        }
    }
}