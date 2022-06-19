using System;
using System.Linq;
using System.Text;
using PascalABCCompiler.SyntaxTree;
using PascalABCCompiler.Errors;
using PascalABCCompiler;
using PascalABCCompiler.TreeConverter;

namespace PasCompiler
{
    class PABCCompilerRunner
    {
        static string Compile(Compiler c, string myfilename) {
            var co = new CompilerOptions(myfilename, CompilerOptions.OutputType.ConsoleApplicaton);
            co.UseDllForSystemUnits = false;
            co.Debug = false;
            co.ForDebugging = false;
            return c.Compile(co);
        }
        static string RunProcess(string myexefilename) {
            var outputstring = new StringBuilder();
            var pabcnetcProcess = new System.Diagnostics.Process();
            pabcnetcProcess.StartInfo.FileName = myexefilename;
            pabcnetcProcess.StartInfo.UseShellExecute = false;
            //pabcnetcProcess.StartInfo.CreateNoWindow = true;
            //pabcnetcProcess.StartInfo.RedirectStandardInput = true;
            //pabcnetcProcess.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
            pabcnetcProcess.StartInfo.RedirectStandardOutput = true;
            pabcnetcProcess.EnableRaisingEvents = true;
            var outputOverflow = false;
            pabcnetcProcess.OutputDataReceived += (o, e) => {
                if (e.Data != null) {
                    if (!outputOverflow) {
                        outputstring.Append(e.Data);
                        if (outputstring.Length > 10000) {
                            outputstring.Length = 10000;
                            outputOverflow = true;
                            outputstring.Append("...");
                        }
                        outputstring.AppendLine();
                    }
                }
            };
            pabcnetcProcess.Start();
            pabcnetcProcess.BeginOutputReadLine();
            pabcnetcProcess.WaitForExit(5000);
            if (!pabcnetcProcess.HasExited) { // убить процесс если он работвет больше 5 секунд. Скорее всего он завис
                pabcnetcProcess.Kill();
                outputstring.AppendLine("Program completed. It worked more then 5 seconds and probably stuck");
            }
            return outputstring.ToString();
        }
        static void Main(string[] args)
        {            
            if (args.Length == 0) {
                System.Console.WriteLine("Need param - name .pas-file");
                Environment.Exit(0);
            }
            StringResourcesLanguage.LoadDefaultConfig();
            StringResourcesLanguage.CurrentLanguageName = "English";
            var c = new Compiler();
            var myfilename = args[0];
            var myexefilename = Compile(c, myfilename);

            var output = "";
            if (myexefilename == null) {
                if (c.ErrorsList.Count > 0) {
                    var err = c.ErrorsList[0];
                    output = Helper.EnhanceErrorMsg(err) + '\n';
                }
            }
            else
                output = RunProcess(myexefilename);
            System.Console.WriteLine(output);
        }
    }
}
